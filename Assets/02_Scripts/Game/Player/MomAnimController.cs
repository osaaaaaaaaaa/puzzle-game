using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomAnimController : MonoBehaviour
{
    [SerializeField] GameObject m_normalSkinMom;    // デフォルトスキン
    [SerializeField] GameObject m_abnormalSkinMom;  // 蹴るときのスキン

    // 待機アニメーションの名前
    string m_standbyAnimName;
    // 別のアニメーションが再生される最低値
    const int m_readyAnimMax =  6;
    // 蹴るアニメーションの乱数
    int m_readyAnimNum;

    public enum DEFAULT_STANDBYANIM
    {
        IDLE01 = 1,
        IDLE02
    }

    public DEFAULT_STANDBYANIM m_default_animpattern;

    // Start is called before the first frame update
    void Start()
    {
        // 待機状態のアニメーション再生
        switch (m_default_animpattern)
        {
            case DEFAULT_STANDBYANIM.IDLE01:
                m_standbyAnimName = "Idle01";
                break;
            case DEFAULT_STANDBYANIM.IDLE02:
                m_standbyAnimName = "Idle02";
                break;
        }

        // 待機アニメーションを再生する
        m_normalSkinMom.GetComponent<Animator>().Play(m_standbyAnimName);
    }

    /// <summary>
    /// 通常スキンのIdleアニメーションを再生する
    /// </summary>
    public void PlayStandbyAnim()
    {
        // テクスチャを切り替える
        m_normalSkinMom.SetActive(true);
        m_abnormalSkinMom.SetActive(false);

        // アニメーションを再生する
        m_normalSkinMom.GetComponent<Animator>().Play(m_standbyAnimName);
    }

    /// <summary>
    /// アブノーマルスキンのIdleアニメーションを再生する
    /// </summary>
    public void PlayIdleAnim()
    {
        // テクスチャを切り替える
        m_normalSkinMom.SetActive(false);
        m_abnormalSkinMom.SetActive(true);

        // アニメーションを再生する
        m_abnormalSkinMom.GetComponent<Animator>().Play("Idle");
    }

    /// <summary>
    /// 蹴る姿勢のアニメーションを再生する
    /// </summary>
    public void PlayReadyAnim()
    {
        // テクスチャを切り替える
        m_normalSkinMom.SetActive(false);
        m_abnormalSkinMom.SetActive(true);

        // アニメーション指定
        m_readyAnimNum = (int)Random.Range(1, 11);
        string animName = m_readyAnimNum >= m_readyAnimMax ? "Ready02" : "Ready01";

        // アニメーションを再生する
        m_abnormalSkinMom.GetComponent<Animator>().Play(animName);
    }

    /// <summary>
    /// 蹴るアニメーションを再生する
    /// </summary>
    public void PlayKickAnim()
    {
        // テクスチャを切り替える
        m_normalSkinMom.SetActive(false);
        m_abnormalSkinMom.SetActive(true);

        // アニメーション指定
        string animName = m_readyAnimNum >= m_readyAnimMax ? "kick02" : "kick01";

        // アニメーションを再生する
        m_abnormalSkinMom.GetComponent<Animator>().Play(animName);
    }
}
