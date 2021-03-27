using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject car;
    public int rare = 20;
    public void Start()
    {

        transform.position += new Vector3(0, 1000, 0);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            transform.position = hit.point + new Vector3(0,5,0);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    public void Init()
    {
        if (FindObjectsOfType<PlayerInventory>().ToList().FindAll(x => Vector3.Distance(transform.position, x.transform.position) < 100).Count != 0) return; 
        if (FindObjectsOfType<SitInCar>().ToList().FindAll(x => Vector3.Distance(transform.position, x.transform.position) < 30).Count == 0)
        {
            if (car != null)
            {
                if (car.transform.parent == null)
                {
                    car = null;
                }
                Destroy(car.gameObject);
            }
            int id = Random.Range(0, cars.Length);
            car = Instantiate(cars[id], transform.position, Quaternion.identity);
            car.name = cars[id].name;
            car.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            car.transform.parent = transform;
        }
    }

    public void Init(string name, Vector3 pos, Vector3 rot, int colour)
    {
        if (car != null)
        {
            if (car.transform.parent == null)
            {
                car = null;
            }
            Destroy(car.gameObject);
        }

        car = Instantiate(cars.ToList().Find(x => x.name == name).gameObject, pos, Quaternion.identity);
        car.GetComponent<SitInCar>().material = colour;
        car.name = cars.ToList().Find(x => x.name == name).transform.name;
        car.transform.localEulerAngles = rot;
        car.transform.parent = transform;
    }
}
