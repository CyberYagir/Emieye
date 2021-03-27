using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SitInCar : MonoBehaviour
{
    PlayerUI ui;
    public bool over;
    public List<GameObject> parts;
    [Space]
    public GameObject player1;
    public GameObject player2;
    public Transform player1pos, player2pos;
    public ProTruckController truckController;
    public Rigidbody rb;
    [Space]
    public float fuel, fuelSub, health;
    [Space]
    public GameObject canvas;
    public Image fuelI, healthI;
    public TMP_Text fuelT, healthT;
    public bool dead, exploded;
    public float deadTime;
    public Material deadMat;
    public GameObject fire,explode;
    public Material[] randomMats;
    [Space]
    public float zVel;
    public int material = -1;
    public AudioClip engineSound, explodeSound;
    public AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (material == -1)
        {
            material = Random.Range(0, randomMats.Length);
        }
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].GetComponent<Renderer>() != null)
            {
                parts[i].GetComponent<Renderer>().material = randomMats[material];
            }
        }
        health = Random.Range(10, 100);
        fuel = Random.Range(0, 20);
        ui = FindObjectOfType<PlayerUI>();
    }

    public void TakeDamage(float dmg)
    {
        if (dead == false)
        {
            health -= dmg;
            if (health <= 0)
            {
                if (audioSource.clip != null)
                {
                    audioSource.Stop();
                    audioSource.clip = null;
                    audioSource.enabled = true;
                    audioSource.volume = 1f;
                    audioSource.maxDistance = 500;
                }
            }
        }
    }

    private void FixedUpdate()
    {

        if (dead && exploded) { Destroy(canvas.gameObject); Destroy(fire.gameObject); Destroy(this); }
        zVel = transform.InverseTransformDirection(rb.velocity).z;

        RaycastHit hit;
        if (Physics.Raycast(ui.attack.cam.transform.position, ui.attack.cam.transform.forward, out hit, 3))
        {
            if (hit.collider != null)
            {
                if (parts.Find(x => x.transform == hit.transform) != null)
                {
                    ui.interactInfo.active = true;
                    over = true;
                }
                else
                {
                    over = false;
                }
            }
            else
            {
                over = false;
            }
        }
        else
        {
            ui.interactInfo.active = false;
            over = false;
        }
    }
    private void Update()
    {
        if (dead && exploded) return;
      
        if (health < 0) health = 0;

        if (fuel < 0) fuel = 0;
        if (!dead)
        {
            if (truckController.enabled)
            {
                if (fuel > 0)
                {
                    audioSource.volume += 0.2f * Time.deltaTime;
                }
            }
            else
            {
                audioSource.volume -= 0.5f * Time.deltaTime;
            }
            audioSource.enabled = audioSource.volume > 0;
        }
        if (dead && exploded == false)
        {
            fire.SetActive(true);
            deadTime += 1 * Time.deltaTime;
            if (deadTime > 6)
            {
                fire.SetActive(false);
                explode.SetActive(true);
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i].GetComponent<Renderer>() != null)
                    {
                        parts[i].GetComponent<Renderer>().material = deadMat;
                    }
                }
                var pls = FindObjectsOfType<PlayerInventory>().ToList().FindAll(x => Vector3.Distance(transform.position, x.transform.position) < 10);
                for (int i = 0; i < pls.Count; i++)
                {
                    pls[i].GetComponent<PlayerStats>().TakeDamage(100);
                }

                var zombie = FindObjectsOfType<Zombie>().ToList().FindAll(x => Vector3.Distance(transform.position, x.transform.position) < 20);
                for (int i = 0; i < zombie.Count; i++)
                {
                    zombie[i].TakeDamage(100);
                }

                audioSource.PlayOneShot(explodeSound);
                exploded = true;
            }
        }

        if (transform.position.y <= 15.55f)
        {
            health = 0;
        }



        if (player1 != null)
        {
            truckController.SetBrake(0);
            canvas.SetActive(true);

            player1.transform.localEulerAngles = new Vector3(player1pos.localEulerAngles.x, player1.transform.localEulerAngles.y, player1pos.localEulerAngles.z);
            healthI.rectTransform.sizeDelta = new Vector2(240 * (health / 100), 8);
            healthT.text = health.ToString("F0") + "%";
            fuelI.rectTransform.sizeDelta = new Vector2(240 * (fuel / 100), 8);
            fuelT.text = fuel.ToString("F0") + "%";
            if (fuel > 0 && health > 5)
            {
                fuel -= fuelSub * Time.deltaTime;
                truckController.enabled = true;
            }
            else
            {
                truckController.enabled = false;
            }
            //player1.transform.localEulerAngles = transform.localEulerAngles;
            if (player2 != null)
            {
                if (FindObjectOfType<PlayerInventory>().transform == player2)
                {
                    truckController.enabled = false;
                }

                if (Input.GetKeyDown(KeyCode.E) || player2.GetComponent<PlayerStats>().dead)
                {
                    var rb = player2.AddComponent<Rigidbody>();
                    rb.drag = 1;
                    rb.freezeRotation = true;
                    player2.transform.parent = null;
                    player2.transform.localEulerAngles = new Vector3(0, player2.transform.localEulerAngles.y, 0);
                    rb.GetComponent<PlayerMove>().Rigidbody = rb;
                    player2 = null;
                    rb.GetComponent<CapsuleCollider>().enabled = true;
                    rb.GetComponent<Attack>().enabled = true;
                    return;
                }
                player2.transform.localEulerAngles = new Vector3(player2pos.localEulerAngles.x, player2.transform.localEulerAngles.y, player2pos.localEulerAngles.z);

                player2.transform.position = player1pos.position;
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.E) || player1.GetComponent<PlayerStats>().dead)
            {
                var rb = player1.AddComponent<Rigidbody>();
                rb.drag = 1;
                rb.freezeRotation = true;

                player1.transform.parent = null;
                player1.transform.localEulerAngles = new Vector3(0, player1.transform.localEulerAngles.y, 0);
                rb.GetComponent<PlayerMove>().Rigidbody = rb;
                player1 = null;
                rb.GetComponent<CapsuleCollider>().enabled = true;
                rb.GetComponent<Attack>().enabled = true;
                return;
            }
        }
        else
        {
            truckController.enabled = false;

            truckController.SetBrake(100);
            if (transform.parent == null)
            {
                rb.drag = 5;
            }
        }





        if (health == 0)
        {
            dead = true;
            return;
        }



        canvas.SetActive(over);
        if (over)
        {
            if (!dead)
            {
                healthI.rectTransform.sizeDelta = new Vector2(240 * (health / 100), 8);
                healthT.text = health.ToString("F0") + "%";
                fuelI.rectTransform.sizeDelta = new Vector2(240 * (fuel / 100), 8);
                fuelT.text = fuel.ToString("F0") + "%";

                if (Input.GetKeyDown(KeyCode.E))
                {
                    transform.parent = null;
                    FindObjectOfType<PlayerInventory>().GetComponent<Attack>().enabled = false;
                    Destroy(FindObjectOfType<PlayerInventory>().GetComponent<Rigidbody>());
                    FindObjectOfType<PlayerInventory>().GetComponent<CapsuleCollider>().enabled = false;
                    if (player1 == null)
                    {
                        player1 = FindObjectOfType<PlayerInventory>().gameObject;
                        player1.transform.parent = player1pos;
                        player1.transform.position = player1pos.position;
                    }
                    else
                    {
                        player2 = FindObjectOfType<PlayerInventory>().gameObject;
                    }
                }
            }
        }
    }
}
