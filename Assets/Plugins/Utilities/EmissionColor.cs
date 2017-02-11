/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-12-28 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class EmissionColor : MonoBehaviour {
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
    [SerializeField] Color emission = new Color(1,1,1,1);
    Color color = Color.black;
    new Renderer renderer;

    public Color Emission {
        get { return renderer.material.GetColor("_EmissionColor"); }
        set { foreach (var material in renderer.materials)
            material.SetColor("_EmissionColor", value); } }

    void Awake() {
        renderer = GetComponent<Renderer>();
        Emission = emission;
        foreach (var material in renderer.materials)
            material.EnableKeyword("_EMISSION");
    }

    IEnumerator Start() {
        while (true) {
            Emission = Color.Lerp(Emission,color,1);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
