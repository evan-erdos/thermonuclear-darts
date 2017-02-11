using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    IEnumerator Start() {
        var light = GetComponent<Light>();
        if (light) {
            light.intensity = 0;
            while (light.intensity<7f) {
                yield return null;
                light.intensity += Time.deltaTime * 6;
            }

            while (light.intensity>float.Epsilon) {
                yield return null;
                light.intensity -= Time.deltaTime / 2f;
            }
        }

        yield return new WaitForSeconds(2);
        Destroy(gameObject);

    }

    void Update() {
        var r = Mathf.Sin((Time.time) * (2 * Mathf.PI)) * 0.5f + 0.25f;
        var g = Mathf.Sin((Time.time + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        var b = Mathf.Sin((Time.time + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        var correction = 1f / (r + g + b);
        r *= correction;
        g *= correction;
        b *= correction;
        if (GetComponent<Renderer>())
            GetComponent<Renderer>().material.SetVector("_ChannelFactor", new Vector4(r,g,b,0));
        if (GetComponent<ParticleSystemRenderer>())
            GetComponent<ParticleSystemRenderer>().sharedMaterial.SetVector("_ChannelFactor", new Vector4(r,g,b,0));
    }


}
