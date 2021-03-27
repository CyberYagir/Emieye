using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenClose : MonoBehaviour
{
    public float wait;
    public bool reload;
    public bool open = false;
    PlayerUI ui;
    public bool over;
    public string openAnim, closeAnim;
    public List<GameObject> parts;
    public GameObject hitObj;
    public Town town;
    public AudioClip openSound, closeSound;
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        town = GetComponentInParent<Town>();
        open = true;
        ui = FindObjectOfType<PlayerUI>();
    }
    private void FixedUpdate()
    {
        if (ui == null)
        {
            ui = FindObjectOfType<PlayerUI>();
            return;
        }
        if (town.PlayersIn != 0 || town == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(ui.attack.cam.transform.position, ui.attack.cam.transform.forward, out hit, 3))
            {
                if (hit.collider != null)
                {
                    hitObj = hit.transform.gameObject;
                    if (parts.Find(x => x.transform == hit.transform) != null)
                    {
                        print(transform.name + "|" + hitObj.name);
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
    }
    private void Update()
    {
        if (over)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!reload)
                {
                    open = !open;
                    if (open)
                    {
                        GetComponent<Animator>().Play(closeAnim);
                    }
                    else
                    {
                        GetComponent<Animator>().Play(openAnim);
                    }

                    StartCoroutine(waitWait());
                }
            }
        }
    }

    IEnumerator waitWait()
    {
        reload = true;
        yield return new WaitForSeconds(wait/2);
        if (open)
        {
            audioSource.PlayOneShot(openSound);
        }
        else
        {
            audioSource.PlayOneShot(closeSound);
        }
        yield return new WaitForSeconds(wait/2);
        reload = false;
    }
}
