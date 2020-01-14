using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobFactory : MonoBehaviour {

    public GameObject mob;
    public int StartSpawn = 5, // spawn momentally in random places
               spawnCount = 2, // spawned in doors
               spawnRate = 5000, // ms
               chanceOfHeroic = 5, //%
               chanceOfPanicer = 5; //%
    GameObject[] doors;
    public NavMesh nav;
    float x0, y0, x1, y1;
    Coroutine c_spawner = null;
    [Unity.Collections.ReadOnly]
    public int mobCount = 0;

    void Start() {
        doors = GameObject.FindGameObjectsWithTag("Finish");
        nav = GetComponent<NavMesh>();
    }
    void SpawnPeoples() {
        NavMesh nav = GetComponent<NavMesh>();
        x0 = nav.limR.transform.position.x; x1 = nav.limL.transform.position.x;
        y0 = nav.limB.transform.position.y; y1 = nav.limT.transform.position.y;
        for (int i = 0; i < StartSpawn; i++) {
            Instantiate(mob, rndPoint(), new Quaternion()).GetComponent<Behaviour>().mobFactory = this;
            mobCount++;
        }
        c_spawner = StartCoroutine(c_spawn());
    }

    IEnumerator c_spawn() {
        while(true) {
            yield return new WaitForSeconds((float)spawnRate/1000);
            for (int i = 0; i < spawnCount; i++) {
                GameObject people = Instantiate(mob, doors[Random.Range(0, doors.Length)].transform.position
                    + new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f), 0), new Quaternion());
                people.GetComponent<Behaviour>().StartCoroutine("DoorsInactiveByTime");
                people.GetComponent<Behaviour>().mobFactory = this;
                mobCount++;
            }
        }
    }

    public Vector3 rndPoint() {
        Vector3 v;
        Vector2Int gv;
        while (true) {
            v = new Vector3(Random.Range(x0, x1), Random.Range(y0, y1), 0);
            gv = nav.coordToGridValues(v);
            try {
                if (nav.mesh[gv.y, gv.x] == 0)
                    return v;
            } catch (System.IndexOutOfRangeException e) { Debug.Log("outOfRng"); }
        }
    }
}
