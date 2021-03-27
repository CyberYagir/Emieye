using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMap : MonoBehaviour
{
    public Image spriteRenderer;
    public WorldManager manager;


    private void FixedUpdate()
    {

        if (GetComponentInParent<PlayerInventory>().GetAllItemsByName("Map").Count != 0)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = manager.maps[PlayerPrefs.GetInt("Map")].placeMap;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
}
