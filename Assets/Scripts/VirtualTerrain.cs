using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class VirtualTerrain : MonoBehaviour {

    bool wait;
    EmissiveLight emission;
    [SerializeField] Scoreboard scoreboard;
#pragma warning disable 0108
    SkinnedMeshRenderer renderer;
    MeshCollider collider;
#pragma warning restore 0108

    public int Count {get;private set;}
    public RandList<City> Cities {get;private set;}


    void Awake() {
        emission = transform.parent.GetComponentInChildren<EmissiveLight>();
        renderer = GetComponent<SkinnedMeshRenderer>();
        collider = GetComponent<MeshCollider>();
        Count = renderer.sharedMesh.blendShapeCount;
        Cities = new RandList<City>(GetComponentsInChildren<City>());
        foreach (var city in Cities) city.gameObject.SetActive(false);
    }


    IEnumerator Start() {
        foreach (var city in Cities) city.OnCityHit += HitCity;
        if (scoreboard) scoreboard.Extend();
        yield return new WaitForSeconds(4);
        Render();
    }


    public void HitCity() {
        foreach (var city in Cities)
            city.gameObject.SetActive(false);
        Render();
    }

    public void Render() { Render(Cities.Next()); }


    void Render(City city) {
        if (!wait) StartCoroutine(Rendering(city)); }


    IEnumerator Rendering(City city) {
        wait = true;
        if (scoreboard) scoreboard.Extend();
        foreach (var other in Cities) {
            yield return StartCoroutine(Rendering(
                shape: other.shape,
                blend: other.blend,
                speed: 0,
                delay: 0.2f,
                height: 0,
                spin: 0,
                color: 0));
            other.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1);
        yield return StartCoroutine(Rendering(
            shape: city.shape,
            blend: city.blend,
            speed: city.speed,
            delay: city.delay,
            height: city.height,
            spin: city.spin,
            color: city.color));
        city.gameObject.SetActive(true);
        wait = false;
    }

    IEnumerator Rendering(
                    int shape,
                    float blend,
                    float speed,
                    float delay,
                    float height,
                    float spin=0f,
                    float color=0f) {
        yield return new WaitForSeconds(0.1f);
        while (!Mathf.Approximately(blend,height)) {
            yield return new WaitForFixedUpdate();
            blend = Mathf.SmoothDamp(blend, height, ref speed, delay);
            renderer.SetBlendShapeWeight(shape, blend);
        }

        /*var rotation = Quaternion.Euler(0,spin,0);
        while (transform.localRotation!=rotation) {
            yield return new WaitForFixedUpdate();
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation, rotation,
                Time.fixedDeltaTime * 4f);
        }*/

        float h, s0, v0, s = 0.65f, v = 1f, t = 0.5f;
        while ((t -= Time.fixedDeltaTime) - float.Epsilon > 0) {
            Color.RGBToHSV(emission.EmissiveColor, out h, out s0, out v0);
            emission.EmissiveColor = Color.HSVToRGB(
                (h+Time.fixedDeltaTime)%1, s, v);
            yield return null;
        }

        var mesh = new Mesh();
        renderer.BakeMesh(mesh);
        collider.sharedMesh = null;
        collider.sharedMesh = mesh;
        if (scoreboard) scoreboard.Retract();
    }
}
