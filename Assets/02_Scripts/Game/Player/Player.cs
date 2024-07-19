using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    #region 息子とカメラ関係
    GameObject m_mainCamera;
    [SerializeField] GameObject m_prefabArrow;
    [SerializeField] GameObject m_son;
    const float m_posY_Max = 3f;                // 息子が画面外に出そうになる高さ
    #endregion

    #region プレイヤー関係
    [SerializeField] float m_dragSpeed;
    Vector2 m_dragOffsetSon;           // ドラッグするときの息子のオフセット
    const float m_defaultGravity = 3f; // 母親と息子のデフォルト重量
    public bool m_canDragPlayer;       // プレイヤーをドラッグできるかどうか
    #endregion

    // ゲームマネージャー
    GameManager m_gameManager;

    // 新しく生成した矢印
    GameObject m_arrow;
    // カメラと息子との一定距離
    Vector3 m_offsetSon;
    // 蹴り飛ばしたかどうか
    public bool m_isKicked;

    private void Awake()
    {
        m_canDragPlayer = false;
        m_isKicked = false;

        m_mainCamera = GameObject.Find("Main Camera");
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // 息子のポジションとカメラのオフセット
        var tmp_offset = m_mainCamera.transform.position - m_son.transform.position;
        m_offsetSon = new Vector3(tmp_offset.x, m_mainCamera.transform.position.y, m_mainCamera.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.S)) UnityEditor.EditorApplication.isPaused = true;      // エディタを一時停止する
#endif

        // ゲーム開始していない場合
        if (!m_gameManager.m_isEndAnim) return;

        // プレイヤーをドラッグしている場合
        if (m_canDragPlayer)
        {
            // 母親のドラッグ処理
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (worldPos - transform.position).normalized;
            transform.GetComponent<Rigidbody2D>().velocity = dir * m_dragSpeed;

            // 息子のドラッグ処理
            m_son.transform.position = new Vector2(transform.position.x - m_dragOffsetSon.x, transform.position.y - m_dragOffsetSon.y);
        }

        // 蹴り飛ばしたときカメラを追従させる
        if (m_isKicked)
        {
            Vector3 offset = m_son.transform.position.y < m_posY_Max ? m_offsetSon : new Vector3(m_offsetSon.x, m_son.transform.position.y - m_posY_Max, m_offsetSon.z);

            m_mainCamera.transform.position = new Vector3(m_son.transform.position.x + offset.x, offset.y, -10f);
            m_mainCamera.GetComponent<Camera>().orthographicSize = 5f;

            if (m_mainCamera.transform.position.x <= 0.1f)
            {
                m_mainCamera.transform.position = new Vector3(0f, m_mainCamera.transform.position.y, -10f);
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
                else if (targetObj.tag == "Player")
                {
                    // ドラッグする準備
                    m_canDragPlayer = true;
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                    m_son.GetComponent<Rigidbody2D>().gravityScale = 0;

                    // ドラッグするとき、母親から息子とのオフセット
                    m_dragOffsetSon = new Vector2(transform.position.x - m_son.transform.position.x, transform.position.y - m_son.transform.position.y);
                }
            }
        }

        // 指を離した＆矢印が存在する場合
        if (Input.GetMouseButtonUp(0))
        {
            m_canDragPlayer = false;
            GetComponent<Rigidbody2D>().gravityScale = m_defaultGravity;
            m_son.GetComponent<Rigidbody2D>().gravityScale = m_defaultGravity;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            m_son.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (m_arrow != null)
            {
                // メインカメラの視野を元に戻す
                m_mainCamera.GetComponent<Camera>().DOOrthoSize(5f, 1f);

                // 蹴ることが可能な場合、Tween終了後に関数をcallbackする
                if (m_arrow.GetComponent<Arrow>().isKick)
                {
                    // 蹴るときのパラメータ取得(ラムダ式を使用するためnewでアドレス変更)
                    Vector3 dir = new Vector3();
                    dir = m_arrow.GetComponent<Arrow>().dir.normalized;
                    float power = new float();
                    power = m_arrow.GetComponent<Arrow>().dis * 10;

                    // メインカメラの位置を元に戻したら、息子を蹴る処理
                    m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 1f)
                        .OnComplete(() =>
                        {
                            Debug.Log("方角：" + dir + " , パワー：" + power);
                            m_isKicked = true;
                            m_son.GetComponent<Son>().BeKicked(dir, power);
                        });
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
    }

    /// <summary>
    /// 息子をスタート地点へ戻す
    /// </summary>
    public void OnRestart()
    {
        m_son.GetComponent<Son>().Reset();

        DOTween.Kill(m_mainCamera);
        m_mainCamera.GetComponent<Camera>().DOOrthoSize(5f, 0.2f);
        m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 0.2f);
        m_isKicked = false;
    }
}
