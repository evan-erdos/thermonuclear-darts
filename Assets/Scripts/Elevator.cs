using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour {

    bool once;
    Vector3 speed;
    [SerializeField] float height;
    Vector3 initialPosition;

    void Start() {
        initialPosition = transform.position;
    }

    void OnTriggerEnter(Collider other) {
        if (!once && other.tag=="Player")
            StartCoroutine(Rising()); }

    IEnumerator Rising() {
        once = true;
        yield return new WaitForSeconds(0.5f);
        while (transform.position.y<(initialPosition.y+height)) {
            yield return new WaitForFixedUpdate();
            transform.position = Vector3.SmoothDamp(
                transform.position,
                initialPosition+(Vector3.up*(height+1)),
                ref speed, 2f, Time.fixedDeltaTime*8f);
        }
    }
}
