using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandAnimController : MonoBehaviour
{
    Animator anim;

    public enum AnimPattern
    {
        SHOT = 1,
        DRUG,
    }

    public AnimPattern animPattern;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        switch (animPattern)
        {
            case AnimPattern.SHOT:
                DoShotAnim();
                break;
            case AnimPattern.DRUG:
                DoDrugAnim();
                break;
        }
    }

    /// <summary>
    /// �V���b�g�A�j���[�V����
    /// </summary>
    void DoShotAnim()
    {
        Vector3[] path =
{
            transform.position,
            new Vector3(-2f,-0.73f,0f),
        };

        //Sequence����
        Sequence sequence = DOTween.Sequence();
        // �A�j���[�V���������Ă���Path�ړ�����
        sequence.Append(transform.GetComponent<SpriteRenderer>().DOFade(1.0f, 0.5f).SetDelay(4f).OnComplete(PlayAnim))
            .Append(transform.DOPath(path, 3f).SetDelay(2f).SetEase(Ease.InOutSine))
            .Append(transform.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).SetDelay(2f));
        sequence.SetLoops(3);
    }

    /// <summary>
    /// �h���b�O�A�j���[�V����
    /// </summary>
    void DoDrugAnim()
    {
        Vector3[] path =
{
            transform.position,
            new Vector3(-7.06f,-2.58f,0f),
            new Vector3(-6.91f,0.23f,0f),
            new Vector3(-1.91f,0.76f,0f)
        };

        //Sequence����
        Sequence sequence = DOTween.Sequence();
        // �A�j���[�V���������Ă���Path�ړ�����
        sequence.Append(transform.GetComponent<SpriteRenderer>().DOFade(1.0f, 0.5f).SetDelay(4f).OnComplete(PlayAnim))
            .Append(transform.DOPath(path, 3f,PathType.CatmullRom).SetDelay(2f).SetEase(Ease.InOutSine))
            .Append(transform.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).SetDelay(2f));
        sequence.SetLoops(3);
    }

    /// <summary>
    /// �A�j���[�V�����Đ�
    /// </summary>
    void PlayAnim()
    {
        anim.Play("Hand_ClickAnimation");
    }
}
