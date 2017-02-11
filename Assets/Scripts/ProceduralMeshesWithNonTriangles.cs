using UnityEngine;

public class ProceduralMeshesWithNonTriangles : MonoBehaviour {
	
	void Start () {
		var mesh = new Mesh();
		
		var lineCount = 16;
		var verts = new Vector3[lineCount*2];
		var indices = new int[lineCount*2];
		for (var i = 0; i < lineCount; ++i)
		{
			verts[i*2+0] = Vector3.zero;
			verts[i*2+1] = new Vector3(i*0.1f, 2.0f, 0.0f);
		}
		for (var i = 0; i < lineCount*2; ++i)
			indices[i] = i;
		
		mesh.vertices = verts;
		mesh.subMeshCount = 1;
		mesh.SetIndices (indices, MeshTopology.Lines, 0);
		
		GetComponent<MeshFilter>().mesh = mesh;
	}
}
