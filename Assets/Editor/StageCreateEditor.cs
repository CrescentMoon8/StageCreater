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
    protected Tilemap _targetTilemap = default;

    protected SerializedObject serializedObject = default;

    [Header("対象タイル１（値：１）")]
    [SerializeField]
    protected TileBase _targetTile1 = default;
    [Header("対象タイル２（値：２）")]
    [SerializeField]
    protected TileBase _targetTile2 = default;
    [Header("対象タイル３（値：３）")]
    [SerializeField]
    protected TileBase _targetTile3 = default;
    [Header("対象タイル４（値：４）")]
    [SerializeField]
    protected TileBase _targetTile4 = default;

    [Header("対象タイル")]
    [SerializeField]
    protected List<TileBase> _targetTileList = new List<TileBase>();
    private ReorderableList _reorderableList = default;

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
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTilemap)}"));

        // 対象のタイルをアタッチできるようにウィンドウに表示する
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTile1)}"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTile2)}"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTile3)}"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetTile4)}"));

        SerializedProperty list = serializedObject.FindProperty($"{nameof(_targetTileList)}");

        if(_reorderableList == null)
        {
            _reorderableList = new ReorderableList(serializedObject, list);

            // タイトル描画時のコールバック
            // 上書きしてEditorGUIを使えばタイトル部分を自由にレイアウトできる
            _reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "対象タイル");

            // 要素の描画時のコールバック
            // 上書きしてEditorGUIを使えば自由にレイアウトできる
            _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var elementProperty = list.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, elementProperty, new GUIContent("対象タイル" + (index + 1) + "（値：" + (index + 1) + "）"));
            };
        }

        _reorderableList.DoLayoutList();

        // 入力された値を反映させる
        serializedObject.ApplyModifiedProperties();
    }
}