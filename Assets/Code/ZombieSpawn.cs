using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawn : MonoBehaviour
{
    public GameObject[] zombie;
    public GameObject spawned;
    // Start is called before the first frame update
    void Start()
    {
    }


    public void Init()
    {
        if (Random.Range(0, 2) == 0)
        {
            var gm = Instantiate(zombie[Random.Range(0,zombie.Length)].gameObject, transform.position, Quaternion.identity);
            gm.transform.parent = transform;
            spawned = gm;
        }
    }
}
