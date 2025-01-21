using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundOnAwake : MonoBehaviour
{

    public List<AudioClip> audioClips;

    private AudioSource thisAudioSource;


    void Awake()
    {

        thisAudioSource = GetComponent<AudioSource>();
        
    }

    void Start()
    {

        AudioClip audioClip = audioClips[Random.Range(0,audioClips.Count)];
        thisAudioSource.PlayOneShot(audioClip);
        
    }
    
}
