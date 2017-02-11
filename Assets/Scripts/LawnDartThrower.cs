using UnityEngine;
using System.Collections;

class LawnDartThrower : MonoBehaviour {

    bool wait, fire;
    [SerializeField] float rate = 1f;
    [SerializeField] float force = 100f;
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject spaceCamera;

    void Start() { Instantiate(spaceCamera); }

	void Update() { fire = Input.GetButton("Fire1"); }

    void FixedUpdate() { if (fire && !wait) StartCoroutine(Firing()); }

    IEnumerator Firing() {
        wait = true;
        var instance = Object.Instantiate(
            prefab,
            transform.position+transform.forward,
            Quaternion.LookRotation(
                transform.forward,
                Vector3.up)) as GameObject;
        var rigidbody = instance.GetComponent<Rigidbody>();
        rigidbody.velocity = rigidbody.transform.forward.normalized*force;
        yield return new WaitForSeconds(rate);
        wait = false;
    }
}
