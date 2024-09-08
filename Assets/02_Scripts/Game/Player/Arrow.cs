using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour
{
    // 始点
    Vector3 startMousePos;

    // 始点との距離の最大値
    const float disMax = 2.5f;
    // 初期値
    const float startSizeX = 4f;
    const float startSizeY = 1f;

    // プレイヤーオブジェクト
    GameObject m_player;
    // 軌道予測線
    GameObject m_lineGuide;

    #region パラメータ
    public bool isKick;    // 蹴ることが可能かどうか
    public float dis;      // 蹴るときの力の大きさ
    public Vector3 dir;    // 蹴るときの方角
    #endregion

    private void Start()
    {
        // オブジェクトを検索する
        m_lineGuide = GameObject.Find("LineController");
        m_player = GameObject.Find("Player");

        // 描画off
        transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        // 始点を設定
        startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isKick = false;
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            // 矢印の初期化処理
            transform.localScale = new Vector3(startSizeX, startSizeY, 0);
            // 描画off
            transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (Input.GetMouseButton(0))
        {
            // マウスのワールド座標を取得する
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 始点との距離
            dis = Vector2.Distance(startMousePos, mousePos);

            // 引いた距離が一定以下で蹴ることができない場合
            if (dis < 0.4f)
            {
                // 母親のアニメーションを再生する
                m_player.GetComponent<MomAnimController>().PlayIdleAnim();  // アブノーマルスキンのIdleアニメ

                // 描画off
                transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                // isKickをfalse
                isKick = false;
                // シュミレーションの描画OFF
                m_lineGuide.GetComponent<SimulationController>().vecKick = Vector3.zero;
                return;
            }

            // 蹴ることが可能な距離まで引っ張った場合(1度しか中に入らない)
            if(isKick == false)
            {
                // 母親のアニメーションを再生する
                m_player.GetComponent<MomAnimController>().PlayReadyAnim();  // 蹴る姿勢のアニメ
            }

            // isKickをtrue
            isKick = true;

            // 描画on
            transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;

            //-------------------------------------
            // 距離に応じて矢印の大きさを調整する
            //-------------------------------------

            // disの最大値、最小値を超えた場合
            dis = dis > disMax ? disMax : dis;
            dis = dis < 0f ? 0f : dis;

            transform.localScale = new Vector3(startSizeX - dis, startSizeY + dis);

            //-------------------------------------
            // 矢印をカーソルのある方向に向かせる
            //-------------------------------------

            // 向きたい方向を計算
            dir = (startMousePos - mousePos);

            // ここで向きたい方向に回転
            transform.rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(dir.x, dir.y, 0f));

            //--------------------------------------------
            // 軌道予測線を描画するパラメータを設定する
            //--------------------------------------------
            m_lineGuide.GetComponent<SimulationController>().enabled = true;
            m_lineGuide.GetComponent<SimulationController>().vecKick = dir.normalized * dis * m_player.GetComponent<Player>().m_mulPower;
        }
    }
}
