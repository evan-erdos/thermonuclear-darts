using UnityEngine;
using System.Collections;


public class Missile : MonoBehaviour {

    bool wait, waitRocket, isArmed, once;
    Transform exhaust;

    [SerializeField] float thrust = 100f;
    [SerializeField] GameObject explosion;
	[SerializeField] GameObject debris;

    void Awake() {
        exhaust = transform.Find("exhaust");
        if (exhaust) exhaust.gameObject.SetActive(false);
    }

    void Start() { StartCoroutine(Starting()); }

    IEnumerator Starting() {
        yield return new WaitForSeconds(20);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision) {
        if (once) return;
        once = true;
        Vector3 point = Vector3.zero, normal = Vector3.zero;
        foreach (var contact in collision.contacts) {
            point += contact.point;
            normal += contact.normal;
        }
        point /= collision.contacts.Length;
        normal /= collision.contacts.Length;
        //instance.transform.parent = collision.rigidbody.transform;
        if (!waitRocket) StartCoroutine(Detonating(point, normal, collision));


    }

    IEnumerator Detonating(Vector3 point, Vector3 normal, Collision collision) {
        waitRocket = true;
		foreach (var mesh in GetComponentsInChildren<MeshRenderer>()) Destroy(mesh.gameObject);
		var instance = Instantiate(debris, transform.position, transform.rotation) as GameObject;
		yield return new WaitForSeconds (0.1f);
		foreach (var rigidbody in instance.GetComponentsInChildren<Rigidbody>()) {
            rigidbody.isKinematic = false;
            //rigidbody.AddExplosionForce(1f, transform.position, 10f, 1f);
        }
        Destroy(GetComponentInChildren<ConstantForce>());
        exhaust.transform.parent = null;
		Instantiate(explosion, point, Quaternion.LookRotation(point+normal));
        yield return new WaitForSeconds(0.5f);
        Destroy(exhaust.gameObject);
        if (collision.gameObject.CompareTag("EarthTarget"))
        {
            long kills = GameObject.FindGameObjectWithTag("Terrain").GetComponent<VirtualTerrain>().getNextCityPopulation();
            GameObject.FindGameObjectWithTag("ScoreBoard").GetComponent<PopulationCounter>().killPeopleInstant(kills);
        }

        waitRocket = false;
    }

    public void CountDown() {
        // T-540: onboard computer transition to launch configuration
        // T-510: start fuel cell thermal conditioning
        // T-500: transition backup flight system to launch configuration
    }


    public void Launch(string launchCode) {
        if (launchCode!="Let's Go Crazy!") return;
        if (!wait) StartCoroutine(LaunchSequence());
    }

    IEnumerator Arming() {
        // T-300: arm SRBs and devices
        // T-235: run aerosurface profile tests
        // T-200: run main engine gimbal profile test
        yield return new WaitForSeconds(0.1f);
    }


    IEnumerator LaunchSequence() {
        wait = true;
        yield return StartCoroutine(Arming());
        // T-50: transfer from ground to internal power
        // T-31: ground launch sequencer is go for auto sequence start
        yield return new WaitForSeconds(0.1f);
        // T-10: main engine hydrogen burnoff system
        // T-6.6: ground launch sequencer is go for main engine start
        yield return StartCoroutine(Ignition());
        wait = false;
    }


    IEnumerator Ignition() {
        // T-0: SRBs ignite
        // T+0: explosive bolts release the boosters
        // T+1: liftoff
        exhaust.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        foreach (var audio in GetComponents<AudioSource>()) audio.Play();
        GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,thrust), ForceMode.VelocityChange);
        var thrustForce = gameObject.AddComponent<ConstantForce>();
        thrustForce.relativeForce = new Vector3(0,0,thrust);
        thrustForce.relativeTorque = new Vector3(0,0,0.5f);
    }

}
