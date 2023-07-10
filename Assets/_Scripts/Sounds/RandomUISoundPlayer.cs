public class RandomUISoundPlayer : IRandomSoundPlayer
{
	public override void PlayRandomSound()
	{
		AudioManager.instance.PlaySoundEffect(GetRandomSound());
	}
}
