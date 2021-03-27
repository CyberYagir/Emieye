using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Camera cam;

    public Animator animator, secondAnimator, centerUIAnimator;
    public PlayerRigAnimator rigAnimator;

    public Item item = null;
    public Item olditem = null;

    public bool shootCooldown, reloadCooldown, weaponSwitch;
    public Item hands;
    public GameObject leftHand, rightHand;
    public float time;
    int mousekey;
    bool shotbool, waitbool, doptimebool;
    [Space]
    public AudioSource audioSource;
    public AudioClip emptyWeapon;

    AudioClip currAttack;
    private void Start()
    {
        olditem = item;
    }

    private void Update()
    {
        if (!reloadCooldown && !weaponSwitch && !shootCooldown)
        {
            if (Input.GetKey("1"))
            {
                item = GetComponent<PlayerInventory>().main;
            }
            if (Input.GetKey("2"))
            {
                item = GetComponent<PlayerInventory>().second;
            }
        }

        if (shootCooldown)
        {
            time += Time.deltaTime;
            shotWait(item, mousekey);
        }
        
        if (!audioSource.isPlaying)
        {

            audioSource.pitch = 1;
        }



        if (item == null)
        {
            item = hands;
            return;
        }
        if (olditem != item)
        {
            shotbool = false;
            waitbool = false;
            shootCooldown = false;
            time = 0;
            secondAnimator.Play("Switch");
            foreach (Transform item in leftHand.transform)
            {
                Destroy(item.gameObject);
            }
            foreach (Transform item in rightHand.transform)
            {
                Destroy(item.gameObject);
            }
            if (item.leftPrefab != null)
            {
                var l = Instantiate(item.leftPrefab, leftHand.transform);
                l.name = item.leftPrefab.name;
                l.GetComponent<BoxCollider>().enabled = false;
            }
            if (item.rightPrefab != null)
            {
                var r = Instantiate(item.rightPrefab, rightHand.transform);
                r.name = item.rightPrefab.name;
                if (r.GetComponent<BoxCollider>() != null)
                {
                    r.GetComponent<BoxCollider>().enabled = false;
                }
                if (r.GetComponent<MeshCollider>() != null)
                {
                    r.GetComponent<MeshCollider>().enabled = false;
                }
            }
            rigAnimator.SpawnInRig(item);
            olditem = item;
            GetComponent<PlayerUI>().UpdateInventory();
        }
        animator.runtimeAnimatorController = item.runtimeAnimator;

        if (!PlayerUI.UIOn()) return;
        if (!weaponSwitch)
        {
            if (!shootCooldown && !reloadCooldown)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (item.bulletsIn >= item.bulletSub)
                    {

                    }
                    else
                        audioSource.PlayOneShot(emptyWeapon);
                }
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (item.bulletsIn >= item.bulletSub)
                    {
                        shootCooldown = true;
                        currAttack = item.shooSound;
                        mousekey = 0;
                        animator.Play(item.attackAnim);
                        rigAnimator.Play(item.RattackAnim,1);
                        item.bulletsIn -= item.bulletSub;
                        if (item.arrowBullet == true)
                        {
                            StartCoroutine(relWait(item.relTime));
                        }
                    }
                }
                if (item.secondAttack)
                {
                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        if (GetComponent<PlayerStats>().energy > 20)
                        {
                            if (item.bulletsIn >= item.bulletSub)
                            {
                                currAttack = item.shooSound2;
                                shootCooldown = true;
                                mousekey = 1;
                                animator.Play(item.sattackAnim);
                                rigAnimator.Play(item.RsattackAnim, 1);
                                item.bulletsIn -= item.bulletSub;
                                GetComponent<PlayerStats>().energy -= 20;
                            }
                        }
                    }
                }
                if (item.canReload)
                {
                    if (reloadCooldown) return;

                    if (Input.GetKey(KeyCode.R))
                    {
                        animator.Play(item.reloadAnim);
                        rigAnimator.Play(item.RreloadAnim, 1);
                        StartCoroutine(relWait(item.relTime));
                    }
                }
            }
        }
    }
    public void DestroySpawnBullet()
    {
        var allbullets = GetComponent<PlayerInventory>().GetAllItemsByName(item.bullets.name);
    }
    IEnumerator switchWeapon()
    {
        weaponSwitch = true;
        yield return new WaitForSeconds(0.4f);
        weaponSwitch = false;
    }
    public void Shoot(float dmg,float dist)
    {
        audioSource.pitch = item.attackPitch;
        audioSource.PlayOneShot(currAttack);
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, dist))
        {
            if (hit.collider != null)
            {
                if (item.arrowBullet)
                {
                    var arrow = Instantiate(item.bullets.dropPrefab, hit.point, Quaternion.identity);
                    arrow.transform.LookAt(transform.position);

                    arrow.transform.parent = hit.transform;
                    var allbullets = GetComponent<PlayerInventory>().GetAllItemsByName(item.bullets.name);
                    allbullets = allbullets.FindAll(x => x.bulletsIn == 0);
                    for (int i = 0; i < allbullets.Count; i++)
                    {
                        GetComponent<PlayerInventory>().items.Remove(allbullets[i]);
                    }
                    GetComponent<PlayerUI>().UpdateInventory();
                }

                if (hit.transform.GetComponent<Zombie>() != null)
                {
                    centerUIAnimator.Play("Attack");
                    hit.transform.GetComponent<Zombie>().TakeDamage(dmg);
                    return;
                }
                if (hit.transform.GetComponent<SitInCar>() != null)
                {
                    centerUIAnimator.Play("Attack");
                    hit.transform.GetComponent<SitInCar>().TakeDamage(dmg);
                    return;
                }
                if (item.spawndecal == false) return;

                if (item.arrowBullet)
                {
                    return;
                }



                    RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
                cam.targetTexture = rt;
                cam.Render();
                RenderTexture.active = rt;


                var texture = new Texture2D(Screen.width, Screen.height);
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
                texture.Apply();
                var decal = Instantiate(FindObjectOfType<WorldManager>().decal, hit.point, Quaternion.identity);

                decal.transform.rotation = Quaternion.LookRotation(hit.normal);
                decal.GetComponentInChildren<Renderer>().material.color = texture.GetPixel(Screen.width / 2, Screen.height / 2);
                decal.GetComponentInChildren<Renderer>().material.color = new Color(decal.GetComponentInChildren<Renderer>().material.color.r, decal.GetComponentInChildren<Renderer>().material.color.g, decal.GetComponentInChildren<Renderer>().material.color.b, 1);
                decal.transform.parent = hit.transform;

                cam.targetTexture = null;
                RenderTexture.active = null; // added to avoid errors 
                DestroyImmediate(rt);
                Destroy(decal, 10);

            }
        }
    }

    IEnumerator relWait(float wait)
    {
        reloadCooldown = true;
        yield return new WaitForSeconds(wait);
        var allbullets = GetComponent<PlayerInventory>().GetAllItemsByName(item.bullets.name);
        allbullets = allbullets.FindAll(x => x.bulletsIn != 0);
        for (int i = 0; i < allbullets.Count; i++)
        {
            if (item.bulletsMax == item.bulletsIn)
            {
                break;
            }
            var count = allbullets[i].bulletsIn;
            for (int j = 0; j < count; j++)
            {
                item.bulletsIn++;
                allbullets[i].bulletsIn--;
                if (allbullets[i].bulletsIn <= 0 || item.bulletsMax == item.bulletsIn)
                {
                    break;
                }
            }
        }
        if (item.arrowBullet)
        {

            allbullets = GetComponent<PlayerInventory>().GetAllItemsByName(item.bullets.name);
            allbullets = allbullets.FindAll(x => x.bulletsIn == 0);
            for (int i = 0; i < allbullets.Count; i++)
            {
                GetComponent<PlayerInventory>().items.Remove(allbullets[i]);
            }
            GetComponent<PlayerUI>().UpdateInventory();
        }
        reloadCooldown = false;
    }
    void shotWait(Item itemlocal, int key = 0)
    {
        animator.speed = item.attackSpeed;
        float wait1 = itemlocal.shootPrevTime;
        float wait2 = itemlocal.shootOutTime;
        float dmg = Random.Range(itemlocal.dmg, itemlocal.maxdmg);
        float dist = itemlocal.dist;
        float secondAttackWait = 0;
        if (key == 1)
        {
            secondAttackWait = item.secondAttackDopTime;
            dmg = Random.Range(item.dmg * 2, item.maxdmg * 2);
        }
        if (shotbool == false)
        {
            if (time >= wait1)
            {
                Shoot(dmg, dist);
                if (item.ItemType == Item.type.head || item.ItemType == Item.type.torso || item.ItemType == Item.type.pants || item.ItemType == Item.type.eat)
                {
                    if (item.ItemType == Item.type.head)
                    {
                        GetComponent<PlayerInventory>().hat = item;
                    }
                    if (item.ItemType == Item.type.torso)
                    {
                        GetComponent<PlayerInventory>().torso = item;
                    }
                    if (item.ItemType == Item.type.pants)
                    {
                        GetComponent<PlayerInventory>().pants = item;
                    }
                    if (item.ItemType == Item.type.eat)
                    {
                        GetComponent<PlayerStats>().hunger += item.hunger;
                        GetComponent<PlayerStats>().water += item.water;
                        GetComponent<PlayerStats>().radiation += item.rad;
                        GetComponent<PlayerStats>().health += item.heal;
                    }


                    if (GetComponent<PlayerInventory>().main == item)
                    {
                        GetComponent<PlayerInventory>().main = null;
                    }
                    if (GetComponent<PlayerInventory>().second == item)
                    {
                        GetComponent<PlayerInventory>().second = null;
                    }
                }
                shotbool = true;
                return;
            }
        }
        if (shotbool == false) return;
        if (waitbool == false)
        {
            if (time >= wait2)
            {
                if (item.ItemType == Item.type.head || item.ItemType == Item.type.torso || item.ItemType == Item.type.pants || item.ItemType == Item.type.eat)
                {
                    print("NULL");
                    item = null;
                }
                if (item != null)
                {
                    if (item.ItemType == Item.type.use)
                    {
                        if (item.name == "Gas")
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, dist))
                            {
                                if (hit.collider != null)
                                {
                                    if (hit.transform.GetComponent<SitInCar>() != null)
                                    {
                                        hit.transform.GetComponent<SitInCar>().fuel += item.bulletsIn;
                                        if (hit.transform.GetComponent<SitInCar>().fuel > 100)
                                        {
                                            hit.transform.GetComponent<SitInCar>().fuel = 100;
                                        }
                                        item.bulletsIn = 0;
                                    }
                                    if (hit.transform.tag == "GasStation")
                                    {
                                        item.bulletsIn = 100;
                                    }
                                }
                            }
                        }

                        if (item.name == "Wrench")
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, dist))
                            {
                                if (hit.collider != null)
                                {
                                    if (hit.transform.GetComponent<SitInCar>() != null)
                                    {
                                        hit.transform.GetComponent<SitInCar>().health += 10;
                                        if (hit.transform.GetComponent<SitInCar>().health > 100)
                                        {
                                            hit.transform.GetComponent<SitInCar>().health = 100;
                                        }
                                    }
                                }
                            }
                        }


                        if (item.useDestroy)
                        {
                            item = null;
                        }
                    }
                }
                waitbool = true;
                return;
            }
        }
        if (waitbool == false) return;

        if (time >= wait1 + wait2 + secondAttackWait)
        {
            shotbool = false;
            waitbool = false;
            shootCooldown = false;
            time = 0;

            animator.speed = 1;
            return;
        }
    }


}
