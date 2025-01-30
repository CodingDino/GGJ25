using PuzzleBobble;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlienMeter : MonoBehaviour
{
    public float maxMeter;
    private float curMeter = 0;

    public Slider meter;

    public BubbleMatrix sideToSpawn;


    public void AddToMeter(float points)
    {
        curMeter += points;
        if (curMeter >= maxMeter)
        {
            curMeter -=maxMeter;
            // TODO: Celebration?

            sideToSpawn.SpawnMonster();
        }
        meter.value = curMeter / maxMeter;
    }

}
