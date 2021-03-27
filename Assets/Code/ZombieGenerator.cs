using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGenerator : MonoBehaviour
{
    public Material[] materials, eyesmaterials;
    public GameObject headHolder;
    public Renderer[] hats;
    public Renderer torso, pants;
    public Renderer[] eyes;


    private void Start()
    {
        torso.material = materials[Random.Range(0, materials.Length)];
        pants.material = materials[Random.Range(0, materials.Length)];
        int hatid = Random.Range(0, hats.Length);
        for (int i = 0; i < hats.Length; i++)
        {
            if (hatid != i)
            {
                hats[i].gameObject.SetActive(false);
            }
            else
            {
                hats[i].gameObject.SetActive(true);
                hats[i].material = materials[Random.Range(0, materials.Length)];
            }
        }
        var ideyes = Random.Range(0, eyesmaterials.Length);
        for (int i = 0; i < eyes.Length; i++)
        {
            eyes[i].material = eyesmaterials[ideyes];
            eyes[i].gameObject.SetActive(false);
        }
        int eyesCount = Random.Range(1, 4);

        if (eyesCount == 1)
        {
            var left = Random.Range(0, 2);
            if (left == 0)
            {
                eyes[0].gameObject.SetActive(true);
            }
            else
            {
                eyes[1].gameObject.SetActive(true);
            }
        }else
        if (eyesCount == 2)
        {
            eyes[0].gameObject.SetActive(true);
            eyes[1].gameObject.SetActive(true);
        }
        else
        {
            eyes[0].gameObject.SetActive(true);
            eyes[1].gameObject.SetActive(true);
            eyes[2].gameObject.SetActive(true);
        }
    }

}
