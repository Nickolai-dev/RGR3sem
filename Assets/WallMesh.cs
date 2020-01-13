using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WallMesh : MonoBehaviour {

    public float gridSize;
    public GameObject Wall;
    List<Vector3> stagingNodes = new List<Vector3>();
    List<GameObject> UInodes = new List<GameObject>(); // TODO: make a tracing

    void FixedUpdate() {
        //there
    }

    void AssignWall(Vector3 pos) {
        pos = new Vector3(Mathf.RoundToInt(pos.x/gridSize)*gridSize, Mathf.RoundToInt(pos.y/gridSize)*gridSize, 0);

        if(stagingNodes.Count == 0 || stagingNodes.Last() != pos)
            stagingNodes.Add(pos);
    }

    void ConfirmWall() {
        stagingNodes = stagingNodes.Distinct().ToList();
        foreach (Vector3 pos in stagingNodes)
            Instantiate(Wall, pos, new Quaternion());
        stagingNodes.Clear();
        foreach(GameObject i in UInodes)
            Destroy(i);
    }

    void Cancel() {
        stagingNodes.Clear();
        foreach(GameObject i in UInodes)
            Destroy(i);
    }
}
