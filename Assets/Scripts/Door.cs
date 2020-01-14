using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "People" && collision.GetComponent<Behaviour>().ignoreDoors == false)
            Destroy(collision.gameObject);
    }
}
