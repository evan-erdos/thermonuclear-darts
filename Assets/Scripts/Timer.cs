using UnityEngine;
using System.Collections;
using ui = UnityEngine.UI;

[RequireComponent(typeof(ui::Text))]
public class Timer : MonoBehaviour {

	[SerializeField] VirtualTerrain terrain;
	bool stop = true;
	private int timeLeft;
	public int timeLimit;
	ui::Text text;
	private string header = "TIME LEFT: ";

	void Awake() {
		//enabled = false;
	}

	// Use this for initialization
	void Start () {
		text = GetComponent<ui::Text>();
		timeLeft = timeLimit;
		StartCoroutine(updateClock());
	}

	// Update is called once per frame
	void Update () {
		if (timeIsUp ()) return; // missiles, comin at cha
	}

	int MinutesUsed() {
		return timeLimit / 60 - timeLeft / 60;
	}

	IEnumerator updateClock(){
		while(timeLeft > 0){
			timeLeft -= 1;
			if(timeLeft <= 60) text.color = text.color == Color.white ? Color.red : Color.white;
			text.text = header + formatTime();
			yield return new WaitForSeconds(1f);
		}
	}

	private string formatTime(){
		return timeLeft / 60 + " : " + (timeLeft % 60).ToString("00");
	}

	void StopTimer() { stop = false; }

	bool timeIsUp() { return timeLeft == 0; }
}
