using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour {

    bool once;
    [SerializeField] GameObject explosion;

	// Use this for initialization
	void Start () {
        print("Running");
    }

	void OnCollisionEnter(Collision c) {
        print("LASKDJVDVLK");
        if (once) return;
        once = true;
        Vector3 point = Vector3.zero, normal = Vector3.zero;
        foreach (var contact in c.contacts)
        {
            point = contact.point;
            normal = contact.normal;
            break;
        }
            Instantiate(explosion, point, Quaternion.LookRotation(point+normal));
        Destroy(GetComponentInParent<LawnDart>().gameObject);
	}
}
