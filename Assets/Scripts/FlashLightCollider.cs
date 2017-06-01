using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightCollider : MonoBehaviour {

    public List<GameObject> currentCollisions = new List<GameObject>();

    void OnTriggerEnter(Collider col)
    {

        // Add the GameObject collided with to the list.
        currentCollisions.Add(col.gameObject);

        // Print the entire list to the console.
        foreach (GameObject gObject in currentCollisions)
        {
            print(gObject.name);
        }
    }

    void OnTriggerExit(Collider col)
    {

        // Remove the GameObject collided with from the list.
        currentCollisions.Remove(col.gameObject);

        // Print the entire list to the console.
        foreach (GameObject gObject in currentCollisions)
        {
            print(gObject.name);
        }
    }

	
}
