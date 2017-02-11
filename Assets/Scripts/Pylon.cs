using UnityEngine;
using System.Collections;

public class Pylon : MonoBehaviour {

	bool wait;
	[SerializeField] float delay = 0.75f;
	[SerializeField] Color color = new Color(1,1,1,1);
	Renderer[] renderers;

	void Awake() {
		renderers = GetComponentsInChildren<Renderer>();
	}

	void Start() {
		ChangeColor(Color.black);
	}


	void OnTriggerEnter(Collider other) {
		if (!wait && other.attachedRigidbody.GetComponent<LawnDart>())
		 	StartCoroutine(Triggered());
	}


	IEnumerator Triggered() {
		wait = true;
		ChangeColor(color);
		yield return new WaitForSeconds(delay);
		ChangeColor(Color.black);
		wait = false;
	}


	void ChangeColor(Color value) {
		value *= Mathf.LinearToGammaSpace(12);
        foreach (var renderer in renderers)
        	foreach (var material in renderer.materials)
            	material.SetColor("_EmissionColor", value);
    }


}
