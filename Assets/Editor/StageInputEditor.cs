// ---------------------------------------------------------
// StageInputEditor.cs
//
// 作成日:2024/03/07
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.IO;


public class StageInputEditor : StageCreateEditor
{
    // 除外するファイルの拡張子
    private const string EXCLUSION_EXTENSION = ".meta";
    // 削除する拡張子
    private const string DELETE_EXTENSION = ".csv";
    // 検索するフォルダー
    private const string STAGE_DATA_FOLDE = "Assets/StageData";
    private int _stageIndex = 0;

    [MenuItem("Stage/StageInput", false, 1)]
    private static void ShowWindow()
    {
        StageInputEditor window = GetWindow<StageInputEditor>();
        window.titleContent = new GUIContent("StageInput");
    }

    protected override void OnGUI()
    {
        // レイアウトを調整するために隙間を作る
        GUILayout.Space(30f);

        // 指定したフォルダーのファイルを全て取得する
        var fileNames = Directory.GetFiles(STAGE_DATA_FOLDE);

        List<string> stageFileList = new List<string>();

        for (int i = 0; i < fileNames.Length; i++)
        {
            // 指定した拡張子のファイルを除外する
            if (!fileNames[i].Contains(EXCLUSION_EXTENSION))
            {
                stageFileList.Add(Path.GetFileName(fileNames[i]));
            }
        }

        // ドロップダウン表示用に配列に変換する
        string[] stageFiles = stageFileList.ToArray();

        _stageIndex = EditorGUILayout.Popup(
            label: new GUIContent("生成ステージ名"),
            selectedIndex: _stageIndex,
            displayedOptions: stageFiles
            );

        base.InputProperty();

        if (GUILayout.Button("ステージを生成"))
        {
            // タイルをすべて削除する
            base._targetTilemap.ClearAllTiles();

            // ファイルから拡張子を取り除く
            string stageName = stageFiles[_stageIndex].Replace(DELETE_EXTENSION, "");

            // CSVファイルを読み込む
            TextAsset inputData = Addressables.LoadAssetAsync<TextAsset>(stageName).WaitForCompletion();
            string textData = inputData.text;

            // 行単位に分割する
            // そのままだと改行も入るため、RemoveEmptyEntriesで削除する
            string[] row = textData.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            // 行数を取得する
            base._verticalMaxSize = row.Length;

            // 列単位に分割する
            string[] columnCheck = row[0].Split(",");
            // 列数を取得する
            base._horizontalMaxSize = columnCheck.Length;

            for (int y = 0; y < base._verticalMaxSize; y++)
            {
                string[] column = row[y].Split(",");

                for (int x = 0; x < base._horizontalMaxSize; x++)
                {
                    SetTile(int.Parse(column[x]), x, y);
                }
            }
        }
    }

    /// <summary>
    /// 指定したタイルマップ座標に対象のタイルを設置する
    /// </summary>
    /// <param name="number"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
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