using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOverTime : MonoBehaviour
{
    public float timetodisable;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timetodisable -= Time.deltaTime;

        if (timetodisable <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
