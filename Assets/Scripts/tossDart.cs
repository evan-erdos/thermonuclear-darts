using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class tossDart : MonoBehaviour
{

    // [Range(1.0f, 5.0f)]
    // public  float v_factor;
    // [Range(1.0f, 5.0f)]
    // public  float a_factor;
	static tossDart instance;
    float v_factor = 3.5F;
    bool wait, waitPickup;
    bool triggerDown, triggerUp;

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

    [SerializeField]
    GameObject prefab;
    [SerializeField]
    GameObject rocket;
    [SerializeField]
    MissileSolver solver;
    public Transform dart;
    public Transform controller;
    public Transform ring_center;
    public Transform VR_camera;

    int maxID;
    float rotate_angle;
    Vector3 target_dir;
    Vector3[] controller_axis;

	void Awake() { instance = this; }

    void Start()
    {
        dart = (Instantiate(prefab) as GameObject).transform;
        //solver.dart = dart.GetComponent<LawnDart>();
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
        controller_axis = new Vector3[3];
        ring_center.rotation = Quaternion.identity;
    }

    void Update()
    {
        triggerUp = device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger);
        triggerDown = device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger);
        // ring_center.rotation = Quaternion.identity;
        // Debug.DrawLine(ring_center.position, ring_center.position+ring_fwd, Color.red);

        if (triggerDown)
        {
            make_dart();
            make_pose();
            //solver.dart = dart.GetComponent<LawnDart>();
        }

        if (triggerUp)
        {
			HapticPulse ();
			if (!dart) return;
            dart.transform.parent = null;
            dart.GetComponent<LawnDart>().isHeld = false;
            dart.GetComponent<Rigidbody>().isKinematic = false;
            StartCoroutine(tossObject(dart.GetComponent<Rigidbody>()));
        }
    }

	public static void DoHapticPulse() {
		instance.HapticPulse ();
	}

	public void HapticPulse() {
		StartCoroutine(hapticPulse(device.velocity.magnitude * 0.05f));
	}


    void make_dart()
    {
        dart = (Instantiate(prefab) as GameObject).transform;
        dart.transform.position = ring_center.position;
        dart.transform.parent = ring_center;
        dart.GetComponent<Rigidbody>().velocity = Vector3.zero;
        dart.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        dart.GetComponent<Rigidbody>().isKinematic = true;
        dart.GetComponent<Rigidbody>().useGravity = false;
        dart.GetComponent<LawnDart>().isHeld = true;
    }

    void make_pose()
    {
        if (VR_camera.forward.z > 0)
        {
            target_dir = new Vector3(0, 0, 1);
        }
        else
        {
            target_dir = new Vector3(0, 0, -1);
        }
        // controller_axis[0] = Vector3.Normalize(controller.right); //ring_right
        controller_axis[0] = Quaternion.AngleAxis(-32, controller.right) * Vector3.Normalize(controller.forward); //ring_up
        controller_axis[1] = Quaternion.AngleAxis(58, controller.right) * Vector3.Normalize(controller.forward); //ring_fwd
        controller_axis[2] = -controller_axis[0]; //ring_down
        // controller_axis[4] = -controller_axis[0]; //ring_left

        float maxDot = Mathf.NegativeInfinity;
        for (int i = 0; i < 3; i++)
        {
            float temp = Vector3.Dot(target_dir, controller_axis[i]);
            if (temp > maxDot)
            {
                maxDot = temp;
                maxID = i;
            }
        }

        switch (maxID)
        {
            case 0: //ring_up  
                rotate_angle = -58f;
                break;
            case 1: //ring_fwd
                rotate_angle = 58f;
                break;
            case 2: //ring_down
                rotate_angle = 108f;
                break;
            default:
                break;
        }

        if (VR_camera.forward.z > 0)
        {
            dart.transform.localRotation = Quaternion.Euler(rotate_angle, 0, 0);
        }
        else
        {
            dart.transform.localRotation = Quaternion.Euler(180 - rotate_angle, -180, 0);
        }
    }

    IEnumerator hapticPulse(float hapticLength)
    {
        for (float i = 0; i < hapticLength; i += Time.deltaTime)
        {
            device.TriggerHapticPulse();
            yield return null;
        }
    }

    IEnumerator tossObject(Rigidbody rigidBody)
    {
        dart.GetComponent<LawnDart>().isHeld = false;
        dart.GetComponent<Rigidbody>().useGravity = true;

        Transform origin = trackedObj.origin ?? trackedObj.transform.parent;
        if (origin != null)
        {
            rigidBody.velocity = origin.TransformVector(device.velocity) * v_factor;
            rigidBody.angularVelocity = origin.TransformVector(device.angularVelocity) / 4.0F;
        }
        else
        {
            rigidBody.velocity = device.velocity * v_factor;
            rigidBody.angularVelocity = device.angularVelocity / 4.0F;
        }
        yield return new WaitForSeconds(1);
    }
}