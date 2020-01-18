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
                { i+= (cur.vector-cur.prev.vector).sqrMagnitude > 1 ? 14 : 10; }
                return i; } }
    };
    public bool ignoreDoors = false;
    public MobFactory mobFactory;
    List<Node> open = new List<Node>(), closed = new List<Node>();
    List<Vector2Int> path = new List<Vector2Int>();
    Vector2 pursuitPoint = new Vector2();

    public GameObject aaaa;
    void Start() {
        IEnumerator coroutineHandler = FindTheWay();
        //StartCoroutine(protectFromLoop(coroutineHandler));
        StartCoroutine(coroutineHandler);
    }

    IEnumerator FindTheWay() {
        closed.Clear();
        open.Clear();
        Node start = new Node(){vector=mobFactory.nav.coordToGridValues(transform.position), prev=null}, current;
        Vector2Int end = mobFactory.nav.coordToGridValues(pursuitPoint);
        open.Add(start);
        List<Vector2Int> nbrs;
        while (open.Count > 0) {
            //if(Random.Range(0,1) == 1) open.Reverse(); // randomize equal paths
            open.Sort(Comparer1);
            current = open[0];
            closed.Add(current);
            open.RemoveAt(0);
            //Instantiate( aaaa, mobFactory.nav.gridToRealCoords(current.vector), new Quaternion() )
            //    .GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
            //yield return new WaitForFixedUpdate();//WaitForSeconds(0.0f);
            nbrs = getNeighbours(current, mobFactory.nav.mesh);
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
                    foreach (Vector2Int pos in path)
                        Instantiate( aaaa, mobFactory.nav.gridToRealCoords(pos), new Quaternion() );
                    yield break;
                }
            }
        }
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
    List<Vector2Int> getNeighbours(Node cur, int[,] field) {
        List<Vector2Int> nbrs = new List<Vector2Int>();
        foreach(int i in new int[]{-1, 0, 1})
            foreach(int q in new int[]{-1, 0, 1}){
                if(i==0 && q==0) continue;
                Vector2Int doubt = new Vector2Int(cur.vector.x + q, cur.vector.y + i);
                try {
                    if( field[cur.vector.y+i, cur.vector.x+q] == 0
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
}

