using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public enum MoveType {Move, Sit, Lying};
    public MoveType moveType;


    public float speed;
   
    float normalSpeed;
    public float JumpForce;
    public float StopForce;
    public float sens;
    public Rigidbody Rigidbody;
    public bool isJumped;
    public GameObject Camera;
    public Quaternion LastRot;
    float rotationY = 0f;
    public bool jumpCooldown, run;
    public float slope = 0;
    public PlayerStats stats;
    public bool InWater;
    public bool staminaWait;
    public AudioClip jumpSound;
    private void Start()
    {
        normalSpeed = speed;
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1);
    }
    void FixedUpdate()
    {
        if (Rigidbody == null)
        {
            GetComponent<Attack>().secondAnimator.Play("Idle"); return;
        }
        if (Camera.transform.position.y <= 16f)
        {
            InWater = true;
        }
        else
        {
            InWater = false;
        }
        if (InWater)
        {
            if (transform.position.y < 14f)
            {
                Physics.gravity = new Vector3(0, -9.81f / 3, 0);
            }
            else if (transform.position.y > 14f && transform.position.y < 15.55f)
            {
                Physics.gravity = new Vector3(0, -9.81f * 0, 0);
            }

            if (Rigidbody.velocity.magnitude > 0.01f)
            {
                if (GetComponent<Attack>().secondAnimator.GetCurrentAnimatorStateInfo(0).IsName("Switch") == false)
                {
                    if (run)
                    {
                        if (Input.GetKey(KeyCode.W))
                        {
                            GetComponent<Attack>().secondAnimator.Play("CameraRun");
                        }
                        else
                        {
                            GetComponent<Attack>().secondAnimator.Play("CameraWalk");
                        }
                    }
                    else
                    {
                        GetComponent<Attack>().secondAnimator.Play("CameraWalk");
                    }
                }
            }
            else
            {
                GetComponent<Attack>().secondAnimator.Play("Idle");
            }
            if (!PlayerUI.UIOn()) return;


            if (Input.GetKey(KeyCode.W))
            {
                Rigidbody.AddForce(Camera.transform.forward * (speed));
            }
            if (Input.GetKey(KeyCode.S))
            {
                Rigidbody.AddForce(-Camera.transform.forward * (speed));
            }
            if (Input.GetKey(KeyCode.A))
            {
                Rigidbody.AddRelativeForce(Vector3.left * (speed));
            }
            if (Input.GetKey(KeyCode.D))
            {
                Rigidbody.AddRelativeForce(Vector3.right * (speed));
            }
            if (Input.GetKey(KeyCode.Space))
            {
                Rigidbody.AddRelativeForce(Vector3.up * (speed));
            }
        }


        if (!InWater)
        {
            Physics.gravity = new Vector3(0, -9.81f, 0);
            if (Rigidbody.velocity.magnitude > 0.01f)
            {
                if (GetComponent<Attack>().secondAnimator.GetCurrentAnimatorStateInfo(0).IsName("Switch") == false)
                {
                    if (run)
                    {
                        if (Input.GetKey(KeyCode.W))
                        {
                            GetComponent<Attack>().secondAnimator.Play("CameraRun");
                        }
                        else
                        {
                            GetComponent<Attack>().secondAnimator.Play("CameraWalk");
                        }
                    }
                    else
                    {
                        GetComponent<Attack>().secondAnimator.Play("CameraWalk");
                    }
                }
            }
            else
            {
                GetComponent<Attack>().secondAnimator.Play("Idle");
            }

            if (!PlayerUI.UIOn()) return;

            if (staminaWait)
            {
                if (stats.energy > 60)
                {
                    staminaWait = false;
                }
            }

            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {

                if (hit.collider.isTrigger == false)
                {
                    if (hit.transform != transform)
                    {
                        float d = Vector3.Distance(hit.point, transform.position);

                        if (d <= 1.2f)
                        {
                            isJumped = false;
                        }
                        else
                        {
                            isJumped = true;
                        }
                    }
                }
            }

            if (moveType == MoveType.Move)
            {
                GetComponent<Animator>().Play("Move");
                speed = normalSpeed;
                if (stats.energy > 0)
                {
                    if (!staminaWait)
                        run = Input.GetKey(KeyCode.LeftShift);
                }
                else
                {
                    run = false;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    if (isJumped == false)
                    {
                        if (jumpCooldown == false)
                        {
                            if (stats.energy > 20)
                            {

                                if (GetComponent<PlayerStats>().playerDamage.isPlaying == false)
                                {
                                    if (Random.Range(0, 3) == 1)
                                    {
                                        GetComponent<PlayerStats>().playerDamage.PlayOneShot(jumpSound);
                                    }
                                }
                                StartCoroutine(jumpWait());
                                stats.energy -= 20;
                                Rigidbody.AddRelativeForce(Vector3.up * JumpForce);
                            }
                        }
                    }
                }
            }
            else if (moveType == MoveType.Sit)
            {
                GetComponent<Animator>().Play("Crouch");
                run = false;
                speed = normalSpeed - 1;
                if (!jumpCooldown)
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (upDist() >= 2.1f || upDist() == -1)
                        {
                            StartCoroutine(jumpWait());
                            Rigidbody.AddRelativeForce(Vector3.up * JumpForce);
                            moveType = MoveType.Move;
                        }
                        return;
                    }
            }
            else
            {
                run = false;
                GetComponent<Animator>().Play("Lying");
                speed = normalSpeed - 3;
                if (!jumpCooldown)
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        print(upDist());
                        if (upDist() >= 1.5f || upDist() == -1)
                        {
                            StartCoroutine(jumpWait());
                            Rigidbody.AddRelativeForce(Vector3.up * JumpForce);
                            moveType = MoveType.Sit;
                        }
                        return;
                    }
            }


            ///slope 
            if (isJumped == false)
            {
                if (Physics.Raycast(transform.position - new Vector3(0, GetComponent<CapsuleCollider>().height / 2, 0), transform.forward, out hit, 2f))
                {
                    if (hit.collider.isTrigger == false)
                    {
                        if (Vector3.Angle(Vector3.up, hit.normal) <= 75f)
                        {
                            slope = (Vector3.Angle(Vector3.up, hit.normal) / 2.5f);
                        }
                        else
                        {
                            slope = 0;
                        }
                    }
                    else
                    {
                        slope = 0;
                    }
                }
                else
                {
                    slope = 0;
                }
            }
            else
            {
                slope = 0;
            }

            if (Input.GetKey(KeyCode.W))
            {
                Rigidbody.AddRelativeForce(Vector3.forward * ((speed + slope) * (run ? 1.5f : 1)));
                if (run)
                {
                    stats.energy -= 0.4f;
                    if (stats.energy <= 0)
                    {
                        staminaWait = true;
                    }
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                Rigidbody.AddRelativeForce(Vector3.back * (speed + slope));
            }
            if (Input.GetKey(KeyCode.A))
            {
                Rigidbody.AddRelativeForce(Vector3.left * (speed + slope));
            }
            if (Input.GetKey(KeyCode.D))
            {
                Rigidbody.AddRelativeForce(Vector3.right * (speed + slope));
            }

            if ((!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.S)) && (!Input.GetKey(KeyCode.A)) && (!Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.Space)))
            {
                if (Rigidbody.velocity.x > StopForce)
                {
                    Rigidbody.velocity -= new Vector3(StopForce, 0, 0);
                }
                if (Rigidbody.velocity.y > StopForce)
                {
                    Rigidbody.velocity -= new Vector3(0, StopForce, 0);
                }
                if (Rigidbody.velocity.z > StopForce)
                {
                    Rigidbody.velocity -= new Vector3(0, 0, StopForce);
                }
            }
            ////DoubleForse();
        }
    }
    void Update()
    {
        if (PlayerUI.UIOn())
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            float yrot = Input.GetAxisRaw("Mouse X");
            Vector3 rot = new Vector3(0, yrot, 0f) * sens;
            transform.rotation = (transform.rotation * Quaternion.Euler(rot));

            rotationY += Input.GetAxis("Mouse Y") * sens;
            rotationY = Mathf.Clamp(rotationY, -80, 80);

            Camera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!jumpCooldown)
            {
                if (moveType == MoveType.Sit)
                {
                    moveType = MoveType.Move;
                    Rigidbody.AddRelativeForce(Vector3.up * JumpForce); return;
                };
                if (moveType == MoveType.Lying)
                {
                    Rigidbody.AddRelativeForce(Vector3.up * JumpForce);
                    Rigidbody.AddRelativeForce(Vector3.up * JumpForce); return;
                };
                moveType = MoveType.Sit;
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!jumpCooldown)
            {
                if (moveType == MoveType.Lying)
                {
                    moveType = MoveType.Sit;
                    Rigidbody.AddRelativeForce(Vector3.up * JumpForce); return;
                };
                moveType = MoveType.Lying;
            }
        }
    }


    public float upDist()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.transform.position, Vector3.up, out hit))
        {
            if (hit.collider != null)
            {
                return Vector3.Distance(hit.point, Camera.transform.position);
            }
            else
            {
                return -2;
            }
        }
        return -1;
    }

    IEnumerator jumpWait()
    {
        jumpCooldown = true;
        yield return new WaitForSeconds(1);
        jumpCooldown = false;
    }
}