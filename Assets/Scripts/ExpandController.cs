using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandController : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab;
    // 2
    private GameObject laser;
    // 3
    private Transform laserTransform;
    // 4
    private Vector3 hitPoint;

    public Material selected;

    public GameObject platesChild;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }


    private void ShowLaser(RaycastHit hit)
    {
        // 1
        laser.SetActive(true);
        // 2
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        // 3
        laserTransform.LookAt(hitPoint);
        // 4
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    void Start()
    {
        // 1
        laser = Instantiate(laserPrefab);
        // 2
        laserTransform = laser.transform;


    }

    // Update is called once per frame
    void Update()
    {
        // 1
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            // 2
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
            {
                hitPoint = hit.point;
                ShowLaser(hit);
                // 1
                if (Controller.GetHairTriggerDown())
                {
                    if (hit.collider.gameObject.name == "Table")
                    {
                        platesChild.SetActive(true);
                    }
                    if (hit.collider.gameObject.tag == "ChildObj")
                    {
                        hit.collider.gameObject.GetComponent<ExpandObjectController>().parentObject.GetComponent<Renderer>().material = selected;
                        platesChild.SetActive(false);
                    }
                }

            }
        }
        else // 3
        {
            laser.SetActive(false);
        }
        
    }

}
