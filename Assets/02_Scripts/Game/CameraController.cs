using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    // メインカメラ
    [SerializeField] GameObject m_mainCamera;

    #region カメラのパラメータ
    Vector3 m_startCameraPos;       // カメラの初期位置
    [SerializeField] float m_cameraSizeMin;  // 視野の最小サイズ
    [SerializeField] float m_cameraSizeMax;  // 視野の最大サイズ
    [SerializeField] Vector3 m_camera_movePos;       // カメラが移動する座標
    #endregion  

    #region 息子関係
    GameObject m_son;
    GameObject m_son_run;
    Vector3 m_offsetSon;           // カメラと息子との一定距離
    Vector3 m_tmpSonPos;           // 息子が最後にいた座標
    const float m_posY_Max = 3f;   // 息子が画面外に出そうになる高さ
    #endregion

    // プレイヤー
    GameObject m_player;

    // ズームインアウトボタン
    GameObject m_buttonZoomIn;
    GameObject m_buttonZoomOut;

    // カメラのモード
    public enum CAMERAMODE
    {
        ZOOMIN = 0,
        ZOOMOUT,
    }

    public CAMERAMODE m_cameraMode;

    // カメラの追尾が可能かどうか
    bool m_isFollow;

    private void Awake()
    {
        // オブジェクトを取得する
        m_son = GameObject.Find("Son");
        m_son_run = GameObject.Find("son_run");
        m_player = GameObject.Find("Player");

        // 息子のポジションとカメラのオフセット
        var tmp_offset = m_mainCamera.transform.position - m_son.transform.position;
        m_offsetSon = new Vector3(tmp_offset.x, tmp_offset.y, m_mainCamera.transform.position.z);

        // カメラの初期位置を取得
        m_startCameraPos = m_mainCamera.transform.position;

        // モード変更
        m_cameraMode = CAMERAMODE.ZOOMIN;
        // 追尾OFF
        m_isFollow = false;
    }

    private void Update()
    {
        if (!(!m_son.activeSelf && !m_son_run.activeSelf))
        {
            // 息子が最後にいた座標を取得する
            m_tmpSonPos = m_son.activeSelf ? m_son.transform.position : m_son_run.transform.position;
        }
    }

    /// <summary>
    /// ズームアウト処理
    /// </summary>
    public void ZoomOut(float time)
    {
        // モード変更
        m_cameraMode = CAMERAMODE.ZOOMOUT;

        // Tween破棄する
        DOTween.Kill(m_mainCamera.transform);

        //Sequence生成
        var sequence = DOTween.Sequence();
        //Tweenをつなげる
        sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMax, time))
                .Join(m_mainCamera.transform.DOMove(m_camera_movePos, time));

        // Tween再生する
        sequence.Play();

        // ズームインアウトボタンの表示切り替え
        m_buttonZoomIn.SetActive(true);
        m_buttonZoomOut.SetActive(false);
    }

    /// <summary>
    /// ズームイン処理
    /// </summary>
    public void ZoomIn()
    {
        // カメラの追尾OFF
        m_isFollow = false;

        // モード変更
        m_cameraMode = CAMERAMODE.ZOOMIN;

        // ズームインする対象の座標を設定する
        Vector3 target;
        if (!m_son.activeSelf && !m_son_run.activeSelf)
        {
            target = m_tmpSonPos;
        }
        else
        {
            // ズームインする対象の座標を設定する
            target = m_son.activeSelf ? m_son.transform.position : m_son_run.transform.position;
            m_tmpSonPos = target;
        }

        // Tween破棄する
        DOTween.Kill(m_mainCamera.transform);
        //Sequence生成
        var sequence = DOTween.Sequence();

        // プレイヤーが蹴る前の場合
        if (!m_player.GetComponent<Player>().m_isKicked)
        {
            // ズームインする座標を設定
            Vector3 zoomin = m_player.transform.position.y < m_startCameraPos.y ?
                m_startCameraPos : new Vector3(m_startCameraPos.x, m_player.transform.position.y, m_startCameraPos.z);

            //Tweenをつなげる
            sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMin, 0.3f))
                    .Join(m_mainCamera.transform.DOMove(zoomin, 0.3f)
                    .OnComplete(() => { m_isFollow = true; }));
        }
        // 蹴り飛ばした後の場合
        else
        {
            // 息子とのオフセットを設定
            Vector3 setPos = target.y < m_posY_Max ?
                new Vector3(target.x + m_offsetSon.x, m_startCameraPos.y, m_offsetSon.z) :
                new Vector3(target.x + m_offsetSon.x, target.y - m_posY_Max, m_offsetSon.z);

            // カメラが範囲外にでるのを阻止する
            if (setPos.x <= m_startCameraPos.x)
            {
                setPos = new Vector3(m_startCameraPos.x, setPos.y, m_startCameraPos.z);
            }

            // Tween終了後に追尾させる
            sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMin, 0.05f))
                    .Join(m_mainCamera.transform.DOMove(new Vector3(setPos.x, setPos.y, -10f), 0.05f)
                    .OnComplete(()=> { m_isFollow = true; }));
        }

        // Tween再生する
        sequence.Play();

        // ズームインアウトボタンの表示切り替え
        m_buttonZoomIn.SetActive(false);
        m_buttonZoomOut.SetActive(true);
    }

    /// <summary>
    /// 息子を追従する
    /// </summary>
    public void Follow()
    {
        if (!m_son.activeSelf && !m_son_run.activeSelf) return;

        // カメラのモードがズームインになっている場合
        if (m_cameraMode == CAMERAMODE.ZOOMIN && m_isFollow)
        {
            // ズームインする対象の座標を設定する
            Vector3 target;
            if (!m_son.activeSelf && !m_son_run.activeSelf)
            {
                target = m_tmpSonPos;
            }
            else
            {
                // ズームインする対象の座標を設定する
                target = m_son.activeSelf ? m_son.transform.position : m_son_run.transform.position;
            }

            // 息子とのオフセットを設定
            Vector3 offset = target.y < m_posY_Max ? 
                new Vector3(target.x + m_offsetSon.x, m_startCameraPos.y, m_offsetSon.z) : 
                new Vector3(target.x + m_offsetSon.x, target.y - m_posY_Max, m_offsetSon.z);

            // カメラの視野と追従を設定
            m_mainCamera.transform.position = new Vector3(offset.x, offset.y, -10f);
            m_mainCamera.GetComponent<Camera>().orthographicSize = m_cameraSizeMin;

            // カメラが範囲外にでるのを阻止する
            if (m_mainCamera.transform.position.x <= m_startCameraPos.x)
            {
                m_mainCamera.transform.position = new Vector3(m_startCameraPos.x, m_mainCamera.transform.position.y, m_startCameraPos.z);
            }
        }
    }

    /// <summary>
    /// メンバ変数初期化処理
    /// </summary>
    public void InitMemberVariable(GameObject zoomInBtn, GameObject zoomOutBtn)
    {
        // ズームインアウトボタンにイベントを設定する
        m_buttonZoomIn = zoomInBtn;
        m_buttonZoomIn.GetComponent<Button>().onClick.AddListener(() => ZoomIn());
        m_buttonZoomOut = zoomOutBtn;
        m_buttonZoomOut.GetComponent<Button>().onClick.AddListener(() => ZoomOut(0.3f));

        // ゲームのパネルUIを非表示にする
        GameObject.Find("UiController").GetComponent<UiController>().SetActiveGameUI(false);
    }
}
