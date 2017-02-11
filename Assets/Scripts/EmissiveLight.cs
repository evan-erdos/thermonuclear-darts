using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Light))]
public class EmissiveLight : MonoBehaviour {

    Light emissiveLight;
    [SerializeField] List<Renderer> renderers = new List<Renderer>();
    [SerializeField] float emission = 3f;


    void Awake() { emissiveLight = GetComponent<Light>(); }

    void Start() { StartCoroutine(HueShiftRainbows()); }

    public Color EmissiveColor {
        get { return emissiveLight.color; }
        set {
            value *= Mathf.LinearToGammaSpace(emission*4);
            emissiveLight.color = value;
            //renderers.ForEach(o => DynamicGI.SetEmissive(o,value));
            foreach (var renderer in renderers)
                foreach (var material in renderer.materials)
                    material.SetColor("_EmissionColor", value);
        }
    }


    IEnumerator HueShiftRainbows() {
        float s = 0.65f, v = 1f;
        while (true) {
            float h, s0, v0;
            Color.RGBToHSV(EmissiveColor, out h, out s0, out v0);
            EmissiveColor = Color.HSVToRGB((h+0.001f)%1, s, v);
            yield return null;
        }
    }
}
