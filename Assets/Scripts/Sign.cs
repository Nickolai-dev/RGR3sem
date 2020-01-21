using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "People")
        collision.gameObject.SendMessage("knowPlan");
        if(collision.transform.parent != null)
            if (collision.transform.parent.gameObject.tag == "People")
                collision.transform.parent.gameObject.SendMessage("knowPlan");
    }
}
