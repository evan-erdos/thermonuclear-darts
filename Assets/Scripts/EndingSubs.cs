using UnityEngine;
using System.Collections;
using ui = UnityEngine.UI;

public class EndingSubs : MonoBehaviour {
	// Use this for initialization

	private string result;
	private ui::Text text;
	private bool showed;
	public float subsInterval = 3f;

	void Awake() {
		enabled = false;
		text = GetComponent<ui::Text>();
	}
	void Start () {
		showed = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(GameObject.Find("Control").GetComponent<AudioSource>().time > 12f && !showed){
			showed = true;
			StartCoroutine(showScript());
		}
	}

	IEnumerator showScript() {
		result += "You destroyed Earth in " + 5 + " minutes.\n";
		text.text = result;
		yield return new WaitForSeconds(subsInterval);
		result += "In this time, about " + GameObject.FindWithTag ("ScoreBoard").GetComponent<PopulationCounter> ().getInstantKills () + " people were vaporized during the tremendous nuclear explosions.\n";
		text.text = result;
		yield return new WaitForSeconds(subsInterval);
		result += "5 people were making a ridiculous nuclear lawn dart game when the first nuke went off.";
		text.text = result;
		yield return new WaitForSeconds(subsInterval);
	}
}
