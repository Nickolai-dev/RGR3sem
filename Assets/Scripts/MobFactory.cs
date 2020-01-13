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
    [Unity.Collections.ReadOnly]
    public int mobCount = 0;

    void Start() {
        doors = GameObject.FindGameObjectsWithTag("Finish");
    }
    void SpawnPeoples() {
        NavMesh nav = GetComponent<NavMesh>();
        float[] x = { nav.limR.transform.position.x, nav.limL.transform.position.x },
                y = { nav.limB.transform.position.y, nav.limT.transform.position.y };
        for (int i = 0; i < StartSpawn; i++) {
            Instantiate(mob, rndPoint(x[0], y[0], x[1], y[1]), new Quaternion());
            mobCount++;
        }
        StartCoroutine(c_spawn());
    }

    IEnumerator c_spawn() {
        yield return new WaitForSeconds((float)spawnRate/1000);
        for (int i = 0; i < spawnCount; i++) {
            Instantiate( mob, doors[Random.Range(0, doors.Length-1)].transform.position
                + new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f), 0), new Quaternion() )
            .GetComponent<Behaviour>().StartCoroutine("DoorsInactiveByTime");
            mobCount++;
        }
    }

    Vector3 rndPoint(float x0, float y0, float x1, float y1) {
        return new Vector3( Random.Range(x0, x1), Random.Range(y0, y1), 0 );
    }
}
