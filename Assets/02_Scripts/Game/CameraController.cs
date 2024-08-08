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
    Vector3 m_offsetSon;                        // カメラと息子との一定距離
    const float m_posY_Max = 3f;                // 息子が画面外に出そうになる高さ
    #endregion

    // プレイヤー
    Player m_player;

    // ズームインアウトボタン
    GameObject m_buttonZoomIn;
    GameObject m_buttonZoomOut;

    // カメラのモード
    enum CAMERAMODE
    {
        ZOOMIN = 0,
        ZOOMOUT,
    }

    CAMERAMODE m_cameraMode;

    // カメラの追尾が可能かどうか
    bool m_isFollow;

    // Start is called before the first frame update
    void Start()
    {
        // オブジェクトを取得する
        m_son = GameObject.Find("Son");

        // 息子のポジションとカメラのオフセット
        var tmp_offset = m_mainCamera.transform.position - m_son.transform.position;
        m_offsetSon = new Vector3(tmp_offset.x, m_mainCamera.transform.position.y, m_mainCamera.transform.position.z);

        // カメラの初期位置を取得
        m_startCameraPos = m_mainCamera.transform.position;

        // モード変更
        m_cameraMode = CAMERAMODE.ZOOMIN;
        // 追尾OFF
        m_isFollow = false;

        // ズームインアウトボタンにイベントを設定する
        m_buttonZoomIn = GameObject.Find("ButtonZoomIn");
        m_buttonZoomIn.GetComponent<Button>().onClick.AddListener(() => ZoomIn());
        m_buttonZoomOut = GameObject.Find("ButtonZoomOut");
        m_buttonZoomOut.GetComponent<Button>().onClick.AddListener(() => ZoomoOut());

        // プレイヤーを取得する
        m_player = GameObject.Find("Player").GetComponent<Player>();
    }

    /// <summary>
    /// ズームアウト処理
    /// </summary>
    public void ZoomoOut()
    {
        // モード変更
        m_cameraMode = CAMERAMODE.ZOOMOUT;

        // Tween破棄する
        DOTween.Kill(m_mainCamera.transform);

        //Sequence生成
        var sequence = DOTween.Sequence();
        //Tweenをつなげる
        sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMax, 0.3f))
                .Join(m_mainCamera.transform.DOMove(m_camera_movePos, 0.3f));

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

        // Tween破棄する
        DOTween.Kill(m_mainCamera.transform);

        //Sequence生成
        var sequence = DOTween.Sequence();

        // プレイヤーが蹴る前の場合
        if (!m_player.m_isKicked)
        {
            //Tweenをつなげる
            sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMin, 1f))
                    .Join(m_mainCamera.transform.DOMove(m_startCameraPos, 1f)
                    .OnComplete(() => { m_isFollow = true; }));
        }
        // 蹴り飛ばした後の場合
        else
        {
            // Tween終了後に追尾させる
            sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMin, 0.3f))
                    .Join(m_mainCamera.transform.DOMove(new Vector3(m_son.transform.position.x + m_offsetSon.x, m_offsetSon.y, -10f), 0.3f)
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
        // カメラのモードがズームインになっている場合
        if (m_cameraMode == CAMERAMODE.ZOOMIN && m_isFollow)
        {        
            // 息子とのオフセットを設定
            Vector3 offset = m_son.transform.position.y < m_posY_Max ? m_offsetSon : new Vector3(m_offsetSon.x, m_son.transform.position.y - m_posY_Max, m_offsetSon.z);

            // カメラの視野と追従を設定
            m_mainCamera.transform.position = new Vector3(m_son.transform.position.x + offset.x, offset.y, -10f);
            m_mainCamera.GetComponent<Camera>().orthographicSize = m_cameraSizeMin;

            // カメラが範囲外にでるのを阻止する
            if (m_mainCamera.transform.position.x <= m_startCameraPos.x)
            {
                m_mainCamera.transform.position = m_startCameraPos;
            }
        }
    }
}
