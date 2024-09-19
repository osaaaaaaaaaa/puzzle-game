using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonCow : MonoBehaviour
{
    [SerializeField] GameObject m_son;
    [SerializeField] GameObject m_son_cow;
    [SerializeField] LayerMask m_obstacleLayer1;     // 障害物のレイヤータグ
    [SerializeField] LayerMask m_obstacleLayer2;     // 障害物のレイヤータグ
    Rigidbody2D m_rb;
    Animator m_animator;
    public float m_speed = 1f;
    public int m_direction = 1;
    float m_JumpPower = 150f;
    Vector2 m_jumpDir = new Vector2(0,1);  // ジャンプする方向
    bool m_isBeKicked;  // 蹴り飛ばされたかどうか
    bool m_isJump;      // ジャンプしたかどうか
    GameObject m_player;
    GameManager m_gameManager;

    #region Rayの始点と終点
    float m_lineWallStart = 0.7f;
    float m_lineWallEnd = 0.7f;
    float m_lineWallSpace1 = 1f;
    float m_lineWallSpace2 = 0.64f;
    Vector3 m_lineBotomStart1 = new Vector3(-1.83f, 0.26f, 0f);
    Vector3 m_lineBotomStart2 = new Vector3(1.25f, 0.26f, 0f);
    Vector3 m_lineBotomEnd = new Vector3(0.52f, -0.2f, 0f);
    #endregion

    #region 蹴り飛ばすときに必要なパラメータ
    public Vector3 m_offset { get; private set; }      // 母親とのオフセット
    // 初速度
    float m_initialSpeed = 50f;
    // 重量スケール
    float m_gravityScale = 10f;
    #endregion

    private void Awake()
    {
        m_isBeKicked = false;
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 蹴り飛ばした後にタップした場合
        if (m_isBeKicked && Input.GetMouseButtonDown(0) && !m_gameManager.GetComponent<GameManager>().m_isPause && transform.tag != "Ghost")
        {
            SeparateSon();
        }

        // 向きを更新
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * m_direction, transform.localScale.y, transform.localScale.z);
    }

    private void FixedUpdate()
    {
        // プレイヤーがまだ蹴り飛ばしていない場合
        if (!m_isBeKicked)
        {
            m_animator.SetBool("IsGround", false);
            return;
        }

        // アニメーションパラメータ更新
        m_animator.SetBool("IsGround", IsGround());

        // 足が着いているかどうか
        if (!IsGround()) return;

        // 壁に衝突した場合
        if (IsWall())
        {
            m_direction *= -1;  // 移動方向を反転させる
            m_rb.velocity = Vector3.zero;
        }

        // 移動処理
        if(!m_gameManager.m_isPause) m_rb.velocity = new Vector2(m_speed * m_direction, m_rb.velocity.y);
    }

    /// <summary>
    /// 判定の先に壁があるかどうか
    /// </summary>
    /// <returns></returns>
    private bool IsWall()
    {
        Vector3 startVec1 = Vector3.zero, endVec1 = Vector3.zero;
        Vector3 startVec2 = Vector3.zero, endVec2 = Vector3.zero;

        startVec1 = transform.position + transform.right * m_lineWallSpace1 * transform.localScale.x + transform.up * m_lineWallStart;    // 始点
        endVec1 = startVec1 + transform.up * transform.localScale.y * m_lineWallEnd;    // 終点

        startVec2 = transform.position + transform.right * m_lineWallSpace2 * transform.localScale.x + transform.up * m_lineWallStart;    // 始点
        endVec2 = startVec2 + transform.up * transform.localScale.y * m_lineWallEnd;    // 終点

        Debug.DrawLine(startVec1, endVec1, Color.red);   // 描写する
        Debug.DrawLine(startVec2, endVec2, Color.red);   // 描写する

        return Physics2D.Linecast(startVec1, endVec1, m_obstacleLayer1) 
            || Physics2D.Linecast(startVec2, endVec2, m_obstacleLayer1)
            || Physics2D.Linecast(startVec1, endVec1, m_obstacleLayer2)
            || Physics2D.Linecast(startVec2, endVec2, m_obstacleLayer2);
    }

    /// <summary>
    /// 地面に着いている
    /// </summary>
    /// <returns></returns>
    public bool IsGround()
    {
        // 足元に２つの始点と終点を作成する
        Vector3 leftStartPosition = transform.position + new Vector3(m_lineBotomStart1.x * m_direction, m_lineBotomStart1.y, m_lineBotomStart1.z);     // 左側の始点
        Vector3 rightStartPosition = transform.position + new Vector3(m_lineBotomStart2.x * m_direction, m_lineBotomStart2.y, m_lineBotomStart2.z);    // 右側の始点
        Vector3 endPosition = transform.position - new Vector3(m_lineBotomEnd.x * m_direction, m_lineBotomEnd.y, m_lineBotomEnd.z);             // 終点(下)

        // 描写する
        Debug.DrawLine(leftStartPosition, endPosition, Color.red);
        Debug.DrawLine(rightStartPosition, endPosition, Color.red);

        return Physics2D.Linecast(leftStartPosition, endPosition, m_obstacleLayer1)
        || Physics2D.Linecast(rightStartPosition, endPosition, m_obstacleLayer1)
        || Physics2D.Linecast(leftStartPosition, endPosition, m_obstacleLayer2)
        || Physics2D.Linecast(rightStartPosition, endPosition, m_obstacleLayer2);
    }

    /// <summary>
    /// 牛の座標のパラメータ設定
    /// </summary>
    public void SetCowParam(int direction,Vector3 pos)
    {
        transform.position = pos;
        m_direction = direction;

        Debug.Log(pos);
    }

    /// <summary>
    /// 息子と分離する
    /// </summary>
    void SeparateSon()
    {
        // 牛に乗った息子が非アクティブの場合 || ポーズ中の場合
        if (!m_son_cow.activeSelf || m_isJump || m_gameManager.m_isPause) return;

        m_son.transform.position = m_son_cow.transform.position;

        // 息子の表示・非表示
        m_son_cow.SetActive(false);
        m_son.SetActive(true);

        // 息子を飛ばす処理
        float addJumpX = m_rb.velocity.x;
        float addJumpY = m_rb.velocity.y > 0 ? m_rb.velocity.y : 0f;
        m_son.GetComponent<Son>().DOKick(new Vector3(m_jumpDir.x + addJumpX, m_jumpDir.y + addJumpY, 0f),m_JumpPower,false);
        m_isJump = true;
    }

    /// <summary>
    /// 蹴り飛ばされる処理
    /// </summary>
    /// <param name="dir">方角</param>
    /// <param name="power">パワー</param>
    public void DOKick(Vector3 dir, float power)
    {
        var rb = GetComponent<Rigidbody2D>();

        // 重力を設定する
        rb.gravityScale = m_gravityScale;
        // 速度を設定する
        rb.velocity = transform.forward * m_initialSpeed;

        Vector3 force = new Vector3(dir.x * power, dir.y * power);  // 力を設定
        rb.AddForce(force, ForceMode2D.Impulse);  // 力を加える

        m_isBeKicked = true;
    }

    /// <summary>
    /// ゲストに蹴り飛ばされる処理
    /// </summary>
    public void DOKick(Vector3 vecKick)
    {
        var rb = GetComponent<Rigidbody2D>();

        // 重力を設定する
        rb.gravityScale = m_gravityScale;
        // 速度を設定する
        rb.velocity = transform.forward * m_initialSpeed;

        Vector3 force = vecKick;  // 力を設定
        rb.AddForce(force, ForceMode2D.Impulse);  // 力を加える

        m_isBeKicked = true;
    }

    /// <summary>
    /// リセット
    /// </summary>
    public void ResetSonCow()
    {
        m_isBeKicked = false;
        m_isJump = false;

        // 向きをリセットする
        m_direction = 1;

        m_son_cow.SetActive(true);
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.gravityScale = 0;
        transform.position = m_player.transform.position + m_offset;
    }

    /// <summary>
    /// ゲストによるリセット処理
    /// </summary>
    public void ResetSonCow(Vector3 startPos)
    {
        m_isBeKicked = false;
        m_isJump = false;

        // 向きをリセットする
        m_direction = 1;

        m_son_cow.SetActive(true);
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.gravityScale = 0;
        transform.position = startPos + m_offset;
    }

    /// <summary>
    /// 中心座標を取得する
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPivotPos()
    {
        var offsetCollider = GetComponent<BoxCollider2D>().offset;
        return new Vector3(transform.position.x + offsetCollider.x, transform.position.y + offsetCollider.y, transform.position.z);
    }

    /// <summary>
    /// メンバ変数初期化処理
    /// </summary>
    public void InitMemberVariable()
    {
        m_player = GameObject.Find("Player");
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_offset = transform.position - m_player.transform.position;
    }

    public void InitState()
    {
        m_isBeKicked = true;
        m_rb.gravityScale = m_gravityScale;
    }
}
