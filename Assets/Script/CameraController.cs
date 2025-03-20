using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Car target;
    private Vector3 offsetDir;
    public float minDist,maxDist;
    private float activeDist;
    public Transform startOffset;

    // Start is called before the first frame update
    void Start()
    {

        offsetDir = transform.position - startOffset.position;
        activeDist = minDist;
        offsetDir.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        activeDist = minDist + ((maxDist - minDist) * target.theRB.linearVelocity.magnitude / target.maxSpeed);
        transform.position = target.transform.position + offsetDir * activeDist;
    }
}
