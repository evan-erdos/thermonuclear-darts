using UnityEngine;
using System.Collections;

public class StartMovie : MonoBehaviour {
    void Start() {
        var movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
        movie.loop = true;
        movie.Play();
    }
}
