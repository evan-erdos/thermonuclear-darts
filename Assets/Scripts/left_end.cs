using UnityEngine;
using System.Collections;

public class left_end : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.attachedRigidbody.GetComponent<turnDial1> ())
			print ("yes1");
	}
}
