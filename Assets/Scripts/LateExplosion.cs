using UnityEngine;
using System.Collections;

public class LateExplosion : MonoBehaviour {
    [SerializeField] GameObject prefab;

    IEnumerator Start() {
        yield return new WaitForSeconds(4);
        Instantiate(prefab, transform.position, transform.rotation);
    }
}
