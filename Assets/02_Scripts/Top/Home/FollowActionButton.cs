using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowActionButton : MonoBehaviour
{
    [SerializeField] Sprite m_texActive;    // 繧｢繧ｯ繝��ぅ繝悶↑縺ｨ縺阪��逕ｻ蜒�
    [SerializeField] Sprite m_texPassive;   // 繝代ャ繧ｷ繝悶↑縺ｨ縺阪��逕ｻ蜒�
    GameObject m_userController;

    // true:繧｢繧ｯ繝��ぅ繝也憾諷� , false:繝代ャ繧ｷ繝也憾諷�
    bool m_isActive;
    int m_userID;

    public void Init(GameObject uiUserManager, int user_id)
    {
        m_userController = uiUserManager;
        m_userID = user_id;

        // 繧｢繧ｯ繝��ぅ繝悶↑縺ｨ縺阪��逕ｻ蜒上ｒ險ｭ螳壹＠縺ｦ縺��ｋ蝣ｴ蜷�
        if (transform.GetComponent<Image>().sprite == m_texActive)
        {
            m_isActive = true;
        }
        // 繝代ャ繧ｷ繝悶↑縺ｨ縺阪��逕ｻ蜒上ｒ險ｭ螳壹＠縺ｦ縺��ｋ蝣ｴ蜷�
        else
        {
            m_isActive = false;
        }
    }

    public void OnTaskButton()
    {
        SEManager.Instance.PlayButtonSE();
        if (m_isActive)
        {
            m_userController.GetComponent<UIUserManager>().ActionFollow(m_isActive, m_userID,transform.gameObject);
        }
        else
        {
            m_userController.GetComponent<UIUserManager>().ActionFollow(m_isActive, m_userID,transform.gameObject);
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
