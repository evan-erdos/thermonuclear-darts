using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody))]
public class LawnDart : MonoBehaviour {

    bool waitParticles, waitBlowUp;
    Transform center;
    float aerodynamicFactor = 100f;

	static List<LawnDart> Darts = new List<LawnDart> ();

    int limit = 32;

    Vector3 oldPosition, contactPoint, contactNormal;
    Queue<GameObject> trailInstances = new Queue<GameObject> ();
    public bool isHeld = false;

    [SerializeField]  float drawInterval = 0.01f;
    [SerializeField] GameObject particles;
    [SerializeField] GameObject trailObject;
    [SerializeField] AudioClip onHitAnything;
    [SerializeField] AudioClip onHitStation;
    [SerializeField] AudioClip onHitPlatform;
    [SerializeField] AudioClip onHitTarget;


    // disables a silly warning
#pragma warning disable 0108
    Rigidbody rigidbody;
    AudioSource audio;
#pragma warning restore 0108

    public float ForwardVelocity
    {
        get
        {
            return Mathf.Max(0, transform.InverseTransformDirection(rigidbody.velocity).z);
        }
    }


    void Awake() {
		Darts.Add(this);
        center = transform.Find("center");
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = center.localPosition;
        audio = GetComponent<AudioSource>();
        StartCoroutine (generateTrail ());
        StartCoroutine (deleteTrail ());
    }



    IEnumerator Start()
    {
        StartCoroutine(LateDetonation(20));
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            waitParticles = false;
        }
    }

    IEnumerator LateDetonation(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (rigidbody.isKinematic || rigidbody.velocity.magnitude < 0) return;

        var deltaDirection = Vector3.Dot(
            transform.forward,
            rigidbody.velocity.normalized);
        deltaDirection *= deltaDirection;

        rigidbody.velocity = Vector3.Lerp(
            rigidbody.velocity,
            transform.forward * ForwardVelocity,
            deltaDirection * Time.fixedDeltaTime * ForwardVelocity);

       rigidbody.rotation = Quaternion.Slerp(
            rigidbody.rotation,
            Quaternion.LookRotation(rigidbody.velocity - Physics.gravity* Time.fixedDeltaTime,  Vector3.up),
            Time.fixedDeltaTime * aerodynamicFactor);
    }


    IEnumerator generateTrail() {
        while (true) {
            oldPosition = transform.position;
            yield return new WaitForSeconds (drawInterval);
            if (!isHeld && Mathf.Abs((oldPosition-transform.position).sqrMagnitude) > 0.1f * drawInterval) {
                GameObject trail = (GameObject) Instantiate (trailObject, this.transform.position, Quaternion.identity);
                trailInstances.Enqueue (trail);
            }
        }
    }

    IEnumerator deleteTrail() {
        yield return new WaitForSeconds(3);
        while (true) {
            if (trailInstances.Count<limit)
                yield return new WaitForSeconds (8*drawInterval);
            else yield return new WaitForSeconds (0.01f);
            if (trailInstances.Count > 0) Destroy(trailInstances.Dequeue());
            else if (rigidbody.isKinematic && !isHeld) { 
                yield return new WaitForSeconds(1f);
                if (rigidbody.isKinematic || rigidbody.useGravity && !isHeld) {
                    BlowUp(onHitAnything);
                    yield return new WaitForSeconds(0.5f);
                    Destroy(gameObject);
                }
            }
        }
    }

	public static void DestroyAll() {
		Darts.ToList ().ForEach (dart => Destroy (dart.gameObject)); }

    void OnDestroy() { DestroyTrail(); Darts.Remove(this); }

    public void DestroyTrail() {
        while (trailInstances.Count > 0) {
            GameObject i = trailInstances.Dequeue ();
            Destroy (i);
        }
    }
   
    void OnCollisionEnter(Collision collision)
    {
        rigidbody.isKinematic = true;
        if (collision.rigidbody)
            transform.parent = collision.rigidbody.transform;
        foreach (var contact in collision.contacts) {
            contactPoint = contact.point;
            contactNormal = contact.normal;
			if (gameObject.activeInHierarchy)
            if (collision.rigidbody && collision.rigidbody.tag=="Station") 
                BlowUp(onHitStation);
            else if (collision.rigidbody && collision.rigidbody.tag=="Terrain") 
                BlowUp(onHitPlatform);
            else BlowUp(onHitAnything);
            waitParticles = true;
            break;
        }
    }

    void BlowUp(AudioClip clip) { 
        if (!waitBlowUp && !waitParticles) 
            StartCoroutine(BlowingUp(clip)); }

    IEnumerator BlowingUp(AudioClip clip) {
        waitBlowUp = true;
        var instance = Instantiate(
            particles, 
            contactPoint, 
            Quaternion.LookRotation(contactPoint+contactNormal)) as GameObject;
        if (GameObject.Find("Radio").GetComponent<Radio>().isInterrupting())
            audio.PlayOneShot(clip, 0.3f);
        else audio.PlayOneShot(clip, 0.5f);
        yield return new WaitForSeconds(2);
        Destroy(instance);
        waitBlowUp = true;
    }
}