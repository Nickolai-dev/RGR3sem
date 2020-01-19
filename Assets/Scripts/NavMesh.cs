using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMesh : MonoBehaviour {

    public float meshSize = 0.1f;
    public GameObject aaaa;
    public int[,] mesh;
    public int width, height;
    public GameObject limT, limR, limL, limB;
    public List<Vector3> doors = new List<Vector3>();
	void InitNavMesh() {
        width = (int)((limR.transform.position.x - limL.transform.position.x) / meshSize);
            height = (int)((limT.transform.position.y-limB.transform.position.y)/meshSize);
        mesh = new int[height, width]; // zeros by default, certainly
        doors.Clear();
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Finish")) { doors.Add(g.transform.position); }
        for(int i = 0; i < height; i++)
            for(int q = 0; q < width; q++) {
                foreach(RaycastHit2D hit in Physics2D.RaycastAll(gridToRealCoords(new Vector2Int(q, i)), new Vector2()))
                    if(hit.transform.gameObject.tag != "People" && hit.transform.gameObject.tag != "Finish")
                        mesh[i,q] = 1;
                //if (mesh[i, q] == 0)
                //    Instantiate(aaaa, gridToRealCoords(new Vector2Int(q, i)), new Quaternion());
            }
    }

    public Vector2 gridToRealCoords(Vector2Int gv) {
        return new Vector2( limL.transform.position.x + gv.x*meshSize,
                            limB.transform.position.y + gv.y*meshSize);
    }

    public Vector2Int coordToGridValues(Vector2 coo) {
        return new Vector2Int( (int)Mathf.Round( (coo.x-limL.transform.position.x)/meshSize ),
                               (int)Mathf.Round( (coo.y-limB.transform.position.y)/meshSize ) );
    }
}
