using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Manage : MonoBehaviour {
    public float wallWidth = 1.0f;
    public Animator panel;
    private Camera cam;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    List<RaycastResult> m_result = new List<RaycastResult>();
    void Start() {
        cam = Camera.main;
        m_Raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        m_EventSystem = GameObject.Find("Canvas").GetComponent<EventSystem>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
    }

    enum st_ { REDRAW, DEFAULT };
    private st_ state = st_.DEFAULT;
    public bool mouseNowPressingUIElement = false;
    void FixedUpdate() {
        if (!Input.GetMouseButton(0) ) mouseNowPressingUIElement = false;
        switch (state) {
            case st_.REDRAW: {
                if(mouseNowPressingUIElement) break;
                if (Input.GetMouseButton(0)) {
                    //Debug.Log(Input.mousePosition);
                    Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
                    //RaycastHit2D[] hits = Physics2D.RaycastAll(p, new Vector2());
                    m_PointerEventData.position = Input.mousePosition;
                    m_Raycaster.Raycast(m_PointerEventData, m_result);
                    if (m_result.Count == 0)//(hits.Length == 0)// || hits[0].transform.gameObject.GetComponent<RectTransform>() == null)
                        gameObject.SendMessage("AssignWall", p);
                    m_result.Clear();
                    break;
                }
                if(Input.GetMouseButton(1)) { 
                    gameObject.SendMessage("Cancel", cam.ScreenToWorldPoint(Input.mousePosition));
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
    void Reset() { foreach(GameObject g in GameObject.FindGameObjectsWithTag("EditorOnly")) Destroy(g); Debug.Log("Reset"); }

}
