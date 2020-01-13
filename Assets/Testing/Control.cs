using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

    Vector2 velocity;
    void Start () {
        velocity = new Vector2();
	}
    //float l = 0;
    void Update() {
        velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //if ( velocity.magnitude  < 1f ) velocity = new Vector2(0,0);
        //l = velocity.magnitude;
        velocity.Normalize(); velocity *= 10;
    }
    void FixedUpdate () {
        gameObject.GetComponent<Rigidbody2D>().AddForce( velocity);
        //Debug.Log(velocity);
	}
}
