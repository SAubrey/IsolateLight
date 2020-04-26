using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour {
    private Transform trans;
    private static Quaternion q = new Quaternion(0, 0, 0, 0); 
    private static Vector3 v = new Vector3(0, 0, 0);
    private Transform pTrans;
    private Vector3 camPos;

    void Start() {
        trans = gameObject.transform;
        pTrans = GameObject.Find("Player").GetComponent<Transform>();
        camPos = gameObject.transform.position;
    }

    void Update() {
        if (pTrans)
            trans.position = new Vector3(pTrans.position.x, pTrans.position.y, camPos.z);
        //trans.rotation = Quaternion.Euler(pTrans.rotation);
    }
}
