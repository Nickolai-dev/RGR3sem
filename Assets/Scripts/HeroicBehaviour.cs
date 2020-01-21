using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroicBehaviour : DefaultBehaviour {

    int gotExt = 0;
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Extinguisher") { gotExt = 4;
            StopAllCoroutines();
            getDoorPosition();
            StartCoroutine(h_FindTheWay = FindTheWay());
        }
        if(gotExt > 0 && collision.tag == "Fire") {
            Destroy(collision.gameObject);
            gotExt--;
        }
    }

    void controllableEvacuation() {
        speed = evacSpeed;
        ignoreDoors = false;
    }

    void knowPlan() {
        if(speed == evacSpeed && once) { once = false;
            StopAllCoroutines();
            try {
            getExtPosition();} catch(System.InvalidOperationException) { getDoorPosition(); }
            StartCoroutine(h_FindTheWay = FindTheWay());
        }
    }

    void getExtPosition() {
        List<GameObject> lst = GameObject.FindGameObjectsWithTag("Extinguisher").ToList();
        if(lst.Count == 0)
            throw new System.InvalidOperationException();
        lst.Sort((A, B) => {
            int a = (int)(A.transform.position-transform.position).sqrMagnitude,
                b = (int)(B.transform.position-transform.position).sqrMagnitude;
            return a < b ? 1 : a > b ? -1 : 0; });
        pursuitPoint = lst[0].transform.position;
    }
}
