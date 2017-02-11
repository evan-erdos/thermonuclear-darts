using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour {

    Vector3 speed;
    [SerializeField] float height;
	[SerializeField] AudioClip clip;
	[SerializeField] GameObject thatThing;
    Vector3 initialPosition;
	AudioSource audioSource;

	void Awake() {
		audioSource = GetComponent<AudioSource> ();
	}

    void Start() {
        initialPosition = transform.position;
		StartCoroutine (Rising ());
    }

    IEnumerator Rising() {
        yield return new WaitForSeconds(4f);
		audioSource.clip = clip;
		audioSource.Play ();
		while (transform.position.y<(initialPosition.y+height)) {
            yield return new WaitForFixedUpdate();
            transform.position = Vector3.SmoothDamp(
                transform.position,
				initialPosition+(Vector3.up*(height+1)), ref speed, 2.5f);
        }
		audioSource.Stop ();
		Destroy (thatThing);
    }
}
