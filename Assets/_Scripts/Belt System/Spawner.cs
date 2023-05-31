using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Pooling_System;
using UnityEngine;
using System;

public class Spawner : Belt
{
    [Expandable]
    public ItemData SpawnedItemData;
    public float SpawnRate;

    [SerializeField]
    private ScriptablesRelativeAudio _thescriptablesRelativeAudio;
    private AudioSource _theaudioSource;
    private float _thevolume;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = $"Spawner: {BeltID++}";
        PoolManager.instance.AddSpawnerToList(gameObject);
        StartCoroutine(Spawn());

        _theaudioSource = GetComponent<AudioSource>();
    }

    private IEnumerator Spawn()
    {
        while(true)
        {
            yield return new WaitForSeconds(SpawnRate);
            if(isSpaceTaken == false)
            {
                CallTheSound(EnumRelativeSounds.Activate);
                BeltItem = PoolManager.instance.SpawnObject(SpawnedItemData, transform.position);
                isSpaceTaken = true;
            }
        }
    }

    public void CallTheSound(EnumRelativeSounds _action)
    {
        if(_thescriptablesRelativeAudio == null)
        {
            Debug.LogWarning(gameObject.name + " has no scriptables relative audio");
            return;
        }

        _thevolume = _thescriptablesRelativeAudio.volume;

        switch(_action)
        {
            case EnumRelativeSounds.Spawn:
                PlayTheRelativeSound(_thescriptablesRelativeAudio._spawnAudio, false);
                // Debug.Log("Spawn audio");
                break;
            case EnumRelativeSounds.Activate:
                PlayTheRelativeSound(_thescriptablesRelativeAudio._activateAudio, false);
                // Debug.Log("Activate audio");
                break;
            case EnumRelativeSounds.Problem:
                PlayTheRelativeSound(_thescriptablesRelativeAudio._problemAudio, false);
                // Debug.Log("Problem audio");
                break;
            default:
                break;
        }
    }

    protected void PlayTheRelativeSound(AudioClip _audioClip, bool loop)
    {
        _theaudioSource.loop = loop;
        if(loop)
        {
            _theaudioSource.volume = _thevolume / 2;
        }
        else
        {
            _theaudioSource.volume = _thevolume;
        }
        _theaudioSource.clip = _audioClip;
        _theaudioSource.Play();

        // Debug.Log(_audioClip);
    }

}
