using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SimpleCarController))]
public class GravityManager : MonoBehaviour {

    SimpleCarController m_carController;

	// Use this for initialization
	void Start () {
        m_carController = GetComponentInParent<SimpleCarController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {

    }



    private void OnTriggerEnter(Collider other)
    {
        if (m_carController.airborne && )
        {

        }
    }


}
