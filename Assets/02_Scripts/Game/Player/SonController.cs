using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonController : MonoBehaviour
{
    [SerializeField] GameObject m_son;
    [SerializeField] GameObject m_son_run;
    [SerializeField] GameObject m_ride_cow;

    public GameObject Son { get { return m_son; } }

    Vector3 m_offsetRun;    // 走るテクスチャのオフセット

    /// <summary>
    /// ゲーム開始時の息子のテクスチャ設定
    /// </summary>
    public enum STARTSON_TEXTURE
    {
        SON = 0,
        SON_COW
    }

    public STARTSON_TEXTURE m_startSonTex = STARTSON_TEXTURE.SON;

    private void Start()
    {
        m_offsetRun = m_son_run.transform.position - m_son.transform.position;
    }

    /// <summary>
    /// 元のテクスチャに戻す
    /// </summary>
    public void ChangeDefaultTexture()
    {
        if (m_son_run.activeSelf)
        {
            m_son.transform.position = m_son_run.transform.position;
        }

        // テクスチャを切り替える
        m_son.SetActive(true);
        m_son_run.SetActive(false);
        m_ride_cow.SetActive(false);
    }

    /// <summary>
    /// 走るテクスチャに切り替える
    /// </summary>
    public void ChangeRunTexture(int direction,float posY)
    {
        // 通常のテクスチャの息子がアクティブ状態の場合
        if (m_son.activeSelf)
        {
            // 座標を修正
            m_son_run.transform.position = new Vector3(m_son.transform.position.x, posY - 0.8f, m_son.transform.position.z);    // -0.8fは犬を親オブジェクトにしたときのローカル座標
        }
        // 走る方向
        m_son_run.GetComponent<SonRun>().m_direction = direction;

        // テクスチャを切り替える
        m_son_run.SetActive(true);
        m_son.SetActive(false);
        m_ride_cow.SetActive(false);
    }

    /// <summary>
    /// 牛のテクスチャに切り替える
    /// </summary>
    public void ChangeCowTexture(int direction, Vector3 cowPos)
    {
        // 牛のパラメータ設定
        m_ride_cow.GetComponent<SonCow>().SetCowParam(direction,cowPos);

        // テクスチャを切り替える
        m_son_run.SetActive(false);
        m_son.SetActive(false);
        m_ride_cow.SetActive(true);
        m_ride_cow.GetComponent<SonCow>().InitState();
    }

    /// <summary>
    /// リセット処理
    /// </summary>
    public void ResetSon()
    {
        if (m_ride_cow.activeSelf)
        {
            m_son_run.SetActive(false);
            m_son.SetActive(false);
            m_ride_cow.GetComponent<SonCow>().ResetSonCow();
        }
        else
        {
            m_son_run.SetActive(false);
            m_son.SetActive(true);
            m_son.GetComponent<Son>().ResetSon();
        }
    }

    /// <summary>
    /// メンバ変数初期化処理
    /// </summary>
    public void InitMemberVariable()
    {
        switch (m_startSonTex)
        {
            case STARTSON_TEXTURE.SON:
                m_son_run.SetActive(false);
                m_ride_cow.SetActive(false);
                break;
            case STARTSON_TEXTURE.SON_COW:
                m_son_run.SetActive(false);
                m_son.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// ゲストが    シュミレーションするときに使用する情報
    /// </summary>
    public Vector3 GetSonOfset()
    {
        switch (m_startSonTex)
        {
            case STARTSON_TEXTURE.SON:
                return m_son.GetComponent<Son>().m_offset;
            case STARTSON_TEXTURE.SON_COW:
                return m_ride_cow.GetComponent<SonCow>().m_offset;
            default:
                return Vector3.zero;
        }
    }
}
