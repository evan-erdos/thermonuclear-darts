using UnityEngine;
using System.Collections;

public class PlanetSpin : MonoBehaviour {

    [SerializeField] float speed = 10f;

	void FixedUpdate() {
	   transform.Rotate(speed * Time.fixedDeltaTime, speed*Time.fixedDeltaTime,0);
	}
}
