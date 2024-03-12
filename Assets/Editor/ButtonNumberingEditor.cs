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
                button.GetComponentInChildren<TMP_Text>().SetText(index.ToString());
                index++;
            }
        }
    }
}