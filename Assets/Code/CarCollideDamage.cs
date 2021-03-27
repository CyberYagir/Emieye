using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollideDamage : MonoBehaviour
{
    public SitInCar car;
    public float speed;
    private void Update()
    {
        
    }



    private void OnTriggerEnter(Collider other)
    {
        print("Enter");
        if (car.zVel >= Mathf.Abs(speed))
        {
            if (other.GetComponent<Zombie>() != null)
            {
                other.GetComponent<Zombie>().TakeDamage(100);
            }
            else if (other.GetComponent<PlayerStats>() != null)
            {
                other.GetComponent<PlayerStats>().TakeDamage(100);
            }
            else
            {
                car.TakeDamage(Mathf.Abs(speed));
            }
        }
        else
        {
            if (car.zVel >= Mathf.Abs(speed) / 2)
            {
                if (other.GetComponent<Zombie>() != null)
                {
                    other.GetComponent<Zombie>().TakeDamage(Random.Range(40, 60));
                }
                else if (other.GetComponent<PlayerStats>() != null)
                {
                    other.GetComponent<PlayerStats>().TakeDamage(Random.Range(40, 60));
                }
                else
                {
                    car.TakeDamage(Mathf.Abs(speed));
                }
            }
        }
    }
}
