using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationController : MonoBehaviour
{
    Scene m_simulationScene;            // シュミレーション用シーン
    PhysicsScene2D m_physicsScene;      // 物理演算の再生
    Transform m_obstacleParent;         // ステージの障害物

    [SerializeField] LineRenderer m_line;
    const int m_iMaxPhysicsFrame = 5;    // ラインを描画するフレーム数

    #region 息子関係
    [SerializeField] GameObject m_somPrefab;           // シュミレーション対象オブジェクト
    [SerializeField] GameObject m_rideCowPrefab;       // シュミレーション対象オブジェクト
    GameObject m_son;             // 息子
    GameObject m_ride_cow;        // 牛に乗った息子
    public Vector3 vecKick;
    #endregion

    GameObject m_player;
    [SerializeField] bool m_isTargetGuest = false;

    private void Awake()
    {
        vecKick = Vector3.zero;

        // オブジェクトを取得する
        m_obstacleParent = GameObject.Find("ObstacleParent").transform;
        m_player = GameObject.Find("Player");
        m_son = GameObject.Find("Son");
        m_ride_cow = GameObject.Find("ride_cow");

        // シュミレーションで使用するシーンを作成する
        CreatePhysicsScene();
    }

    void Update()
    {
        if (m_isTargetGuest) return;

        // まだ蹴っていない && ベクトルが0以外のとき
        if (!m_player.GetComponent<Player>().m_isKicked && vecKick != Vector3.zero)
        {
            // 通常スキンの息子がアクティブの場合
            if (m_son.activeSelf)
            {
                Simulation(m_son.transform.position);
            }
            // 牛に乗った息子がアクティブの場合
            else if (m_ride_cow.activeSelf)
            {
                Simulation(m_ride_cow.transform.position);
            }
        }
        else
        {
            // 軌道予測線をリセット
            m_line.GetComponent<LineRenderer>().positionCount = 0;
        }
    }

    /// <summary>
    /// シュミレーションで使用するシーンを作成する(生成したシーンにブロックなど障害物になるもを生成する)
    /// </summary>
    void CreatePhysicsScene()
    {
        // シーンが既に存在するかチェック
        Scene scene = SceneManager.GetSceneByName("Simulation");
        if (scene.IsValid())
        {
            m_simulationScene = scene;
            m_physicsScene = m_simulationScene.GetPhysicsScene2D();
            return;
        }

        m_simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        m_physicsScene = m_simulationScene.GetPhysicsScene2D();

        // 親オブジェクトを複製する
        var parent = Instantiate(m_obstacleParent.gameObject, m_obstacleParent.position, m_obstacleParent.rotation);

        // シュミレーション用シーンにオブジェクト(ブロックなど障害物になるもの)を生成する
        foreach (Transform tf in parent.transform)
        {
            // コンポーネントが存在する場合
            if (tf.gameObject.GetComponent<Renderer>() != null)
            {
                // 非表示にする
                tf.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }

        // 親オブジェクトを指定したシーンへ移動する
        SceneManager.MoveGameObjectToScene(parent, m_simulationScene);
    }

    /// <summary>
    /// 軌道予測線を描画する
    /// </summary>
    /// <param name="_somPrefab">シュミレーション対象オブジェクト</param>
    /// <param name="_pos"></param>
    /// <param name="_velocity">移動量</param>
    public void Simulation(Vector2 _pos)
    {
        GameObject _somPrefab;
        // 通常スキンの息子がアクティブの場合
        if (m_son.activeSelf)
        {
            _somPrefab = m_somPrefab;
        }
        // 牛に乗った息子がアクティブの場合
        else
        {
            _somPrefab = m_rideCowPrefab;
        }

        // 息子のゴースト作成(非表示にする)
        var ghost = Instantiate(_somPrefab, _pos, Quaternion.identity);
        ghost.tag = "Ghost";
        if (ghost.GetComponent<Renderer>() != null)
        {
            ghost.GetComponent<Renderer>().enabled = false;
        }

        // オブジェクトをシュミレーション用シーンへ移動する
        SceneManager.MoveGameObjectToScene(ghost.gameObject, m_simulationScene);

        // 通常スキンの息子がアクティブの場合
        if (m_son.activeSelf)
        {
            // ※シーンに移動させてから力を加える
            ghost.GetComponent<Son>().DOKick(vecKick, true);
        }
        // 牛に乗った息子がアクティブの場合
        else
        {
            // ※シーンに移動させてから力を加える
            ghost.GetComponent<SonCow>().DOKick(vecKick);
        }

        //---------------------------------------------------------
        // 指定したフレーム数の間でどのくらい動いたかの軌道を作成
        //---------------------------------------------------------
        m_line.positionCount = m_iMaxPhysicsFrame;  // フレーム数分の配列を生成

        for (int i = 0; i < m_iMaxPhysicsFrame; i++)
        {
            // ゴーストの中心座標を取得する
            var offsetCollider = ghost.GetComponent<BoxCollider2D>() != null ? ghost.GetComponent<BoxCollider2D>().offset : Vector2.zero;
            Vector3 pivot = m_son.activeSelf ? ghost.transform.position 
                : ghost.transform.position + new Vector3(offsetCollider.x, offsetCollider.y, 0f);

            // 描画開始
            m_physicsScene.Simulate(Time.fixedDeltaTime);       // 物理演算を指定秒数進める
            m_line.SetPosition(i, pivot);    // ラインオブジェクトの座標(頂点？)を追加する
        }

        // シュミレーション終了でゴーストを破棄
        Destroy(ghost.gameObject);
    }
}