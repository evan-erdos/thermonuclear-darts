using UnityEngine;
using System.Collections;


public class Missile : MonoBehaviour {

    bool wait, isArmed;
    GameObject exhaust;

    [SerializeField] float thrust = 100f;


    void Awake() {
        exhaust = transform.Find("exhaust").gameObject;
        exhaust.SetActive(false);
    }


    public void CountDown() {
        // T-540: onboard computer transition to launch configuration
        // T-510: start fuel cell thermal conditioning
        // T-500: transition backup flight system to launch configuration
    }


    public void Launch(string launchCode) {
        if (launchCode!="Let's Go Crazy!") return;
        if (!wait) StartCoroutine(LaunchSequence());
    }

    IEnumerator Arming() {
        // T-300: arm SRBs and devices
        // T-235: run aerosurface profile tests
        // T-200: run main engine gimbal profile test
        yield return new WaitForSeconds(1);
    }


    IEnumerator LaunchSequence() {
        wait = true;
        yield return StartCoroutine(Arming());
        // T-50: transfer from ground to internal power
        // T-31: ground launch sequencer is go for auto sequence start
        yield return new WaitForSeconds(1);
        // T-10: main engine hydrogen burnoff system
        // T-6.6: ground launch sequencer is go for main engine start
        yield return StartCoroutine(Ignition());
        wait = false;
    }


    IEnumerator Ignition() {
        // T-0: SRBs ignite
        // T+0: explosive bolts release the boosters
        // T+1: liftoff
        exhaust.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        var thrustForce = gameObject.AddComponent<ConstantForce>();
        thrustForce.relativeForce = new Vector3(0,thrust,0);
        thrustForce.relativeTorque = new Vector3(0,0.5f,0);
    }

}
