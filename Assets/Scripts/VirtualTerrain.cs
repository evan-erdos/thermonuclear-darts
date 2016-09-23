using UnityEngine;
using System.Collections;

public class VirtualTerrain : MonoBehaviour {

    bool wait, isRendering;
    float speed0, speed1, blend0, blend1;
    [SerializeField] float terrain0 = 60f;
    [SerializeField] float terrain1 = 80f;
    [SerializeField] float delay = 0.5f;
    [SerializeField] float spin = 12f;
    SkinnedMeshRenderer skinnedRenderer;
    EmissiveLight emissiveLight;

    public bool IsRendering { get { return isRendering; } }


    void Awake() {
        skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        emissiveLight = transform.parent.GetComponentInChildren<EmissiveLight>();
    }


    IEnumerator Start() {
        StartCoroutine(HueShift());
        yield return new WaitForSeconds(2);
        Render();
        yield return new WaitForSeconds(2);
//        while (true) {
//            yield return new WaitForFixedUpdate();
//            transform.Rotate(0,spin*Time.fixedDeltaTime,0);
//        }
    }


    IEnumerator HueShift() {
        float s = 0.65f, v = 1f;
        while (true) {
            float h, s0, v0;
            Color.RGBToHSV(emissiveLight.EmissiveColor, out h, out s0, out v0);
            emissiveLight.EmissiveColor = Color.HSVToRGB((h+0.001f)%1, s, v);
            yield return null;
        }
    }



    public void Render() { if (!wait) StartCoroutine(Rendering()); }

    IEnumerator Rendering() {
        wait = true;
        isRendering = true;
        while (Mathf.Abs(blend0-terrain0) < float.Epsilon) {
            yield return new WaitForFixedUpdate();
            blend0 = Mathf.SmoothDamp(
                blend0, IsRendering?terrain0:0, ref speed0, delay);
            skinnedRenderer.SetBlendShapeWeight(0, blend0);
        }

        while (Mathf.Abs(blend1-terrain1) < float.Epsilon) {
            yield return new WaitForFixedUpdate();
            blend1 = Mathf.SmoothDamp(
                blend1, IsRendering?terrain1:0, ref speed1, delay);
            skinnedRenderer.SetBlendShapeWeight(1, blend1);
        }

        while (true) {
            yield return new WaitForFixedUpdate();

            blend0 = Mathf.SmoothDamp(
                blend0,
                terrain0*Mathf.Sin(0.5f+Time.time*0.5f),
                ref speed0, delay);
            skinnedRenderer.SetBlendShapeWeight(0, blend0);

            blend1 = Mathf.SmoothDamp(
                blend1, terrain1*Mathf.Max(1.5f,2*Mathf.Sin(Time.time)),
                ref speed1, delay);
            skinnedRenderer.SetBlendShapeWeight(1, blend1);
        }
    }
}
