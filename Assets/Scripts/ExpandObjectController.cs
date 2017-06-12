using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandObjectController : MonoBehaviour {

    public int tagg = 0;
    public bool isChild = true;
    public GameObject parentObject;

    // Use this for initialization

    void Start()
    {
        SetParent();
    }

    private void SetParent()
    {
        if (isChild)
        {
            var objs = GameObject.FindGameObjectsWithTag("ParentObj");
            foreach (var item in objs)
            {
                if (item.GetComponent<ExpandObjectController>().tagg == this.tagg)
                {
                    parentObject = item;
                    break;
                }
            }

        }
    }
   
	
	// Update is called once per frame
	void Update () {
		
	}
}
