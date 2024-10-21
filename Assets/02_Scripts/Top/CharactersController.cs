using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharactersController : MonoBehaviour
{
    [SerializeField] GameObject m_mom;
    [SerializeField] GameObject m_grandfather;
    [SerializeField] GameObject m_son;
    [SerializeField] GameObject m_sonRun;
    [SerializeField] GameObject m_sonCow;
    [SerializeField] GameObject m_dog;
    [SerializeField] GameObject m_horse_blackHole;

    private void Start()
    {
        var stageID = NetworkManager.Instance.StageID;

        m_mom.SetActive(true);
        m_mom.GetComponent<Animator>().Play("Idle01");
        m_son.SetActive(true);

        if (stageID >= 11)
        {
            m_dog.SetActive(true);
            m_sonRun.SetActive(true);
            m_sonRun.GetComponent<Animator>().Play("IdleAnimation");
        }
        if (stageID >= 15)
        {
            m_grandfather.SetActive(true);
        }
        if (stageID >= 20)
        {
            m_sonCow.SetActive(true);
            PlaySonCowAnim();
        }
        if (stageID >= 26)
        {
            m_horse_blackHole.SetActive(true);
        }

    }

    void PlaySonCowAnim()
    {
        m_sonCow.GetComponent<Animator>().Play("TopWalkAnimation");

        float time = 20;

        var sequence = DOTween.Sequence();
        sequence.Append(m_sonCow.transform.DOMoveX(14.89f, time).SetEase(Ease.Linear)
            .OnComplete(() => { 
                m_sonCow.GetComponent<Animator>().Play("IdleAnimation");
                Invoke("InvertCow", 2f);
            }))
            .AppendInterval(2f)
            .Append(m_sonCow.transform.DOMoveX(41.76f, time).SetEase(Ease.Linear).OnComplete(() => { m_sonCow.transform.localScale = new Vector3(-1.3f, 1.3f, 1f); }));
        sequence.SetLoops(-1,LoopType.Restart);
        sequence.Play();
    }

    void InvertCow()
    {
        m_sonCow.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
        m_sonCow.GetComponent<Animator>().Play("TopWalkAnimation");
    }
}
