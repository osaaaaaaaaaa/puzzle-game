using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        // UIシーンを追加
        SceneManager.LoadScene("02_UiScene", LoadSceneMode.Additive);
    }
}
