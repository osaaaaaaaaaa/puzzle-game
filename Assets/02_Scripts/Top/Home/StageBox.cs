using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageBox : MonoBehaviour
{
    [SerializeField] List<Image> m_medalContainers;
    [SerializeField] List<Sprite> m_texMedals;
    [SerializeField] Sprite m_texMedalContainer;
    [SerializeField] Text m_textStageID;
    [SerializeField] Text m_textClearTime;
    [SerializeField] Text m_textScore;
    [SerializeField] Image m_imgRank;
    [SerializeField] List<Sprite> m_texRanks;
    [SerializeField] Button m_btnRecruiting;
    [SerializeField] Text m_textRecruiting;

    [SerializeField] TopManager managerTop;
    ShowDistressSignalResponse m_distressSignal;

    public void InitStatus(ShowStageResultResponse resultData)
    {
        if(resultData == null)
        {
            resultData = new ShowStageResultResponse
            {
                StageID = TopManager.stageID,
                IsMedal1 = false,
                IsMedal2 = false,
                Time = 0,
                Score = 0
            };
        }

        m_textStageID.text = "�X�e�[�W  " + TopManager.stageID;

        // ���_����UI���X�V����
        if (resultData.IsMedal1) m_medalContainers[0].sprite = m_texMedals[0];
        if (resultData.IsMedal2) m_medalContainers[1].sprite = m_texMedals[1];

        // �N���A�^�C���\�L
        string text = "" + Mathf.Floor(resultData.Time * 100);
        text = text.Length == 3 ? "0" + text : text;
        text = text.Length == 2 ? "00" + text : text;
        text = text.Length == 1 ? "000" + text : text;
        m_textClearTime.text = "�N���A�^�C��   " + text.Insert(2, ":");

        // �X�R�A��\�L
        m_textScore.text = "�X�R�A   " + resultData.Score;

        // �]����\�L
        m_imgRank.color = new Color(1, 1, 1, 1);
        m_imgRank.sprite = TopManager.GetScoreRank(m_texRanks, resultData.Score);

        // ��W�{�^����ҏW (��W�ς̏ꍇ=> text���W�� & �{�^���������Ȃ�����)
        m_distressSignal = NetworkManager.Instance.dSignalList.FirstOrDefault(item => item.StageID == TopManager.stageID);    // ���X�g���猟�����Ď擾
        m_textRecruiting.text = m_distressSignal != null ? "��W��" : "��W����";
        m_btnRecruiting.interactable = m_distressSignal != null ? false : true;

        gameObject.SetActive(true);
    }

    public void OnCloseButton()
    {
        SEManager.Instance.PlayCanselSE();
        m_medalContainers[0].sprite = m_texMedalContainer;
        m_medalContainers[1].sprite = m_texMedalContainer;
        m_textClearTime.text = "�N���A�^�C��";
        m_textScore.text = "�X�R�A";
        m_imgRank.color = new Color(1, 1, 1, 0);
        m_imgRank.sprite = null;
        gameObject.SetActive(false);
    }

    public void OnRecruiting()
    {
        SEManager.Instance.PlayButtonSE();
        // �~��M���o�^����
        StartCoroutine(NetworkManager.Instance.StoreDistressSignal(
            TopManager.stageID,
            result =>
            {
                if (result == null) return;
                m_textRecruiting.text = "��W��";
                m_btnRecruiting.interactable = false;
                m_distressSignal = result;
            }));
    }

    public void OnTransitionButton()
    {
        // ��W���̏ꍇ�̓z�X�g���[�h�A��W���Ă��Ȃ��ꍇ�̓\�����[�h
        var mode = m_distressSignal != null ? TopSceneDirector.PLAYMODE.HOST : TopSceneDirector.PLAYMODE.SOLO;
        int signalID = m_distressSignal != null ? m_distressSignal.SignalID : 0;
        int stageID = m_distressSignal != null ? m_distressSignal.StageID : 0;
        managerTop.OnPlayStageButton(mode, signalID, stageID, false);
    }
}
