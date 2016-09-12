using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Rigidbody))]
class LawnDart : MonoBehaviour {

    bool wait;
    float length = 0.25f;
    Transform center;

// disables a silly warning
#pragma warning disable 0108
    Rigidbody rigidbody;
#pragma warning restore 0108

    void Awake() {
        center = transform.Find("center");
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = center.position;
    }


    // callback for collisions, must have this exact function signature
    //void OnCollisionEnter(Collision collision) { }

    // called when the front "trigger" collider is tripped
    void OnTriggerEnter(Collider other) {
        if (!wait) StartCoroutine(Colliding());
    }

    // coroutines are a great way to deal with timed sequences of events
    IEnumerator Colliding() {
        wait = true;
        yield return new WaitForSeconds(rigidbody.velocity.magnitude*length);
        rigidbody.isKinematic = true;
        //wait = false;
    }

}
