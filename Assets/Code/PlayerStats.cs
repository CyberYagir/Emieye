using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float health, oxy;
    float healthOld;
    public float energy, water, hunger, radiation;
    public float energysub, watersub, hungersub, radiationsub;
    [Space]
    public Image hungerI, waterI, radiationI, energyI, healthI, oxyI;
    public TMP_Text hungerT, waterT, radiationT, energyT, healthT, oxyT;
    PlayerMove playerMove;
    public bool dead, startResp = true;
    [Space]
    public AudioSource playerDamage;
    public AudioClip[] playerDamages;
    private void Start()
    {
        healthOld = health;

        playerMove = GetComponent<PlayerMove>();
        if (startResp)
            Respawn();
    }

    public void Respawn()
    {
        var inv = GetComponent<PlayerInventory>();
        if (inv.main != null)
        {
            inv.AddItem(inv.main);
            inv.main = null;
        }
        GetComponent<Attack>().item = null;
        GetComponent<Attack>().shootCooldown = false;
        GetComponent<Attack>().reloadCooldown = false;
        if (inv.second != null)
        {
            inv.AddItem(inv.second);
            inv.second = null;
        }
        if (inv.hat != null)
        {
            inv.AddItem(inv.hat);
            inv.hat = null;
        }
        if (inv.torso != null)
        {
            inv.AddItem(inv.torso);
            inv.torso = null;
        }
        if (inv.pants != null)
        {
            inv.AddItem(inv.pants);
            inv.pants = null;
        }

        var c = inv.items.Count;
        for (int i = 0; i < c; i++)
        {
            inv.DropItem(0);
        }
        GetComponent<PlayerUI>().UpdateInventory();
        var all = GameObject.FindGameObjectsWithTag("Respawn");
        GameObject reps = all[Random.Range(0, all.Length)];
        RaycastHit hit;
        if (Physics.Raycast(reps.transform.position, Vector3.down, out hit))
        {
            transform.position = hit.point + new Vector3(0, 2, 0);
        }
        health = 100;
        energy = 100;
        water = 100;
        hunger = 100;
        radiation = 0;
        dead = false;
        GetComponent<PlayerMove>().enabled = true;
        GetComponent<Attack>().secondAnimator.gameObject.SetActive(true);
    } 


    private void FixedUpdate()
    {
        if (dead) return;
        hungerI.rectTransform.sizeDelta = new Vector2(240 * (hunger / 100), 8);
        waterI.rectTransform.sizeDelta = new Vector2(240 * (water / 100), 8);
        radiationI.rectTransform.sizeDelta = new Vector2(240 * (radiation / 100), 8);
        energyI.rectTransform.sizeDelta = new Vector2(240 * (energy / 100), 8);
        healthI.rectTransform.sizeDelta = new Vector2(240 * (health / 100), 8);
        oxyI.rectTransform.sizeDelta = new Vector2(240 * (oxy / 100), 8);

        if (health < healthOld-10)
        {
            if (!playerDamage.isPlaying)
            {
                playerDamage.PlayOneShot(playerDamages[Random.Range(0, playerDamages.Length)]);
            }
            healthOld = health;
        }

        hungerT.text = hunger.ToString("F0") + "%";
        waterT    .text = water.ToString("F0") + "%";
        radiationT.text = radiation.ToString("F0") + "%";
        energyT   .text = energy.ToString("F0") + "%";
        healthT   .text = health.ToString("F0") + "%";
        oxyT   .text = oxy.ToString("F0") + "%";


        water -= watersub * (playerMove.run ? 1.7f : 1);
        hunger -= hungersub * (playerMove.run ? 1.5f : 1);
        radiation += radiationsub;
        oxyI.transform.parent.gameObject.SetActive(playerMove.InWater || oxy < 99);
        if (transform.position.y < 14f)
        {
            oxy -= energysub/2;
            if (oxy <= 0.2f)
            {
                health -= energysub;
            }
        }
        else
        {
            oxy += energysub * 10;
        }
        energy += energysub;

        if (hunger > 70 && water > 60 && radiation < 60)
        {
            health += 0.05f;
        }
       

        if (hunger < 5 || water < 10 || radiation > 90)
        {
            health -= 0.01f;
        }


        if (health >= 100) health = 100;
        if (health <= 0) health = 0;

        if (oxy >= 100) oxy = 100;
        if (oxy <= 0) oxy = 0;


        if (water <= 0) water = 0;
        if (hunger <= 0) hunger = 0;
        if (radiation <= 0) radiation = 0;
        if (energy <= 0) energy = 0;

        if (water >= 100) water = 100;
        if (hunger >= 100) hunger = 100;
        if (radiation >= 100) water = 100;
        if (energy >= 100) energy = 100;
    }


    public void TakeDamage(float damage)
    {
        health -= (damage - GetComponent<PlayerInventory>().armor*0.5f);
    }
}
