// ---------------------------------------------------------
// ButtonNumberingEditor.cs
//
// 作成日:2024/03/12
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[ExecuteAlways]
[CustomEditor(typeof(GridLayoutGroup))]
public class ButtonNumberingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10f);

        if(GUILayout.Button("ボタンテキストのナンバリング"))
        {
            GridLayoutGroup _gridLayoutGroup = target as GridLayoutGroup;

            Transform _buttonParent = _gridLayoutGroup.gameObject.transform;

            int index = 1;

            foreach (Button button in _buttonParent.GetComponentsInChildren<Button>())
            {
                button.gameObject.name = "Button" + index;
                button.GetComponentInChildren<TMP_Text>().SetText(index.ToString());
                index++;
            }

            // 操作しないとシーンの描画がされないため、強制的にシーンの再描画を行う
            SceneView.RepaintAll();
        }
    }
}