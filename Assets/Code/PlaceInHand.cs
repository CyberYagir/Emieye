using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceInHand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerUI ui;
    public bool main;
    public int body = -1;

    private void Start()
    {
        ui = FindObjectOfType<PlayerUI>();
    }


    private void Update()
    {
        if (over)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (ui.mouseItem != null)
                {
                    if ((ui.mouseItem.ItemType == Item.type.weapon || ui.mouseItem.ItemType == Item.type.head || ui.mouseItem.ItemType == Item.type.pants || ui.mouseItem.ItemType == Item.type.torso || ui.mouseItem.ItemType == Item.type.eat || ui.mouseItem.ItemType == Item.type.use) && body == -1)
                    {
                        if (ui.mouseItem.ItemType == Item.type.use)
                        {
                            if (ui.mouseItem.leftPrefab == null && ui.mouseItem.rightPrefab == null)
                            {
                                ui.playerInventory.AddItem(ui.mouseItem);
                                ui.mouseItem = null;
                                return;
                            }
                        }

                        if (main)
                        {
                            if (ui.playerInventory.main != null)
                            {
                                ui.playerInventory.AddItem(ui.playerInventory.main);
                            }
                            ui.playerInventory.main = ui.mouseItem;
                            ui.mouseItem = null;
                            return;
                        }
                        if (!main)
                        {
                            if (ui.playerInventory.second != null)
                            {
                               
                                ui.playerInventory.AddItem(ui.playerInventory.second);
                            }
                            ui.playerInventory.second = ui.mouseItem;
                            ui.mouseItem = null;
                            return;
                        }
                    }
                    else if (ui.mouseItem.ItemType == Item.type.head && body == 0)
                    {
                        if (ui.playerInventory.hat != null)
                        {
                            ui.playerInventory.AddItem(ui.playerInventory.hat);
                        }
                        ui.playerInventory.hat = ui.mouseItem;
                        ui.mouseItem = null;
                        return;
                    }
                    else if (ui.mouseItem.ItemType == Item.type.torso && body == 1)
                    {
                        print("Torso");
                        if (ui.playerInventory.torso != null)
                        {
                            ui.playerInventory.AddItem(ui.playerInventory.torso);
                        }
                        ui.playerInventory.torso = ui.mouseItem;
                        ui.mouseItem = null;
                        return;
                    }
                    else if (ui.mouseItem.ItemType == Item.type.pants && body == 2)
                    {
                        if (ui.playerInventory.pants != null)
                        {
                            ui.playerInventory.AddItem(ui.playerInventory.pants);
                        }
                        ui.playerInventory.pants = ui.mouseItem;
                        ui.mouseItem = null;
                        return;
                    }
                    else
                    {
                        ui.playerInventory.AddItem(ui.mouseItem);
                        ui.mouseItem = null;
                        return;
                    }
                }
                else
                {
                    
                    ui.playerInventory.AddItem(ui.mouseItem);
                    ui.mouseItem = null;
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (ui.mouseItem == null)
                {
                    if (body == -1)
                    {
                        if (main)
                        {
                            if (ui.attack.item == ui.playerInventory.main)
                            {
                                ui.attack.item = null;
                            }
                            ui.mouseItem = ui.playerInventory.main;
                            ui.playerInventory.main = null;
                        }
                        if (!main)
                        {
                            if (ui.attack.item == ui.playerInventory.second)
                            {
                                ui.attack.item = null;
                            }
                            ui.mouseItem = ui.playerInventory.second;
                            ui.playerInventory.second = null;
                        }
                    }
                    else if (body == 0)
                    {
                        ui.mouseItem = ui.playerInventory.hat;
                        ui.playerInventory.hat = null;
                    }
                    else if (body == 1)
                    {
                        ui.mouseItem = ui.playerInventory.torso;
                        ui.playerInventory.torso = null;
                    }
                    else if (body == 2)
                    {
                        ui.mouseItem = ui.playerInventory.pants;
                        ui.playerInventory.pants = null;
                    }
                }
            }
        }
    }

    public bool over;

    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
    }
}
