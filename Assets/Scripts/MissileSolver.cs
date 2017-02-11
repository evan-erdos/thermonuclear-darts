using UnityEngine;
using System.Collections;

public class MissileSolver : MonoBehaviour {

	public Missile missile;
	public LawnDart dart;
	public City city;
	public Transform missileTarget;

	void FixedUpdate () {
		//if (!dart || !city || !missile || !missileTarget) return;
		//var dartDist = (dart.GetComponent<Rigidbody>().position.y - city.transform.position.y);
		//var missileDist = (missile.transform.position.y - missileTarget.position.y);
		//var speed = new Vector3(0, 0, dart.ForwardVelocity * (missileDist/dartDist));
		//missile.GetComponent<Rigidbody>().velocity = speed;
	}
}
