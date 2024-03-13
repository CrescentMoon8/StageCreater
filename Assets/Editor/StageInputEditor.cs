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
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;


public class StageInputEditor : StageCreateEditor
{
    // 除外するファイルの拡張子
    private const string EXCLUSION_EXTENSION = ".meta";
    // 削除する拡張子
    private const string DELETE_EXTENSION = ".csv";
    // 検索するフォルダー
    private const string STAGE_DATA_FOLDE = "Assets/StageData";
    // ドロップダウンのインデックス
    private int _stageIndex = 0;

    [Header("生成するステージの縦の大きさ")]
    [SerializeField]
    private int _verticalStageSize = 0;
    [Header("生成するステージの横の大きさ")]
    [SerializeField]
    private int _horizontalStageSize = 0;
    [Header("対象タイル")]
    [SerializeField]
    protected List<int> _createAmountList = new List<int>();
    private ReorderableList _reorderableList = default;

    [Header("ターゲットエリアを描画用のタイルマップにコピーするか")]
    [SerializeField]
    private bool _isTargetAreaCopy = false;
    [Header("描画用のターゲットエリアを持つタイルマップ")]
    [SerializeField]
    private Tilemap _targetAreaTilemap = default;
    [Header("ターゲットエリアのタイル")]
    [SerializeField]
    private TileBase _targetAreaTile = default;

    // ターゲットのタイルをリストから引き出すためのインデックス
    private int _targetTileIndex = 0;

    // ターゲットエリアのタイルの座標を保管するリスト
    private List<Vector3Int> _targetTilePosList = new List<Vector3Int>();

    // タイルをリストから引き出すためのインデックス
    private int _setTileIndex = 0;

    private Vector2 _scrollPos = Vector2.zero;
    private Vector2 _dropDownScrollPos = Vector2.zero;

    private bool _isCreate = false;

    [MenuItem("Stage/StageInput", false, 1)]
    private static void ShowWindow()
    {
        StageInputEditor window = GetWindow<StageInputEditor>();
        window.titleContent = new GUIContent("StageInput");
    }

    protected override void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        // レイアウトを調整するために隙間を作る
        GUILayout.Space(15f);

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
            StageInput(stageFiles);
        }

        // レイアウトを調整するために隙間を作る
        GUILayout.Space(7f);

        InputRandomProperty();

        if (GUILayout.Button("ランダムステージ生成"))
        {
            RandomStageCreate();
        }

        EditorGUILayout.EndScrollView();

        if (!_isTargetAreaCopy && _isCreate)
        {
            _targetAreaTilemap.ClearAllTiles();
        }

        _targetTilePosList.Clear();
    }

    #region ステージを読み込んで生成
    /// <summary>
    /// CSVファイルを読み込んでステージを生成する
    /// </summary>
    /// <param name="stageFiles"></param>
    private void StageInput(string[] stageFiles)
    {
        // タイルをすべて削除する
        base._stageTilemap.ClearAllTiles();

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

        int currentMaxSize = 0;

        for (int i = 0; i < row.Length; i++)
        {
            // 列単位に分割する
            string[] columnCheck = row[0].Split(",");

            if (currentMaxSize < columnCheck.Length)
            {
                currentMaxSize = columnCheck.Length;
            }
        }

        // 列数を取得する
        base._horizontalMaxSize = currentMaxSize;

        for (int y = 0; y < base._verticalMaxSize; y++)
        {
            string[] column = row[y].Split(",");

            for (int x = 0; x < base._horizontalMaxSize; x++)
            {
                // ステージ情報とリストのインデックスを合わせるためにー１する
                // 例：動かせないブロック　ステージ情報→１　インデックス→０
                SetTile(int.Parse(column[x]) - 1, x, y);
            }
        }

        if (_targetAreaTilemap != null)
        {
            _targetAreaTilemap.ClearAllTiles();

            for (int i = 0; i < _targetTilePosList.Count; i++)
            {
                _targetAreaTilemap.SetTile(_targetTilePosList[i], _targetTileList[_targetTileIndex]);
            }
        }

        _isCreate = true;
    }

    /// <summary>
    /// 指定したタイルマップ座標に対象のタイルを設置する
    /// </summary>
    /// <param name="tileIndex">配置するタイルを引き出すためのインデックス</param>
    /// <param name="x">横座標</param>
    /// <param name="y">縦座標</param>
    private void SetTile(int tileIndex, int x, int y)
    {
        // 値がー１以下だったら処理をスキップする
        if (tileIndex < 0)
        {
            return;
        }

        Vector3Int setTilePos = new Vector3Int(x, -y);

        _stageTilemap.SetTile(setTilePos, _targetTileList[tileIndex]);

        if (_targetTileList[tileIndex] == _targetAreaTile)
        {
            _targetTileIndex = tileIndex;

            _targetTilePosList.Add(setTilePos);
        }
    }
    #endregion

    #region ステージのランダム生成
    private void RandomStageCreate()
    {
        // タイルがない座標を保管するリスト
        List<Vector3Int> _emptyTilePosList = new List<Vector3Int>();

        _setTileIndex = 0;

        // タイルをすべて削除する
        base._stageTilemap.ClearAllTiles();

        // 外壁の作成
        for (int y = 0; y < _verticalStageSize; y++)
        {
            for (int x = 0; x < _horizontalStageSize; x++)
            {
                if ((x == 0) || (x == _horizontalStageSize - 1) || (y == 0) || (y == _verticalStageSize - 1))
                {
                    SetTile(_setTileIndex, x, y);
                }
                else
                {
                    AddEmptyList(_emptyTilePosList, x, y);
                }
            }
        }

        for (int i = 0; i < _createAmountList.Count; i++)
        {
            for (int j = 0; j < _createAmountList[i]; j++)
            {
                int index = Random.Range(0, _emptyTilePosList.Count);

                Vector3Int setTilePos = _emptyTilePosList[index];

                _stageTilemap.SetTile(setTilePos, _targetTileList[_setTileIndex]);

                if (_targetTileList[_setTileIndex] == _targetAreaTile)
                {
                    _targetTileIndex = _setTileIndex;

                    _targetTilePosList.Add(setTilePos);
                }

                _emptyTilePosList.RemoveAt(index);
            }

            _setTileIndex++;
        }

        if (_targetAreaTilemap != null)
        {
            _targetAreaTilemap.ClearAllTiles();

            for (int i = 0; i < _targetTilePosList.Count; i++)
            {
                _targetAreaTilemap.SetTile(_targetTilePosList[i], _targetTileList[_targetTileIndex]);
            }
        }

        _isCreate = true;
    }

    /// <summary>
    /// タイルが空の座標をリストに追加する
    /// </summary>
    /// <param name="emptyTilePosList">タイルがない座標を保管するリスト</param>
    /// <param name="x">横座標</param>
    /// <param name="y">縦座標</param>
    private void AddEmptyList(List<Vector3Int> emptyTilePosList, int x, int y)
    {
        Vector3Int setTilePos = new Vector3Int(x, -y);

        emptyTilePosList.Add(setTilePos);
    }

    /// <summary>
    /// ステージのランダム生成に用いるプロパティを表示、反映させる
    /// </summary>
    private void InputRandomProperty()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_verticalStageSize)}"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_horizontalStageSize)}"));

        SerializedProperty list = serializedObject.FindProperty($"{nameof(_createAmountList)}");

        // レイアウトを調整するために隙間を作る
        GUILayout.Space(15f);

        if (_reorderableList == null)
        {
            _reorderableList = new ReorderableList(serializedObject, list);

            // プロパティの高さを指定
            _reorderableList.elementHeight = 30f;

            // タイトル描画時のコールバック
            // 上書きしてEditorGUIを使えばタイトル部分を自由にレイアウトできる
            _reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "タイルの生成量");

            // 要素の描画時のコールバック
            // 上書きしてEditorGUIを使えば自由にレイアウトできる
            _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var elementProperty = list.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, elementProperty, new GUIContent("（値：" + (index + 1) + "）のタイルの生成量"));
            };
        }

        _reorderableList.DoLayoutList();

        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_isTargetAreaCopy)}"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetAreaTilemap)}"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty($"{nameof(_targetAreaTile)}"));

        // 入力された値を反映させる
        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}