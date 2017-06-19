using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PrismController : MonoBehaviour {

    public enum EIndex
    {
        None = -1,
        Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
        Device1,
        Device2,
        Device3,
        Device4,
        Device5,
        Device6,
        Device7,
        Device8,
        Device9,
        Device10,
        Device11,
        Device12,
        Device13,
        Device14,
        Device15
    }

    public EIndex index;
    public Transform origin; // if not set, relative to parent
    public bool isValid = false;

    // 1
    private GameObject collidingObject;
    // 2
    private GameObject objectInHand = null;

    private void OnNewPoses(TrackedDevicePose_t[] poses)
    {
        if (index == EIndex.None)
            return;

        var i = (int)index;

        isValid = false;
        if (poses.Length <= i)
            return;

        if (!poses[i].bDeviceIsConnected)
            return;

        if (!poses[i].bPoseIsValid)
            return;

        isValid = true;

        var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);
        var HMDpose = new SteamVR_Utils.RigidTransform(poses[(int)EIndex.Hmd].mDeviceToAbsoluteTracking);

        if (origin != null)
        {

            transform.position = origin.transform.TransformPoint(pose.pos);
            transform.rotation = origin.rotation * pose.rot;
        }
        else
        {
            var distance = Vector3.Distance(HMDpose.pos, pose.pos);

            if (distance > threshold)
            {
                if (initPose == Vector3.zero)
                {
                    Debug.LogError("Initiate controller Pos");
                    transform.localPosition = pose.pos;
                    transform.localRotation = pose.rot;
                    return;
                }

                float k = 3;
                var diffPos = pose.pos - initPose;
                diffPos *= (1f + k * Mathf.Pow(distance, 2));
                //print((1f + k * Mathf.Pow(distance, 2)));
                //  print(Mathf.Pow((Mathf.Abs(diff.z) - threshold), 2));
                pose.pos += diffPos;

            }

            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;

        }
    }

    SteamVR_Events.Action newPosesAction;
    private float threshold = 0.2f;
    private Vector3 initPose = Vector3.zero;

    void Awake()
    {
        newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
    }

    void OnEnable()
    {
        var render = SteamVR_Render.instance;
        if (render == null)
        {
            enabled = false;
            return;
        }

        newPosesAction.enabled = true;
    }

    void OnDisable()
    {
        newPosesAction.enabled = false;
        isValid = false;
    }

    public void SetDeviceIndex(int index)
    {
        if (System.Enum.IsDefined(typeof(EIndex), index))
            this.index = (EIndex)index;
    }

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)index); }
    }

    void Update()
    {
        if (initPose == Vector3.zero && Controller.GetHairTriggerDown())
        {
            initPose = transform.localPosition;
            return;
        }

        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }

        // 2
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }




    private void SetCollidingObject(Collider col)
    {
        // 1
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // 2
        collidingObject = col.gameObject;
    }

    // 1
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // 2
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 3
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void GrabObject()
    {
        // 1
        objectInHand = collidingObject;
        collidingObject = null;
        // 2
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    // 3
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        // 1
        if (GetComponent<FixedJoint>())
        {
            // 2
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // 3
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        // 4
        objectInHand = null;
    }
}
