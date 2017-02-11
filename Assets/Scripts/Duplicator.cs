using UnityEngine;
using System.Collections;

public class Duplicator : MonoBehaviour {

    [SerializeField] GameObject copy;
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 rotation;
    [SerializeField] Transform parent;

    void Start() {
        foreach (Transform child in parent) {
            var instance = Instantiate(copy) as GameObject;
            instance.transform.parent = child;
            instance.transform.localPosition = position;
            instance.transform.localRotation = Quaternion.Euler(rotation);
        }
    }


}
