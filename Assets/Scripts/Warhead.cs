using UnityEngine;
using System.Collections;


public class Warhead : MonoBehaviour {

    bool wait;
    [SerializeField] float force = 2000f;
    [SerializeField] GameObject explosion;
	[SerializeField] GameObject debris;

    void OnCollisionEnter(Collision collision) {
        if (wait) return;
        wait = true;
		foreach (var mesh in GetComponentsInChildren<MeshRenderer>()) Destroy(mesh.gameObject);
		Instantiate(debris, transform.position, Quaternion.identity);
        Instantiate(explosion, transform.position, Quaternion.identity);
        foreach (var rigidbody in GetComponentsInChildren<Rigidbody>()) {
            rigidbody.isKinematic = false;
            rigidbody.AddExplosionForce(force, transform.position, 10f, 1f);
            rigidbody.transform.parent = null;
        }
    }
}
