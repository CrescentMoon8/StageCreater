// ---------------------------------------------------------
// StageOutputEditor.cs
//
// 作成日:2024/03/05
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Text;

public class StageOutputEditor : EditorWindow
{
    [SerializeField]
    [Header("出力対象のタイルマップ")]
    private Tilemap _targetTilemap = default;

    [SerializeField]
    [Header("ステージ名（必ずナンバリングすること）例：Stage1,Stage2")]
    private string _stageName = default;

    private SerializedObject serializedObject = default;

    [Header("対象タイル１（出力値：１）")]
    [SerializeField]
    private TileBase _targetTile1 = default;
    [Header("対象タイル２（出力値：２）")]
    [SerializeField]
    private TileBase _targetTile2 = default;
    [Header("対象タイル３（出力値：３）")]
    [SerializeField]
    private TileBase _targetTile3 = default;
    [Header("対象タイル４（出力値：４）")]
    [SerializeField]
    private TileBase _targetTile4 = default;

    // ステージの横の最大サイズ
    private int _horizontalMaxSize = default;
    // ステージの縦の最大サイズ
    private int _verticalMaxSize = default;

    private int[,] _stageArray = default;

    [MenuItem("Stage/StageOutput", false, 1)]
    private static void ShowWindow()
    {
        StageOutputEditor window = GetWindow<StageOutputEditor>();
        window.titleContent = new GUIContent("StageOutput");
    }

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
    }

    private void OnGUI()
    {
        // 対象のタイルマップをアタッチできるようにウィンドウに表示する
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTilemap)}"));

        // 対象のタイルをアタッチできるようにウィンドウに表示する
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTile1)}"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTile2)}"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTile3)}"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTile4)}"));

        // ステージ名を入力できるようウィンドウに表示する
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_stageName)}"));

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("ステージ情報を出力"))
        {
            // ステージ名を空のまま出力させない
            if (_stageName == "")
            {
                Debug.LogError("ステージ名を入力してください");
                return;
            }

            // マップの最大サイズを設定する
            SetStageMaxSize();
            // ステージ、ターゲットの配列の大きさを設定する
            _stageArray = new int[_verticalMaxSize, _horizontalMaxSize];
            // マップイメージを配列に格納する
            ImageToArray();

            // string path = Application.persistentDataPath + "/" + _stageName + ".csv";
            string path = "Assets/StageData/" + _stageName + ".csv";

            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8")))
            {
                for (int y = 0; y < _verticalMaxSize; y++)
                {
                    string[] array = new string[_horizontalMaxSize];

                    for (int x = 0; x < _horizontalMaxSize; x++)
                    {
                        array[x] = _stageArray[y, x].ToString();
                    }

                    string row = string.Join(",", array);

                    streamWriter.WriteLine(row);
                }
            }

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            string guid = AssetDatabase.AssetPathToGUID(path);

            AddressableAssetGroup group = settings.DefaultGroup;

            AddressableAssetEntry stageFile = settings.CreateOrMoveEntry(guid, group);

            stageFile.SetAddress(_stageName);

            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
	/// マップの最大サイズを設定する
	/// </summary>
	private void SetStageMaxSize()
    {
        _horizontalMaxSize = _targetTilemap.cellBounds.max.x;
        _verticalMaxSize = -_targetTilemap.cellBounds.min.y;
    }

    private void ImageToArray()
    {
        for (int i = 0; i < _verticalMaxSize; i++)
        {
            for (int j = 0; j < _horizontalMaxSize; j++)
            {
                // ワールド座標とタイルマップ座標のずれをなくすため＋１する
                // 座標と配列番号を合わせるためにマイナスをつける
                Vector3Int searchPos = new Vector3Int(j, -i);

                // 指定した座標にタイルがなければ処理をスキップする
                if (!_targetTilemap.HasTile(searchPos))
                {
                    continue;
                }

                // 指定した座標のタイルによって配列情報をセットする
                if (_targetTilemap.GetTile(searchPos).Equals(_targetTile1))
                {
                    _stageArray[i, j] = 1;
                }
                else if (_targetTilemap.GetTile(searchPos).Equals(_targetTile2))
                {
                    _stageArray[i, j] = 2;
                }
                else if (_targetTilemap.GetTile(searchPos).Equals(_targetTile3))
                {
                    _stageArray[i, j] = 3;
                }
                else if (_targetTilemap.GetTile(searchPos).Equals(_targetTile4))
                {
                    _stageArray[i, j] = 4;
                }
            }
        }
    }
}