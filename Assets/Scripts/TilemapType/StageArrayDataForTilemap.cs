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
using System.IO;
using System.Text;

public class StageArrayDataForTilemap : MonoBehaviour
{
	#region 変数
	private Tilemap _tilemap = default;
    [SerializeField]
    private List<TileBase> _setTileList = new List<TileBase>();
    [SerializeField]
    private TileBase _playerTile = default;

    [Header("ステージの横の最大サイズ")]
	[SerializeField]
	private int _horizontalMaxSize = default;
	[Header("ステージの縦の最大サイズ")]
	[SerializeField]
	private int _verticalMaxSize = default;

    private string[] _row = default;
    private string _stageName = "Stage";
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
	#endregion

	#region メソッド
	/// <summary>
	/// 配列内の初期化処理
	/// </summary>
	public void Initialize()
	{
		_tilemap = GameObject.FindWithTag("StageMap").GetComponent<Tilemap>();
		// マップの最大サイズを設定する
		SetStageMaxSize();
		// ステージ、ターゲットの配列の大きさを設定する
		StageArray = new int[_verticalMaxSize, _horizontalMaxSize];
		TargetData = new int[_verticalMaxSize, _horizontalMaxSize];
        CreateStage();
		// マップイメージを配列に格納する
		ImageToArray();

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
        _horizontalMaxSize = _tilemap.cellBounds.max.x;
        _verticalMaxSize = -_tilemap.cellBounds.min.y;

        string stageNumber = PlayerPrefs.GetString("StageNumber");

        // CSVファイルを読み込む
        TextAsset inputData = Addressables.LoadAssetAsync<TextAsset>(_stageName + stageNumber).WaitForCompletion();
        string textData = inputData.text;

        // 行単位に分割する
        // そのままだと改行も入るため、RemoveEmptyEntriesで削除する
        _row = textData.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        // 行数を取得する
        _verticalMaxSize = _row.Length;

        int currentMaxSize = 0;

        for (int i = 0; i < _row.Length; i++)
        {
            // 列単位に分割する
            string[] columnCheck = _row[0].Split(",");

            if (currentMaxSize < columnCheck.Length)
            {
                currentMaxSize = columnCheck.Length;
            }
        }

        // 列数を取得する
        _horizontalMaxSize = currentMaxSize;
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

        TargetData = StageArray;
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

                if(_setTileList[k].Equals(_playerTile))
                {
                    // プレイヤーの座標を代入する
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
    public TileBase GetStageObject(int row, int col)
	{
        // rootObject内の全てのオブジェクトを検索する
        foreach (Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
			/*Debug.LogError(_tilemap.GetTile(pos));
			Vector3Int tilePos = pos + Vector3Int.up;

            // オブジェクトの座標が渡された引数と同じだったら
            if (tilePos.x == col && tilePos.y == -row)
            {
                return _tilemap.GetTile(tilePos);
            }*/

			if(pos.x == col && pos.y == -row)
            {
				return _tilemap.GetTile(pos);
            }
        }

        return null;
    }
	#endregion
}