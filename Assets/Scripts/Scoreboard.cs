using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Scoreboard : MonoBehaviour {

    bool wait;
    Vector3 speed, initial;

    void Awake() { initial = transform.position; }

    public void Extend() {
        if (!wait) StartCoroutine(Moving(6f)); }

    public void Retract() {
        if (!wait) StartCoroutine(Moving(0)); }

    IEnumerator Moving(float dist) {
        wait = true;
        var dest = new Vector3(initial.x, initial.y+dist, initial.z);
        while (Mathf.Abs((transform.position-dest).magnitude)>0.1f) {
            yield return new WaitForFixedUpdate();
            transform.position = Vector3.SmoothDamp(
                transform.position, dest, ref speed, 1.5f);
        }
        wait = false;
    }

}

