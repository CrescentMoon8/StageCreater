// ---------------------------------------------------------
// StageInputEditor.cs
//
// 作成日:2024/03/07
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;
using System.IO;
using System.Text;

public class StageInputEditor : EditorWindow
{
    [SerializeField]
    [Header("入力対象のタイルマップ")]
    private Tilemap _targetTilemap = default;

    [SerializeField]
    [Header("生成したいステージ名")]
    private string _stageName = default;

    private SerializedObject serializedObject = default;

    [Header("配置対象タイル１（入力値：１）")]
    [SerializeField]
    private TileBase _targetTile1 = default;
    [Header("配置対象タイル２（入力値：２）")]
    [SerializeField]
    private TileBase _targetTile2 = default;
    [Header("配置対象タイル３（入力値：３）")]
    [SerializeField]
    private TileBase _targetTile3 = default;
    [Header("配置対象タイル４（入力値：４）")]
    [SerializeField]
    private TileBase _targetTile4 = default;

    // ステージの横の最大サイズ
    private int _horizontalMaxSize = default;
    // ステージの縦の最大サイズ
    private int _verticalMaxSize = default;

    private int[,] _stageArray = default;

    [MenuItem("Stage/StageInput", false, 1)]
    private static void ShowWindow()
    {
        StageInputEditor window = GetWindow<StageInputEditor>();
        window.titleContent = new GUIContent("StageInput");
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

        if (GUILayout.Button("ステージを生成"))
        {
            //TextAsset inputData = Addressables.LoadAssetAsync<TextAsset>(_stageName).WaitForCompletion();
            string path = Path.GetFullPath(_stageName + ".csv");

            using (StreamReader streamReader = new StreamReader(path, Encoding.GetEncoding("UTF-8")))
            {
                string[] data = streamReader.ReadToEnd().Split(new char[] { '\r', '\n' });
                _verticalMaxSize = data.Length;
                _horizontalMaxSize = data[0].Length;

                for (int i = 0; i < _verticalMaxSize; i++)
                {
                    string line = streamReader.ReadLine();

                    for (int j = 0; j < _horizontalMaxSize; j++)
                    {
                        SetTile(int.Parse(line.Split(",")[j]), j, i);
                        _stageArray[i, j] = int.Parse(line.Split(",")[j]);
                    }
                }
            }

            
        }
    }

    private void SetTile(int number, int x, int y)
    {
        Vector3Int setPos = new Vector3Int(x, -y);

        switch (number)
        {
            case 1:
                _targetTilemap.SetTile(setPos, _targetTile1);
                break;

            case 2:
                _targetTilemap.SetTile(setPos, _targetTile2);
                break;

            case 3:
                _targetTilemap.SetTile(setPos, _targetTile3);
                break;

            case 4:
                _targetTilemap.SetTile(setPos, _targetTile4);
                break;

            default:
                break;
        }
    }
}