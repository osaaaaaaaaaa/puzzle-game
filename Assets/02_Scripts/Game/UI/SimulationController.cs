using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationController : MonoBehaviour
{
    Scene m_simulationScene;                        // シュミレーション用シーン
    PhysicsScene2D m_physicsScene;                  // 物理演算の再生
    [SerializeField] Transform m_obstacleParent;    // ステージの障害物

    [SerializeField] LineRenderer m_line;
    [SerializeField] int m_iMaxPhysicsFrame;    // ラインを描画するフレーム数

    #region 息子関係
    [SerializeField] Transform m_tfSonPos;      // シュミレーション開始地点
    [SerializeField] GameObject m_sonPrefab;    // シュミレーション対象プレファブ
    public Vector2 m_sonVelocity;               // 対象オブジェクトの移動ベクトル
    #endregion

    void Start()
    {
        m_sonVelocity = Vector2.zero;

        // シュミレーションで使用するシーンを作成する
        CreatePhysicsScene();
    }

    void Update()
    {
        // まだ蹴っていない && 移動ベクトルが0以外のとき
        if (!GetComponent<Player>().m_isKicked && m_sonVelocity != Vector2.zero)
        {
            Simulation(m_sonPrefab, m_tfSonPos.position, m_sonVelocity);
        }
        else
        {
            // 軌道予測線をリセット
            m_line.GetComponent<LineRenderer>().positionCount = 0;
        }
    }

    /// <summary>
    /// シュミレーションで使用するシーンを作成する
    /// </summary>
    void CreatePhysicsScene()
    {
        m_simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        m_physicsScene = m_simulationScene.GetPhysicsScene2D();

        // シュミレーション用シーンにオブジェクト(ブロックなど障害物になるもの)を生成する
        foreach (Transform tf in m_obstacleParent)
        {
            var ghost = Instantiate(tf.gameObject, tf.position, tf.rotation);
            // 非表示にする
            ghost.GetComponent<Renderer>().enabled = false;
            // オブジェクトを指定したシーンへ移動する
            SceneManager.MoveGameObjectToScene(ghost, m_simulationScene);
        }
    }

    /// <summary>
    /// 軌道予測線を描画する
    /// </summary>
    /// <param name="_ballPrefab">シュミレーション対象オブジェクト</param>
    /// <param name="_pos"></param>
    /// <param name="_velocity">移動量</param>
    void Simulation(GameObject _ballPrefab, Vector2 _pos, Vector2 _velocity)
    {
        // 息子のゴースト作成(非表示にする)
        var ghost = Instantiate(_ballPrefab, _pos, Quaternion.identity);
        ghost.GetComponent<Renderer>().enabled = false;
        // オブジェクトをシュミレーション用シーンへ移動する
        SceneManager.MoveGameObjectToScene(ghost.gameObject, m_simulationScene);
        ghost.GetComponent<Rigidbody2D>().AddForce(_velocity, ForceMode2D.Impulse);     // ※シュミレーションシーンに移動させてから力を加える

        //---------------------------------------------------------
        // 指定したフレーム数の間でどのくらい動いたかの軌道を作成
        //---------------------------------------------------------
        m_line.positionCount = m_iMaxPhysicsFrame;  // フレーム数分の配列を生成

        for (int i = 0; i < m_iMaxPhysicsFrame; i++)
        {
            m_physicsScene.Simulate(Time.fixedDeltaTime);       // 物理演算を指定秒数進める
            m_line.SetPosition(i, ghost.transform.position);    // ラインオブジェクトの座標(頂点？)を追加する
        }

        // シュミレーション終了でゴーストを破棄
        Destroy(ghost.gameObject);
    }
}