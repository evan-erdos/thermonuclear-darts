using UnityEngine;
using System.Collections;

public class turnDial1 : MonoBehaviour {

	float lastAngle;

	GameObject controller;
	float knob_z, speed;
	float controller_z;
	float delta_z;
	bool hasEnter;

	void FixedUpdate () {
	}

	private void OnTriggerEnter(Collider collider)
	{
		hasEnter = true;
		controller = collider.gameObject;
		knob_z = transform.localEulerAngles.z;
		controller_z = controller.transform.eulerAngles.z;
		StartCoroutine (turnKnob());
	}



	IEnumerator turnKnob()
	{
		while (true) {
			delta_z = controller_z - controller.transform.eulerAngles.z;
			if (Mathf.Abs (delta_z - lastAngle) > 1f) {
				lastAngle = delta_z;
				tossDart.DoHapticPulse ();
			}
			Vector3 r = new Vector3 (0, 0, delta_z);
			transform.Rotate(r, Space.Self);
			controller_z = controller.transform.eulerAngles.z;
			yield return new WaitForFixedUpdate();
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		StopAllCoroutines (); //StopCoroutine (coroutine1);
	}

}


