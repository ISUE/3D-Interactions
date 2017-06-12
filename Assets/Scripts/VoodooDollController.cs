using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoodooDollController : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab;
    // 2
    private GameObject laser;
    // 3
    private Transform laserTransform;
    // 4
    private Vector3 hitPoint;
    private GameObject parentObj = null;
    private GameObject newObj;


    // 4

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
                print(hit.collider.gameObject.name);
                if (Controller.GetHairTriggerDown() && hit.collider.gameObject.name.Contains("Robot"))
                {
                    // hit.collider.gameObject.GetComponent<Renderer>().material = selected;
                    parentObj = hit.collider.gameObject;
                    newObj = GameObject.Instantiate(hit.collider.gameObject);
                    newObj.transform.localScale *= 0.1f;
                    newObj.transform.position = transform.position;
                    newObj.transform.parent = transform;
                    newObj.GetComponent<Animator>().applyRootMotion = false;
                }
            }


        }
        else // 3
        {
            laser.SetActive(false);

        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip) && parentObj != null)
        {
            parentObj.GetComponent<Animator>().enabled = !parentObj.GetComponent<Animator>().enabled;
            newObj.GetComponent<Animator>().enabled = !newObj.GetComponent<Animator>().enabled;


        }



    }
}
