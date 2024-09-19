using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskBar : MonoBehaviour
{
    [SerializeField] Text m_textAchievedText;             // 達成条件
    [SerializeField] Text m_textProgressValue;            // 達成状況値
    [SerializeField] Slider m_sliderAchievement;          // 達成状況を表すメーター
    [SerializeField] Text m_textReaardPoint;              // 獲得可能ポイント

    public void UpdateTask(string achievedText, float achievedValue, float progressVal, int amountItem, bool isReceived)
    {
        m_textAchievedText.text = achievedText;
        m_textProgressValue.text = "<size=30>" + progressVal + "</size><size=25>/" + achievedValue + "</size>";
        m_sliderAchievement.value = progressVal == 0 ? 0 : progressVal / achievedValue;   // スライダー限界値が0~1になっている
        if(isReceived)
        {
            transform.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
            m_textReaardPoint.text = "達成";
            m_textReaardPoint.color = new Color(1f, 0f, 0.3f, 1f);
        }
        else
        {
            m_textReaardPoint.text = amountItem + "pt";
        }
    }
}
