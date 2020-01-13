using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMesh : MonoBehaviour {

    public float meshSize = 0.1f;// public GameObject aaaa;
    int[,] mesh;
    public GameObject limT, limR, limL, limB;
	void InitNavMesh() {
        int width = (int)((limR.transform.position.x-limL.transform.position.x)/meshSize),
            height = (int)((limT.transform.position.y-limB.transform.position.y)/meshSize);
        mesh = new int[height, width]; // zeros by default, certainly
        for(int i = 0; i < height; i++)
            for(int q = 0; q < width; q++) {
                foreach(RaycastHit2D hit in Physics2D.RaycastAll(gridToRealCoords(new Vector2Int(q, i)), new Vector2()))
                    if(hit.transform.gameObject.tag != "People" && hit.transform.gameObject.tag != "Finish")
                        mesh[i,q] = 1;
                //if (mesh[i, q] == 0)
                //    Instantiate(aaaa, gridToRealCoords(new Vector2Int(q, i)), new Quaternion());
            }
    }

    Vector2 gridToRealCoords(Vector2Int gv) {
        return new Vector2( limL.transform.position.x + gv.x*meshSize + meshSize/2,
                            limB.transform.position.y + gv.y*meshSize + meshSize/2);
    }

    Vector2Int coordToGridValues(Vector2 coo) {
        return new Vector2Int( (int)( (coo.x-meshSize/2-limL.transform.position.x)/meshSize ),
                               (int)( (coo.y-meshSize/2-limB.transform.position.y)/meshSize ) );
    }
}
