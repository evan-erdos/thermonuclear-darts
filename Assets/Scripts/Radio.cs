using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Radio : MonoBehaviour {

    private bool interruptPlaying;
    private float interruptDelay;
    private Queue<AudioClip> interruptQueue;
	[SerializeField] AudioClip[] clips;
	List<AudioClip> list;
	AudioSource audioSource;
    bool bgMusicShouldPlay = true;
	IEnumerator Start() {
        interruptQueue = new Queue<AudioClip>();
        interruptPlaying = false;
        interruptDelay = 5f;
		var n = 0;
		audioSource = GetComponent<AudioSource>();
		list = new List<AudioClip>(clips);
		while(bgMusicShouldPlay) {
			audioSource.clip = list[n];
			audioSource.Play();
			n = (n+1)%list.Count;
			yield return new WaitForSeconds(audioSource.clip.length);
		}
	}

    public void StopBGMusic()
    {
        audioSource.Stop();
    }
    public void StartBGMusic()
    {
        audioSource.Play();
        Start();
    }


    public void playRadioResponse(AudioClip radioResponse)
    {
        //bgMusicShouldPlay = false;

        StartCoroutine(interrupt(radioResponse));

    }


    IEnumerator interrupt(AudioClip radioResponse)
    {
        interruptQueue.Enqueue(radioResponse);
        while (interruptPlaying)
        {
            yield return new WaitForSeconds(1f);
        }
        interruptPlaying = true;
        yield return new WaitForSeconds(interruptDelay);
        AudioClip shouldPlay = interruptQueue.Dequeue();
        audioSource.Stop();
        audioSource.PlayOneShot(shouldPlay, 1.2f);
        yield return new WaitForSeconds(shouldPlay.length);
        audioSource.Play();
        interruptDelay = 4f;
        interruptPlaying = false;
    }
    //loop through bg music
    //stop looping when radio plays
        //wait 2 seconds 
        //stop playing bg music or radio music
        //play nextCitySoundNoise
        //begin next bg music song

    public bool isInterrupting()
    {
        return interruptPlaying;
    }


}
