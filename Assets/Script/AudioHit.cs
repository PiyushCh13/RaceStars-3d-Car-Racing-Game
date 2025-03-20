using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHit : MonoBehaviour
{

    public AudioSource soundHit;
    public int groundLayer = 8;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.layer != 8)
        {

            soundHit.Stop();
            soundHit.pitch = Random.Range(0.8f, 1.2f);
            soundHit.Play();
        }

    }


}
