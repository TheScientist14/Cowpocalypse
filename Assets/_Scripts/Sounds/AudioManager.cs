using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioSource m_MusicAudioSource;
    [SerializeField] AudioSource m_SoundEffectsAudioSource;

    [SerializeField] AudioMixer m_MasterMixer;
    private const string s_MusicVolumeKey = "MusicVolume";
    private const string s_SfxVolumeKey = "SoundEffectsVolume";

    // caching volumes as percentages
    private float m_MusicVolume = 50;
    private float m_SfxVolume = 50;

    public float m_PercentToDbPow = 1f;

    public UnityEvent OnMixerInit;

    protected override void Awake()
    {
        base.Awake();

        if(OnMixerInit == null)
            OnMixerInit = new UnityEvent();
    }

    protected void Start()
    {
        m_MusicVolume = PlayerPrefs.GetFloat(s_MusicVolumeKey, 50);
        m_SfxVolume = PlayerPrefs.GetFloat(s_SfxVolumeKey, 50);

        StartCoroutine(DelayMixerUpdateOnStart());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        PlayerPrefs.SetFloat(s_MusicVolumeKey, m_MusicVolume);
        PlayerPrefs.SetFloat(s_SfxVolumeKey, m_SfxVolume);
    }

    IEnumerator DelayMixerUpdateOnStart()
    {
        yield return new WaitForEndOfFrame();
        UpdateMixer();
        OnMixerInit.Invoke();
    }

    private void UpdateMixer()
    {
        m_MasterMixer.SetFloat(s_MusicVolumeKey, PercentToDb(m_MusicVolume));
        m_MasterMixer.SetFloat(s_SfxVolumeKey, PercentToDb(m_SfxVolume));
    }

    private float PercentToDb(float iVolumePercent)
    {
        return Mathf.Lerp(-80, 0, Mathf.Pow(iVolumePercent / 100, m_PercentToDbPow));
    }

    public void PlaySoundEffect(AudioClip iClip)
    {
        if(m_SoundEffectsAudioSource.isPlaying)
            m_SoundEffectsAudioSource.Stop();

        m_SoundEffectsAudioSource.clip = iClip;
        m_SoundEffectsAudioSource.Play();
    }

    public void PlayMusic(AudioClip iClip)
    {
        if(m_MusicAudioSource.isPlaying)
            m_MusicAudioSource.Stop();

        m_MusicAudioSource.clip = iClip;
        m_MusicAudioSource.Play();
    }

    public float GetMusicVolume()
    {
        return m_MusicVolume;
    }

    public void SetMusicVolume(float iVolume)
    {
        m_MusicVolume = iVolume;
        UpdateMixer();
    }

    public float GetSfxVolume()
    {
        return m_SfxVolume;
    }

    public void SetSfxVolume(float iVolume)
    {
        m_SfxVolume = iVolume;
        UpdateMixer();
    }
}
