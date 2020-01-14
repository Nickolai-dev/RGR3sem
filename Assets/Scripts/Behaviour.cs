using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Vert {
    public Vert(Vector2Int _v, Vert _p) { v = _v; p = _p; }
    public Vector2Int v { get; set; }
    public Vert p { get; set; }
};

public class Behaviour : MonoBehaviour {

    public bool ignoreDoors = false;
    public MobFactory mobFactory;
    List<Vector2Int> closed;
    List<Vert> open;
    Vector2 pursuitPoint = new Vector2();

    void Start() {
        StartCoroutine(changePPoint());
        FindTheWay();
    }

    protected void FindTheWay() {
        closed = new List<Vector2Int>();
        open = new List<Vert>();
        Vert start = new Vert(mobFactory.nav.coordToGridValues(transform.position), null), current;
        open.Add( start );
        while(open.Count > 0) {
            open.Sort(Comparer);
            current = open[0];
            open.RemoveAt(0);

        }
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

    private int Comparer(Vert A, Vert B) {
        Vector2Int dest = mobFactory.nav.coordToGridValues(pursuitPoint);
        Vector2Int v1 = A.v-dest, v2 = B.v-dest;
        int a = v1.sqrMagnitude, b = v2.sqrMagnitude;
        if (a == b) return 0;
        else if (a > b) return 1;
        else return -1;
    }
}
