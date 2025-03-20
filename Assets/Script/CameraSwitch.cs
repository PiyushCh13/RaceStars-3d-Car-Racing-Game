using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public static CameraSwitch instance;
     
    public GameObject[] Camera;
    private int currentCam;

    public CameraController topDown;
    public CinemachineVirtualCamera backCam;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCam++;

            if(currentCam >= Camera.Length)
            {
                currentCam = 0;
            }
       
            for(int i = 0; i < Camera.Length; i++)
            {
                if(i == currentCam)
                {
                    Camera[i].SetActive(true);
                }
                else
                {
                    Camera[i].SetActive(false);
                }
            }
        
        }
    }

    public void SetTarget(Car playerCar)
    {
        topDown.target = playerCar;
        backCam.m_Follow = playerCar.transform;
        backCam.m_Follow = playerCar.transform;
        backCam.m_LookAt = playerCar.transform;
    }



}
