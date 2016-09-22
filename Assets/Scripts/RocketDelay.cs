using UnityEngine;
using System.Collections;

public class RocketDelay : MonoBehaviour {

    IEnumerator Start() {
        yield return new WaitForSeconds(100);
        GetComponent<Missile>().Launch("Let's Go Crazy!");
    }

}
