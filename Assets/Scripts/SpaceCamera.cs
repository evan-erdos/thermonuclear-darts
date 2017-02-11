using UnityEngine;
using System.Collections;

class SpaceCamera : MonoBehaviour {
    IEnumerator Start() {
        //yield return new WaitForFixedUpdate();
        //GetComponent<GvrHead>().trackRotation = false;
        //GetComponent<GvrHead>().trackPosition = false;
        while(true) {
            yield return null;
            transform.localPosition = Camera.main.transform.position/100000;
            transform.localRotation = Camera.main.transform.rotation;
        }
    }
}
