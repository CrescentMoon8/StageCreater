// ---------------------------------------------------------
// StageCreateEditor.cs
//
// 作成日:2024/03/05
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageArrayDataForTilemap))]
public class StageCreateEditor : Editor
{
    [Tooltip("（必ずナンバリングすること）例：Stage1")]
    private string _stageName = default;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _stageName = EditorGUILayout.TextField("ステージ名", _stageName);

        StageArrayDataForTilemap stageArrayData = target as StageArrayDataForTilemap;

        if(GUILayout.Button("ステージ情報を出力"))
        {
            stageArrayData.Initialize();

            Debug.Log(_stageName);

            //配列を出力する
            Debug.Log("Field--------------------------------------------");
            for (int y = 0; y < stageArrayData.VerticalMaxSize; y++)
            {
                string outPutString = "";
                for (int x = 0; x < stageArrayData.HorizontalMaxSize; x++)
                {
                    outPutString += stageArrayData.StageArray[y, x];
                }
                Debug.Log(outPutString);
            }
            Debug.Log("Field--------------------------------------------");
        }
    }
}