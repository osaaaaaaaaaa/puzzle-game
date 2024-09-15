using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskBar : MonoBehaviour
{
    [SerializeField] Text m_textAchievedText;             // �B������
    [SerializeField] Text m_textProgressValue;            // �B���󋵒l
    [SerializeField] Slider m_sliderAchievement;          // �B���󋵂�\�����[�^�[
    [SerializeField] Text m_textReaardPoint;              // �l���\�|�C���g

    public void UpdateTask(string achievedText, float achievedValue, float progressVal, int amountItem, bool isReceived)
    {
        m_textAchievedText.text = achievedText;
        m_textProgressValue.text = "<size=30>" + progressVal + "</size><size=25>/" + achievedValue + "</size>";
        m_sliderAchievement.value = progressVal == 0 ? 0 : progressVal / achievedValue;   // �X���C�_�[���E�l��0~1�ɂȂ��Ă���
        if(isReceived)
        {
            transform.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
            m_textReaardPoint.text = "�B��";
            m_textReaardPoint.color = new Color(1f, 0f, 0.3f, 1f);
        }
        else
        {
            m_textReaardPoint.text = amountItem + "pt";
        }
    }
}
