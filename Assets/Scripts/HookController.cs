using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class HookController : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private GameObject[] balls = null;
    private int increaseBallCount = 5;
    private GameObject objectInHand;
    public Material blueBallMat;
    public Material redBallMat;
    private Transform beforeParent;
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

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();


    }


    void FixedUpdate()
    {
        // 1
        if (Controller.GetHairTrigger())
        {
            CalcualteDistance();
        }

        // 2
        if (Controller.GetHairTriggerUp())
        {

            GrabObject();
        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            ReleaseObject();
        }
    }

    private void GrabObject()
    {
        
        var ordered = Balls.OrderByDescending(go => go.GetComponent<BallCounter>().score).ToArray();
        objectInHand = ordered[0];
        objectInHand.GetComponent<BallMove>().StopMove();
        objectInHand.GetComponent<Renderer>().material = redBallMat;
        //var joint = AddFixedJoint();
        //joint.connectedBody = objectInHand.AddComponent<Rigidbody>();
        //beforeParent = objectInHand.transform.parent;
        //objectInHand.transform.parent = transform;
    }

    private void ResetBallsScore()
    {
        foreach (var ball in Balls)
        {
            ball.GetComponent<BallCounter>().score = 0;
        }
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
        //// 1
        //if (GetComponent<FixedJoint>())
        //{
        //    // 2
        //    GetComponent<FixedJoint>().connectedBody = null;
        //    Destroy(GetComponent<FixedJoint>());
        //    // 3
        //    objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
        //    objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        //    Destroy(GetComponent<Rigidbody>());

            


        //}

        //objectInHand.transform.parent = beforeParent;
        objectInHand.GetComponent<BallMove>().StartMove();
        objectInHand.GetComponent<Renderer>().material = blueBallMat;
        ResetBallsScore();
        // 4
        objectInHand = null;
    }

    private void CalcualteDistance()
    {
        foreach (var ball in Balls)
        {
            var ballCounter = ball.GetComponent<BallCounter>();
            ballCounter.distance = Vector3.Distance(ball.transform.position, transform.position);
        }
        var ordered = Balls.OrderBy(go => go.GetComponent<BallCounter>().distance).ToArray();
        for (int i = 0; i < ordered.Length; i++)
        {
            var ball = ordered[i].GetComponent<BallCounter>();

            if (i < increaseBallCount)
            {
                ball.score++;
            }
            else
            {
                ball.score--;
                if (ball.score < 0) ball.score = 0;
            }
        }

    }
}
