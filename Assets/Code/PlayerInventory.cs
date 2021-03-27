using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int maxWeight, currWeight;
    public GameObject dropPrefab;
    public Item main, second;

    public Item hat, torso, pants;
    public float armor;
    public MonoBehaviour globalFog;
    public AudioSource audioSource;
    public AudioClip addInv, openInv;
    private void Update()
    {
        float arm = 0;
        if (hat != null)
        {
            arm += hat.armor;
        }
        if (torso != null)
        {
            arm += torso.armor;
        }
        if (pants != null)
        {
            arm += pants.armor;
        }
        armor = arm;
    }

    public void SetMain(int invId)
    {
        main = items[invId];
        items.RemoveAt(invId);
        GetComponent<PlayerUI>().UpdateInventory();
    }
    public void SetSecons(int invId)
    {
        second = items[invId];
        items.RemoveAt(invId);
        GetComponent<PlayerUI>().UpdateInventory();
    }
    private void Start()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = Item.Clone(items[i]);
        }
        GetComponent<PlayerUI>().UpdateInventory();
    }

    public bool AddItem(Item item)
    {
        if (item != null)
        {
            if (item.weight + currWeight <= maxWeight)
            {
                items.Add(item);

                GetComponent<PlayerUI>().UpdateInventory();
                audioSource.PlayOneShot(addInv);
                return true;
            }
        }
        return false;
    }

    public void GetItem(int id)
    {
    }
    public void RemoveItem(Item item)
    {
        audioSource.PlayOneShot(addInv);
        items.Remove(item);
        GetComponent<PlayerUI>().UpdateInventory();
    }
    public List<Item> GetAllItemsByName(string name)
    {
        return items.FindAll(x => x.name == name);
    }

    public void DropItem(int invID)
    {
        audioSource.PlayOneShot(addInv);
        Instantiate(dropPrefab, transform.position, Quaternion.identity, null).GetComponent<Drop>().item = items[invID];
        items.RemoveAt(invID);
        
    }
}
