// ---------------------------------------------------------
// StageNumberSetting.cs
//
// 作成日:2024/03/12
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class StageNumberSetting : MonoBehaviour
{
	#region 変数
	[SerializeField]
	private EventSystem eventSystem = default;

    private Color32 _clearColor = new Color32(0, 125, 8, 255);
    #endregion

    #region メソッド
    private void Awake()
    {
        List<int> stageNumberList = ButtonColorManager._clearStageNumberList;

        // クリアされてれば１、されてなければ０
        if(stageNumberList.Count > 0)
        {
            eventSystem.SetSelectedGameObject(this.transform.GetChild(stageNumberList[stageNumberList.Count - 1] - 1).gameObject);

            for (int i = 0; i < stageNumberList.Count; i++)
            {
                Button button = this.transform.GetChild(stageNumberList[i] - 1).GetComponent<Button>();
                ColorBlock colorBlock = button.colors;

                colorBlock.normalColor = _clearColor;

                button.colors = colorBlock;

                ButtonColorManager._clearStageNumberList.RemoveAt(i);
            }
        }
        else
        {
            eventSystem.SetSelectedGameObject(this.transform.GetChild(0).gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {

        }
    }

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