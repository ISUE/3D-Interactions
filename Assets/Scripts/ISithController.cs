using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISithController : MonoBehaviour
{

    public static ISithController Instance;
    private Vector3 rsp = Vector3.zero;
    private Vector3 rep = Vector3.zero;
    private Vector3 lsp = Vector3.zero;
    private Vector3 lep = Vector3.zero;
    private Vector3 closestP1 = Vector3.zero;
    private Vector3 closestP2 = Vector3.zero;
    private GameObject cube;
    public GameObject collidedObject = null;
    public bool trigger;

    void Awake()
    {
        Instance = this;
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localScale *= 0.05f;
        cube.GetComponent<BoxCollider>().isTrigger = true;
        cube.AddComponent<ISithCubeCollider>();
    }

    public void UpadteRightController(Vector3 startPoint, Vector3 endPoint)
    {
        rsp = startPoint;
        rep = endPoint;
    }

    public void UpadteLeftController(Vector3 startPoint, Vector3 endPoint)
    {
        lsp = startPoint;
        lep = endPoint;
    }

    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);

        float d = a * e - b * b;

        //lines are not parallel
        if (d != 0.0f)
        {

            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
        }

        else {
            return false;
        }
    }

 

    // Update is called once per frame
    void Update()
    {
        if (rsp != Vector3.zero && lsp != Vector3.zero)
        {
            if (ClosestPointsOnTwoLines(out closestP1, out closestP2, rsp, (rsp - rep), lsp, (lsp - lep)))
            {
                var line = GetComponent<LineRenderer>();
                line.SetPositions(new Vector3[] { closestP1, closestP2 });
                cube.transform.position = (closestP1 + closestP2) / 2;
            }
        }

        if (trigger)
        {
            if (collidedObject != null)
            {
                collidedObject.transform.position = cube.transform.position;
                collidedObject.GetComponent<Rigidbody>().useGravity = false;
            }

        }
        else
        {
            if (collidedObject != null)
            {
                collidedObject.GetComponent<Rigidbody>().useGravity = true;
                collidedObject = null;
            }

        }
    }

    
}
