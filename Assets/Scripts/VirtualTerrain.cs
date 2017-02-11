using UnityEngine;
using ui=UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class VirtualTerrain : MonoBehaviour {

    bool wait, waitRockets;
    EmissiveLight emission;

	ui::Scrollbar scrollbar;


	[SerializeField] GameObject[] enables;
	[SerializeField] GameObject[] disables;
    [SerializeField] Scoreboard scoreboard;
    [SerializeField] AudioClip sound;
    [SerializeField] AudioClip citySound;
    [SerializeField] MissileSolver solver;
	[SerializeField] EndingScript endscript;
    [SerializeField] GameObject rocket;
	[SerializeField] GameObject doomsday;
	[SerializeField] GameObject silo;
    [SerializeField] Transform rockets;
    [SerializeField] Material vector;
    [SerializeField] Material grass;
    [SerializeField] long nextCityCasualties;
    private AudioClip nextCityHitSound;
    [SerializeField] Radio radio;
#pragma warning disable 0108
    AudioSource audio;
    SkinnedMeshRenderer renderer;
    MeshCollider collider;
#pragma warning restore 0108

	public bool IsDoomsday { get; set; }
    public int Count {get;private set;}
    public OrderList<City> Cities {get;private set;} 
    List<GameObject> darts = new List<GameObject>();
    RandList<Transform> rocketList;


    void Awake() {
		scrollbar = GetComponentInChildren<ui::Scrollbar> ();
        emission = transform.parent.GetComponentInChildren<EmissiveLight>();
        audio = GetComponent<AudioSource>();
        renderer = GetComponent<SkinnedMeshRenderer>();
        collider = GetComponent<MeshCollider>();
        Count = renderer.sharedMesh.blendShapeCount;
        Cities = new OrderList<City>(transform.parent.Find("Cities").GetComponentsInChildren<City>());
        foreach (var city in Cities) city.gameObject.SetActive(false);
        var list = new List<Transform>();
        foreach (Transform child in rockets) list.Add(child);
        rocketList = new RandList<Transform>(list);
    }


    IEnumerator Start() {
        if (scoreboard) scoreboard.Extend();
        yield return new WaitForSeconds(4);
        foreach (var city in Cities) city.OnCityHit += HitCity;
        Render();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody.GetComponent<LawnDart>())
            Add(collision.rigidbody.gameObject);
    }

    void OnTriggerEnter(Collider collider) {
        if (!collider.GetComponentInParent<Rigidbody>()) return;
        if (!collider.GetComponentInParent<Rigidbody>().GetComponent<LawnDart>()) return;

        //ShootRocket();
    }

    public void ShootRocket() {
        if (!waitRockets) StartCoroutine(ShootRockets()); }

    IEnumerator ShootRockets() {
        waitRockets = true;
		silo.GetComponent<Animator> ().SetBool ("IsOpened", true);
        var instance = Instantiate(rocket) as GameObject;
        var missile = instance.GetComponentInChildren<Missile>();
        var rotation = Quaternion.Lerp(missile.transform.rotation, rocketList.Next().rotation, 0.5f);
        instance.GetComponentInChildren<Missile>().transform.rotation = rotation;
        yield return new WaitForSeconds(4);
		silo.GetComponent<Animator> ().SetBool ("IsOpened", false);
		yield return new WaitForSeconds(4);
		waitRockets = false;
    }

    void Add(GameObject dart) { darts.Add(dart); }


    public void HitCity() {
        //var instance = Instantiate(rocketList.Next()) as GameObject;
        //var missile = instance.GetComponentInChildren<Missile>();
        //missile.transform.localRotation = Quaternion.LookRotation(
        //    earthTargets.Next().transform.forward, Vector3.up);
        //solver.missile = instance.GetComponentInChildren<Missile>();
        tutorial.cityHit = true;
        ShootRocket();
		scrollbar.size += 0.125f;
        if (radio.isInterrupting()) audio.PlayOneShot(citySound, 0.15f);
        else audio.PlayOneShot(citySound,0.5f);
        radio.playRadioResponse(nextCityHitSound);
        foreach (var city in Cities)
            city.gameObject.SetActive(false);     
        Render();
    }


	public void BeginEndOfDays() {
		//GameObject.FindGameObjectWithTag ("EarthTarget").GetComponent<Rigidbody>().isKinematic = false;
		Instantiate (doomsday);
		endscript.enabled = true;
		endscript.enabled = true;

		foreach (var elem in disables)
			elem.SetActive (false);
		foreach (var elem in enables)
			elem.SetActive (true);

	}


    public void Render() { Render(Cities.Next()); }


    void Render(City city) {
        if (!wait && city)
        {
            StartCoroutine(Rendering(city));
            nextCityHitSound = city.afterHitRadio;
            nextCityCasualties = city.population;
        }

    }


    IEnumerator Rendering(City city) {
        wait = true;
        //solver.city = city;
        //renderer.sharedMaterial = vector;
        if (scoreboard) scoreboard.Extend();
        foreach (var dart in darts) Destroy(dart);
        for (var i=0; i<Count; ++i) {
            var blend = renderer.GetBlendShapeWeight(i);
            float speed = 0f;
            while (!Mathf.Approximately(blend,0)) {
                yield return new WaitForFixedUpdate();
                blend = Mathf.SmoothDamp(blend, 0, ref speed, 0.1f);
                renderer.SetBlendShapeWeight(i, blend);
                GetComponentInChildren<SkinnedMeshRenderer>().SetBlendShapeWeight(i, blend); // HACK
            }
        }
        foreach (var other in Cities) {
            yield return StartCoroutine(Rendering(
                shape: other.shape,
                blend: 0,
                speed: 0,
                delay: 0.2f,
                height: 0,
                spin: 0,
                color: 0));
            other.gameObject.SetActive(false);
        }
		LawnDart.DestroyAll ();
        yield return new WaitForSeconds(0.1f);
        if (radio.isInterrupting())
            audio.PlayOneShot(sound, 0.15f);
        else audio.PlayOneShot(sound, 0.3f);
        yield return StartCoroutine(Rendering(
            shape: city.shape,
            blend: city.blend,
            speed: city.speed,
            delay: city.delay,
            height: city.height,
            spin: city.spin,
            color: city.color));
        yield return new WaitForSeconds(0.1f);
        //renderer.sharedMaterial = grass;
        city.gameObject.SetActive(true);
        wait = false;
    }


    IEnumerator RenderingHeight(
                    int shape,
                    float blend,
                    float speed,
                    float delay,
                    float height,
                    float spin=0f,
                    float color=0f) {
        while (!Mathf.Approximately(blend,height)) {
            yield return new WaitForFixedUpdate();
        
            blend = Mathf.SmoothDamp(blend, height, ref speed, delay);
            renderer.SetBlendShapeWeight(shape, blend);
            GetComponentInChildren<SkinnedMeshRenderer>().SetBlendShapeWeight(shape, blend); // HACK

        }
    }

    IEnumerator RenderingAngle(
                    int shape,
                    float blend,
                    float speed,
                    float delay,
                    float height,
                    float spin=0f,
                    float color=0f) {
        var rotation = Quaternion.Euler(0,spin,0);
        while (transform.localRotation!=rotation) {
            yield return new WaitForFixedUpdate();
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation, rotation,
                Time.fixedDeltaTime * 4f);
        }
    }

    IEnumerator RenderingColor(
                    int shape,
                    float blend,
                    float speed,
                    float delay,
                    float height,
                    float spin=0f,
                    float color=0f) {
        float h, s0, v0, s = 0.65f, v = 1f, t = 0.5f;
        while ((t -= Time.fixedDeltaTime) - float.Epsilon > 0) {
            Color.RGBToHSV(emission.EmissiveColor, out h, out s0, out v0);
            emission.EmissiveColor = Color.HSVToRGB(
                (h+Time.fixedDeltaTime)%1, s, v);
            yield return null;
        }
    }

    IEnumerator Rendering(
                    int shape,
                    float blend,
                    float speed,
                    float delay,
                    float height,
                    float spin=0f,
                    float color=0f) {
        if (height == 0) yield return new WaitForSeconds(1);
        yield return StartCoroutine(RenderingHeight(shape,blend,speed,delay,height,spin,color));
        if (emission)
            yield return StartCoroutine(RenderingColor(shape,blend,speed,delay,height,spin,color));
        //yield return StartCoroutine(RenderingAngle(shape,blend,speed,delay,height,spin,color));
        var mesh = new Mesh();
        renderer.BakeMesh(mesh);
        collider.sharedMesh = null;
        collider.sharedMesh = mesh;
        if (scoreboard) scoreboard.Retract();
    }

    public long getNextCityPopulation()
    {
        return nextCityCasualties;
    }
}