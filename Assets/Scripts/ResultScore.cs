// ---------------------------------------------------------
// ResultScore.cs
//
// 作成日:2023/11/02
// 改修開始日:2024/02/27
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// リザルト画面でスコアを表示する
/// </summary>
public class ResultScore : MonoBehaviour
{
    #region 変数
    #region 入力
    // ステージセレクトシーンへ移動するための入力
    private string _stageSelectInput = "Select";
	// 次のステージシーンへ移動するための入力
	private string _nextStageInput = "NextStage";
	// タイトルへ移動するための入力
	private string _goTitleInput = "GoTitle";
    #endregion

    #region UI
    //スコアを表示するためのテキスト
    [SerializeField] 
	private TMPro.TMP_Text _scoreText = default;
    #endregion

    #region タグ
    //スコアを表示するためのテキストのタグ
    private string _scoreTextTag = "ScoreText";
    #endregion

    #region スコア
    // スコアの保存名称
    private string _score = "Score";
	// ハイスコアの保存名称
	private string _highScore = "HighScore";
    #endregion

    #region シーン名
    // タイトルシーン
    private string _title = "Title";
	// ステージセレクトシーン
	private string _select = "Select";
	// ゲームシーン
	private string _main = "Main";
	#endregion

	#endregion

	#region メソッド
	/// <summary>
	/// テキストの初期化処理
	/// </summary>
	private void Awake()
	{
		
	}

	/// <summary>
	/// テキストの更新処理、入力判定
	/// </summary>
	private void Update ()
	{
		//Nキー、Yボタンが押されたら
		if (Input.GetButtonDown(_nextStageInput))
		{
			PlayerPrefs.SetString("StageNumber", (int.Parse(PlayerPrefs.GetString("StageNumber")) + 1).ToString());
			//タイトル画面へ移動する
			SceneManager.LoadScene(_main);
		}

		//Tキー、Bボタンが押されたら
		if (Input.GetButtonDown(_goTitleInput))
        {
			//タイトル画面へ移動する
			SceneManager.LoadScene(_title);
        }

		//Rキー、Aボタンが押されたら
		if (Input.GetButtonDown(_stageSelectInput))
		{
			//ゲーム画面へ移動する
			SceneManager.LoadScene(_select);
		}
	}
	#endregion
}