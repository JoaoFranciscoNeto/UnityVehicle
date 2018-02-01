using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVehicleFollow : MonoBehaviour {

    GameObject target;

    GravityManager m_gravMan;

	// Use this for initialization
	void Start () {
        m_gravMan = target.GetComponent<GravityManager>();
        
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = target.transform.position + m_gravMan.surfaceNormal * 4;
	}
}
