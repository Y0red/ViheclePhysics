using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAudioSource : MonoBehaviour
{
    public static AudioSource NewAudioSource(AudioSource go, string audioName,
        float minDistance, float volume, AudioClip audioClip, bool loop, bool playNow,
        bool destroyAfterFinished)
    {
        // GameObject audioSource = new GameObject(audioName);
        AudioSource audioSource = go.GetComponent<AudioSource>();
        //audioSource.transform.position = go.transform.position;
       // audioSource.transform.rotation = go.transform.rotation;
       // audioSource.transform.parent = go.transform;
        //audioSource.AddComponent<AudioSource>();
        audioSource.GetComponent<AudioSource>().minDistance = minDistance;
        audioSource.GetComponent<AudioSource>().volume = volume;
        audioSource.GetComponent<AudioSource>().clip = audioClip;
        audioSource.GetComponent<AudioSource>().loop = loop;
        audioSource.GetComponent<AudioSource>().spatialBlend = 1f;

        if (playNow)
            audioSource.GetComponent<AudioSource>().Play();

        if (destroyAfterFinished)
        {
            //if (audioClip)
              //  Destroy(audioSource, audioClip.length);
            //else
               // Destroy(audioSource);
        }

        audioSource.transform.SetParent(go.transform);

        return audioSource.GetComponent<AudioSource>();
    }
}
