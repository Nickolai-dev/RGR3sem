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
    List<Vert> open, closed;
    List<Vector2Int> path = new List<Vector2Int>();
    Vector2 pursuitPoint = new Vector2();

    public GameObject aaaa;
    void Start() {
        //StartCoroutine(changePPoint());
        //StartCoroutine(c_FindTheWay());
        FindTheWay();
        

    }

    protected virtual void FindTheWay() {
        Debug.Log("fuck");
        int ii = 0;
        closed = new List<Vert>();
        open = new List<Vert>();
        Vert start = new Vert(mobFactory.nav.coordToGridValues(transform.position), null), current;
        open.Add( start );
        while(open.Count > 0) {
            Debug.Log(gameObject.name + " " + ii);
            open.Sort(Comparer);
            Debug.Log(gameObject.name + " " + ii);
            current = open[0];
            open.RemoveAt(0);
            int pass = 1;
            for(int i = -1; i < 2; i++)
                for(int q = -1; q < 2; q++) {
                    if((i==0)&&(q==0)) continue;
                    try { pass = mobFactory.nav.mesh[current.v.y + i, current.v.x + q]; }
                    catch (System.IndexOutOfRangeException) { continue; }
                    Vector2Int doubt = new Vector2Int(current.v.x+q, current.v.y+i);
                    if(pass==0 && !closed.Exists((Vert v) => { return v.v == doubt; })) {
                        open.Add( new Vert(doubt, current) );
                        if(doubt == mobFactory.nav.coordToGridValues(pursuitPoint)) {
                            path = new List<Vector2Int>();
                            path.Add(current.v);
                            while(current.p != null) {
                                current = current.p;
                                path.Add(current.v);
                            }
                            foreach (Vector2Int pos in path)
                                Instantiate(aaaa, mobFactory.nav.gridToRealCoords(pos), new Quaternion());
                            return;
                        }
                    }
                }
            closed.Add( current );

        }
        throw new System.TimeoutException("Can`t get a way");
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
    IEnumerator c_safeCall(System.Action func) {
        func();
        yield break;
    }
    IEnumerator c_FindTheWay() {
        Coroutine c = StartCoroutine(c_safeCall(FindTheWay));
        yield return new WaitForSeconds(3);
        if (c != null) { StopCoroutine(c); Debug.Log("Failed to path"); }
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
