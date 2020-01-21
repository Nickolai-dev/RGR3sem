using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Smoke : MonoBehaviour {
    WallMesh grid;
    public float timeSpr = 3f;

    protected void Start() {
        grid = GameObject.Find("root").GetComponent<WallMesh>();
        StartCoroutine(c_burn());
    }

    protected IEnumerator c_burn() {
        Vector2[] offset = { new Vector2(-1,-1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, -1), new Vector2(1, 0), new Vector2(1, 1), };
        while(true) {
            yield return new WaitForSeconds(timeSpr);
            for(int i = 0; i < 8; i++) {
                Vector3 pnt = grid.cooToGrid(transform.position, offset[Random.Range(0,7)]);
                List<RaycastHit2D> lst = Physics2D.RaycastAll(pnt, new Vector2()).ToList();
                if(grid.smokeCount < grid.maxSmokeCount)
                if(!lst.Exists(a => a.collider.tag == "Smoke")   ) {
                    Instantiate(gameObject, pnt, new Quaternion());
                    grid.smokeCount++;
                    break;
                }else;else yield break;
            }
            
        }
    }
}
