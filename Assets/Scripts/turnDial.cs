using UnityEngine;
using System.Collections;

public class turnDial : MonoBehaviour {



	GameObject controller;
	float knob_z, speed;
	float controller_z;
	float delta_z;
	Coroutine coroutine;


	void FixedUpdate () {
		var angle = transform.localEulerAngles.z % 360;
		if (angle > 150f - float.Epsilon || angle < -120f - float.Epsilon)
			StartCoroutine(Suicide());
	}

	private void OnTriggerEnter(Collider collider)
	{
		controller = collider.gameObject;
		knob_z = transform.localEulerAngles.z;
		controller_z = controller.transform.eulerAngles.z;
		coroutine = StartCoroutine (turnKnob());
	}

	IEnumerator Suicide() {
		if (coroutine!=null) StopCoroutine (coroutine);
		yield return new WaitForSeconds (1);
		while (Mathf.Abs (transform.localEulerAngles.z) > 0.01f) {
			yield return new WaitForFixedUpdate ();
			transform.localEulerAngles = new Vector3 (0, 0, Mathf.SmoothDampAngle (transform.localEulerAngles.z, 0f, ref speed, 2f));
		}
	}


	IEnumerator turnKnob()
	{
		while (true) {
			//transform.localRotation = controller.transform.rotation;
			delta_z = controller_z - controller.transform.eulerAngles.z;
			transform.localEulerAngles = new Vector3(0, 0, (knob_z+delta_z)%360);
			yield return new WaitForFixedUpdate();
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		StopAllCoroutines (); //StopCoroutine (coroutine1);
	}

}


