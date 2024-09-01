using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowActionButton : MonoBehaviour
{
    [SerializeField] Sprite m_texActive;    // �A�N�e�B�u�ȂƂ��̉摜
    [SerializeField] Sprite m_texPassive;   // �p�b�V�u�ȂƂ��̉摜
    GameObject m_userController;

    // true:�A�N�e�B�u��� , false:�p�b�V�u���
    bool m_isActive;
    int m_userID;

    public void Init(GameObject userController, int user_id)
    {
        m_userController = userController;
        m_userID = user_id;

        // �A�N�e�B�u�ȂƂ��̉摜��ݒ肵�Ă���ꍇ
        if (transform.GetComponent<Image>().sprite == m_texActive)
        {
            m_isActive = true;
        }
        // �p�b�V�u�ȂƂ��̉摜��ݒ肵�Ă���ꍇ
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
