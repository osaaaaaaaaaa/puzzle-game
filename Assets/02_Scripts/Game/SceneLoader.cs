using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene("02_UIScene", LoadSceneMode.Additive);
#endif
    }
}
