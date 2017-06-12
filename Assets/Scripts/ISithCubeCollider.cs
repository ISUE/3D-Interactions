using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISithCubeCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {

        // Print the entire list to the console.
        if (col.gameObject.tag == "HookBall")
            ISithController.Instance.collidedObject = col.gameObject;
    }

    void OnTriggerExit(Collider col)
    {

        // Remove the GameObject collided with from the list.
        if (!ISithController.Instance.trigger)
            ISithController.Instance.collidedObject = null;
    }
}
