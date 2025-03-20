using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float maxSpeed;
    public float forwardAcceleration = 8f, backwardAcceleration = 4f;
    public float speedInput;
    public Rigidbody theRB;
    public float turnInput;
    public float turnStrength = 180f;
    public float currentSpeed;
    public Transform leftfrontwheel, rightfrontwheel;
    public float maxwheelTurn;
    public ParticleSystem[] smoke;
    public float maxEmission = 25f, emissonfadeSpeed = 20;
    private float emissionRate;
    public AudioSource engineSound;
    public AudioSource skidSound;
    public int skidfadeSpeed;
    public int nextCheckpoint;
    public int currentLap;
    public float lapTime, bestLapTime;
    public bool isAI;
    public int currentTarget;
    private Vector3 targetPoint;
    public float aiAccelerateSpeed = 1f;
    public float aiturnSpeed = 0.8f ;
    public float aireachPoint = 5f;
    public float aipointVariance = 3f;
    private float aisppedInput , aispeedmod;
    public float aimaxturn = 15f;
    public float aimaxspeed = 25f;
    public float resetCooldown = 5f;
    private float resetCounter;

    // Start is called before the first frame update
    void Start()
    {

        resetCounter = resetCooldown;

        UIManager.instance.LapText.text = currentLap + "/" + RaceManager.instance.totalLaps;

        if (isAI)
        {
            targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
            randomizeAItarget();

            aispeedmod = Random.Range(0.1f, 0.2f);
        }

    }

    // Update is called once per frame
    void Update()
    {
 
         
        if (!RaceManager.instance.isStarted)
        {
            


            if (!isAI)
            {
                lapTime += Time.deltaTime;

                var ts = System.TimeSpan.FromSeconds(lapTime);
                UIManager.instance.currentlapTime.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);
            }
            else
            {
                

            }


            currentSpeed = theRB.linearVelocity.magnitude;
           
         
        }

       
    }



    private void FixedUpdate()
    {
        if (!RaceManager.instance.isStarted)
        {
            if (!isAI)
            {
                CarMovement();
                CarSteering();
            }
            else
            {
                AiCarSteering();
                AICarMovement();
            }

            CarForce();
            AICarForce();

        }      
    }


    public void AICarForce()
    {
        theRB.AddForce(transform.forward * aisppedInput);

        if (theRB.linearVelocity.magnitude > aimaxspeed)
        {
            theRB.linearVelocity = theRB.linearVelocity.normalized * aimaxspeed;
        }
    }

    public void CarForce()
    {
        theRB.AddForce(transform.forward * speedInput);

        if (theRB.linearVelocity.magnitude > maxSpeed)
        {
            theRB.linearVelocity = theRB.linearVelocity.normalized * maxSpeed;
        }
    }

    public void AICarMovement()
    {
        targetPoint.y = transform.position.y;

        if (Vector3.Distance(transform.position, targetPoint) < aireachPoint)
        {
            SetNextAITarget();
        }

        Vector3 targetdir = targetPoint - transform.position;
        float angle = Vector3.Angle(targetdir, transform.forward);

        Vector3 localPos = transform.InverseTransformPoint(targetPoint);


        if (localPos.x < 0f)
        {
            angle = -angle;
        }


        turnInput = Mathf.Clamp(angle / aimaxturn, -1f, 1f);

        if (Mathf.Abs(angle) < aimaxturn)
        {
            aisppedInput = Mathf.MoveTowards(aisppedInput, 1f, aiAccelerateSpeed);
        }
        else
        {
            aisppedInput = Mathf.MoveTowards(aisppedInput, aiturnSpeed, aiAccelerateSpeed);
        }


        aisppedInput = 7f;
        speedInput = aisppedInput * forwardAcceleration * aispeedmod * Time.deltaTime;
    }

    public void CarMovement()
    {
        speedInput = 0f;

        if (Input.GetAxis("Vertical") > 0)
        {
            
            speedInput = Input.GetAxis("Vertical") * forwardAcceleration * Time.deltaTime * 1.5f;
        }

        else if (Input.GetAxis("Vertical") < 0)
        {
            
            speedInput = Input.GetAxis("Vertical") * backwardAcceleration * Time.deltaTime * 1.5f;
        }

        leftfrontwheel.localRotation = Quaternion.Euler(leftfrontwheel.localRotation.eulerAngles.x, (turnInput * maxwheelTurn) - 180, leftfrontwheel.localRotation.eulerAngles.z);
        rightfrontwheel.localRotation = Quaternion.Euler(rightfrontwheel.localRotation.eulerAngles.x, (turnInput * maxwheelTurn), rightfrontwheel.localRotation.eulerAngles.z);

        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissonfadeSpeed * Time.deltaTime);



        if (resetCounter > 0)
        {
            resetCounter -= Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.R) && resetCounter <= 0)
        {
            Resettotrack();
        }
        

        if((Mathf.Abs(turnInput) > .5f || theRB.linearVelocity.magnitude < 0.5f && theRB.linearVelocity.magnitude != 0))
        {
            emissionRate = maxEmission;
        }

        if(theRB.linearVelocity.magnitude <= 0.5f)
        {
            emissionRate = 0;
        }



        for ( int i = 0; i < smoke.Length; i++)
        {
            var emissionModule = smoke[i].emission;
            emissionModule.rateOverTime = emissionRate;

        }

        if (engineSound != null) 
        {
            engineSound.pitch = 1f + ((theRB.linearVelocity.magnitude / maxSpeed) * 2f);
        }


        if (skidSound != null)
        {
            if(Mathf.Abs(turnInput) > 0.5f)
            {
                skidSound.volume = 1f;
            }
            else
            {
                skidSound.volume = Mathf.MoveTowards(skidSound.volume, 0f ,skidfadeSpeed);
            }
        }


    }
   
    public void CarSteering()
    {

        turnInput = Input.GetAxis("Horizontal");

        if (speedInput != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.linearVelocity.magnitude / maxSpeed), 0f));
        }


    }


    public void AiCarSteering()
    {
        if (aisppedInput != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(aisppedInput) * (theRB.linearVelocity.magnitude / aimaxspeed), 0f));
        }
    }


    public void CheckpointHit(int cpNu)
    {
        if(cpNu == nextCheckpoint)
        {
            nextCheckpoint++;

            if(nextCheckpoint == RaceManager.instance.allCheckpoints.Length)
            {
                nextCheckpoint = 0;
                LapCompleted();
            }
        }
        if (isAI)
        {
            if (cpNu == currentTarget)
            {
                SetNextAITarget();
            }
        }
    }

    public void SetNextAITarget()
    {
        currentTarget++;

        if (currentTarget >= RaceManager.instance.allCheckpoints.Length)
        {
            currentTarget = 0;
        }

        targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
        randomizeAItarget();
    }

    public void LapCompleted()
    {
        currentLap++;

        if (lapTime < bestLapTime || bestLapTime == 0)
        {
            bestLapTime = lapTime;
        }

        if(currentLap <= RaceManager.instance.totalLaps)
        {
            lapTime = 0f;

            if (!isAI)
            {
                var ts = System.TimeSpan.FromSeconds(bestLapTime);
                UIManager.instance.bestlapTime.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

                UIManager.instance.LapText.text = currentLap + "/" + RaceManager.instance.totalLaps;
            }

        }
        else
        {
            if (!isAI)
            {
                isAI = true;
                aispeedmod = 1f;
                targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
                randomizeAItarget();

                var ts = System.TimeSpan.FromSeconds(bestLapTime);
                UIManager.instance.bestlapTime.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

                RaceManager.instance.RaceFinished();
            }
        }
    }

    public void randomizeAItarget()
    {
        targetPoint += new Vector3(Random.Range(-aipointVariance ,aipointVariance),0, (Random.Range(-aipointVariance, aipointVariance)));
    }

    public void Resettotrack()
    {
        int pointtoGoto = nextCheckpoint - 1;

        if(pointtoGoto < 0)
        {
            pointtoGoto = RaceManager.instance.allCheckpoints.Length - 1;
        }

        transform.position = RaceManager.instance.allCheckpoints[pointtoGoto].transform.position;
        theRB.transform.position = transform.position;
        theRB.linearVelocity = Vector3.zero;

        speedInput = 0f;
        turnInput = 0f;


        resetCounter = resetCooldown;

    }

}
