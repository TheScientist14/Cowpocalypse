using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance
    {
        get
        {
            return _instance ??= FindAnyObjectByType<T>() ?? new GameObject(typeof(T).Name + " " + nameof(Singleton<T>)).AddComponent<T>();
        }
    }
    private static T _instance;

    private void OnDestroy()
    {
        _instance = null;
    }
}
