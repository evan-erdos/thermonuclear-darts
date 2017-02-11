using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Light))]
public class StaticEmissiveLight : MonoBehaviour {

    Light emissiveLight;
    [SerializeField] List<Renderer> renderers = new List<Renderer>();
    [SerializeField] float emission = 3f;


    void Start() {
        emissiveLight = GetComponent<Light>();
        renderers.ForEach(o => {
            o.sharedMaterial.EnableKeyword("_EMISSION");
            //var flags = o.sharedMaterial.globalIlluminationFlags;
            //flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            //flags &= MaterialGlobalIlluminationFlags.RealtimeEmissive;
            //o.sharedMaterial.globalIlluminationFlags = flags;
        });
        StartCoroutine(HueShiftRainbows());
    }

    public Color EmissiveColor {
        get { return emissiveLight.color; }
        set {
            value *= Mathf.LinearToGammaSpace(emission*4);
            emissiveLight.color = value;
            renderers.ForEach(o => {
                DynamicGI.SetEmissive(o,value);
                //DynamicGI.UpdateMaterials(o);
                });
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
