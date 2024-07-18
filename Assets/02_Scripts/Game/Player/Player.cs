using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    #region 息子とカメラ関連
    [SerializeField] GameObject m_mainCamera;
    [SerializeField] GameObject m_prefabArrow;
    [SerializeField] GameObject m_son;
    #endregion

    // 新しく生成した矢印
    GameObject m_arrow;
    // カメラと息子との一定距離
    float m_fixedDis;
    // 蹴り飛ばしたかどうか
    public bool m_isKicked;

    #region 蹴るときのパラメータ
    Vector3 m_keepDir;
    float m_keepPower;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // 息子とカメラ関連のパラメータ初期化
        m_fixedDis = m_mainCamera.transform.position.x - m_son.transform.position.x;
        m_isKicked = false;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.S)) UnityEditor.EditorApplication.isPaused = true;      // エディタを一時停止する
        if(Input.GetKey(KeyCode.P)) UnityEditor.EditorApplication.isPaused = false;     // エディタを再生する
#endif

        // 蹴り飛ばしたときカメラを追従させる
        if (m_isKicked)
        {
            m_mainCamera.transform.position = new Vector3(m_son.transform.position.x + m_fixedDis, 0, -10f);
            m_mainCamera.GetComponent<Camera>().orthographicSize = 5f;

            if (m_mainCamera.transform.position.x <= 0.1f)
            {
                m_mainCamera.transform.position = new Vector3(0f, 0f, -10f);
            }
        }

        // 現在矢印を生成できる場合
        if (Input.GetMouseButtonDown(0) && m_isKicked == false)
        {
            // タッチした場所にRayを飛ばす
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit2d)
            {
                GameObject targetObj = hit2d.collider.gameObject;
                if (targetObj.tag == "StartArea")
                {// スタートエリアと重なっていた場合

                    // 矢印を生成＆シュミレーションコントローラーを設定
                    m_arrow = Instantiate(m_prefabArrow, m_son.transform.position, Quaternion.identity);
                    m_arrow.GetComponent<Arrow>().m_simulationController = GetComponent<SimulationController>();

                    // メインカメラの視野と位置を調整する
                    m_mainCamera.transform.DOKill();
                    m_mainCamera.GetComponent<Camera>().DOOrthoSize(8f, 0.3f);
                    m_mainCamera.transform.DOMove(new Vector3(5.4f, 3f, -10f), 0.3f);
                }
            }
        }

        // 指を離した＆矢印が存在する場合
        if (Input.GetMouseButtonUp(0) && m_arrow != null)
        {
            // メインカメラの視野と位置を元に戻す
            m_mainCamera.GetComponent<Camera>().DOOrthoSize(5f, 1f);

            // 蹴ることが可能な場合、Tween終了後に関数をcallbackする
            if (m_arrow.GetComponent<Arrow>().isKick)
            {
                m_keepDir = m_arrow.GetComponent<Arrow>().dir;
                m_keepPower = m_arrow.GetComponent<Arrow>().dis;
                m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 1f)
                    .OnComplete(() => CallBackSonMethod());
            }
            else
            {
                // カメラの位置をリセットする
                m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 1f);
            }
            
            // 生成した矢印を破棄する
            Destroy(m_arrow);
            m_arrow = null;

            // シュミレーションコントローラーの移動ベクトルをリセット
            GetComponent<SimulationController>().m_sonVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// sonの関数を呼ぶ
    /// </summary>
    public void CallBackSonMethod()
    {
        Debug.Log("方角：" + m_keepDir.normalized + " , パワー：" + m_keepPower * 10);
        m_isKicked = true;
        m_son.GetComponent<Son>().BeKicked(m_keepDir.normalized, m_keepPower * 10);
    }

    /// <summary>
    /// 息子をスタート地点へ戻す
    /// </summary>
    public void OnRestart()
    {
        m_son.GetComponent<Son>().Reset();

        m_mainCamera.GetComponent<Camera>().DOOrthoSize(5f, 0.2f);
        m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 0.2f);
        m_isKicked = false;
    }
}
