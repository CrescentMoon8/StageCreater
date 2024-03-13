// ---------------------------------------------------------
// StageArrayDataForTilemap.cs
//
// 作成日:2024/02/16
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using TMPro;

public class StageArrayDataForTilemap : MonoBehaviour
{
	#region 変数
	private Tilemap _tilemap = default;
	private Tilemap _targetTilemap = default;
    [SerializeField]
    private List<TileBase> _setTileList = new List<TileBase>();

    [SerializeField]
    private int _targetAreaTileIndex = 3;
    [SerializeField]
    private int _playerTileIndex = 4;

	private int _horizontalMaxSize = default;

	private int _verticalMaxSize = default;

    private string[] _row = default;
    private string _stageName = "Stage";

    // ブロックがターゲットの上にある個数
    private int _targetCount = 0;
    // ターゲットの最大数
    private int _targetMaxCount = 0;

    private Camera _mainCamera = default;
    private Vector3 _defaultPos = new Vector3(0, 1f, -10);
    private const float DIFFICULT_POS = 0.5f;

    [SerializeField]
    private TMP_Text _stageText = default;

    [SerializeField]
    private bool _isPlayDebug = false;
    [SerializeField]
    private bool _isLoadDebug = false;
    [SerializeField]
    private int _testStageNumber = 1;

    #endregion

    #region プロパティ
    public Tilemap GettingTileMap { get { return _tilemap; } }
	// プレイヤーの座標
	public Vector2Int PlayerPosition { get; set; }
	// ステージの情報を入れるための配列
	public int[,] StageArray { get; set; }
	// ターゲットの情報を入れるための配列
	public int[,] TargetData { get; set; }
	// ステージの横の最大サイズ
	public int HorizontalMaxSize { get { return _horizontalMaxSize; } }
	// ステージの縦の最大サイズ
	public int VerticalMaxSize { get { return _verticalMaxSize; } }
    public int TargetCount { get { return _targetCount; } set { _targetCount = value; } }
	#endregion

	#region メソッド
	/// <summary>
	/// 配列内の初期化処理
	/// </summary>
	public void Initialize()
	{
		_tilemap = GameObject.FindWithTag("StageMap").GetComponent<Tilemap>();
		_targetTilemap = GameObject.FindWithTag("TargetMap").GetComponent<Tilemap>();

        if(!_isPlayDebug)
        {
            _targetTilemap.ClearAllTiles();
        }

        // マップの最大サイズを設定する
        SetStageMaxSize();
		// ステージ、ターゲットの配列の大きさを設定する
		StageArray = new int[_verticalMaxSize, _horizontalMaxSize];
		TargetData = new int[_verticalMaxSize, _horizontalMaxSize];
        if(!_isPlayDebug)
        {
            CreateStage();
        }
		// マップイメージを配列に格納する
		ImageToArray();

        _mainCamera = Camera.main;
        _mainCamera.transform.position = _defaultPos + new Vector3(_horizontalMaxSize * DIFFICULT_POS, -_verticalMaxSize * DIFFICULT_POS, 0);
	}

	/// <summary>
	/// 配列内を確認するための出力処理
	/// </summary>
	private void Update()
	{
		//テスト用
		if (Input.GetKeyDown(KeyCode.H))
		{
			//配列を出力する
			print("Field--------------------------------------------");
			for (int y = 0; y < _verticalMaxSize; y++)
			{
				string outPutString = "";
				for (int x = 0; x < _horizontalMaxSize; x++)
				{
					outPutString += StageArray[y, x];
				}
				print(outPutString);
			}
			print("Field--------------------------------------------");
		}
	}

    /// <summary>
    /// マップの最大サイズを設定する
    /// </summary>
    private void SetStageMaxSize()
    {
        if(_isPlayDebug)
        {
            _horizontalMaxSize = _tilemap.cellBounds.max.x + (-_tilemap.cellBounds.min.x);
            _verticalMaxSize = _tilemap.cellBounds.max.y + (-_tilemap.cellBounds.min.y);
        }
        else
        {
            if (_isLoadDebug)
            {
                _stageName = "Test" + _stageName;
            }
            else
            {
                string stageNumber = PlayerPrefs.GetString("StageNumber");

                _stageName += stageNumber;

                _stageText.SetText(stageNumber);
            }

            // CSVファイルを読み込む
            TextAsset inputData = Addressables.LoadAssetAsync<TextAsset>(_stageName).WaitForCompletion();
            string textData = inputData.text;

            // 行単位に分割する
            // そのままだと改行も入るため、RemoveEmptyEntriesで削除する
            _row = textData.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            // 行数を取得する
            _verticalMaxSize = _row.Length;

            int currentHorizontalMaxSize = 0;

            // 列の大きさを比較し、一番大きい列を見つける
            for (int i = 0; i < _row.Length; i++)
            {
                // 列単位に分割する
                string[] columnCheck = _row[0].Split(",");

                if (currentHorizontalMaxSize < columnCheck.Length)
                {
                    currentHorizontalMaxSize = columnCheck.Length;
                }
            }

            // 列数を取得する
            _horizontalMaxSize = currentHorizontalMaxSize;
        }
    }

	private void CreateStage()
    {
        // タイルをすべて削除する
        _tilemap.ClearAllTiles();

        for (int y = 0; y < _verticalMaxSize; y++)
        {
            string[] column = _row[y].Split(",");

            for (int x = 0; x < _horizontalMaxSize; x++)
            {
                // ステージ情報とリストのインデックスを合わせるためにー１する
                // 例：動かせないブロック　ステージ情報→１　インデックス→０
                SetTile(int.Parse(column[x]) - 1, x, y);
            }
        }
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

        _tilemap.SetTile(setTilePos, _setTileList[tileIndex]);

        if (tileIndex + 1 == _targetAreaTileIndex)
        {
            _targetTilemap.SetTile(setTilePos, _setTileList[tileIndex]);
        }
    }

    private void ImageToArray()
    {
        for (int i = 0; i < _verticalMaxSize; i++)
        {
            for (int j = 0; j < _horizontalMaxSize; j++)
            {
                // ワールド座標とタイルマップ座標のずれをなくすため＋１する
                // 座標と配列番号を合わせるためにマイナスをつける
                Vector3Int searchPos = new Vector3Int(j, -i);

                // 指定した座標にタイルがなければ処理をスキップする
                if (!_tilemap.HasTile(searchPos))
                {
                    continue;
                }

                TileCheck(i, j, searchPos);
            }
        }

        TargetData = (int[,])StageArray.Clone();
    }

    private void TileCheck(int i, int j, Vector3Int searchPos)
    {
        for (int k = 0; k < _setTileList.Count; k++)
        {
            // 指定した座標のタイルによって配列情報をセットする
            if (_tilemap.GetTile(searchPos).Equals(_setTileList[k]))
            {
                // タイルが空の部分を０にするために＋１する
                StageArray[i, j] = k + 1;

                if (k + 1 == _targetAreaTileIndex)
                {
                    _targetMaxCount++;
                }
                else if (k + 1 == _playerTileIndex)
                {
                    PlayerPosition = new Vector2Int(i, j);
                }
            }
        }
    }

    /// <summary>
    /// ステージにあるオブジェクトを取得する
    /// </summary>
    /// <param name="tagId">取得するオブジェクトを指定</param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public TileBase GetStageTile(int row, int col)
	{
        // rootObject内の全てのオブジェクトを検索する
        foreach (Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
			if(pos.x == col && pos.y == -row)
            {
				return _tilemap.GetTile(pos);
            }
        }

        return null;
    }

    /// <summary>
	/// ブロックがターゲットに乗っているかの判定を行う
	/// </summary>
	/// <returns>ブロックがそろっているかの有無</returns>
	public bool OnBlockAllTargetCheck()
    {
        // ターゲットクリア数とターゲットの最大数が一致しているか
        if (_targetCount == _targetMaxCount)
        {
            // ターゲットクリア数を初期化する
            _targetCount = 0;
            return true;
        }
        return false;
    }
    #endregion
}