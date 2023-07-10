using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T instance
	{
		get
		{
			return _instance;
		}
	}
	private static T _instance;

	protected virtual void Awake()
	{
		if(_instance != null && _instance != this as T)
		{
			Destroy(gameObject);
			return;
		}

		if(IsPersistent())
			DontDestroyOnLoad(gameObject);

		_instance = this as T;
	}

	protected virtual void OnDestroy()
	{
		if(_instance == this as T)
			_instance = null;
	}

	protected virtual bool IsPersistent()
	{
		return false;
	}
}
