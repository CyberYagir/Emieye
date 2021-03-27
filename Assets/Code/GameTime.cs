using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public Light sun;
    public float time, intence;
    public float timespeed;
    public bool day;
    public Vector3 fogcolouradd;
    private void Update()
    {
        time += timespeed * Time.deltaTime;
        if (time >= 100)
        {
            time = 0;
        }
        if (day)
        {
            if (time < 20)
            {
                if (time < 50)
                {
                    intence += timespeed * Time.deltaTime;
                }
            }
            if (time >= 20)
            {
                intence = 20;
                day = false;
            }
        }
        if (!day)
        {
            if (time > 0)
            {
                if (time > 40)
                {
                    intence -= timespeed * Time.deltaTime;
                }
            }
            if (time <= 0)
            {
                intence = 0;
                   day = true;
            }
        }

        sun.transform.localEulerAngles = new Vector3(360 * ((time+1)/100), 0, 0);
        sun.intensity = (intence * 2 * 2)/100;
        RenderSettings.ambientIntensity = sun.intensity / 2 + 0.2f;
        RenderSettings.fogColor = new Color((sun.intensity / 2)+0.05f + fogcolouradd.x, (sun.intensity / 2) + 0.05f + fogcolouradd.y, (sun.intensity / 2) + 0.05f +fogcolouradd.z);
    }
}
