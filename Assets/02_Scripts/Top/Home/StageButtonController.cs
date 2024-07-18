using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButtonController : MonoBehaviour
{
    [SerializeField] TopManager manager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("TopManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
