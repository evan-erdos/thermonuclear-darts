using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Shortcuts : MonoBehaviour {

	[SerializeField] VirtualTerrain terrain;

	void Update () {
		if (Input.GetKey(KeyCode.Backspace)) Restart();
		if (Input.GetKey(KeyCode.Return)) Next();
	}

	void Restart() { 
		SceneManager.LoadScene(
			SceneManager.GetActiveScene().name); }

	void Next() { terrain.HitCity(); }
}
