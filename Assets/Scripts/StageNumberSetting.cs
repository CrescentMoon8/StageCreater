// ---------------------------------------------------------
// StageNumberSetting.cs
//
// 作成日:2024/03/12
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageNumberSetting : MonoBehaviour
{
	#region 変数
	[SerializeField]
	private EventSystem eventSystem = default;
	#endregion

	#region プロパティ

	#endregion

	#region メソッド
	public void PushButton()
    {
		// 押されたボタンが何番目のオブジェクトか取得する
		// ０スタートのため＋１する
		int buttonNumber = eventSystem.currentSelectedGameObject.transform.GetSiblingIndex() + 1;

		PlayerPrefs.SetString("StageNumber", buttonNumber.ToString());

		SceneManager.LoadScene("Main");
    }
	#endregion
}