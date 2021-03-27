using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public Item item;
    public GameObject spawnedPrefab;
    PlayerUI ui;
    public bool over;
    public float time;
    bool started;
    private void Start()
    {
        Init();
        ui = FindObjectOfType<PlayerUI>();
    }
    public void FixedUpdate()
    {
        if (spawnedPrefab != null)
        {
            if (started == false)
            {
                time += 1 * Time.deltaTime;
                if (time >= 10)
                {
                    GetComponent<Rigidbody>().isKinematic = true;
                    
                    started = true;
                    transform.gameObject.isStatic = true;
                }
            }
        }
    }
    private void Update()
    {
        if (over)
        {
            if (Vector3.Distance(transform.position, ui.transform.position) <= 3)
            {
                ui.interactInfo.active = true;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (FindObjectOfType<PlayerInventory>().AddItem(item))
                {
                    ui.interactInfo.active = false;
                    Destroy(gameObject);
                }
            }
        }
    }

    public void Init()
    {
        Destroy(spawnedPrefab);
        GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10)) * 10 * Time.deltaTime, ForceMode.Impulse);
        if (spawnedPrefab == null)
        {
            spawnedPrefab = Instantiate(item.dropPrefab, transform.position, Quaternion.identity, transform);
            spawnedPrefab.gameObject.layer = 0;
            foreach (Transform item in spawnedPrefab.transform)
            {
                item.gameObject.layer = 0;
            }
            spawnedPrefab.transform.parent = transform;
        }
    }

    private void OnMouseEnter()
    {
        over = true;
    }

    private void OnMouseExit()
    {
        ui.interactInfo.active = false;
        over = false;
    }

}
