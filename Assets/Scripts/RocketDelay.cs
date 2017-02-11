using UnityEngine;
using System.Collections;

public class RocketDelay : MonoBehaviour {

	[SerializeField] float delay = 1f;


    IEnumerator Start() {
        yield return new WaitForSeconds(delay);
        GetComponent<Missile>().Launch("Let's Go Crazy!");
    }

}
