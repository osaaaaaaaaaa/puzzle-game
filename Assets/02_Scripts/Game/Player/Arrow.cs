using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // 始点
    Vector3 startMousePos;

    // 始点との距離の最大値
    const float disMax = 2.5f;
    // 初期値
    const float startSizeX = 4f;
    const float startSizeY = 1f;

    #region パラメータ
    public bool isKick;    // 蹴ることが可能かどうか
    public float dis;      // 蹴るときの力の大きさ
    public Vector3 dir;    // 蹴るときの方角
    #endregion

    // シュミレーションコントローラー
    public SimulationController m_simulationController;

    private void Start()
    {
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

            if (dis < 0.4f)
            {
                // 描画off
                transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                // isKickをfalse
                isKick = false;
                // シュミレーションの描画OFF
                m_simulationController.m_sonVelocity = Vector2.zero;
                return;
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

            //-------------------------------------
            // 軌道予測線を描画する
            //-------------------------------------
            float power = dis * 10;
            m_simulationController.enabled = true;
            m_simulationController.m_sonVelocity = new Vector2(dir.normalized.x * power, dir.normalized.y * power);
        }
    }
}
