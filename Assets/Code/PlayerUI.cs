using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public Transform inventoryItem, inventoryHolder;
    public InvetoryItem info;
    public Attack attack;

    public Image main, second, hat, body, pants, damage, underWater, dead;
    public TMP_Text deadButtonText;
    public Button deadButton, savebutton;
    [Space]
    public GameObject inventoryW;
    public GameObject interactInfo;
    [Space]
    public Item mouseItem;
    public Image mouseItemImage;
    public List<PlaceInHand> placeInHands = new List<PlaceInHand>();
    public PlayerStats playerStats;
    float deadTime, saveTime;

    private void Update()
    {
        if (savebutton.gameObject.active == false)
        {
            saveTime = 0;
        }
        else
        {
            saveTime += 1 * Time.deltaTime;
            if (saveTime > 10) { savebutton.enabled = true; savebutton.GetComponentInChildren<TMP_Text>().text = "Save"; }
            else
            {
                savebutton.enabled = false;
                savebutton.GetComponentInChildren<TMP_Text>().text = "Save: " + (int)(10 - saveTime);
            }
        }
        if (attack.cam.transform.position.y <= 15.555f)
        {
            underWater.gameObject.SetActive(true);
        }
        else
        {
            underWater.gameObject.SetActive(false);
        }

        if (playerStats.health <= 1)
        {
            playerStats.dead = true;
            deadTime += 1 * Time.deltaTime;
            if (deadTime < 5)
            {
                deadButton.enabled = false;
                deadButtonText.text = "Respawn: " + (5 - (int)deadTime);
            }
            else
            {
                deadButton.enabled = true;
                deadButtonText.text = "Respawn";
            }
            transform.parent = null;
            transform.Rotate(Vector3.up, 10 * Time.deltaTime);
            GetComponent<Attack>().secondAnimator.gameObject.SetActive(false);
            dead.gameObject.SetActive(true);
            GetComponent<PlayerMove>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }
        else
        {
            dead.gameObject.SetActive(false);
            deadTime = 0;
        }

            if (mouseItem != null && !UIOn())
        {
            mouseItemImage.rectTransform.position = Input.mousePosition;
            mouseItemImage.gameObject.SetActive(true);
            mouseItemImage.sprite = mouseItem.sprite;
            mouseItemImage.rectTransform.sizeDelta = new Vector2(100 * mouseItem.horSize, 100);
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                playerInventory.AddItem(mouseItem);
                mouseItem = null;
            }
        }
        else
        {
            mouseItemImage.gameObject.SetActive(false);
        }

        main.enabled = playerInventory.main != null;
        second.enabled = playerInventory.second != null;
        hat.enabled = playerInventory.hat != null;
        body.enabled = playerInventory.torso != null;
        pants.enabled = playerInventory.pants != null;

        damage.color = new Color(1, 1, 1,  1 - ((GetComponent<PlayerStats>().health + 20) / 100));


        if (main.enabled)
        {
            main.sprite = playerInventory.main.sprite;
        }
        if (second.enabled)
        {
            second.sprite = playerInventory.second.sprite;
        }

        if (hat.enabled)
        {
            hat.sprite = playerInventory.hat.sprite;
        }
        if (body.enabled)
        {
            body.sprite = playerInventory.torso.sprite;
        }
        if (pants.enabled)
        {
            pants.sprite = playerInventory.pants.sprite;
        }

        main.rectTransform.sizeDelta = new Vector2(100 * (playerInventory.main != null ? playerInventory.main.horSize: 0), 100);
        second.rectTransform.sizeDelta = new Vector2(100 * (playerInventory.second != null ? playerInventory.second.horSize : 0), 100);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            playerInventory.audioSource.PlayOneShot(playerInventory.openInv);
            inventoryW.active = !inventoryW.active;
            for (int i = 0; i < placeInHands.Count; i++)
            {
                placeInHands[i].over = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryW.active)
            {
                playerInventory.audioSource.PlayOneShot(playerInventory.openInv);
            }
            inventoryW.active = false;
            for (int i = 0; i < placeInHands.Count; i++)
            {
                placeInHands[i].over = false;
            }
        }


        if (attack.item != null)
        {
            info.image.sprite = attack.item.sprite;
            info.name.text = attack.item.name;
            info.image.rectTransform.sizeDelta = new Vector2(info.startSize.x * attack.item.horSize, info.image.rectTransform.sizeDelta.y);

            if (attack.item.canReload)
            {
                info.info.text = attack.item.bulletsIn + "/" + attack.item.bulletsMax;
            }
            else
            {
                info.info.text = "";
            }
        }
        else
        {
            info.name.text = "";
            info.info.text = "";
        }
    }

    public void UpdateInventory()
    {
        foreach (Transform item in inventoryHolder)
        {
            Destroy(item.gameObject);
        }
        int weight = 0;
        for (int i = 0; i < playerInventory.items.Count; i++)
        {
            var obj = Instantiate(inventoryItem, inventoryHolder).GetComponent<InvetoryItem>();
            obj.gameObject.active = true;
            obj.image.sprite = playerInventory.items[i].sprite;
            obj.name.text = playerInventory.items[i].name;
            obj.idInv = i;
            obj.image.rectTransform.sizeDelta = new Vector2(obj.startSize.x * playerInventory.items[i].horSize, obj.image.rectTransform.sizeDelta.y);
            weight += playerInventory.items[i].weight;
            if (playerInventory.items[i].ItemType == Item.type.weapon)
            {
                obj.info.text = "Damage: " + playerInventory.items[i].dmg + "~" + playerInventory.items[i].maxdmg + "\n" +
                    "Speed: " + playerInventory.items[i].shootOutTime;
                if (playerInventory.items[i].bullets != null)
                {
                    obj.info.text += "\nBullets: " + playerInventory.items[i].bullets.name;
                }
            }
            if (playerInventory.items[i].ItemType == Item.type.eat)
            {
                obj.info.text = "You can eat/drink this";
            }
            if (playerInventory.items[i].ItemType == Item.type.bullets)
            {
                if (playerInventory.items[i].bulletsMax != 0)
                {
                    obj.info.text = "What a clip with cartridges.\nBullets: " + playerInventory.items[i].bulletsIn + "/" + playerInventory.items[i].bulletsMax;
                }
            }
            if (playerInventory.items[i].ItemType == Item.type.use)
            {
                obj.info.text = "You can use this";

            }
            if (playerInventory.items[i].ItemType == Item.type.head || playerInventory.items[i].ItemType == Item.type.pants || playerInventory.items[i].ItemType == Item.type.torso)
            {
                obj.info.text = "Clothing item";
            }
        }
        playerInventory.currWeight = weight;
    }

    public void SaveClick()
    {
        if (saveTime >= 10)
        {
            FindObjectOfType<Save_Load>().Save();
            StartCoroutine(saveClick());
        }
    }
    IEnumerator saveClick()
    {
        savebutton.GetComponent<Image>().color = new Color(0, 0.4f, 0, 0.4f);
        yield return new WaitForSeconds(1f);
        savebutton.GetComponent<Image>().color = new Color(0, 0.0f, 0, 0.4f);
        yield break;
    }

    public static bool UIOn()
    {
        return (FindObjectsOfType<OverWindow>().ToList().Count == 0);
    }
}
