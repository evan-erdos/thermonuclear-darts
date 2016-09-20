using UnityEngine;
using System.Collections;

class SpaceCamera : MonoBehaviour {
    IEnumerator Start() {
        yield return new WaitForFixedUpdate();
        //GetComponent<GvrHead>().trackRotation = false;
        //GetComponent<GvrHead>().trackPosition = false;
        while(true) {
            yield return new WaitForFixedUpdate();
            transform.localPosition = Camera.main.transform.position/1000000;
            transform.localRotation = Camera.main.transform.rotation;
        }
    }
}
