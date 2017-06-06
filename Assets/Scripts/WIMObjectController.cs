using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIMObjectController : MonoBehaviour {


    public int tag = 0;
    public bool isChild = true;

    private GameObject parentObject = null;
    private Vector3 currentPos;
    private Quaternion currentRot;
    private bool imitating = false;
	// Use this for initialization
	void Start () {
        SetParent();
	}

    private void SetParent()
    {
        if (isChild)
        {
            var objs = GameObject.FindGameObjectsWithTag("ParentObj");
            foreach (var item in objs)
            {
                if (item.GetComponent<WIMObjectController>().tag == this.tag)
                {
                    parentObject = item;
                    break;
                }
            }

            currentPos = transform.position;
            currentRot = transform.rotation;
            imitating = true;
        }
    }

    public void StartImitation()
    {
        //currentPos = transform.position;
        //currentRot = transform.rotation;
        //imitating = true;
    }

    public void StopImitation()
    {
       // imitating = false;

    }

	
	// Update is called once per frame
	void Update () {
        if (imitating)
        {
            var difPos =  transform.position - currentPos;
            parentObject.transform.position += difPos * 10;

            parentObject.transform.rotation = transform.rotation;//new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);


            currentPos = transform.position;
            currentRot = transform.rotation;
        }
	}
}
