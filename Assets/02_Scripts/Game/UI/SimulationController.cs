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
    const int m_iMaxPhysicsFrame = 8;    // ラインを描画するフレーム数

    #region 息子関係
    [SerializeField] GameObject m_somPrefab;           // シュミレーション対象オブジェクト
    GameObject m_som;             // 息子
    public Vector3 m_kickDir;     // 対象オブジェクトのベクトル
    public float m_kickPower;     // 力の大きさ
    #endregion

    GameObject m_player;

    void Start()
    {
        // 初期化
        m_kickDir = Vector3.zero;
        m_kickPower = 0;

        // オブジェクトを取得する
        m_som = GameObject.Find("Son");
        m_player = GameObject.Find("Player");

        // シュミレーションで使用するシーンを作成する
        CreatePhysicsScene();
    }

    void Update()
    {
        // まだ蹴っていない && 移動ベクトルが0以外のとき
        if (!m_player.GetComponent<Player>().m_isKicked && m_kickDir != Vector3.zero)
        {
            Simulation(m_somPrefab, m_som.transform.position, m_kickDir,m_kickPower);
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
    /// <param name="_somPrefab">シュミレーション対象オブジェクト</param>
    /// <param name="_pos"></param>
    /// <param name="_velocity">移動量</param>
    void Simulation(GameObject _somPrefab, Vector2 _pos, Vector3 _dir, float _power)
    {
        // 息子のゴースト作成(非表示にする)
        var ghost = Instantiate(_somPrefab, _pos, Quaternion.identity);
        ghost.GetComponent<Renderer>().enabled = false;
        // オブジェクトをシュミレーション用シーンへ移動する
        SceneManager.MoveGameObjectToScene(ghost.gameObject, m_simulationScene);

        //=====================================
        // ※シーンに移動させてから力を加える
        //=====================================
        var rb = ghost.GetComponent<Rigidbody2D>();
        rb.drag = ghost.GetComponent<Son>().m_dragNum;  // 空気抵抗を設定
        rb.velocity = transform.forward * ghost.GetComponent<Son>().m_initialSpeed; // 速度を設定
        Vector3 force = new Vector3(_dir.x * _power, _dir.y * _power);  // 力を設定

        // 力を加える
        rb.AddForce(force, ForceMode2D.Impulse);

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