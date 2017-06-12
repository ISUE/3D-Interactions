using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

public class DepthRayController : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab;

    public GameObject selectedSpherePrefab;

    private GameObject selectedSphere;
    // 2
    private GameObject laser;
    // 3
    private Transform laserTransform;
    // 4
    private Vector3 hitPoint;

    public Material selected;

    public LayerMask teleportMask;
    private GameObject cube;
    private float preTouchY;
    private float diffThreshold = 0.02f;

    private GameObject[] objs = null;
    private GameObject selectedObj;

    private GameObject[] Objs
    {
        get
        {
            if (objs == null)
                return GameObject.FindGameObjectsWithTag("HookBall");
            else
                return objs;
        }
    }



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

        selectedSphere = Instantiate(selectedSpherePrefab);

        selectedSphere.SetActive(false);

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = transform;
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localScale *= 0.05f;
        cube.GetComponent<BoxCollider>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        // 1
        //if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        //{
        RaycastHit hit;

        // 2
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
        {
            hitPoint = hit.point;
            ShowLaser(hit);

        }

        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {

            var touchpad = Controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

            var diff = preTouchY - touchpad.y;

            if (Mathf.Abs(diff) > diffThreshold)
            {
                var pos = cube.transform.localPosition;

                if (diff < 0)
                {
                    pos.z += 0.1f;
                }
                else
                {
                    pos.z -= 0.1f;

                }
                cube.transform.localPosition = pos;

            }

            preTouchY = touchpad.y;
        }

        if (Controller.GetHairTrigger())
        {
            if (selectedObj != null)
            {
                selectedObj.transform.parent = null;
                selectedObj.GetComponent<Rigidbody>().useGravity = true;
                selectedObj = null;

            }
            selectedSphere.SetActive(true);
            selectedObj = CalcualteDistance();
            selectedSphere.transform.position = selectedObj.transform.position;
        }
        else
        {
            if (selectedObj != null)
            {
                selectedObj.transform.parent = transform;
                selectedObj.GetComponent<Rigidbody>().useGravity = false;
            }
            selectedSphere.SetActive(false);
        }

        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            if (selectedObj != null)
            {
                selectedObj.transform.parent = null;
                selectedObj.GetComponent<Rigidbody>().useGravity = true;
                selectedObj = null;
            }
        }
    }

    private GameObject CalcualteDistance()
    {
        foreach (var obj in Objs)
        {
            var ballCounter = obj.GetComponent<BallCounter>();
            ballCounter.distance = Vector3.Distance(obj.transform.position, cube.transform.position);
        }
        var ordered = Objs.OrderBy(go => go.GetComponent<BallCounter>().distance).ToArray();
        return ordered[0];
    }
}
