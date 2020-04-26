using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RisingInfo : MonoBehaviour {
    public Color BLUE = new Color(.1f, .1f, 1, 1);
    public Color RED = new Color(1, .1f, .1f, 1);
    public Color ORANGE = new Color(1, .6f, 0, 1);
    protected float timeout = 2f;
    protected float time_alive = 0;
    public Text fg_T, bg_T;

    public void init(string resource, int value) {
        string readout = "";
        if (value > 0) {
            readout += "+ ";
        } else {
            //readout += "- ";
        }
        readout += value.ToString() + " " + resource;
        set_text(readout);
    }

    protected virtual void Update() {
        time_alive += Time.deltaTime;
        translate_up(1f);
        fade();
        if (time_alive > timeout)
            die(); 
    }

    protected void translate_up(float dy) {
        this.transform.Translate(0, dy, 0);
    }

    protected void fade() {
        fg_T.color = new Color(fg_T.color.r, fg_T.color.g, 
            fg_T.color.b, 1 - (time_alive / timeout));
        bg_T.color = new Color(.9f, .9f, .9f, 1 - (time_alive / timeout));
    }


    protected void set_text(string text) {
        fg_T.text = text;
        bg_T.text = text;
    }

    protected void die() {
        Destroy(gameObject);
    }
}
