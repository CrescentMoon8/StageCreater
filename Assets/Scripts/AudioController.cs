// ---------------------------------------------------------
// AudioController.cs
//
// 作成日:2023/11/04
// 改修開始日:2024/02/27
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;

/// <summary>
/// SEを鳴らすためのクラス
/// </summary>
public class AudioController : MonoBehaviour
{
	#region 変数
	//ステージクリア時のSE
	[SerializeField] 
	private AudioClip _gameClearSe = default;

	private string _seObjectTag = "SE";

	private AudioSource _audioSource = default;
	#endregion

	#region プロパティ

	#endregion

	#region メソッド
	/// <summary>
	/// 各クラスの初期化処理
	/// </summary>
	private void Awake()
	{
		// 各クラスの初期化
		_audioSource = GameObject.FindWithTag(_seObjectTag).GetComponent<AudioSource>();
	}
	/// <summary>
	/// ゲームレベルが上がった時のSEを再生する
	/// </summary>
	public void GameClearSe()
	{
		// ゲームレベルが上がった時のSEを再生する
		_audioSource.PlayOneShot(_gameClearSe);
	}
	#endregion
}