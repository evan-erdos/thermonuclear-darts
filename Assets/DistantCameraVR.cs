using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantCameraVR : MonoBehaviour {
	void Update () => transform.position = Vector3.zero;
	void FixedUpdate () => transform.position = Vector3.zero;
	void LateUpdate () => transform.position = Vector3.zero;
}
