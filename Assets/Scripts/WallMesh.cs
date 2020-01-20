﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WallMesh : MonoBehaviour {

    public float gridSize;
    public GameObject Wall, Fire, Extinguisher, Door, Sign;
    GameObject inst;
    List<Vector3> stagingNodes = new List<Vector3>();
    List<GameObject> UInodes = new List<GameObject>(); // TODO: make a tracing

    void Start() {
        inst = Wall;
    }
    void FixedUpdate() {
        //there
    }
    void setWall() { inst = Wall; }
    void setExt() { inst = Extinguisher; }
    void setFire() { inst = Fire; }
    void setDoor() { inst = Door; }
    void setSign() { inst = Sign; }
    void AssignWall(Vector3 pos) {
        pos = new Vector3(Mathf.RoundToInt(pos.x/gridSize)*gridSize, Mathf.RoundToInt(pos.y/gridSize)*gridSize, 0);

        if(stagingNodes.Count == 0 || stagingNodes.Last() != pos)
            stagingNodes.Add(pos);
    }

    void ConfirmWall() {
        stagingNodes = stagingNodes.Distinct().ToList();
        foreach (Vector3 pos in stagingNodes)
            Instantiate(inst, pos, new Quaternion());
        stagingNodes.Clear();
        foreach(GameObject i in UInodes)
            Destroy(i);
    }

    void Cancel(Vector3 pos) {
        stagingNodes.Clear();
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, new Vector2());
        foreach( RaycastHit2D hit in hits) {
            Destroy(hit.collider.gameObject);
        }
        foreach(GameObject i in UInodes)
            Destroy(i);
    }
}
