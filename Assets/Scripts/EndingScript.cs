using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EndingScript : MonoBehaviour {
	private AudioSource audioSource;
	// Use this for initialization

	void Awake() {
		audioSource = GetComponent<AudioSource>();
		enabled = false;
		//StartCoroutine(test());
	}

	void Start () {
		audioSource.Play();
		//GameObject.Find("Panel").GetComponent<DimToDark>().enabled = true;
		transform.parent.GetComponentInChildren<EndingSubs>().enabled = true;
	}


	IEnumerator test() {
		yield return new WaitForSeconds(2f);
		enabled = true;
	}
}
