using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SoundOnClick : MonoBehaviour
{
    [SerializeField] List<AudioClip> m_ClickSounds = new List<AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        if(m_ClickSounds.Count == 0)
        {
            Debug.LogError("No click sound to play on " + gameObject.name);
            Destroy(this);
            return;
        }

        Button button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        AudioManager.instance.PlaySoundEffect(m_ClickSounds[Random.Range(0, m_ClickSounds.Count - 1)]);
    }
}
