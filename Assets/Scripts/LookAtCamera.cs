using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    public Camera cameraa;
    private float distance = 2f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = cameraa.transform.position + cameraa.transform.forward * distance;
        //gameObject.transform.rotation = new Quaternion(0.0f, cameraa.transform.rotation.y, 0.0f, cameraa.transform.rotation.w);
        gameObject.transform.LookAt(cameraa.transform);
    }
}
