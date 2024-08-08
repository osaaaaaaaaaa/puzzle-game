using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    #region 息子関係
    [SerializeField] GameObject m_prefabArrow;
    [SerializeField] GameObject m_son;
    float m_son_defaultGravityScale;
    #endregion

    #region プレイヤー関係
    const float m_dragSpeed = 100f;    // ドラッグスピード
    Vector2 m_offsetPlayer;            // 母親と息子とのオフセット
    float m_mom_defaultGravityScale;   // 母親の重量
    public bool m_canDragPlayer;       // プレイヤーをドラッグできるかどうか
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
    public int m_mulPower = 10;

    private void Start()
    {
        m_canDragPlayer = false;
        m_isKicked = false;

        // 重量スケールを取得する
        m_mom_defaultGravityScale = GetComponent<Rigidbody2D>().gravityScale;
        m_son_defaultGravityScale = m_son.GetComponent<Rigidbody2D>().gravityScale;

        // オブジェクト・コンポーネントを取得する
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uiController = GameObject.Find("UiController").GetComponent<UiController>();
        m_cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();

        // オフセットを取得する
        m_offsetPlayer = new Vector2(transform.position.x - m_son.transform.position.x, transform.position.y - m_son.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.S)) UnityEditor.EditorApplication.isPaused = true;      // エディタを一時停止する
#endif

        // ゲーム開始していない || ポーズ中の場合
        if (!m_gameManager.m_isEndAnim || m_gameManager.m_isPause) return;

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
            m_son.transform.position = new Vector2(transform.position.x - m_offsetPlayer.x, transform.position.y - m_offsetPlayer.y);
        }

        // 画面タッチした&&現在矢印を生成できる場合
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

                    // 矢印を生成
                    m_arrow = Instantiate(m_prefabArrow, m_son.transform.position, Quaternion.identity);

                    // カメラをズームアウトさせる
                    m_cameraController.ZoomoOut();

                    // リセットボタンを非表示にする
                    uiController.SetActiveButtonReset(false);
                }
                else if (targetObj.tag == "Player")
                {// プレイヤータグを持つコライダーの場合

                    // ドラッグする準備
                    m_canDragPlayer = true;
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                    m_son.GetComponent<Rigidbody2D>().gravityScale = 0;
                }
            }
        }

        // 指を離した
        if (Input.GetMouseButtonUp(0))
        {
            // ドラッグ処理のパラメータを初期化
            m_canDragPlayer = false;
            GetComponent<Rigidbody2D>().gravityScale = m_mom_defaultGravityScale;
            m_son.GetComponent<Rigidbody2D>().gravityScale = m_son_defaultGravityScale;
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
                    GetComponent<MomAnimController>().PlayStandbyAnim();  // 通常スキンのIdleアニメ

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
        // 蹴るときのパラメータ取得(ラムダ式を使用するためnewでアドレス変更)
        Vector3 dir = new Vector3();
        dir = m_arrow.GetComponent<Arrow>().dir.normalized;
        float power = new float();
        power = m_arrow.GetComponent<Arrow>().dis * m_mulPower;

        // 母親のアニメーションを再生する
        GetComponent<MomAnimController>().PlayKickAnim();  // 蹴るアニメ

        // リセットボタンを有効化する
        uiController.SetInteractableButtonReset(true);

        // 息子を蹴り飛ばす処理
        Debug.Log("方角：" + dir + " , パワー：" + power);
        m_son.GetComponent<Son>().BeKicked(dir, power);

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
        m_son.GetComponent<Son>().Reset();

        // 再度蹴ることができるようにする
        m_isKicked = false;

        // カメラをズームインする
        m_cameraController.ZoomIn();

        // 母親のアニメーションを再生する
        GetComponent<MomAnimController>().PlayStandbyAnim();  // 通常スキンのIdleアニメ
    }
}
