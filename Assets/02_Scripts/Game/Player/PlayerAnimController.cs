using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    [SerializeField] GameObject m_normalSkinMom;    // 母親のデフォルトスキン
    [SerializeField] GameObject m_abnormalSkinMom;  // 母親の蹴るときのスキン
    [SerializeField] GameObject m_normalSkinGrandpa;    // 祖父のデフォルトスキン
    [SerializeField] GameObject m_abnormalSkinGrandpa;  // 祖父の打つときのスキン

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
        PlayStandbyAnim();
    }

    /// <summary>
    /// 通常スキンのIdleアニメーションを再生する
    /// </summary>
    public void PlayStandbyAnim()
    {
        // テクスチャを切り替えてアニメーション再生
        if (TopManager.isUseItem)
        {
            // アイテムを使用している場合は祖父を再生
            ToggleSkinVisivility(m_normalSkinGrandpa);
            m_normalSkinGrandpa.GetComponent<Animator>().Play("IdleNormalAnim");
        }
        else
        {
            // 母親を再生
            ToggleSkinVisivility(m_normalSkinMom);
            m_normalSkinMom.GetComponent<Animator>().Play(m_standbyAnimName);
        }
    }

    /// <summary>
    /// アブノーマルスキンのIdleアニメーションを再生する
    /// </summary>
    public void PlayIdleAnim()
    {
        // テクスチャを切り替えてアニメーション再生
        if (TopManager.isUseItem)
        {
            // アイテムを使用している場合は祖父を再生
            ToggleSkinVisivility(m_abnormalSkinGrandpa);
            m_abnormalSkinGrandpa.GetComponent<Animator>().Play("IdleAbnormalAnim");
        }
        else
        {
            // 母親を再生
            ToggleSkinVisivility(m_abnormalSkinMom);
            m_abnormalSkinMom.GetComponent<Animator>().Play("Idle");
        }
    }

    /// <summary>
    /// 蹴る姿勢のアニメーションを再生する
    /// </summary>
    public void PlayReadyAnim()
    {
        // テクスチャを切り替える
        if (TopManager.isUseItem)
        {
            // アイテムを使用している場合は祖父を再生
            ToggleSkinVisivility(m_abnormalSkinGrandpa);
            m_abnormalSkinGrandpa.GetComponent<Animator>().Play("IdleAbnormalAnim");
        }
        else
        {
            // 母親の再生するアニメーション指定
            ToggleSkinVisivility(m_abnormalSkinMom);
            m_readyAnimNum = (int)Random.Range(1, 11);
            string animName = m_readyAnimNum >= m_readyAnimMax ? "Ready02" : "Ready01";

            // アニメーションを再生する
            m_abnormalSkinMom.GetComponent<Animator>().Play(animName);
        }
    }

    /// <summary>
    /// 蹴るアニメーションを再生する
    /// </summary>
    public void PlayKickAnim()
    {
        // テクスチャを切り替える
        if (TopManager.isUseItem)
        {
            // アイテムを使用している場合は祖父を再生
            ToggleSkinVisivility(m_abnormalSkinGrandpa);
            m_abnormalSkinGrandpa.GetComponent<Animator>().Play("ShotAnim");
        }
        else
        {
            // 母親の再生するアニメーション指定
            ToggleSkinVisivility(m_abnormalSkinMom);
            string animName = m_readyAnimNum >= m_readyAnimMax ? "kick02" : "kick01";

            // アニメーションを再生する
            m_abnormalSkinMom.GetComponent<Animator>().Play(animName);
        }
    }

    /// <summary>
    /// スキンを表示・非表示にする
    /// </summary>
    /// <param name="showSkin">指定したスキンだけを表示する</param>
    public void ToggleSkinVisivility(GameObject showSkin)
    {
        m_normalSkinMom.SetActive(showSkin == m_normalSkinMom);
        m_abnormalSkinMom.SetActive(showSkin == m_abnormalSkinMom);
        m_normalSkinGrandpa.SetActive(showSkin == m_normalSkinGrandpa);
        m_abnormalSkinGrandpa.SetActive(showSkin == m_abnormalSkinGrandpa);
    }
}
