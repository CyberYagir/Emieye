using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawner : MonoBehaviour
{
    public List<Item> items;
    public GameObject drop;
    public GameObject spawned;
    public int rare;

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        Destroy(spawned);
        if (Random.Range(0, rare) == 0)
        {
           var gm =  Instantiate(drop, transform.position, Quaternion.identity);
            var drops = gm.GetComponent<Drop>();
            drops.item = Item.Clone(items[Random.Range(0, items.Count)]);
            if (drops.item.ItemType == Item.type.bullets || drops.item.ItemType == Item.type.use)
                drops.item.bulletsIn = Random.Range(0, drops.item.bulletsMax);

            if (drops.item.ItemType == Item.type.weapon)
                drops.item.bulletsIn = Random.Range(0, drops.item.bulletsMax);

            if (drops.item.name == "Arrow")
            {
                drops.item.bulletsIn = 1;
            }

            spawned = gm;
            spawned.transform.name = gm.GetComponent<Drop>().item.name + ": " + transform.GetComponentInParent<Town>().transform.name;
        }
    }

    public void Init(string name, int bulletsIn)
    {
        Destroy(spawned);
        var it = items.Find(x => x.name == name);
        var gm = Instantiate(drop, transform.position, Quaternion.identity);
        var drops = gm.GetComponent<Drop>();
        drops.item = Item.Clone(it);
        drops.item.bulletsIn = bulletsIn;
        spawned = gm;
        spawned.transform.name = gm.GetComponent<Drop>().item.name + ": " + transform.GetComponentInParent<Town>().transform.name;
    }
}
