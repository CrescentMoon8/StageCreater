// ---------------------------------------------------------
// StageCreateEditor.cs
//
// 作成日:2024/03/09
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public abstract class StageCreateEditor : EditorWindow
{
    [SerializeField]
    [Header("対象のタイルマップ")]
    protected Tilemap _stageTilemap = default;

    protected SerializedObject serializedObject = default;

    [Header("対象タイル")]
    [SerializeField]
    protected List<TileBase> _targetTileList = new List<TileBase>();
    private ReorderableList _baseReorderableList = default;

    // ステージの横の最大サイズ
    protected int _horizontalMaxSize = default;
    // ステージの縦の最大サイズ
    protected int _verticalMaxSize = default;

    protected int[,] _stageArray = default;

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
    }

    protected abstract void OnGUI();

    /// <summary>
    /// インスペクターに変数を表示し、入力された値を反映させる
    /// </summary>
    protected void InputProperty()
    {
        // 対象のタイルマップをアタッチできるようにウィンドウに表示する
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_stageTilemap)}"));

        SerializedProperty list = serializedObject.FindProperty($"{nameof(_targetTileList)}");

        // レイアウトを調整するために隙間を作る
        GUILayout.Space(15f);

        if (_baseReorderableList == null)
        {
            _baseReorderableList = new ReorderableList(serializedObject, list);

            // プロパティの高さを指定
            _baseReorderableList.elementHeight = 30f;

            // タイトル描画時のコールバック
            // 上書きしてEditorGUIを使えばタイトル部分を自由にレイアウトできる
            _baseReorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "対象タイル");

            // 要素の描画時のコールバック
            // 上書きしてEditorGUIを使えば自由にレイアウトできる
            _baseReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var elementProperty = list.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, elementProperty, new GUIContent("対象タイル" + (index + 1) + "（値：" + (index + 1) + "）"));
            };
        }

        _baseReorderableList.DoLayoutList();

        // 入力された値を反映させる
        serializedObject.ApplyModifiedProperties();
    }
}