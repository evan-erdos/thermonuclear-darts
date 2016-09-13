using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Rigidbody))]
class LawnDart : MonoBehaviour {

    bool wait;
    float length = 0.1f, aerodynamicFactor = 100f;
    Transform center;

// disables a silly warning
#pragma warning disable 0108
    Rigidbody rigidbody;
#pragma warning restore 0108

    public float ForwardVelocity {
        get {
            return Mathf.Max(0,transform.InverseTransformDirection(rigidbody.velocity).z);
        }
    }


    void Awake() {
        center = transform.Find("center");
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = center.position;
    }


    // runs on GPU time / with the physics step
    void FixedUpdate() {
        if (rigidbody.isKinematic || rigidbody.velocity.magnitude<0) return;

        var deltaDirection = Vector3.Dot(
            transform.forward,
            rigidbody.velocity.normalized);
        deltaDirection *= deltaDirection;

        rigidbody.velocity = Vector3.Lerp(
            rigidbody.velocity,
            transform.forward*ForwardVelocity,
            deltaDirection*Time.fixedDeltaTime*ForwardVelocity);

        rigidbody.rotation = Quaternion.Slerp(
            rigidbody.rotation,
            Quaternion.LookRotation(rigidbody.velocity, transform.up),
            Time.fixedDeltaTime*aerodynamicFactor);
    }


    // callback for collisions, must have this exact function signature
    void OnCollisionEnter(Collision collision) {
        rigidbody.isKinematic = true;
        wait = true;
    }

    // called when the front "trigger" collider is tripped
    void OnTriggerEnter(Collider other) { if (!wait) StartCoroutine(Colliding()); }

    // coroutines are a great way to deal with timed sequences of events
    IEnumerator Colliding() {
        wait = true;
        yield return new WaitForSeconds(rigidbody.velocity.magnitude*length);
        rigidbody.isKinematic = true;
        //wait = false;
    }

}
