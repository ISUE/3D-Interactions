using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Valve.VR;

public class FlashlightController : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab;
    // 2
    private GameObject laser;
    // 3
    private Transform laserTransform;
    // 4
    private Vector3 hitPoint;

    public Material selected;

    private GameObject[] balls = null;
    private float preTouchY;
    private float diffThreshold =0.02f;
    private float scaleFactor = 0.05f;
    private float maxScale =5;
    private float minScale = 0.5f;

    private GameObject[] Balls
    {
        get
        {
            if (balls == null)
                return GameObject.FindGameObjectsWithTag("HookBall");
            else
                return balls;
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

        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
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

            var touchpad = Controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

            var diff = preTouchY - touchpad.y;

            if (Mathf.Abs(diff) > diffThreshold)
            {
                var scale = laserTransform.localScale;

                if (diff < 0)
                {
                    scale.x += scaleFactor;
                    if (scale.x > maxScale) scale.x = maxScale;
                }
                else
                {
                    scale.x -= scaleFactor;
                    if (scale.x < minScale) scale.x = minScale;
                }
                laserTransform.localScale = new Vector3(scale.x, scale.x, laserTransform.localScale.z);

            }

            preTouchY = touchpad.y;


            RaycastHit hit;

            // 2
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
            {
                hitPoint = hit.point;
                ShowLaser(hit);
                if (Controller.GetHairTriggerDown())// && hit.collider.gameObject.name.Contains("Ball"))
                {
                    CalcualteDistance().GetComponent<Renderer>().material = selected;
                }
            }

        }
        else // 3
        {
            laser.SetActive(false);
            preTouchY = 0f;
        }



    }

    private GameObject CalcualteDistance()
    {
        foreach (var ball in Balls)
        {
            var ballCounter = ball.GetComponent<BallCounter>();
            ballCounter.distance = Vector3.Distance(ball.transform.position, laser.transform.FindChild("Cube").position);
        }
        var ordered = Balls.OrderBy(go => go.GetComponent<BallCounter>().distance).ToArray();
        return ordered[0];

    }
}
