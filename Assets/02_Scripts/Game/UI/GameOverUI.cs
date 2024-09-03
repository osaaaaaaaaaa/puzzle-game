using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject m_BG;
    [SerializeField] List<GameObject> m_textList;
    [SerializeField] List<float> m_movePosY;

    public void PlayAnim(UiController controller, bool isMedal1, bool isMedal2, float time, int score, bool isStageClear)
    {
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(1f)
            .Append(transform.GetComponent<Image>().DOFade(0.5f, 0.5f).SetEase(Ease.OutQuad))
            .Append(m_BG.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutQuad))
            .Join(m_BG.GetComponent<Image>().DOFade(1f, 0.5f).SetEase(Ease.OutQuad))
            .AppendInterval(1f);

        for (int i = 0; i < m_textList.Count; i++)
        {
            sequence.Join(m_textList[i].transform.DOLocalMoveY(m_movePosY[i], 1f + (i % 2) * 0.3f).SetEase(Ease.OutBack))
                .Join(m_textList[i].GetComponent<Text>().DOFade(1, 1f).SetEase(Ease.OutQuad));
        }

        sequence.AppendInterval(2f)
            .OnComplete(() => { controller.SetResultUI(isMedal1, isMedal2, time, score, isStageClear); });

        sequence.Play();
    }
}
