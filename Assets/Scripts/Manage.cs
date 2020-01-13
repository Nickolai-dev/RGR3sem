﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Manage : MonoBehaviour {

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
        mouseNowPressingUIElement = true;
        if (state == st_.DEFAULT)
            state = st_.REDRAW;
        else if (state == st_.REDRAW)
            state = st_.DEFAULT;
    }
    void MapStart() {
        float T = 0, B = -0, L = -0, R = 0;
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Wall")) {
            if(g.transform.position.x < )
        }
        gameObject.SendMessage("InitNavMesh");

    }
    void FireStart() { Debug.Log("Start"); }
    void Reset() { Debug.Log("Reset"); }

}
