using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    MobFactory mobFactory;
    private void Start() {
        mobFactory = GameObject.Find("root").GetComponent<MobFactory>();
    }
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "People" && collision.GetComponent<Behaviour>().ignoreDoors == false
            //&& (collision.gameObject.transform.position-transform.position).magnitude < 0.6f
            ) {
            Destroy(collision.gameObject);
            mobFactory.mobCount--;
        }
        if(collision.gameObject.transform.parent!=null && collision.gameObject.transform.parent.tag == "People")
            collision.transform.parent.gameObject.SendMessage("knowPlan"); // can read the plan on the doors; ok
    }
}
