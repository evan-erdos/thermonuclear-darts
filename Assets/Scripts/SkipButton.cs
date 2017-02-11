using UnityEngine;
using System.Collections;

public class SkipButton : MonoBehaviour {

    [SerializeField] VirtualTerrain terrain;

    void OnCollisionEnter(Collision collision) {
        if (!collision.rigidbody.GetComponent<LawnDart>()) return;
        terrain.HitCity();
        Destroy(collision.rigidbody.GetComponent<LawnDart>().gameObject);
    }
}
