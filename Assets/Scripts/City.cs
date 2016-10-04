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

    [SerializeField] GameObject rocket;
    [SerializeField] GameObject particles;


    public event UnityAction OnCityHit {
        add {
            if (onCityHit==null) onCityHit = new UnityEvent();
            onCityHit.AddListener(value); }
        remove {
            if (onCityHit==null) onCityHit = new UnityEvent();
            onCityHit.RemoveListener(value);
        }
    } UnityEvent onCityHit;

    void Awake() {
        onCityHit = new UnityEvent();
    }

    void Start() {
        OnCityHit += OnHitCity;
    }

    void OnCollision(Collision collision) {
        var dart = collision.rigidbody.GetComponent<LawnDart>();
        if (!dart) return;

    }


    void OnTriggerEnter(Collider other) {
        var dart = other.attachedRigidbody.GetComponent<LawnDart>();
        if (!dart) return;
        Destroy(dart.gameObject);
        HitCity();
    }

    void HitCity() { if (!wait) StartCoroutine(HittingCity()); }

    IEnumerator HittingCity() {
        wait = true;
        Instantiate(particles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1);
        onCityHit.Invoke();
        wait = false;
    }

    void OnHitCity() { Instantiate(rocket); }

}
