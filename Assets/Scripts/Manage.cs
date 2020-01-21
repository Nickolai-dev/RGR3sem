using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Manage : MonoBehaviour {
    public float wallWidth = 1.0f;
    public Animator panel, panel2;
    private Camera mainCam;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    List<RaycastResult> m_result = new List<RaycastResult>();
    void Start() {
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        m_Raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        m_EventSystem = GameObject.Find("Canvas").GetComponent<EventSystem>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
        mainCam.enabled = false;
        mainCam.enabled = true;

        DontDestroyOnLoad(mainCam.gameObject);
        DontDestroyOnLoad(GameObject.Find("root"));
        DontDestroyOnLoad(GameObject.Find("Canvas"));
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
        DontDestroyOnLoad(GameObject.Find("limiterL"));
        DontDestroyOnLoad(GameObject.Find("limiterR"));
        DontDestroyOnLoad(GameObject.Find("limiterB"));
        DontDestroyOnLoad(GameObject.Find("limiterT"));
    }

    enum st_ { REDRAW, DEFAULT };
    private st_ state = st_.DEFAULT;
    public bool mouseNowPressingUIElement = false;

    void FixedUpdate() {
        if (!Input.GetMouseButton(0) ) mouseNowPressingUIElement = false;
        float zoom = -Input.GetAxis("Mouse ScrollWheel")*2;
        if (zoom + mainCam.orthographicSize < 15 && zoom + mainCam.orthographicSize > 5)
            mainCam.orthographicSize += zoom;
        switch (state) {
            case st_.REDRAW: {
                if(mouseNowPressingUIElement) break;
                if (Input.GetMouseButton(0)) {
                    //Debug.Log(Input.mousePosition);
                    Vector3 p = mainCam.ScreenToWorldPoint(Input.mousePosition);
                    //RaycastHit2D[] hits = Physics2D.RaycastAll(p, new Vector2());
                    m_PointerEventData.position = Input.mousePosition;
                    m_Raycaster.Raycast(m_PointerEventData, m_result);
                    if (m_result.Count == 0)//(hits.Length == 0)// || hits[0].transform.gameObject.GetComponent<RectTransform>() == null)
                        gameObject.SendMessage("AssignWall", p);
                    m_result.Clear();
                    break;
                }
                if(Input.GetMouseButton(1)) { 
                    gameObject.SendMessage("Cancel", mainCam.ScreenToWorldPoint(Input.mousePosition));
                }
                if(Input.GetMouseButtonUp(0)) {
                    gameObject.SendMessage("ConfirmWall");
                }
                break;
            }
        }
    }

    void Redraw() {
        panel.GetComponent<Animator>().enabled = true;
        panel.SetBool("isHidden", !panel.GetBool("isHidden") );
        mouseNowPressingUIElement = true;
        if (state == st_.DEFAULT)
            state = st_.REDRAW;
        else if (state == st_.REDRAW)
            state = st_.DEFAULT;
    }
    void MapStart() {
        float T = -1000, B = 1000, L = 1000, R = -1000;
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Wall")) {
            if (g.transform.position.x <= L) L = g.transform.position.x;
            if (g.transform.position.x >= R) R = g.transform.position.x;
            if (g.transform.position.y <= B) B = g.transform.position.y;
            if (g.transform.position.y >= T) T = g.transform.position.y;
        }
        GameObject limL = GameObject.Find("limiterL"),
                   limR = GameObject.Find("limiterR"),
                   limB = GameObject.Find("limiterB"),
                   limT = GameObject.Find("limiterT");
        limL.transform.position = new Vector3(L-wallWidth/2, 0, 0);
        limR.transform.position = new Vector3(R+wallWidth/2, 0, 0);
        limB.transform.position = new Vector3(0, B-wallWidth/2, 0);
        limT.transform.position = new Vector3(0, T+wallWidth/2, 0);
        limL.GetComponent<SpriteRenderer>().enabled = false;
        limR.GetComponent<SpriteRenderer>().enabled = false;
        limB.GetComponent<SpriteRenderer>().enabled = false;
        limT.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.SendMessage("InitNavMesh");
        gameObject.SendMessage("SpawnPeoples");

    }
    public GameObject Fire, Smoke;
    void FireStart() {
        GetComponent<MobFactory>().StopAllCoroutines();
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("People")) {
            g.SendMessage("controllableEvacuation");
        }
        Vector3 pos;
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Fire")) {
            pos = g.transform.position;
            Destroy(g);
            Instantiate(Fire, pos, new Quaternion());
            Instantiate(Smoke, pos, new Quaternion());
        }
    }
    void Reset() {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("EditorOnly")) Destroy(g);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Fire")) Destroy(g);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Smoke")) Destroy(g);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("People")) Destroy(g);
        gameObject.GetComponent<MobFactory>().mobCount = 0;
        gameObject.GetComponent<WallMesh>().smokeCount = 0;
    }

    void Presets() {
        panel2.GetComponent<Animator>().enabled = true;
        panel2.SetBool("isHidden", !panel2.GetBool("isHidden"));
    }
    void sc1() { SceneManager.LoadScene(0); }
    void sc2() { SceneManager.LoadScene(1); }
    void sc3() { SceneManager.LoadScene(2); }
    void sc4() { SceneManager.LoadScene(3); }
    void sc5() { SceneManager.LoadScene(4); }

}
