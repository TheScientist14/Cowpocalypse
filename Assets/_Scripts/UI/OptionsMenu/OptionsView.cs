using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsView : MonoBehaviour
{
    [SerializeField] Image m_MusicVolumeSlider;
    [SerializeField] Image m_SfxVolumeSlider;
    [SerializeField] float m_VolumeStep;
    [SerializeField] TextMeshProUGUI m_VersionText;

    private AudioManager m_Master;

    void Start()
    {
        m_VersionText.text = $"v{Application.version}";

        m_Master = AudioManager.instance;
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

    private void Refresh()
    {
        m_MusicVolumeSlider.fillAmount = m_Master.GetMusicVolume() / 100;
        m_SfxVolumeSlider.fillAmount = m_Master.GetSfxVolume() / 100;
    }
}