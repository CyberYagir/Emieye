using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProTruckController : MonoBehaviour
{
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
    }
    public Rigidbody rb;
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    private void Start()
    {
            rb = GetComponent<Rigidbody>();
    }

    public void SetBrake(int brake)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.leftWheel.brakeTorque = brake;
            axleInfo.rightWheel.brakeTorque = brake;
        }
    }
    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        int down = 1;
        if (Input.GetKey(KeyCode.Space))
            down = 0;

        if (down == 0)
        {
            SetBrake(10);
            rb.drag = 10;
        }
        else
        {
            rb.drag = 0.2f;
        }
        float plus = 0;
        
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                if (down == 0)
                {
                    if (axleInfo.leftWheel.motorTorque < 0)
                    {
                        plus = Mathf.Abs(axleInfo.leftWheel.motorTorque);
                    }
                    if (axleInfo.leftWheel.motorTorque > 0)
                    {
                        plus = -Mathf.Abs(axleInfo.rightWheel.motorTorque);
                    }
                }
                axleInfo.leftWheel.motorTorque = motor + plus;
                axleInfo.rightWheel.motorTorque = motor + plus;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
}
    
