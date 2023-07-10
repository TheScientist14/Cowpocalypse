using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List of sounds")]
public class SoundList : ScriptableObject
{
	public List<AudioClip> Sounds = new List<AudioClip>();
}
