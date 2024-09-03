using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonCow : MonoBehaviour
{
    [SerializeField] GameObject m_son;
    [SerializeField] GameObject m_son_cow;
    [SerializeField] LayerMask m_obstacleLayer1;     // ��Q���̃��C���[�^�O
    [SerializeField] LayerMask m_obstacleLayer2;     // ��Q���̃��C���[�^�O
    Rigidbody2D m_rb;
    Animator m_animator;
    public float m_speed = 1f;
    public int m_direction = 1;
    float m_JumpPower = 150f;
    Vector2 m_jumpDir = new Vector2(0,1);  // �W�����v�������
    bool m_isBeKicked;  // �R���΂��ꂽ���ǂ���
    bool m_isJump;      // �W�����v�������ǂ���
    GameObject m_player;
    GameObject m_gameManager;

    #region Ray�̎n�_�ƏI�_
    float m_lineWallStart = 0.7f;
    float m_lineWallEnd = 0.7f;
    float m_lineWallSpace1 = 1f;
    float m_lineWallSpace2 = 0.64f;
    Vector3 m_lineBotomStart1 = new Vector3(-1.83f, 0.26f, 0f);
    Vector3 m_lineBotomStart2 = new Vector3(1.25f, 0.26f, 0f);
    Vector3 m_lineBotomEnd = new Vector3(0.52f, -0.2f, 0f);
    #endregion

    #region �R���΂��Ƃ��ɕK�v�ȃp�����[�^
    Vector3 m_offset;      // ��e�Ƃ̃I�t�Z�b�g
    // �����x
    float m_initialSpeed = 50f;
    // �d�ʃX�P�[��
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
        // �R���΂�����Ƀ^�b�v�����ꍇ
        if (m_isBeKicked && Input.GetMouseButtonDown(0) && !m_gameManager.GetComponent<GameManager>().m_isPause && transform.tag != "Ghost")
        {
            SeparateSon();
        }

        // �������X�V
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * m_direction, transform.localScale.y, transform.localScale.z);
    }

    private void FixedUpdate()
    {
        // �v���C���[���܂��R���΂��Ă��Ȃ��ꍇ
        if (!m_isBeKicked)
        {
            m_animator.SetBool("IsGround", false);
            return;
        }

        // �A�j���[�V�����p�����[�^�X�V
        m_animator.SetBool("IsGround", IsGround());

        // ���������Ă��邩�ǂ���
        if (!IsGround()) return;

        // �ǂɏՓ˂����ꍇ
        if (IsWall())
        {
            m_direction *= -1;  // �ړ������𔽓]������
            m_rb.velocity = Vector3.zero;
        }

        // �ړ�����
        m_rb.velocity = new Vector2(m_speed * m_direction, m_rb.velocity.y); ;
    }

    /// <summary>
    /// ����̐�ɕǂ����邩�ǂ���
    /// </summary>
    /// <returns></returns>
    private bool IsWall()
    {
        Vector3 startVec1 = Vector3.zero, endVec1 = Vector3.zero;
        Vector3 startVec2 = Vector3.zero, endVec2 = Vector3.zero;

        startVec1 = transform.position + transform.right * m_lineWallSpace1 * transform.localScale.x + transform.up * m_lineWallStart;    // �n�_
        endVec1 = startVec1 + transform.up * transform.localScale.y * m_lineWallEnd;    // �I�_

        startVec2 = transform.position + transform.right * m_lineWallSpace2 * transform.localScale.x + transform.up * m_lineWallStart;    // �n�_
        endVec2 = startVec2 + transform.up * transform.localScale.y * m_lineWallEnd;    // �I�_

        Debug.DrawLine(startVec1, endVec1, Color.red);   // �`�ʂ���
        Debug.DrawLine(startVec2, endVec2, Color.red);   // �`�ʂ���

        return Physics2D.Linecast(startVec1, endVec1, m_obstacleLayer1) 
            || Physics2D.Linecast(startVec2, endVec2, m_obstacleLayer1)
            || Physics2D.Linecast(startVec1, endVec1, m_obstacleLayer2)
            || Physics2D.Linecast(startVec2, endVec2, m_obstacleLayer2);
    }

    /// <summary>
    /// �n�ʂɒ����Ă���
    /// </summary>
    /// <returns></returns>
    public bool IsGround()
    {
        // �����ɂQ�̎n�_�ƏI�_���쐬����
        Vector3 leftStartPosition = transform.position + new Vector3(m_lineBotomStart1.x * m_direction, m_lineBotomStart1.y, m_lineBotomStart1.z);     // �����̎n�_
        Vector3 rightStartPosition = transform.position + new Vector3(m_lineBotomStart2.x * m_direction, m_lineBotomStart2.y, m_lineBotomStart2.z);    // �E���̎n�_
        Vector3 endPosition = transform.position - new Vector3(m_lineBotomEnd.x * m_direction, m_lineBotomEnd.y, m_lineBotomEnd.z);             // �I�_(��)

        // �`�ʂ���
        Debug.DrawLine(leftStartPosition, endPosition, Color.red);
        Debug.DrawLine(rightStartPosition, endPosition, Color.red);

        return Physics2D.Linecast(leftStartPosition, endPosition, m_obstacleLayer1)
        || Physics2D.Linecast(rightStartPosition, endPosition, m_obstacleLayer1)
        || Physics2D.Linecast(leftStartPosition, endPosition, m_obstacleLayer2)
        || Physics2D.Linecast(rightStartPosition, endPosition, m_obstacleLayer2);
    }

    /// <summary>
    /// ���̍��W�̃p�����[�^�ݒ�
    /// </summary>
    public void SetCowParam(int direction,Vector3 pos)
    {
        transform.position = pos;
        m_direction = direction;

        Debug.Log(pos);
    }

    /// <summary>
    /// ���q�ƕ�������
    /// </summary>
    void SeparateSon()
    {
        // ���ɏ�������q����A�N�e�B�u�̏ꍇ
        if (!m_son_cow.activeSelf || m_isJump) return;

        m_son.transform.position = m_son_cow.transform.position;

        // ���q�̕\���E��\��
        m_son_cow.SetActive(false);
        m_son.SetActive(true);

        // ���q���΂�����
        float addJumpX = m_rb.velocity.x;
        float addJumpY = m_rb.velocity.y > 0 ? m_rb.velocity.y : 0f;
        m_son.GetComponent<Son>().DOKick(new Vector3(m_jumpDir.x + addJumpX, m_jumpDir.y + addJumpY, 0f),m_JumpPower,false);
        m_isJump = true;
    }

    /// <summary>
    /// �R���΂���鏈��
    /// </summary>
    /// <param name="dir">���p</param>
    /// <param name="power">�p���[</param>
    public void DOKick(Vector3 dir, float power)
    {
        var rb = GetComponent<Rigidbody2D>();

        // �d�͂�ݒ肷��
        rb.gravityScale = m_gravityScale;
        // ���x��ݒ肷��
        rb.velocity = transform.forward * m_initialSpeed;

        Vector3 force = new Vector3(dir.x * power, dir.y * power);  // �͂�ݒ�
        rb.AddForce(force, ForceMode2D.Impulse);  // �͂�������

        m_isBeKicked = true;
    }

    /// <summary>
    /// ���Z�b�g
    /// </summary>
    public void ResetSonCow()
    {
        m_isBeKicked = false;
        m_isJump = false;

        // ���������Z�b�g����
        m_direction = 1;

        m_son_cow.SetActive(true);
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.gravityScale = 0;
        transform.position = m_player.transform.position + m_offset;
    }

    /// <summary>
    /// ���S���W���擾����
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPivotPos()
    {
        var offsetCollider = GetComponent<BoxCollider2D>().offset;
        return new Vector3(transform.position.x + offsetCollider.x, transform.position.y + offsetCollider.y, transform.position.z);
    }

    /// <summary>
    /// �����o�ϐ�����������
    /// </summary>
    public void InitMemberVariable()
    {
        m_player = GameObject.Find("Player");
        m_gameManager = GameObject.Find("GameManager");
        m_offset = transform.position - m_player.transform.position;
    }

    public void InitState()
    {
        m_isBeKicked = true;
        m_rb.gravityScale = m_gravityScale;
    }
}
