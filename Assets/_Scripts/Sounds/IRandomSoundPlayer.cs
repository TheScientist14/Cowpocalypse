using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IRandomSoundPlayer : MonoBehaviour
{
    [SerializeField] SoundList m_Sounds;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if(m_Sounds.Sounds == null || m_Sounds.Sounds.Count == 0)
        {
            Debug.LogError("No sound to play on " + gameObject.name);
            Destroy(this);
            return;
        }
    }

    protected AudioClip GetRandomSound()
    {
        return m_Sounds.Sounds[Random.Range(0, m_Sounds.Sounds.Count - 1)];
    }

    public abstract void PlayRandomSound();
}
