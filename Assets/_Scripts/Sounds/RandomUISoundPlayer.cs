using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUISoundPlayer : IRandomSoundPlayer
{
    public override void PlayRandomSound()
    {
        AudioManager.instance.PlaySoundEffect(GetRandomSound());
    }
}
