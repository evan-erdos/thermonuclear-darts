using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class tossDart : MonoBehaviour {

    bool triggerDown, triggerUp;

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

    public Transform dart;

    void Start() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }
	
	void Update () {

        triggerUp = device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger);
        triggerDown = device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger);

        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You are holding 'Touch' on the Trigger");
        }

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You activated TouchDown on the Trigger");
        }

        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You activated TouchUp on the Trigger");
        }

        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You are holding 'Press' on the Trigger");
        }

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You activated PressDown on the Trigger");
        }

        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You activated PressUp on the Trigger");
        }


        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("You activated PressUp on the Touchpad");
            dart.transform.position = transform.position;
            dart.GetComponent<Rigidbody>().velocity = Vector3.zero;
            dart.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            dart.GetComponent<Rigidbody>().isKinematic = true;
            dart.transform.rotation = new Quaternion();
        }
    }

    // Pick up the dart when colliding
    void OnTriggerStay (Collider col)
    {
        //Debug.Log("You have collided with " + col.name + " and activated OnTriggerStay");
        if (triggerDown)
        {
            //Debug.Log("You have collided with " + col.name + " while holding down Touch");
            col.attachedRigidbody.isKinematic = true;
            //col.attachedRigidbody.GetComponent<ConstantForce>().enabled = false;
            col.attachedRigidbody.transform.parent = transform;
        }
        if (triggerUp)
        {
            //Debug.Log("You have released Touch while colliding with " + col.name);
            col.attachedRigidbody.transform.parent = null;
            col.attachedRigidbody.isKinematic = false;
            tossObject(col.attachedRigidbody);
        }
    }

    void tossObject(Rigidbody rigidBody)
    {
        
        //rigidBody.GetComponent<ConstantForce>().enabled = true;
        Transform origin = trackedObj.origin ?? trackedObj.transform.parent;
        if (origin != null)
        {
            rigidBody.velocity = origin.TransformVector(device.velocity);
            rigidBody.angularVelocity = origin.TransformVector(device.angularVelocity);
        } else
        {
            //var v = device.velocity;
            //v += rigidbody.transform.forward * 100f;
            rigidBody.velocity = device.velocity * 2;
            rigidBody.angularVelocity = device.angularVelocity / 4;
        }
        
    }
}
