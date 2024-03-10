// ---------------------------------------------------------
// StageCreateEditor.cs
//
// 作成日:2024/03/09
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

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

        // 入力された値を反映させる
        serializedObject.ApplyModifiedProperties();
    }
}