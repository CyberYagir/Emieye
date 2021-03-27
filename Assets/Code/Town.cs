using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Town : MonoBehaviour
{
    public List<DropSpawner> dropSpawn;
    public List<Zombie> zombies;
    public List<ZombieSpawn> zombiesSpawns;
    public List<CarSpawn> carsSpawns;
    public List<OpenClose> openCloses;

    public int oldPlayerIn, PlayersIn;
    public int oldRespPlayerIn;
    public bool drawGizmoz;
    public float townRadius;
    public float zombieRadius;
    public float fullRespawnDist;
    public WorldManager worldManager;
    private void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();
        Init();
        var repawnDist = worldManager.players.FindAll(x => Vector3.Distance(transform.position, x.transform.position) <= fullRespawnDist);

        if (repawnDist.Count != oldRespPlayerIn)
        {
            FullRespawn(repawnDist);
            oldRespPlayerIn = repawnDist.Count;
        }
        if (repawnDist.Count != 0)
        {
            var players = worldManager.players.FindAll(x => Vector3.Distance(transform.position, x.transform.position) < townRadius);
            PlayersIn = players.Count;
            if (PlayersIn != oldPlayerIn)
            {
                Set(players);
                oldPlayerIn = PlayersIn;
            }
        }


        oldRespPlayerIn = repawnDist.Count;
    }
    public void Init()
    {
        dropSpawn = GetComponentsInChildren<DropSpawner>().ToList();
        zombiesSpawns = GetComponentsInChildren<ZombieSpawn>().ToList();
        openCloses = GetComponentsInChildren<OpenClose>().ToList();
        carsSpawns = GetComponentsInChildren<CarSpawn>().ToList();
    }
    private void FixedUpdate()
    {
        var repawnDist = worldManager.players.FindAll(x => Vector3.Distance(transform.position, x.transform.position) <= fullRespawnDist);
        if (repawnDist.Count != oldRespPlayerIn)
        {
            FullRespawn(repawnDist);
            oldRespPlayerIn = repawnDist.Count;
        }
        if (repawnDist.Count != 0)
        {
            var players = worldManager.players.FindAll(x => Vector3.Distance(transform.position, x.transform.position) < townRadius);
            PlayersIn = players.Count;
            if (PlayersIn != oldPlayerIn)
            {
                Set(players);
                oldPlayerIn = PlayersIn;
            }
        }

    }
    public void FullRespawn(List<PlayerInventory> players)
    {
        if (players.Count == 0)
        {
            //FULLRESPAWN
            for (int i = 0; i < dropSpawn.Count; i++)
            {
                if (dropSpawn[i].spawned != null)
                {
                    Destroy(dropSpawn[i].spawned.gameObject);
                }
            }
            for (int i = 0; i < zombiesSpawns.Count; i++)
            {
                if (zombiesSpawns[i].spawned != null)
                {
                    Destroy(zombiesSpawns[i].spawned.gameObject);
                }
            }
            for (int i = 0; i < openCloses.Count; i++)
            {
                openCloses[i].enabled = false;
            }
            for (int i = 0; i < carsSpawns.Count; i++)
            {
                if (carsSpawns[i].car != null)
                {
                    if (carsSpawns[i].car.transform.parent != null)
                    {
                        Destroy(carsSpawns[i].car.gameObject);
                    }
                }
            }
        }
    }
    public void Set(List<PlayerInventory> players)
    {
        print("Players: " + players.Count);
        if (players.Count == 0)
        {
            for (int i = 0; i < dropSpawn.Count; i++)
            {
                if (dropSpawn[i].spawned != null)
                {
                    dropSpawn[i].spawned.gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < zombiesSpawns.Count; i++)
            {
                if (zombiesSpawns[i].spawned != null)
                {
                    zombiesSpawns[i].spawned.GetComponent<Zombie>().animator.Play("Idle");
                    zombiesSpawns[i].spawned.gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < openCloses.Count; i++)
            {
                openCloses[i].enabled = false;
            }
            for (int i = 0; i < carsSpawns.Count; i++)
            {
                if (carsSpawns[i].car != null)
                {
                    if (carsSpawns[i].car.transform.parent != null)
                    {
                        carsSpawns[i].car.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            //V GORODE
            for (int i = 0; i < carsSpawns.Count; i++)
            {
                if (carsSpawns[i].car == null)
                {
                    carsSpawns[i].Init();
                }
                else
                {
                    if (carsSpawns[i].car.transform.parent != null)
                    {
                        carsSpawns[i].car.gameObject.SetActive(true);
                    }
                }
            }
            for (int i = 0; i < openCloses.Count; i++)
            {
                openCloses[i].enabled = true;
            }
            for (int i = 0; i < dropSpawn.Count; i++)
            {
                if (dropSpawn[i].spawned == null)
                {
                    dropSpawn[i].Init();
                }
                else
                {
                    dropSpawn[i].spawned.gameObject.SetActive(true);
                }
            }
            for (int i = 0; i < zombiesSpawns.Count; i++)
            {
                if (zombiesSpawns[i].spawned == null)
                    zombiesSpawns[i].Init();
                else
                {
                    zombiesSpawns[i].spawned.SetActive(true);
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (drawGizmoz)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, townRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, zombieRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, fullRespawnDist);
        }
    }
}
