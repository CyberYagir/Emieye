using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ToGroundEditor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        var terrs = GameObject.FindGameObjectsWithTag("Rock");
        for (int i = 0; i < terrs.Length; i++)
        {
            if (terrs[i].transform.position.y < 15)
            {
                DestroyImmediate(terrs[i].gameObject);
            }
        }
    }
}
