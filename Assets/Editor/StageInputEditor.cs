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
    private const string STAGE_DATA_FOLDE = "Assets/StageData";
    private List<string> _stageNameList = new List<string>();
    private int _stageIndex = 0;

    [MenuItem("Stage/StageInput", false, 1)]
    private static void ShowWindow()
    {
        StageInputEditor window = GetWindow<StageInputEditor>();
        window.titleContent = new GUIContent("StageInput");
    }

    protected override void OnGUI()
    {
        base.InputProperty();

        var fileNames = Directory.GetFiles(STAGE_DATA_FOLDE);

        /*for (int i = 0; i < fileNames.Length; i++)
        {
            Debug.Log(fileNames[i]);
        }*/

        string[] stageNames = _stageNameList.ToArray();

        _stageIndex = EditorGUILayout.Popup(
            label: new GUIContent("生成ステージ名"),
            selectedIndex: _stageIndex,
            displayedOptions: stageNames
            );

        if (GUILayout.Button("ステージを生成"))
        {
            // タイルをすべて削除する
            base._targetTilemap.ClearAllTiles();

            TextAsset inputData = Addressables.LoadAssetAsync<TextAsset>(stageNames[_stageIndex]).WaitForCompletion();
            string textData =inputData.text;

            string[] data = textData.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            base._verticalMaxSize = data.Length;

            string[] lineCheck = data[0].Split(",", System.StringSplitOptions.RemoveEmptyEntries);
            base._horizontalMaxSize = lineCheck.Length;

            for (int i = 0; i < base._verticalMaxSize; i++)
            {
                string[] line = data[i].Split(",", System.StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < base._horizontalMaxSize; j++)
                {
                    SetTile(int.Parse(line[j]), j, i);
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