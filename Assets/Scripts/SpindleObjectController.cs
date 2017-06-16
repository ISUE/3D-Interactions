using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpindleObjectController : MonoBehaviour
{

    public static SpindleObjectController Instnace;

    public GameObject rightObj;
    public GameObject leftObj;

    public Vector3 rightHitPoint;
    public Vector3 leftHitPoint;

    public Transform rightTransform;
    public Transform leftTransform;

    private Vector3 midPoint;
    public GameObject selectedObj;
    private float lastDist;
    private Vector3 lastVec;

    public Camera cameraa;
    private float threshold = 0.002f;

    private SteamVR_Controller.Device RightController
    {
        get { return SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost)); }
    }

    private SteamVR_Controller.Device LeftController
    {
        get { return SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost)); }
    }

    // Use this for initialization
    void Awake()
    {
        Instnace = this;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (RightController.GetHairTrigger() && LeftController.GetHairTrigger())
        {
            if (selectedObj)
            {
                var dist = Vector3.Distance(rightTransform.position, leftTransform.position);
                float k = 6.5f * dist;
                print(Mathf.Abs(dist - lastDist));
                if (Mathf.Abs(dist - lastDist) > threshold)
                {
                    if (dist > lastDist)
                    {
                        selectedObj.transform.localScale += (new Vector3(k, k, k));
                    }
                    else
                    {
                        selectedObj.transform.localScale -= (new Vector3(k, k, k));

                    }
                }

                lastDist = dist;

                //var firstVec = rightTransform.position - leftTransform.position;

                //var ang = Vector3.Angle(firstVec, lastVec);

                //selectedObj.transform.Rotate(cameraa.transform.right, ang);

                //lastVec = firstVec;

            }
            else if (rightObj == leftObj && leftObj.tag == "HookBall")
            {

                selectedObj = leftObj;
                midPoint = (rightHitPoint + leftHitPoint) / 2;
                //  selectedObj.GetComponent<Rigidbody>().useGravity = false;
                selectedObj.transform.position = midPoint;
                selectedObj.transform.parent = rightTransform;
                lastDist = Vector3.Distance(midPoint, leftHitPoint);
                lastVec = rightTransform.position - leftTransform.position;

            }
            else
            {
                if (selectedObj != null)
                {
                    //   selectedObj.GetComponent<Rigidbody>().useGravity = true;
                    selectedObj.transform.parent = null;
                    selectedObj = null;
                }
            }
        }
        else
        {
            if (selectedObj != null)
            {
                // selectedObj.GetComponent<Rigidbody>().useGravity = true;
                selectedObj.transform.parent = null;
                selectedObj = null;

            }
        }
    }
}
