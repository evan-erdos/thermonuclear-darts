using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateTrail : MonoBehaviour {
	public GameObject trailObject;
	Vector3 oldPosition;
	public float drawInterval = 0.5f;
	private Queue<GameObject> trailInstances = new Queue<GameObject> ();
	public bool stopDrawingTrails = false;
	// Use this for initialization
	void Start () {
		StartCoroutine (generateTrail ());
		StartCoroutine (deleteTrail ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter() {
		stopDrawingTrails = true;
	}


	IEnumerator generateTrail() {
		while (true) {
			oldPosition = transform.position;
			yield return new WaitForSeconds (drawInterval);
			if (!stopDrawingTrails && Mathf.Abs((oldPosition-transform.position).sqrMagnitude) > 0.1f) {
				GameObject trail = (GameObject) Instantiate (trailObject, this.transform.position, Quaternion.identity);
				trailInstances.Enqueue (trail);
			}
		}
	}

	IEnumerator deleteTrail() {
		while (true) {
			yield return new WaitForSeconds (8 * drawInterval);
			if(trailInstances.Count > 0) Destroy(trailInstances.Dequeue());
		}
	}


	void OnDestroy() {
		DestroyTrail();
	}

	public void DestroyTrail() {
		while (trailInstances.Count > 0) {
			GameObject i = trailInstances.Dequeue ();
			Destroy (i);
		}
	}



}
