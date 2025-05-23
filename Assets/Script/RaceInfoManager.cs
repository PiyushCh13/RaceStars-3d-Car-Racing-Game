﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceInfoManager : MonoBehaviour
{
    public static RaceInfoManager instance;
    public string trackToLoad;
    public Car racerToUse;
    public int noOfAI;
    public int noOfLaps;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;


            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
