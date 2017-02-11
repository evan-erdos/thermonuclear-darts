using System.Collections;
using UnityEngine;
using ui=UnityEngine.UI;

[RequireComponent(typeof(ui::Text))]
public class PopulationCounter : MonoBehaviour {
	private long currentPopulation;
	public long initialPopulation = 4000000000;
	public int decayRateBase = 1000;
	private int decayRate;
	private bool isDecay;
	private long instantKills;
	private long gradualKills;
	private string header = "POINTS: ";
	ui::Text text;

	void Start() {
		currentPopulation = initialPopulation;
		isDecay = false;
		decayRate = 0;
		instantKills = 0;
		gradualKills = 0;
		text = GetComponent<ui::Text>();
		text.text = header + (initialPopulation - currentPopulation).ToString();
	}

	void Update() {

	}
	//called after first explosion
	IEnumerator populationDecay() {
		while(currentPopulation > 0){
			killPeople((int)(decayRate * (Random.Range(1f, 1.5f))));
			text.text = header + (initialPopulation - currentPopulation).ToString();
			yield return new WaitForSeconds(1f);
		}
	}
	// decreases the population by a given int
	public void killPeopleInstant(long amount){
		if (!isDecay)
		{
			isDecay = true;
			decayRate += decayRateBase;
			StartCoroutine(populationDecay());
		}
		killPeople(amount);
		instantKills += amount;

	}

	private void killPeople(long amount)
	{
		lock (this)
		{
			currentPopulation -= amount;
		}
	}

	public long getInstantKills(){
		return instantKills;
	}
	public long getGradualKills(){
		return gradualKills;
	}
}
