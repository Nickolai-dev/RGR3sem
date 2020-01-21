using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Behaviour : MonoBehaviour {
    internal class Node {
        public Vector2Int vector { get; set; }
        public Node prev { get; set; }
        public int nodesTraversed { get {
                int i = 0;
                for (Node cur = this; cur.prev != null; cur = cur.prev)
                { i += (cur.vector - cur.prev.vector).sqrMagnitude > 1 ? 14 : 10; }
                return i; } }
    };
    public bool ignoreDoors = false;
    public MobFactory mobFactory;
    public float effectiveTouchRadius = 0.25f, effectiveSeeRadius = 3.0f;
    public bool canSeeTroughWalls = true; // TODO : realize for "can`t see"
    public float normalSpeed = 1, evacSpeed = 2;
    List<Node> open = new List<Node>(), closed = new List<Node>();
    List<Vector2Int> path = new List<Vector2Int>();
    Vector2 pursuitPoint = new Vector2();
    IEnumerator h_FindTheWay = null;

    public GameObject TestingDot;
    void Start() {
        speed = normalSpeed;
        //h_FindTheWay = FindTheWay();
        //StartCoroutine(protectFromLoop(coroutineHandler));
        StartCoroutine(setRandomPPointAndPath());
        //StartCoroutine(h_FindTheWay);
        //controllableEvacuation();
    }

    private void FixedUpdate() { 
        if (path.Count > 0)
            try {
                moveToNextNode();
            } catch(System.ArgumentOutOfRangeException e) { Debug.Log(e.ToString()); }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(h_FindTheWay != null) {
            h_FindTheWay = FindTheWay();
            StartCoroutine(h_FindTheWay);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        
    }

    void controllableEvacuation() {
        speed = evacSpeed;
        //StopAllCoroutines();
        ignoreDoors = false;
        //getDoorPosition(); // TODO : remove
        //StartCoroutine(h_FindTheWay = FindTheWay());
    }

    bool once = true;
    void knowPlan() {
        if(speed == evacSpeed && once) { once = false;
            StopAllCoroutines();
            getDoorPosition();
            StartCoroutine(h_FindTheWay = FindTheWay());
        }
    }

    void getDoorPosition() { // if can see or if read the sign
        pursuitPoint = mobFactory.nav.doors[0];
        Vector3 v = pursuitPoint;
        do {
            pursuitPoint = v;
            v = mobFactory.nav.doors.Find(any => (any-transform.position).sqrMagnitude < (v-transform.position).sqrMagnitude   );
        } while(v != default(Vector3)); // while exists shorter /// sory for (0,0,0) bug
        Debug.Log(v);
    }

    Vector3 nextPoint, direction; float speed;
    void moveToNextNode() { // TODO : upgrade; it needs to be more precious, when peple touches pathpoint, that in path, but not next
        getNextPoint: nextPoint = mobFactory.nav.gridToRealCoords(path[path.Count-1]);
        if( (nextPoint-transform.position).magnitude <= effectiveTouchRadius ) {
            path.RemoveAt(path.Count-1);
            if(GetComponent<Rigidbody2D>().mass < 10)
            GetComponent<Rigidbody2D>().AddForce(-direction*speed*speed); // braking
            goto getNextPoint;
        }
        direction = (nextPoint-transform.position).normalized;
        GetComponent<Rigidbody2D>().AddForce(direction*speed);

    }

    IEnumerator FindTheWay() {
        int kostyl = 0,
            kostylFrames = 30; // adjust; 30 seems to be optimum
        closed.Clear();
        open.Clear();
        Node start = new Node(){vector=mobFactory.nav.coordToGridValues(transform.position), prev=null}, current;
        Vector2Int end = mobFactory.nav.coordToGridValues(pursuitPoint);
        open.Add(start);
        List<Vector2Int> nbrs;
        while (open.Count > 0) {
            //if(Random.Range(0,1) == 1) open.Reverse(); // randomize equal paths (if it really need)
            open.Sort(Comparer1);
            current = open[0];
            closed.Add(current);
            open.RemoveAt(0);
            //Instantiate( aaaa, mobFactory.nav.gridToRealCoords(current.vector), new Quaternion() )
            //    .GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
            //yield return null;
            if(kostyl >= kostylFrames) { yield return null; } kostyl++;
            nbrs = getNeighbours(current);
            foreach(Vector2Int i in nbrs) {
                Node variant = new Node() { vector = i, prev = current };
                Node mbHaveAlrdy = open.Find(node => node.vector == i);
                if ( mbHaveAlrdy == null ) { // if not exists in open
                    open.Add(variant);
                } else {
                    if( current.nodesTraversed
                        + ( (current.vector-mbHaveAlrdy.vector).sqrMagnitude>1 ? 14 : 10 )
                        < mbHaveAlrdy.nodesTraversed ) { // if path is shorter (through current to mbHaveAlrdy)
                        mbHaveAlrdy.prev = current;
                    }
                }
                if(i == end) {
                    path.Clear();
                    path.Add(end);
                    while(current!=null) {
                        path.Add(current.vector);
                        current = current.prev;
                    }
                    h_FindTheWay = null; // it isnt null itself, so do it manually
                    yield break;
                }
            }
        }
        h_FindTheWay = null;
        throw new System.InvalidOperationException("Can`t find a path. Probably it doesn`t exist");
    }
    private int Comparer1(Node A, Node B) {
        int manhattanKoeff = 10;
        Vector2Int dest = mobFactory.nav.coordToGridValues(pursuitPoint);
        Vector2Int v1 = A.vector-dest, v2 = B.vector-dest;
        int a = (Mathf.Abs(v1.x)+ Mathf.Abs(v1.y))*manhattanKoeff+A.nodesTraversed,
            b = (Mathf.Abs(v2.x)+ Mathf.Abs(v2.y))*manhattanKoeff+B.nodesTraversed;
        return (a == b ? 0 : (a > b ? 1 : -1));
    }
    private int Comparer2(Node A, Node B) {
        Vector2Int dest = mobFactory.nav.coordToGridValues(pursuitPoint);
        Vector2Int v1 = A.vector-dest, v2 = B.vector-dest;
        int a = (int)(v1.magnitude*10)+A.nodesTraversed, b = (int)(v2.magnitude*10)+B.nodesTraversed;
        return (a == b ? 0 : (a > b ? 1 : -1));
    }
    private int Comparer3(Node A, Node B) {
        int manhattanKoeffStrt = 10, manhattanKoeffDiagonal = 14;
        Vector2Int dest = mobFactory.nav.coordToGridValues(pursuitPoint);
        Vector2Int v1 = A.vector-dest, v2 = B.vector-dest;
        int a = Mathf.Abs(Mathf.Abs(v1.x) - Mathf.Abs(v1.y))*manhattanKoeffStrt
            + Mathf.Min(Mathf.Abs(v1.x), Mathf.Abs(v1.y))*manhattanKoeffDiagonal
            + A.nodesTraversed,
            b = (Mathf.Abs(v2.x)+ Mathf.Abs(v2.y))*manhattanKoeffStrt
            + Mathf.Min(Mathf.Abs(v2.x), Mathf.Abs(v2.y)) * manhattanKoeffDiagonal
            + B.nodesTraversed;
        return (a == b ? 0 : (a > b ? 1 : -1));
    }
    List<Vector2Int> getNeighbours(Node cur) {
        List<Vector2Int> nbrs = new List<Vector2Int>();
        foreach(int i in new int[]{-1, 0, 1})
            foreach(int q in new int[]{-1, 0, 1}){
                if(i==0 && q==0) continue;
                Vector2Int doubt = new Vector2Int(cur.vector.x + q, cur.vector.y + i);
                try {
                    if( mobFactory.nav.mesh[cur.vector.y+i, cur.vector.x+q] == 0
                        && !closed.Exists(node => node.vector == doubt) ) {
                        nbrs.Add(doubt);
                    }
                } catch (System.IndexOutOfRangeException) { Debug.Log("Out of boundary"); }
            }
        return nbrs;
    }
    IEnumerator protectFromLoop(IEnumerator coroutineHandler) {
        float timeout = 3.0f;
        yield return new WaitForSeconds(timeout);
        if(coroutineHandler != null) {
            StopCoroutine(coroutineHandler);
            throw new System.TimeoutException("Bad loop: " + timeout + "s exceeded");
        }
        yield break;
    }




    IEnumerator DoorsInactiveByTime() {
        ignoreDoors = true;
        yield return new WaitForSeconds(3);
        ignoreDoors = false;
        yield return new WaitForSeconds(Random.Range(20, 30)); // 
        StopCoroutine("setRandomPPointAndPath");
        getDoorPosition(); // and exit
        yield break;
    }
    IEnumerator setRandomPPointAndPath() {
        while (true) {
            pursuitPoint = mobFactory.rndPoint();
            if(h_FindTheWay == null) {
                h_FindTheWay = FindTheWay();
                StartCoroutine(h_FindTheWay);
            }
            yield return new WaitForSeconds(Random.Range(4,8));
        }
    }
    void displayPath() {
        foreach (Vector2Int pos in path)
            Instantiate(TestingDot, mobFactory.nav.gridToRealCoords(pos), new Quaternion());
    }
}

