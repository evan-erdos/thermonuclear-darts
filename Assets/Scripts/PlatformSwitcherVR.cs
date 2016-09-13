using UnityEngine;
using System.Collections;

class PlatformSwitcherVR : MonoBehaviour {

    bool isVREnabled =
#if !UNITY_EDITOR && UNITY_IOS || UNITY_ANDROID || VR_ENABLED
        true;
#elif UNITY_EDITOR
        false;
#endif

    void Awake() {
        GetComponent<GvrViewer>().VRModeEnabled = isVREnabled;
        Camera.main.GetComponent<Look>().enabled = !isVREnabled;
    }

}
