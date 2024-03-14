// ---------------------------------------------------------
// StartGame.cs
//
// 作成日:2023/10/31
// 改修開始日:2024/02/27
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// ゲームの開始、終了を行う
/// </summary>
public class StartGame : MonoBehaviour
{
    #region 変数
    #region 入力
    // ステージセレクトへ移動するための入力
    private string _playGuidInput = "OpenPlayGuid";
    // ステージセレクトへ移動するための入力
    private string _stageSelectInput = "Select";
	// ゲームを終了するための入力
	private string _quitInput = "Quit";
    #endregion

    #region シーン名
    // 遊び方のシーン名
    private string _stageSlect = "Select";
    #endregion

    #region 遊び方
    [SerializeField]
    private GameObject _playGuidPanel = default;

    private bool _isActive = false;
    #endregion
    #endregion

    #region メソッド

    /// <summary>
    /// 入力処理
    /// </summary>
    private void Update ()
	{
		// Sキー、Aボタンが押されたら
		if(Input.GetButtonDown(_stageSelectInput))
        {
			// 遊び方を表示する画面へ移動する
			SceneManager.LoadScene(_stageSlect);
        }

        // Gキー、Xボタンが押されたら
        if (Input.GetButtonDown(_playGuidInput))
        {
            // 遊び方を表示する
            if(!_isActive)
            {
                _playGuidPanel.SetActive(true);
                _isActive = true;
            }
            else
            {
                _playGuidPanel.SetActive(false);
                _isActive = false;
            }
        }

        // Eキー、Bボタンが押されたら
        if (Input.GetButtonDown(_quitInput))
        {
			// ゲームを終了する
			Application.Quit();
        }
	}
	#endregion
}