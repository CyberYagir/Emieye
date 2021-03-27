using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvetoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public TMP_Text name, info;
    public int id, idInv;
    public Vector2 startSize;
    public PlayerUI ui;
    public bool over;
    public GameObject dropButton;

    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
    }
    public void Drop()
    {
        ui.playerInventory.DropItem(idInv);
        ui.UpdateInventory();
    }
    private void Start()
    {
        ui = FindObjectOfType<PlayerUI>();
        startSize = image.rectTransform.sizeDelta;
    }

    private void Update()
    {
        dropButton.SetActive(over);
        image.enabled = image.sprite != null;
        if (over)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ui.mouseItem = ui.playerInventory.items[idInv];
                ui.playerInventory.RemoveItem(ui.playerInventory.items[idInv]);
            }
        }
    }
}
