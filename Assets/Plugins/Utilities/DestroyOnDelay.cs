/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-12-28 */

using UnityEngine;
using System.Collections;

class DestroyOnDelay : MonoBehaviour {
    [SerializeField] float delay = 0.5f;
    void Start() { Invoke("Destroy", delay); }
    void Destroy() { Destroy(gameObject); }
}
