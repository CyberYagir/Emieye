using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLight : MonoBehaviour
{
    public GameObject light;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (transform.parent != null)
            {
                if (transform.parent.GetComponent<Drop>() == null)
                {
                    light.active = !light.active;
                }
            }
        }
    }
}
