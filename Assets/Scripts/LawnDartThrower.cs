using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

class LawnDartThrower : MonoBehaviour {

    bool wait, fire;
    [SerializeField] float rate = 1f;
    [SerializeField] float force = 10f;
    [SerializeField] Transform throwDirection;
    [SerializeField] GameObject prefab;

	void Update() { fire = CrossPlatformInputManager.GetButton("Fire1"); }

    void FixedUpdate() { if (fire && !wait) StartCoroutine(Firing()); }

    IEnumerator Firing() {
        wait = true;
        var instance = Object.Instantiate(
            prefab,
            transform.position+transform.forward,
            throwDirection.rotation) as GameObject;
        var rigidbody = instance.GetComponent<Rigidbody>();
        rigidbody.AddForce(
            force: rigidbody.transform.forward.normalized*force,
            mode: ForceMode.VelocityChange);
        yield return new WaitForSeconds(rate);
        wait = false;
    }
}
