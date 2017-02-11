using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class City : MonoBehaviour {

    bool wait;
    public int shape;
    public float color = 0.2f;
    public float blend;
    public float height = 80f;
    public float speed; // m/s
    public float radius = 10f; // m
    public float delay = 0.25f; // sec
    public float spin = 24; // deg
    public AudioClip afterHitRadio;
    public long population = 1000000;

	[SerializeField] Light warningLight;



    [SerializeField] VirtualTerrain terrain;
    [SerializeField] GameObject particles;


    public event UnityAction OnCityHit {
        add { onCityHit.AddListener(value); }
        remove { onCityHit.RemoveListener(value); }
    } UnityEvent onCityHit = new UnityEvent();

    void Start() {
        OnCityHit += OnHitCity;
    }

    void OnCollision(Collision collision) {
        var dart = collision.rigidbody.GetComponent<LawnDart>();
        if (!dart) return;
    }


    void OnTriggerEnter(Collider other) {
        if (!other.attachedRigidbody) return;
        var dart = other.attachedRigidbody.GetComponent<LawnDart>();
        if (!dart) return;
        Destroy(dart.gameObject);

        HitCity();
    }

    void HitCity() { if (!wait) HittingCity(); }

    void HittingCity() {
        wait = true;
        Instantiate(particles, transform.position, Quaternion.identity);
        onCityHit.Invoke();
        wait = false;
    }

    void OnHitCity() 
    { 
		if (name == "Atlanta") {
			terrain.IsDoomsday = true;	
			warningLight.gameObject.SetActive (true);
		}
    }

}
