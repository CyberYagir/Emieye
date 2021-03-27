using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public List<Item> allItems;
    public List<GameObject> cars;
    public GameObject decal;
    public GameObject drop;
    public List<PlayerInventory> players;
    public GameObject localPlayer;
    public Map[] maps;

    public GameObject[] dotDestroyOnLoad;
    public void FixedUpdate()
    {
        players = FindObjectsOfType<PlayerInventory>().ToList();

    }
    [System.Serializable]
    public class Map
    {
        public string name;
        public Sprite placeMap;
        public int mapID;
    }
    public void Exit()
    {
        for (int i = 0; i < dotDestroyOnLoad.Length; i++)
        {
            Destroy(dotDestroyOnLoad[i].gameObject);
        }
        Application.LoadLevel(0);
    }
    public Item GetByName(string name)
    {
        var it = allItems.Find(x => x.name == name);
        if (it != null)
        {
            return Item.Clone(it);
        }
        return null;
    }
    public Item GetById(int id)
    {
        try
        {
            return allItems[id];
        }
        catch (System.Exception)
        {
            return null;
        }
    }
}
