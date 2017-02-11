using UnityEngine;
using System.Collections;

public class right_end : MonoBehaviour {

	bool once;

	[SerializeField] VirtualTerrain terrain;

	private void OnTriggerEnter(Collider collider) {
		if (collider.attachedRigidbody.GetComponent<turnDial1> ())
		if (terrain.IsDoomsday && !once) {
			terrain.BeginEndOfDays (); once = true; }
		
	}
}
