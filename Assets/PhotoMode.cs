using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoMode : MonoBehaviour
{
    public GameObject canvas, hands, vinniett;
    public bool on = false;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            on = !on;
            if (on == true)
            {
                canvas.SetActive(false);
                hands.SetActive(false);
                vinniett.SetActive(false);
            }
            else
            {
                canvas.SetActive(true);
                hands.SetActive(true);
                vinniett.SetActive(true);
            }
        }
        if (on)
        {
            GetComponent<PlayerStats>().health = 101;
            GetComponent<PlayerStats>().energy = 101;
        }
    }

}
