using UnityEngine;
using System.Collections;
// using VRTK;

public class tutorial : MonoBehaviour {

	public GameObject tooltip;
	public Transform txt_front;
	public Transform txt_back;
    public GameObject diagram1;
    public GameObject diagram2;
    public GameObject diagram3;
    public static bool cityHit;

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;
    // VRTK_ControllerTooltips tip;
    bool triggerDown, triggerUp;
	bool hasRun;
	int stage = 0;

	void Start ()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
        // tip = (VRTK_ControllerTooltips)tooltip.GetComponent(typeof(VRTK_ControllerTooltips));
        // tip.UpdateText(0, "Hold down the trigger.");
		StartCoroutine(hapticPulse(5));
        diagram1.SetActive(true);
        diagram2.SetActive(false);
        diagram3.SetActive(false);

    }

    IEnumerator hapticPulse(float hapticLength)
    {
        for (float i = 0; i < hapticLength; i += Time.deltaTime)
        {
            device.TriggerHapticPulse();
            yield return null;
        }
    }

	IEnumerator hideTip()
    {
		yield return new WaitForSeconds(5f);
		// tip.ToggleTips(false);
    }

	// Update is called once per frame
	void Update ()
	{
        triggerUp = device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger);
        triggerDown = device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger);
		switch (stage)
		{
			case 0:
				if (triggerDown)
		        {
		        	// tip.UpdateText(0, "Release trigger \n with throwing motion.");
		        	txt_front.localScale = new Vector3(0.7f ,0.7f,0.7f);
                    diagram1.SetActive(false);
                    diagram2.SetActive(true);
                    stage++;
		        }
				break;
			case 1:
				if (triggerUp)
		        {
		        	// tip.UpdateText(0, "You got it! \n Now try a under-hand throw.");
                    diagram2.SetActive(false);
                    diagram3.SetActive(true);
                    stage++;
		        }
				break;
            case 2:
                if (triggerUp)
                {
                    // tip.UpdateText(0, "Good job! Now can you \n destroy a city for me?");
                    diagram3.SetActive(false);
                    stage++;
                }
                break;
            case 3:
                if (cityHit)
                {
                    StartCoroutine(hapticPulse(0.5f));
                    // tip.UpdateText(0, "Remember, whatever you do \n DO NOT throw at the nuke behind you.");
                    StartCoroutine(hideTip());
                    stage++;
                }
                break;
            default:
			  break;
		}
	}


}
