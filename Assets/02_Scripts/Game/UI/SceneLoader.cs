using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        // UI�V�[����ǉ�
        SceneManager.LoadScene("02_UiScene", LoadSceneMode.Additive);
    }
}
