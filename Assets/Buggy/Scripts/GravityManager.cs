using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour {

    SimpleCarController m_carController;
    Rigidbody m_vehicleRigidbody;
    ConstantForce m_constantForce;

    public Vector3 surfaceNormal = new Vector3(0,1,0);


	// Use this for initialization
	void Start () {
        m_carController = GetComponent<SimpleCarController>();
        m_vehicleRigidbody = GetComponent<Rigidbody>();

        m_constantForce = GetComponent<ConstantForce>();
        m_constantForce.force = -surfaceNormal * 9.8f * m_vehicleRigidbody.mass;
    }
	
	// Update is called once per frame
	void Update () {

        if (m_carController.airborne)
        {
            RaycastHit hit;
            Ray r = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(r, out hit, 10))
            {
                surfaceNormal = hit.normal;
                m_constantForce.force = -surfaceNormal * 9.8f * m_vehicleRigidbody.mass;
            }
        }
	}

    private void FixedUpdate()
    {
        
    }
    
}
