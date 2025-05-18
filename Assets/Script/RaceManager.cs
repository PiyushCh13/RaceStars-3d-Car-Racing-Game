using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public Checkpoints[] allCheckpoints;
    public static RaceManager instance;
    public int totalLaps;
    public Car playerCar;
    public List<Car> allAICar = new List<Car>();
    public int playerPos;
    public bool isStarted;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countdownCurrent = 3;
    public float aidefaultspeed = 20f, playerdefaultspeed = 35f, rubberbandSpeedMod = 3.5f, rubberbandAcc = 3.5f;

    public int playerStartPosition, aiNumbertoSpawn;
    public Transform[] startPoints;
    public List<Car> carsToSpawn = new List<Car>();
    public string raceCompletedScene;
    public bool raceCompleted;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        totalLaps = RaceInfoManager.instance.noOfLaps;
        aiNumbertoSpawn = RaceInfoManager.instance.noOfAI;
        playerCar = RaceInfoManager.instance.racerToUse;


        for (int i = 0; i > allCheckpoints.Length; i++)
        {
            allCheckpoints[i].cpNu = i;
        }


        isStarted = true;
        startCounter = timeBetweenStartCount;

        UIManager.instance.countdownText.text = countdownCurrent + "!";


        playerStartPosition = Random.Range(0, aiNumbertoSpawn + 1);


        playerCar = Instantiate(RaceInfoManager.instance.racerToUse, startPoints[playerStartPosition].position, startPoints[playerStartPosition].rotation);
        playerCar.isAI = false;
        playerCar.GetComponent<AudioListener>().enabled = true;

        CameraSwitch.instance.SetTarget(playerCar);

        playerCar.transform.position = startPoints[playerStartPosition].position;
        playerCar.theRB.transform.position = startPoints[playerStartPosition].position;



        for (int i = 0; i < aiNumbertoSpawn; i++ )
        {
            if(i != playerStartPosition)
            {
                int selectedCar = Random.Range(0, carsToSpawn.Count);
                allAICar.Add(Instantiate(carsToSpawn[selectedCar], startPoints[i].position, startPoints[i].rotation));

                if(carsToSpawn.Count <= aiNumbertoSpawn - i)
                {
                    carsToSpawn.RemoveAt(selectedCar);
                }

               
            }
        }

        UIManager.instance.carPosition.text = (playerStartPosition + 1) + "/" + (allAICar.Count + 1);

    }

    // Update is called once per frame
    void Update()
    {
       
        if (isStarted)
        {
            startCounter -= Time.deltaTime;

            if (startCounter <= 0)
            {
                countdownCurrent--;
                startCounter = timeBetweenStartCount;

                UIManager.instance.countdownText.text = countdownCurrent + "!";

                if (countdownCurrent == 0)
                {
                    isStarted = false;
                    UIManager.instance.countdownText.gameObject.SetActive(false);
                    UIManager.instance.goText.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            playerPos = 1;

            foreach (Car aicar in allAICar)
            {
                if (aicar.currentLap > playerCar.currentLap)
                {
                    playerPos++;
                }
                else if (aicar.currentLap == playerCar.currentLap)
                {
                    if (aicar.nextCheckpoint > playerCar.nextCheckpoint)
                    {
                        playerPos++;
                    }
                    else if (aicar.nextCheckpoint == playerCar.nextCheckpoint)
                    {
                        if (Vector3.Distance(aicar.transform.position, allCheckpoints[aicar.nextCheckpoint].transform.position) < Vector3.Distance(playerCar.transform.position, allCheckpoints[playerCar.nextCheckpoint].transform.position))
                        {
                            playerPos++;
                        }
                    }
                }

                UIManager.instance.carPosition.text = playerPos + "/" + (allAICar.Count + 1);

            }
            
            if (playerPos == 1)
            {

                foreach (Car aicar in allAICar)
                {
                    aicar.maxSpeed = Mathf.MoveTowards(aicar.maxSpeed, aidefaultspeed + (rubberbandSpeedMod/2), rubberbandAcc * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerdefaultspeed - rubberbandSpeedMod, rubberbandAcc * Time.deltaTime);
            }

            else
            {
                foreach (Car aicar in allAICar)
                {
                    aicar.maxSpeed = Mathf.MoveTowards(aicar.maxSpeed, aidefaultspeed + rubberbandSpeedMod, rubberbandAcc * Time.deltaTime);
                }


                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerdefaultspeed + rubberbandSpeedMod, rubberbandAcc * Time.deltaTime);
            }
        }
     
    }
    
    public void RaceFinished()
    {
        raceCompleted = true;



        switch (playerPos)
        {
            case 1:
                
             UIManager.instance.raceFinishResult.text = "You finished 1st";

                break;


            case 2:

                UIManager.instance.raceFinishResult.text = "You finished 2nd";

                break;


            case 3:

                UIManager.instance.raceFinishResult.text = "You finished 3rd";

                break;


            default:

                UIManager.instance.raceFinishResult.text = "You finished " + playerPos + "th";

                break;


        }

        UIManager.instance.resultScreen.SetActive(true);
    }


    public void ExitRace()
    {
        SceneManager.LoadScene(raceCompletedScene);
    }


  



}


