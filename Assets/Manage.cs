using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage : MonoBehaviour {

    private Camera cam;

    void Start() {
        cam = Camera.main;
    }

    enum st_ { REDRAW, DEFAULT };
    private st_ state = st_.DEFAULT;
    bool mouseNowPressingUIElement = false;
    void FixedUpdate() {
        if (!Input.GetMouseButton(0) ) mouseNowPressingUIElement = false;
        switch (state) {
            case st_.REDRAW: {
                if(mouseNowPressingUIElement) break;
                if (Input.GetMouseButton(0)) {
                    //Debug.Log(Input.mousePosition);
                    gameObject.SendMessage("AssignWall", cam.ScreenToWorldPoint(Input.mousePosition));
                }
                if(Input.GetMouseButtonDown(1)) { 
                    gameObject.SendMessage("Cancel");
                }
                if(Input.GetMouseButtonUp(0)) {
                    gameObject.SendMessage("ConfirmWall");
                }
                break;
            }
        }
    }

    void Redraw() {
        if (state == st_.DEFAULT)
            state = st_.REDRAW;
        else if (state == st_.REDRAW)
            state = st_.DEFAULT;
        mouseNowPressingUIElement = true;
    }
    void FireStart() { Debug.Log("Start"); }
    void Reset() { Debug.Log("Reset"); }

}
