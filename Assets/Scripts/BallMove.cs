using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour {

    private float time = 2;
    private int invers = 1;
    private int move = 4;

	// Use this for initialization
	void Start () {
        time = Random.Range(1f, 5f);
        invers = Random.Range(-1, 1);
        if (invers == 0) invers = 1;
        move = Random.Range(2, 6);
        StartMove();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartMove(){
        iTween.MoveTo(gameObject, iTween.Hash("x", move * invers, "easetype", "linear", "looptype", "pingPong", "time", time));

    }

    public void StopMove(){
        iTween.Stop(gameObject);
    }
}
