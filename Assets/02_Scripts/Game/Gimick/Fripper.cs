using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fripper : MonoBehaviour
{
    AudioSource m_audioSource;
    [SerializeField] AudioClip m_flipperSE;

    // Start is called before the first frame update
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ghost") return;

        if(collision.gameObject.layer == 6 || collision.gameObject.layer == 11)
        {
            m_audioSource.PlayOneShot(m_flipperSE);
        }
    }
}
