using UnityEngine;
using System.Collections;

public class SpaceCamera : MonoBehaviour {

    [SerializeField] GameObject spaceCamera;

    IEnumerator Starting() {
        while(true) {
            yield return new WaitForFixedUpdate();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Camera.main.transform.rotation;
        }
    }
}
