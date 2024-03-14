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
public class ResultScene : MonoBehaviour
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

    #region シーン名
    // タイトルシーン
    private string _title = "Title";
	// ステージセレクトシーン
	private string _select = "Select";
	// ゲームシーン
	private string _main = "Main";
	#endregion

	#region ラストステージ
	private const int LAST_STAGE_NUMBER = 100;
	private bool _isLastStage = false;
    #endregion

    #region イメージ
    [SerializeField]
	private GameObject _nomalResultPanel = default;
	[SerializeField]
	private GameObject _lastResultPanel = default;
    #endregion

    #endregion

    #region メソッド
    private void Awake()
    {
		if (int.Parse(PlayerPrefs.GetString("StageNumber")) >= LAST_STAGE_NUMBER)
        {
			_isLastStage = true;
        }
		else
        {
			_isLastStage = false;
        }

		if(_isLastStage)
        {
			_lastResultPanel.SetActive(true);
			_nomalResultPanel.SetActive(false);
        }
		else
        {
			_nomalResultPanel.SetActive(true);
			_lastResultPanel.SetActive(false);
		}
    }
    /// <summary>
    /// テキストの更新処理、入力判定
    /// </summary>
    private void Update ()
	{
		ButtonColorManager._clearStageNumberList.Add(int.Parse(PlayerPrefs.GetString("StageNumber")));

		//Nキー、Xボタンが押されたら
		if (Input.GetButtonDown(_nextStageInput) && !_isLastStage)
		{
			// クリアしたステージの番号＋１を格納する
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

		//Sキー、Aボタンが押されたら
		if (Input.GetButtonDown(_stageSelectInput))
		{
			//ゲーム画面へ移動する
			SceneManager.LoadScene(_select);
		}
	}
	#endregion
}