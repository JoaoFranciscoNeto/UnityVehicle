using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    Rigidbody rb;

    public Vector3 centerOfMass = new Vector3(0, 0, 0);

    public bool airborne;

    public float jumpForce = 100;

    float maxHandbrakeTorque;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass -= centerOfMass;


        foreach (AxleInfo axleInfo in axleInfos)
        {
            Physics.IgnoreCollision(axleInfo.leftWheel, GetComponentInChildren<Collider>());
            Physics.IgnoreCollision(axleInfo.rightWheel, GetComponentInChildren<Collider>());
        }

        maxHandbrakeTorque = float.MaxValue;
    }

    public void Update()
    {
        airborne = true;
        foreach(AxleInfo axleInfo in axleInfos)
        {
            airborne = airborne && (!axleInfo.leftWheel.isGrounded && !axleInfo.rightWheel.isGrounded);
            if (!airborne)
                break;
        }


    }



    public void FixedUpdate()
    {
        float vertical = maxMotorTorque * Input.GetAxis("Vertical");
        float horizontal = maxSteeringAngle * Input.GetAxis("Horizontal");
        bool handbrake = Input.GetKey(KeyCode.LeftControl);

        if (!airborne)
        {
            HandleGroundedMovement(vertical,horizontal,handbrake);
        } else {
            //HandleAirborneMovement(vertical, horizontal);
        }

        foreach (AxleInfo axleInfo in axleInfos)
        {
            

            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = horizontal;
                axleInfo.rightWheel.steerAngle = horizontal;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = vertical;
                axleInfo.rightWheel.motorTorque = vertical;

                axleInfo.leftWheel.brakeTorque = maxHandbrakeTorque * (handbrake ? 1 : 0);
                axleInfo.rightWheel.brakeTorque = maxHandbrakeTorque * (handbrake ? 1 : 0);
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

    }

    public void HandleGroundedMovement(float motor, float steering, bool handbrake)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * jumpForce * rb.mass);
        }
    }

    public void HandleAirborneMovement(float pitch, float roll)
    {
        rb.AddTorque(transform.right * pitch * Time.deltaTime * 200);
        rb.AddTorque(transform.forward * roll * Time.deltaTime * 200);
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
        visualWheel.transform.rotation = rotation * Quaternion.Euler(0,0,90);
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
