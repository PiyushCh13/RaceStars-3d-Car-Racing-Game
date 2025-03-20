using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointChecker : MonoBehaviour
{
    public Car theCar;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Checkpoint")
        {
            theCar.CheckpointHit(other.GetComponent<Checkpoints>().cpNu);
        }
    }
}
