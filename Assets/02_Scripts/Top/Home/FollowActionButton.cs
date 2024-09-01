using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowActionButton : MonoBehaviour
{
    [SerializeField] Sprite m_texActive;    // アクティブなときの画像
    [SerializeField] Sprite m_texPassive;   // パッシブなときの画像
    GameObject m_userController;

    // true:アクティブ状態 , false:パッシブ状態
    bool m_isActive;
    int m_userID;

    public void Init(GameObject userController, int user_id)
    {
        m_userController = userController;
        m_userID = user_id;

        // アクティブなときの画像を設定している場合
        if (transform.GetComponent<Image>().sprite == m_texActive)
        {
            m_isActive = true;
        }
        // パッシブなときの画像を設定している場合
        else
        {
            m_isActive = false;
        }
    }

    public void OnTaskButton()
    {
        if (m_isActive)
        {
            m_userController.GetComponent<UserController>().ActionFollow(m_isActive, m_userID,transform.gameObject);
        }
        else
        {
            m_userController.GetComponent<UserController>().ActionFollow(m_isActive, m_userID,transform.gameObject);
        }
    }

    public void Invert()
    {
        m_isActive = !m_isActive;

        if (m_isActive)
        {
            transform.GetComponent<Image>().sprite = m_texActive;
        }
        else
        {
            transform.GetComponent<Image>().sprite = m_texPassive;
        }
    }
}
