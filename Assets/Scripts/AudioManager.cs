using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public enum Sound {
        SwordAttack,
        PistolAttack,
        MonsterAttack,
        MoveWalk
    }

    public SoundAudioClip[] soundAudioClips;
    public AudioClip test;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void PlaySoundOnce(GameObject emitter, Sound sound) {
        AudioSource source = emitter.GetComponent<AudioSource>();
        
        foreach(SoundAudioClip soundAudioClip in soundAudioClips) {
            if (soundAudioClip.sound == sound) {
                source.PlayOneShot(soundAudioClip.audioClip);
            }
        }
    }

    public void PlaySound(GameObject emitter) {
        AudioSource source = emitter.GetComponent<AudioSource>();

        if (!source.isPlaying) {
            source.loop = true;
            source.clip = test;
            source.Play();
        }
    }
    public void StopSound(GameObject emitter) {
        AudioSource source = emitter.GetComponent<AudioSource>();

        source.Stop();
    }



    [System.Serializable]
    public class SoundAudioClip {
        public Sound sound;
        public AudioClip audioClip;
    }
}
