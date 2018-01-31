using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour {

    SimpleCarController m_carController;
    Rigidbody m_vehicleRigidbody;

    Vector3 surfaceNormal = new Vector3(0,1,0);


	// Use this for initialization
	void Start () {
        m_carController = GetComponentInParent<SimpleCarController>();
        m_vehicleRigidbody = transform.parent.GetComponent<Rigidbody>();
        
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Ray r = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(r,out hit, 20))
        {
            surfaceNormal = hit.normal;
            Debug.Log("Updated normal to " + surfaceNormal);
        }
	}

    private void FixedUpdate()
    {
        m_vehicleRigidbody.AddForce(-surfaceNormal * 9.8f, ForceMode.Acceleration);


    }
    
}
