using System;
using System.Collections;
using System.Collections.Generic;
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

    protected void Awake()
    {
        if(_instance != null)
            Destroy(gameObject);

        _instance = this as T;
    }

    protected void OnDestroy()
    {
        if(_instance == this as T)
            _instance = null;
    }
}
