using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePlaneController : MonoBehaviour {

    public Camera cameraa;

    public bool isScaleGrab = false;

    private SteamVR_TrackedObject trackedObj;


    public GameObject laserPrefab;
    // 2
    private GameObject laser;
    // 3
    private Transform laserTransform;
    // 4
    private Vector3 hitPoint;

    public LayerMask teleportMask;
    private GameObject selectedObj = null;
    private Vector3 selectedObjInitPos;
    private Vector3 selectedObjInitScale;
    private Vector3 startPos;
    private float dist;

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
        laser.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        // 1

        RaycastHit hit;
        if (Physics.Raycast(cameraa.transform.position,   -1*(cameraa.transform.position - transform.position) , out hit, 100,teleportMask))
        {
            hitPoint = hit.point;
          //  ShowLaser(hit);
            if (selectedObj == null && Controller.GetHairTriggerDown() && hit.collider.gameObject.name.Contains("RwCube"))
            {
                selectedObj = hit.collider.gameObject;
                selectedObj.transform.parent = transform;
                selectedObjInitPos = selectedObj.transform.position;
                selectedObjInitScale = selectedObj.transform.localScale;
                dist = Vector3.Distance(selectedObjInitPos, transform.position);
                selectedObj.transform.localScale /= dist;
                selectedObj.transform.position = transform.position;
                startPos = transform.position;
                return;
            }
           
        }

        if(selectedObj != null && Controller.GetHairTriggerDown())
        {
            selectedObj.transform.parent = null;
            if (isScaleGrab)
            {
                selectedObj.transform.position = selectedObjInitPos + ( transform.position- startPos ) * 5;
            }
            else
              selectedObj.transform.position = selectedObjInitPos;
            selectedObj.transform.localScale = selectedObjInitScale;
            selectedObj = null;

        }


    }
}
