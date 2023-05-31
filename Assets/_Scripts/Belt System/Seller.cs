using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Seller : Belt
{
    [SerializeField]
    private ScriptablesRelativeAudio _thescriptablesRelativeAudio;
    private AudioSource _theaudioSource;
    private float _thevolume;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = $"Seller: {BeltID++}";

        _theaudioSource = GetComponent<AudioSource>();
        CallSound(EnumRelativeSounds.Spawn);
    }

    // Update is called once per frame
    new void Update()
    {
        if(BeltItem == null)
            return;

        isSpaceTaken = false;
        Assert.IsTrue(PoolManager.instance.DespawnObject(BeltItem));
        CallSound(EnumRelativeSounds.Activate);
        Wallet.instance.Money += BeltItem.GetItemData().Price;
        BeltItem = null;
    }

    public void CallSound(EnumRelativeSounds _action)
    {
        _thevolume = _thescriptablesRelativeAudio.volume;

        switch(_action)
        {
            case EnumRelativeSounds.Spawn:
                PlayRelativeSound(_thescriptablesRelativeAudio._spawnAudio, false);
                // Debug.Log("Spawn audio");
                break;
            case EnumRelativeSounds.Activate:
                PlayRelativeSound(_thescriptablesRelativeAudio._activateAudio, false);
                // Debug.Log("Activate audio");
                break;
            case EnumRelativeSounds.Problem:
                PlayRelativeSound(_thescriptablesRelativeAudio._problemAudio, false);
                // Debug.Log("Problem audio");
                break;
            default:
                break;
        }
    }

    protected void PlayRelativeSound(AudioClip _audioClip, bool loop)
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
