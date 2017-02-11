using UnityEngine;
using System.Collections;

public class MasterAlarm : MonoBehaviour {

    bool wait, isOn;
    Renderer render;
    AudioSource audioButton;
    Light alarmLight;
    [SerializeField] float delay = 10f;
    [SerializeField] Material onMaterial;
    [SerializeField] AudioClip sound;
    [SerializeField] AudioClip alarm;
    [SerializeField] Material offMaterial;
    [SerializeField] Radio radio;

    void Awake() { 
        render = GetComponent<Renderer>(); 
        alarmLight = GetComponentInChildren<Light>(); 
        audioButton = GetComponent<AudioSource>();
    }

    void Start() { render.sharedMaterial = offMaterial; }

    void OnTriggerEnter(Collider collider) {
        if (collider.attachedRigidbody) Illuminate(); 
    }

    void Illuminate() { if (!wait) StartCoroutine(Illuminating()); }

    IEnumerator Illuminating() {
        radio.StopBGMusic();
        wait = true;
        isOn = !isOn;
        render.sharedMaterial = (isOn) ? onMaterial : offMaterial;
        AudioSource.PlayClipAtPoint(sound, transform.position,0.5f);
        alarmLight.enabled = true;
        audioButton.clip = alarm;
        audioButton.loop = true;
        audioButton.Play();
        yield return new WaitForSeconds(delay);
        radio.StartBGMusic();
        alarmLight.enabled = false;
        isOn = !isOn;
        render.sharedMaterial = (isOn) ? onMaterial : offMaterial;
        audioButton.Stop();
        wait = false;
    }

}
