using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Vihecles Audio", menuName = "My Vihecles/Audio")]
public class VihecleAudio_SO : ScriptableObject
{
    #region Variables
    [Header("Vehicles Audio System")]
    public AudioClip engineStartClip;

    public AudioClip engineClipOn;

    public AudioClip engineClipOff;

    // public AudioSource gearShiftingSound;
    // public AudioClip[] gearShiftingClips;

    public AudioClip[] crashClips;

    public AudioClip windClip;

    public AudioClip brakeClip;

    public AudioClip asphaltSkidClip;
    public AudioClip grassSkidClip;
    public AudioClip sandSkidClip;


    [Header("Sound Fields")]
    [Range(0, 1)]
    public float minEngineSoundPitch = .75f;
    [Range(0, 2)]
    public float maxEngineSoundPitch = 1.75f;
    [Range(0, 1)]
    public float minEngineSoundVolume = .05f;
    [Range(0, 1)]
    public float maxEngineSoundVolume = .85f;
    [Range(0, 1)]
    public float maxGearShiftingSoundVolume = .3f;
    [Range(0, 1)]
    public float maxCrashSoundVolume = 1f;
    [Range(0, 1)]
    public float maxWindSoundVolume = .25f;
    [Range(0, 1)]
    public float maxBrakeSoundVolume = .35f;

    #endregion

    private void Awake()
    {
        
        //Debug.Log("Editor causes this Update");
       // engineStartClip = Resources.Load<AudioClip>("DefaultEngineOn.wav");
    }

    
}

