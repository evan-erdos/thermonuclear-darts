/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-12-28 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(Renderer))]
public class EmissiveLight : MonoBehaviour {
    void Awake() { GetComponent<Renderer>().material.SetColor(
        "_EmissionColor", GetComponent<Light>().color); }
}
