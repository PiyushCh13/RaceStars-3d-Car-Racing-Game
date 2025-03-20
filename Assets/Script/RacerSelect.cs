using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RacerSelect : MonoBehaviour
{

    public Image racerImage;

    public Car racertoSet;

    public void SelectRacer()

    {
        RaceInfoManager.instance.racerToUse = racertoSet;

        MainMenu.instance.racerSelectImage.sprite = racerImage.sprite;

        MainMenu.instance.CloseRacerSelect();

    }



}
