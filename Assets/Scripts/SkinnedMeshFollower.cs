using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SkinnedMeshFollower : MonoBehaviour {

    [SerializeField] SkinnedMeshRenderer other;
	[SerializeField] float factor = 1f;
    new SkinnedMeshRenderer renderer;
    new MeshCollider collider;

    void Awake() {
        renderer = GetComponent<SkinnedMeshRenderer>();
        collider = GetComponent<MeshCollider>();
	}

	// IEnumerator Start() {
	// 	while (true) {
	// 		yield return new WaitForSeconds(1);
	// 		var mesh = new Mesh();
	//         renderer.BakeMesh(mesh);
 //            if (!collider) break;
	//         collider.sharedMesh = null;
	//         collider.sharedMesh = mesh;
	// 	}
	// }

	void FixedUpdate() {
		for (var i=0; i<renderer.sharedMesh.blendShapeCount; i++)
			if (i<other.sharedMesh.blendShapeCount)
				renderer.SetBlendShapeWeight(i,other.GetBlendShapeWeight(i)*factor);
	}

}
