using UnityEngine;
using System.Collections;

public class ButtonLight : MonoBehaviour {

    bool wait, isOn;
    Renderer render;
    [SerializeField] float delay = 0.25f;
    [SerializeField] Material onMaterial;
    [SerializeField] AudioClip sound;
    [SerializeField] Material offMaterial;

    void Awake() { 
        render = GetComponent<Renderer>(); 
    }

    void Start() { render.sharedMaterial = offMaterial; }

    void OnTriggerEnter(Collider collider) {
        if (collider.attachedRigidbody.GetComponent<tossDart>()) 
            Illuminate(); 
    }

    void Illuminate() { if (!wait) StartCoroutine(Illuminating()); }

    IEnumerator Illuminating() {
        wait = true;
        isOn = !isOn;
        render.sharedMaterial = (isOn) ? onMaterial : offMaterial;
        AudioSource.PlayClipAtPoint(sound, transform.position,0.5f);
        yield return new WaitForSeconds(delay);
        isOn = !isOn;
        render.sharedMaterial = (isOn) ? onMaterial : offMaterial;
        wait = false;
    }

}
