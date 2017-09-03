using UnityEngine;
using System.Collections;
using System;

public class tossDart : MonoBehaviour {

    // [Range(1.0f, 5.0f)] public  float v_factor;
    // [Range(1.0f, 5.0f)] public float a_factor;
    static tossDart instance;
    float v_factor = 3.5f;
    bool wait, waitPickup;
    bool triggerDown, triggerUp, triggerWasUp, triggerWasDown;

    // SteamVR_TrackedObject trackedObj;
    // SteamVR_Controller.Device device;
    OVRInput.Controller device;
    // Rigidbody device;

    [SerializeField] protected GameObject prefab;
    [SerializeField] protected GameObject rocket;
    [SerializeField] protected MissileSolver solver;
    public Transform dart;
    public Transform controller;
    public Transform ring_center;
    public Transform VR_camera;

    int maxID;
    float rotate_angle;
    Vector3 target_dir;
    Vector3[] controller_axis;

    void Awake() { instance = this; }

    void Start() {
        dart = (Instantiate(prefab) as GameObject).transform;
        //solver.dart = dart.GetComponent<LawnDart>();
        // trackedObj = GetComponent<SteamVR_TrackedObject>();
        // device = SteamVR_Controller?.Input((int)trackedObj.index);
        // device = controller.GetComponent<Rigidbody>();
        device = OVRInput.Controller.RTouch;
        controller_axis = new Vector3[3];
        ring_center.rotation = Quaternion.identity;
    }

    void Update() {
        // triggerUp = device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger);
        // triggerDown = device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger);
        triggerWasUp = triggerUp; triggerWasDown = triggerDown;
        triggerUp = Input.GetAxis("Grip")<0.3;
        triggerDown = Input.GetAxis("Grip")>0.6;

        // ring_center.rotation = Quaternion.identity;
        // Debug.DrawLine(ring_center.position, ring_center.position+ring_fwd, Color.red);

        if (triggerDown && !triggerWasDown) { make_dart(); make_pose(); }

        if (triggerUp && !triggerWasUp) {
            HapticPulse();
            if (!dart) return;
            dart.transform.parent = null;
            dart.GetComponent<LawnDart>().isHeld = false;
            dart.GetComponent<Rigidbody>().isKinematic = false;
            if (!wait) StartCoroutine(tossObject(dart.GetComponent<Rigidbody>()));
        }
    }

    public static void DoHapticPulse() => instance.HapticPulse();
    public void HapticPulse() => StartCoroutine(hapticPulse(-1));


    void make_dart() {
        dart = (Instantiate(prefab) as GameObject).transform;
        dart.transform.position = ring_center.position;
        dart.transform.parent = ring_center;
        var rigidbody = dart.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        dart.GetComponent<LawnDart>().isHeld = true;
    }

    void make_pose() {
        if (VR_camera.forward.z > 0) target_dir = new Vector3(0, 0, 1);
        else target_dir = new Vector3(0, 0, -1);
        // controller_axis[0] = Vector3.Normalize(controller.right); //ring_right
        controller_axis[0] = Quaternion.AngleAxis(-32, controller.right) * Vector3.Normalize(controller.forward); //ring_up
        controller_axis[1] = Quaternion.AngleAxis(58, controller.right) * Vector3.Normalize(controller.forward); //ring_fwd
        controller_axis[2] = -controller_axis[0]; //ring_down

        var maxDot = Mathf.NegativeInfinity;
        for (var i=0; i<3; i++) {
            var temp = Vector3.Dot(target_dir, controller_axis[i]);
            if (temp > maxDot) { maxDot = temp; maxID = i; }
        }

        switch (maxID) {
            case 0: rotate_angle = -58; break;
            case 1: rotate_angle = 58; break;
            case 2: rotate_angle = 108; break;
            default: break;
        }

        if (VR_camera.forward.z<=0)
            dart.transform.localRotation = Quaternion.Euler(180-rotate_angle, -180, 0);
        else dart.transform.localRotation = Quaternion.Euler(rotate_angle,0,0);
    }

    IEnumerator hapticPulse(float hapticLength=0) {
        if (hapticLength<0)
            hapticLength = OVRInput.GetLocalControllerVelocity(device).magnitude*0.05f;
        // if (hapticLength<0) hapticLength = device.velocity.magnitude*0.05f;
        for (var i=0f; i<hapticLength; i+=Time.deltaTime) {
            // device?.TriggerHapticPulse();
            yield return null;
        }
    }

    IEnumerator tossObject(Rigidbody rigidbody) {
        wait = true;
        dart.GetComponent<LawnDart>().isHeld = false;
        dart.GetComponent<Rigidbody>().useGravity = true;
        // var origin = trackedObj.origin ?? trackedObj.transform.parent;
        var origin = transform.parent; // origin = null;
        // angularVelocity * Mathf.Deg2Rad
        var velocity = OVRInput.GetLocalControllerVelocity(device);
        var angularVelocity = OVRInput.GetLocalControllerAngularVelocity(device);

        if (origin != null) {
            rigidbody.velocity = origin.TransformVector(velocity) * v_factor;
            rigidbody.angularVelocity = origin.TransformVector(angularVelocity)/4f;
        } else {
            rigidbody.velocity = velocity * v_factor;
            rigidbody.angularVelocity = angularVelocity/4f;
        } yield return new WaitForSeconds(1);
        wait = false;
    }
}
