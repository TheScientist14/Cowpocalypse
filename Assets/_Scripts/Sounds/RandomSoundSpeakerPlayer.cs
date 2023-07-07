using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundSpeakerPlayer : IRandomSoundPlayer
{
    [SerializeField] AudioSource m_Speaker;

    public override void PlayRandomSound()
    {
        m_Speaker.Stop();
        m_Speaker.clip = GetRandomSound();
        m_Speaker.Play();
    }
}
