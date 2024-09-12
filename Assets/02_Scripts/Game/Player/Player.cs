using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region 息子関係
    [SerializeField] GameObject m_prefabArrow;
    [SerializeField] GameObject m_sonController;
    [SerializeField] GameObject m_son;
    [SerializeField] GameObject m_ride_cow;
    #endregion

    #region プレイヤー関係
    const float m_dragSpeed = 100f;    // ドラッグスピード
    public bool m_canDragPlayer;       // プレイヤーをドラッグできるかどうか
    #endregion

    #region SE
    AudioSource m_audioSouse;
    [SerializeField] AudioClip m_kickSE;
    [SerializeField] AudioClip m_cowSE;
    #endregion

    // ゲームマネージャー
    GameManager m_gameManager;
    // UIコントローラー
    UiController uiController;
    // カメラコントローラー
    CameraController m_cameraController;

    // 新しく生成した矢印
    public GameObject m_arrow;
    // 蹴り飛ばしたかどうか
    public bool m_isKicked;
    // 蹴り飛ばすときに乗算する値
    public int m_mulPower = 50;

    /// <summary>
    /// 蹴るときのベクトル
    /// </summary>
    public Vector3 VectorKick { get; private set; } = Vector3.zero;

    private void Start()
    {
        if (TopManager.isUseItem) m_mulPower = 100;
        m_canDragPlayer = false;
        m_isKicked = false;

        // オブジェクト・コンポーネントを取得する
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uiController = GameObject.Find("UiController").GetComponent<UiController>();
        m_cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        m_audioSouse = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.S)) UnityEditor.EditorApplication.isPaused = true;      // エディタを一時停止する
#endif

        // ゲーム開始していない || ポーズ中 || ゲームが終了した || ゲームモードが編集完了モード
        if (!m_gameManager.m_isEndAnim || m_gameManager.m_isPause 
            || m_gameManager.m_isEndGame || m_gameManager.GameMode == GameManager.GAMEMODE.EditDone) return;

        // プレイヤーをドラッグしている場合
        if (m_canDragPlayer)
        {
            // 母親のドラッグ処理
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (worldPos - transform.position).normalized;
            transform.GetComponent<Rigidbody2D>().velocity = dir * m_dragSpeed;
        }

        // 蹴り飛ばしたときカメラを追従させる
        if (m_isKicked)
        {
            // カメラが息子を追従する
            m_cameraController.Follow();
        }
        // 蹴り飛ばす前の場合
        else
        {
            // 息子を固定する
            m_son.GetComponent<Son>().ResetSon();
            m_ride_cow.GetComponent<SonCow>().ResetSonCow();

            if(m_arrow != null)
            {
                // 牛の向きを更新する
                if (m_ride_cow.activeSelf)
                {
                    var cowDir = m_arrow.GetComponent<Arrow>().dir.x < 0 ? -1 : 1;
                    m_ride_cow.GetComponent<SonCow>().m_direction = cowDir;
                }
            }
        }

        // 画面タッチした&&現在矢印を生成できる場合
        if (Input.GetMouseButtonDown(0) && m_isKicked == false)
        {
            // タッチした場所にRayを飛ばす
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast(worldPos, Vector2.zero);

            // プレイヤータグを持つコライダーの場合
            if (hit2d)
            {
                if (hit2d.collider.gameObject.tag == "Player")
                {
                    // ドラッグする準備
                    m_canDragPlayer = true;
                }
            }

            // ドラッグ状態ではない場合
            if(!m_canDragPlayer)
            {
                Vector3 pivotSon;
                if (m_son.activeSelf)
                {
                    pivotSon = m_son.transform.position;
                }
                else
                {
                    pivotSon = m_ride_cow.GetComponent<SonCow>().GetPivotPos();
                }

                // 矢印を生成
                m_arrow = Instantiate(m_prefabArrow, pivotSon, Quaternion.identity);

                // リセットボタンを非表示にする
                if (m_gameManager.GameMode == GameManager.GAMEMODE.Play) uiController.SetActiveButtonReset(false);
            }
        }

        // 指を離した
        if (Input.GetMouseButtonUp(0))
        {
            // ドラッグ処理のパラメータを初期化
            m_canDragPlayer = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            m_son.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            // リセットボタンを表示する
            uiController.SetActiveButtonReset(true);

            // 矢印が存在する場合(蹴る条件を満たした場合) && まだ蹴り飛ばしていない場合
            if (m_arrow != null && m_isKicked == false)
            {
                // 蹴ることが可能な場合、Tween終了後に関数をcallbackする
                if (m_arrow.GetComponent<Arrow>().isKick)
                {
                    // 蹴り飛ばしたことにする
                    m_isKicked = true;

                    // 蹴り飛ばす処理を指定秒数後に実行する
                    Invoke("DOKick", 0.2f);
                }
                else
                {
                    // 母親のアニメーションを再生する
                    GetComponent<PlayerAnimController>().PlayStandbyAnim();  // 通常スキンのIdleアニメ

                    // 生成した矢印を破棄する
                    Destroy(m_arrow);
                    m_arrow = null;
                }
            }
        }
    }

    /// <summary>
    /// 蹴り飛ばす処理
    /// </summary>
    void DOKick()
    {
        if (m_gameManager.m_isEndGame) return;

        // 蹴るときのパラメータ取得(ラムダ式を使用するためnewでアドレス変更)
        Vector3 dir = new Vector3();
        dir = m_arrow.GetComponent<Arrow>().dir.normalized;
        float power = new float();
        power = m_arrow.GetComponent<Arrow>().dis * m_mulPower;

        if(m_gameManager.GameMode == GameManager.GAMEMODE.Edit)
        {
            // 蹴るときのベクトルを保存する(ゲスト用)
            VectorKick = new Vector3(dir.x * power, dir.y * power);
        }

        // 母親のアニメーションを再生する
        GetComponent<PlayerAnimController>().PlayKickAnim();  // 蹴るアニメ

        // リセットボタンを有効化する
        uiController.SetInteractableButtonReset(true);

        // 息子を蹴り飛ばす処理
        Debug.Log("ベクトル：" + new Vector3(dir.x * power, dir.y * power).ToString());
        if (m_son.activeSelf)
        {
            m_son.GetComponent<Son>().DOKick(dir, power, true);
        }
        else
        {
            m_ride_cow.GetComponent<SonCow>().DOKick(dir, power);
            m_audioSouse.PlayOneShot(m_cowSE);
        }
        m_audioSouse.PlayOneShot(m_kickSE);

        // 生成した矢印を破棄する
        Destroy(m_arrow);
        m_arrow = null;
    }

    /// <summary>
    /// プレイヤーの状態をリセットする
    /// </summary>
    public void ResetPlayer()
    {
        // 息子をリセットする
        m_sonController.GetComponent<SonController>().ResetSon();

        // 再度蹴ることができるようにする
        m_isKicked = false;
        m_canDragPlayer = false;

        // カメラをズームイン・ズームアウトする
        switch (m_cameraController.m_cameraMode)
        {
            case CameraController.CAMERAMODE.ZOOMIN:
                m_cameraController.ZoomIn();
                break;
            case CameraController.CAMERAMODE.ZOOMOUT:
                m_cameraController.ZoomOut(0.3f);
                break;
        }

        // 母親のアニメーションを再生する
        GetComponent<PlayerAnimController>().PlayStandbyAnim();  // 通常スキンのIdleアニメ
    }
}
