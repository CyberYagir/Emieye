using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public float health;
    public float agrDist;
    public GameObject agrPlayer;
    public NavMeshAgent meshAgent;
    public bool cooldown, attack;
    public float attackDist;
    public Animator animator;
    public float damage;
    public bool dead;
    public bool back;
    List<PlayerInventory> players;
    public WorldManager worldManager;
    public Town town;
    public bool activate;

    public AudioSource audioSource;
    public AudioClip[] attacks;
    public AudioClip[] deaths;
    public AudioClip[] damages;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
           worldManager = FindObjectOfType<WorldManager>();
        transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        town = GetComponentInParent<Town>();
        health = (int)Random.Range(50, 100);
    }


    public void TakeDamage(float dmg)
    {
        if (dead == true)
        {
            StopAllCoroutines();
            return;
        }
        StopAllCoroutines();
        animator.Play("CoolDown");

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(damages[Random.Range(0, damages.Length)]);
        }
        cooldown = true;
        attack = false;
        health -= dmg;
        StartCoroutine(coolDownWait());
    }

    IEnumerator coolDownWait()
    {
        yield return new WaitForSeconds(0.25f);
        cooldown = false;
    }
    private void FixedUpdate()
    {
        if (dead)
        {
            StopAllCoroutines();
            return;
        }
        if (!dead)
        {
            if (health <= 0)
            {
                GetComponent<CapsuleCollider>().enabled = false;
                print("dead");
                dead = true;
                StopAllCoroutines();
                audioSource.PlayOneShot(deaths[Random.Range(0, deaths.Length)]);
                animator.Play("Death");
                meshAgent.isStopped = true;
                return;
            }
        }
        var players = worldManager.players.FindAll(x => Vector3.Distance(transform.position, x.transform.position) < agrDist / (x.GetComponent<PlayerMove>().moveType == PlayerMove.MoveType.Move ? 1 : (x.GetComponent<PlayerMove>().moveType == PlayerMove.MoveType.Sit ? 2f : 4f)) && Vector3.Distance(town.transform.position, x.transform.position) < town.zombieRadius);
        if (players.Count > 0) activate = true;
        if (activate == false) return;



       if (meshAgent.velocity.sqrMagnitude <= 0)
        {
            if (attack == false && cooldown == false)
            {
                animator.Play("Idle");
            }
        }
        if (!dead)
        {
            if (!attack && !cooldown)
            {
                if (Vector3.Distance(transform.position, town.transform.position) >= town.zombieRadius)
                {
                    if (meshAgent.isOnNavMesh)
                    {
                        back = true;
                        meshAgent.isStopped = false;
                        meshAgent.SetDestination(transform.parent.transform.position);
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                        {
                            animator.Play("Walk");
                        }
                    }
                    StopAllCoroutines();
                    attack = false;
                    cooldown = false;
                }

                float dist = 9999;
                int id = -1;
                for (int i = 0; i < players.Count; i++)
                {
                    float p = Vector3.Distance(players[i].transform.position, transform.position);
                    if (p < dist)
                    {
                        id = i;
                        dist = p;
                    }
                }
                if (id != -1)
                {
                    back = false;
                    if (!cooldown)
                    {
                        agrPlayer = players[id].gameObject;
                        if (Vector3.Distance(transform.position, agrPlayer.transform.position) <= attackDist)
                        {
                            attack = true;
                            StartCoroutine(attackWait(agrPlayer));
                            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                            {
                                animator.Play("Attack");

                                if (!audioSource.isPlaying)
                                {
                                    audioSource.PlayOneShot(attacks[Random.Range(0, attacks.Length)]);
                                }
                            }
                            return;
                        }
                        if (meshAgent.isOnNavMesh)
                        {
                            meshAgent.isStopped = false;
                            meshAgent.SetDestination(agrPlayer.transform.position);
                            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                            {
                                animator.Play("Walk");
                            }
                        }
                    }
                }
                else
                {
                    agrPlayer = null;
                }


            }
            if (attack)
            {
                if (Vector3.Distance(transform.position, town.transform.position) > town.zombieRadius)
                {
                    if (meshAgent.isOnNavMesh)
                    {
                        back = true;
                        meshAgent.isStopped = false;
                        meshAgent.SetDestination(transform.parent.transform.position);
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                        {
                            animator.Play("Walk");
                        }
                    }
                    StopAllCoroutines();
                    attack = false;
                    cooldown = false;
                    return;
                }
                meshAgent.isStopped = true;
            }




        }
        if (back == true)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.Play("Walk");
            }
            if (Vector3.Distance(transform.position, transform.parent.position) < 10)
            {
                back = false;
            }
        }

        
    }

    IEnumerator attackWait(GameObject player)
    {
        yield return new WaitForSeconds(0.84f);
        if (!cooldown)
        {
            //RaycastHit hit;
            //if (Physics.Raycast(transform.position, player.transform.position - transform.position, 2, out hit))
            //{
            //    if (hit != null)
            //    {

            //    }
            //}



            if (Vector3.Distance(transform.position, player.transform.position) <= attackDist)
            {
                player.GetComponent<PlayerStats>().TakeDamage(damage);
            }
        }
         attack = false;
    }
}
