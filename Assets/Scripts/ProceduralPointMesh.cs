using UnityEngine;

public class ProceduralPointMesh : MonoBehaviour {
	
	void Start () {
		var mesh = new Mesh();
		
		var count = 100;
		var verts = new Vector3[count];
		var indices = new int[count];
		for (var i = 0; i < count; ++i)
		{
			var phi = i * Mathf.PI / count;
			verts[i] = new Vector3(Mathf.Cos(phi), Mathf.Sin(phi), 0.0f);
		}
		for (var i = 0; i < count; ++i)
			indices[i] = i;
		
		mesh.vertices = verts;
		mesh.subMeshCount = 1;
		mesh.SetIndices (indices, MeshTopology.Points, 0);
		
		GetComponent<MeshFilter>().mesh = mesh;
	}
}
