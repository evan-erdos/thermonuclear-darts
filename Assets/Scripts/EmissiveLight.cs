using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class EmissiveLight : MonoBehaviour {

    Light emissiveLight;
    [SerializeField] Renderer[] renderers;
    [SerializeField] float emission = 3f;


    void Awake() { emissiveLight = GetComponent<Light>(); }

    public Color EmissiveColor {
        get { return emissiveLight.color; }
        set {
            value *= Mathf.LinearToGammaSpace(emission*4);
            emissiveLight.color = value;
            foreach (var renderer in renderers)
                foreach (var material in renderer.materials)
                    material.SetColor("_EmissionColor", value);
        }
    }
}
