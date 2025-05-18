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

    void Start()
    {
        totalLaps = RaceInfoManager.instance.noOfLaps;
        aiNumbertoSpawn = RaceInfoManager.instance.noOfAI;

        for (int i = 0; i < allCheckpoints.Length; i++)
        {
            allCheckpoints[i].cpNu = i;
        }

        playerStartPosition = Random.Range(0, startPoints.Length);
        playerCar = Instantiate(RaceInfoManager.instance.racerToUse, startPoints[playerStartPosition].position, startPoints[playerStartPosition].rotation);
        playerCar.isAI = false;
        playerCar.GetComponent<AudioListener>().enabled = true;

        CameraSwitch.instance.SetTarget(playerCar);

        playerCar.theRB.transform.position = startPoints[playerStartPosition].position;

        int aiSpawned = 0;
        for (int i = 0; i < startPoints.Length && aiSpawned < aiNumbertoSpawn; i++)
        {
            if (i == playerStartPosition) continue;

            int selectedCar = Random.Range(0, carsToSpawn.Count);
            Car aiCar = Instantiate(carsToSpawn[selectedCar], startPoints[i].position, startPoints[i].rotation);
            aiCar.isAI = true;
            allAICar.Add(aiCar);

            if (carsToSpawn.Count <= aiNumbertoSpawn - aiSpawned)
            {
                carsToSpawn.RemoveAt(selectedCar);
            }

            aiSpawned++;
        }

        UIManager.instance.carPosition.text = (playerStartPosition + 1) + "/" + (allAICar.Count + 1);

        isStarted = true;
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (countdownCurrent > 0)
        {
            UIManager.instance.countdownText.text = countdownCurrent + "!";
            yield return new WaitForSeconds(timeBetweenStartCount);
            countdownCurrent--;
        }

        UIManager.instance.countdownText.gameObject.SetActive(false);
        UIManager.instance.goText.gameObject.SetActive(true);
        isStarted = false;
    }

    void Update()
    {
        if (!isStarted && !raceCompleted)
        {
            UpdatePlayerPosition();
            ApplyRubberbanding();
        }
    }

    void UpdatePlayerPosition()
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
                    if (Vector3.Distance(aicar.transform.position, allCheckpoints[aicar.nextCheckpoint].transform.position) <
                        Vector3.Distance(playerCar.transform.position, allCheckpoints[playerCar.nextCheckpoint].transform.position))
                    {
                        playerPos++;
                    }
                }
            }
        }

        UIManager.instance.carPosition.text = playerPos + "/" + (allAICar.Count + 1);
    }

    void ApplyRubberbanding()
    {
        if (playerPos == 1)
        {
            foreach (Car aicar in allAICar)
            {
                aicar.maxSpeed = Mathf.MoveTowards(aicar.maxSpeed, aidefaultspeed + (rubberbandSpeedMod / 2), rubberbandAcc * Time.deltaTime);
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

    public void RaceFinished()
    {
        raceCompleted = true;
        UIManager.instance.raceFinishResult.text = GetPlacementText(playerPos);
        UIManager.instance.resultScreen.SetActive(true);
    }

    string GetPlacementText(int position)
    {
        return position switch
        {
            1 => "You finished 1st",
            2 => "You finished 2nd",
            3 => "You finished 3rd",
            _ => $"You finished {position}th"
        };
    }

    public void ExitRace()
    {
        SceneManager.LoadScene(raceCompletedScene);
    }
}
