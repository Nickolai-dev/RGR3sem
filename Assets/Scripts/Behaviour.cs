using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour : MonoBehaviour {

    public bool ignoreDoors = false;
    public MobFactory mobFactory;
    List<Vector2Int> closed = new List<Vector2Int>(), open = new List<Vector2Int>();
    Vector2 pursuitPoint = new Vector2();

    void Start() {
        StartCoroutine(changePPoint());
    }

    void FixedUpdate() {

    }

    IEnumerator DoorsInactiveByTime() {
        ignoreDoors = true;
        yield return new WaitForSeconds(3);
        ignoreDoors = false;
        yield break;
    }
    IEnumerator changePPoint() {
        while(true) {
            pursuitPoint = mobFactory.rndPoint();
            yield return new WaitForSeconds(8);
        }
    }

    private int Comparer(Vector2Int A, Vector2Int B) {
        Vector2Int dest = mobFactory.nav.coordToGridValues(pursuitPoint);
        A-=dest;
        B-=dest;
        int a = A.sqrMagnitude, b = B.sqrMagnitude;
        if (a == b) return 0;
        else if (a > b) return 1;
        else return -1;
    }
}
