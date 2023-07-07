using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsView : MonoBehaviour
{
    [SerializeField] Slider m_MusicVolumeSlider;
    [SerializeField] Slider m_SfxVolumeSlider;
    [SerializeField] float m_VolumeStep;
    [SerializeField] TextMeshProUGUI m_VersionText;

    private AudioManager m_Master;

    void Start()
    {
        m_VersionText.text = $"v{Application.version}";

        m_Master = AudioManager.instance;
        Refresh();
    }

    void OnEnable()
    {
        if(m_Master != null)
            Refresh();
    }

    public void MusicUp()
    {
        m_Master.SetMusicVolume(Mathf.Min(100f, m_Master.GetMusicVolume() + m_VolumeStep));
        Refresh();
    }

    public void MusicDown()
    {
        m_Master.SetMusicVolume(Mathf.Max(0f, m_Master.GetMusicVolume() - m_VolumeStep));
        Refresh();
    }

    public void SoundsUp()
    {
        m_Master.SetSfxVolume(Mathf.Min(100f, m_Master.GetSfxVolume() + m_VolumeStep));
        Refresh();
    }

    public void SoundsDown()
    {
        m_Master.SetSfxVolume(Mathf.Max(0f, m_Master.GetSfxVolume() - m_VolumeStep));
        Refresh();
    }

    public void UpdateVolumesFromSliders()
    {
        m_Master.SetMusicVolume(m_MusicVolumeSlider.value);
        m_Master.SetSfxVolume(m_SfxVolumeSlider.value);
    }

    private void Refresh()
    {
        m_MusicVolumeSlider.value = m_Master.GetMusicVolume();
        m_SfxVolumeSlider.value = m_Master.GetSfxVolume();
    }
}