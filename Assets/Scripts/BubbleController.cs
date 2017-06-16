using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BubbleController : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    public GameObject selectedSpherePrefab;

    private GameObject selectedSphere;

    public Material selected;

    private GameObject[] objs = null;
    private GameObject selectedObj;
    private GameObject selectingSphere;

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

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        selectingSphere = transform.Find("SelectingSphere").gameObject;
    }
    
    void Start()
    {
        selectedSphere = Instantiate(selectedSpherePrefab);

        selectedSphere.SetActive(false);
    }

    void Update()
    {

        if (Controller.GetHairTrigger())
        {
            selectedObj = CalcualteDistance();
            var dist = Vector3.Distance(selectedObj.transform.position, transform.position);

            selectingSphere.transform.localScale = new Vector3(dist, dist, dist);

            selectedSphere.SetActive(true);
            selectedSphere.transform.position = selectedObj.transform.position;
        }
        else
        {
            selectedSphere.SetActive(false);

        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (selectedObj!= null)
            {
                selectedObj.GetComponent<Renderer>().material = selected;
            }
        }
    }

    private GameObject CalcualteDistance()
    {
        foreach (var obj in Objs)
        {
            var ballCounter = obj.GetComponent<BallCounter>();
            ballCounter.distance = Vector3.Distance(obj.transform.position, transform.position);
        }
        var ordered = Objs.OrderBy(go => go.GetComponent<BallCounter>().distance).ToArray();
        return ordered[0];
    }
}
