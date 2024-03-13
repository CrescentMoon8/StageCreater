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

public class StageOutputEditor : StageCreateEditor
{
    [SerializeField]
    [Header("ステージ名（推奨：半角、ナンバリング）")]
    protected string _stageName = default;

    [MenuItem("Stage/StageOutput", false, 1)]
    private static void ShowWindow()
    {
        StageOutputEditor window = GetWindow<StageOutputEditor>();
        window.titleContent = new GUIContent("StageOutput");
    }

    protected override void OnGUI()
    {
        // ステージ名を入力できるようウィンドウに表示する
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_stageName)}"));

        base.InputProperty();

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
            _stageArray = new int[base._verticalMaxSize, base._horizontalMaxSize];
            // マップイメージを配列に格納する
            ImageToArray();

            // string path = Application.persistentDataPath + "/" + _stageName + ".csv";
            string path = "Assets/StageData/" + _stageName + ".csv";

            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8")))
            {
                for (int y = 0; y < base._verticalMaxSize; y++)
                {
                    string[] array = new string[base._horizontalMaxSize];

                    for (int x = 0; x < base._horizontalMaxSize; x++)
                    {
                        array[x] = base._stageArray[y, x].ToString();
                    }

                    string row = string.Join(",", array);

                    streamWriter.WriteLine(row);
                }
            }

            // Addressable登録時にファイルが見つからないことがあるため強制的に再読み込みする
            AssetDatabase.Refresh();

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
        _horizontalMaxSize = _stageTilemap.cellBounds.max.x + (-_stageTilemap.cellBounds.min.x);
        _verticalMaxSize = _stageTilemap.cellBounds.max.y + (-_stageTilemap.cellBounds.min.y);
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
                if (!_stageTilemap.HasTile(searchPos))
                {
                    continue;
                }

                TileCheck(i, j, searchPos);
            }
        }
    }

    private void TileCheck(int i, int j, Vector3Int searchPos)
    {
        for (int k = 0; k < _targetTileList.Count; k++)
        {
            // 指定した座標のタイルによって配列情報をセットする
            if (_stageTilemap.GetTile(searchPos).Equals(_targetTileList[k]))
            {
                // タイルが空の部分を０にするために＋１する
                _stageArray[i, j] = k + 1;
            }
        }
    }
}