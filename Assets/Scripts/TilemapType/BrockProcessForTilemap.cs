// ---------------------------------------------------------
// BlockProcess.cs
//
// 作成日:2023/10/24
// 改修開始日:2024/02/27
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// ブロックの生成、削除及びその判定を行うクラス
/// </summary>
public class BlockProcessForTilemap : MonoBehaviour
{
	#region 変数
	#region オブジェクト
	[SerializeField]
	private List<TileBase> _setTileList = new List<TileBase>();
	#endregion

	#region クラス
	// 各クラスの定義
	private StageArrayDataForTilemap _stageArrayDataForTilemap = default;
	#endregion
	#endregion

	#region メソッド
	/// <summary>
	/// ゲーム開始時にステージを構成する
	/// ゲーム途中でブロックを追加する
	/// </summary>
	public void CreateStage()
	{
		
	}
	#endregion
}
