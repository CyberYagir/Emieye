using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigAnimator : MonoBehaviour
{
    public Animator rigAnimator;
    public Rigidbody rb;
    public Attack attack;
    public Transform spine;
    public GameObject lhand, rhand;

    public void Update()
    {
        spine.eulerAngles = attack.cam.transform.eulerAngles;
        if (rb != null)
        {
            if (rb.velocity.magnitude >= 0.1f)
            {
                if (rigAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    rigAnimator.Play("Run", 0);
                if (rigAnimator.GetCurrentAnimatorStateInfo(1).IsTag("Idle"))
                    if (attack.reloadCooldown == false && attack.shootCooldown == false)
                        rigAnimator.Play(attack.item.RmoveAnim, 1);
            }
            else
            {
                rigAnimator.Play("Idle", 0);
                if (attack.reloadCooldown == false && attack.shootCooldown == false)
                {
                    rigAnimator.Play(attack.item.RidleAnim, 1);
                }
            }
        }
        else
        {
            rb = GetComponent<Rigidbody>();
        }
    }


    public void Play(string name, int id)
    {
        rigAnimator.Play(name, id);
    }

    public void SpawnInRig(Item item)
    {
        foreach (Transform it in lhand.transform)
        {
            Destroy(it.gameObject);
        }
        foreach (Transform it in rhand.transform)
        {
            Destroy(it.gameObject);
        }
        if (item.leftPrefab != null)
        {
            var l = Instantiate(item.leftPrefab, lhand.transform);
            l.name = item.leftPrefab.name;
            l.GetComponent<BoxCollider>().enabled = false;
        }
        if (item.rightPrefab != null)
        {
            var r = Instantiate(item.rightPrefab, rhand.transform);
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
    }

}
