using UnityEngine;
using System.Collections;

public class BlendShapeIterator : MonoBehaviour {

#pragma warning disable 0108
    SkinnedMeshRenderer renderer;
#pragma warning restore 0108

    int Count {get;set;}

    IEnumerator Start() {
        renderer = GetComponent<SkinnedMeshRenderer>();
        Count = renderer.sharedMesh.blendShapeCount;

        while (true) {
            yield return new WaitForSeconds(1);
            for (var i=0; i<Count; ++i) {
                var weight = renderer.GetBlendShapeWeight(i);
                while (weight<100) {
                    weight += 10;
                    renderer.SetBlendShapeWeight(i, weight);
                    yield return null;
                }
            }
            for (var i=Count; i>0; --i) {
                var weight = renderer.GetBlendShapeWeight(i);
                while (weight>0) {
                    weight -= 10;
                    renderer.SetBlendShapeWeight(i, weight);
                    yield return null;
                }
            }
        }
    }
}
