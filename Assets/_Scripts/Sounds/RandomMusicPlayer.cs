using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMusicPlayer : IRandomSoundPlayer
{
    private bool m_IsPlaying = false;

    public override void PlayRandomSound()
    {
        if(m_IsPlaying)
            StopAllCoroutines();

        AudioClip music = GetRandomSound();
        AudioManager.instance.PlayMusic(music);

        StartCoroutine(PlayNextTrackAfterCompletion(music));
    }

    IEnumerator PlayNextTrackAfterCompletion(AudioClip iTrack)
    {
        m_IsPlaying = true;
        yield return new WaitForSeconds(iTrack.length);
        m_IsPlaying = false;

        PlayRandomSound();
    }
}
