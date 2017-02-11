using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
class LawnDart : MonoBehaviour {

    bool wait;
    [SerializeField] float aerodynamicFactor = 100f;
    [SerializeField] GameObject particles;
    Transform center;

#pragma warning disable 0108
    Rigidbody rigidbody;
#pragma warning restore 0108

    public float ForwardVelocity { get {
        return Mathf.Max(0,
            transform.InverseTransformDirection(
                rigidbody.velocity).z); } }


    void Awake() {
        center = transform.Find("center");
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = center.localPosition;
    }

    IEnumerator Start() {
        while (true) {
            yield return new WaitForSeconds(0.01f);
            wait = false;
        }
    }

    void FixedUpdate() {
        if (rigidbody.isKinematic) return;
        if (rigidbody.velocity.magnitude<float.Epsilon) return;

        var deltaDirection = Vector3.Dot(
            transform.forward,
            rigidbody.velocity.normalized);
        deltaDirection *= deltaDirection;

        rigidbody.velocity = Vector3.Lerp(
            rigidbody.velocity,
            transform.forward * ForwardVelocity,
            deltaDirection * Time.fixedDeltaTime * ForwardVelocity);

       rigidbody.rotation = Quaternion.Slerp(
            rigidbody.rotation,
            Quaternion.LookRotation(
                rigidbody.velocity-Physics.gravity*Time.fixedDeltaTime,
                Vector3.up),
            Time.fixedDeltaTime * aerodynamicFactor);
    }


    void OnCollisionEnter(Collision collision) {
        if (wait) return;
        rigidbody.isKinematic = true;
        wait = true;
        foreach (var contact in collision.contacts) {
            Instantiate(
                particles,
                contact.point,
                Quaternion.LookRotation(contact.point+contact.normal));
            wait = true;
            break;
        }
    }
}
