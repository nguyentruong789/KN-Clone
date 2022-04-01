using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T m_Instance = null;
    public static T Instance
    {
        get
        {
            // Instance requiered for the first time, we look for it
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

                // Object not found, we create a temporary one
                if (m_Instance == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("No instance of " + typeof(T).ToString() + ", a temporary one is created.");
#endif
                    isTemporaryInstance = true;
                    m_Instance = new GameObject("Temp Instance of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();

                    // Problem during the creation, this should not happen
                    if (m_Instance == null)
                    {
#if UNITY_EDITOR
                        Debug.LogError("Problem during the creation of " + typeof(T).ToString());
#endif
                    }
                }
                if (!_isInitialized)
                {
                    _isInitialized = true;
                    m_Instance.Init();
                }
            }
            return m_Instance;
        }
    }

    public static bool isTemporaryInstance { private set; get; }

    private static bool _isInitialized;

    public static bool _IsDontDestroyOnLoad = true;
    // If no other monobehaviour request the instance in an awake function
    // executing before this one, no need to search the object.
    protected virtual void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
        }
        else if (m_Instance != this)
        {
#if UNITY_EDITOR
            Debug.LogError("Another instance of " + GetType() + " is already exist! Destroying self...");
#endif
            DestroyImmediate(this);
            return;
        }
        if (!_isInitialized)
        {
            _isInitialized = true;
            m_Instance.Init();

            if (_IsDontDestroyOnLoad && transform.parent == null)
                DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// This function is called when the instance is used the first time
    /// Put all the initializations you need here, as you would do in Awake
    /// </summary>
    public virtual void Init() { }

    /// Make sure the instance isn't referenced anymore when the user quit, just in case.
    private void OnApplicationQuit()
    {
        m_Instance = null;
    }

    public static bool HasInstance()
    {
	    return m_Instance != null;
    }
}
// public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T> {
//     private static T _instance;
//
//     public static T Instance {
//         get {
//             CreateInstance();
//             return _instance;
//         }
//     }
//
//     private static void CreateInstance() {
//         if (_instance == null) {
//             _instance = FindObjectOfType<T>();
//             if (_instance == null) {
//                 var go = new GameObject(typeof(T).Name);
//                 _instance = go.AddComponent<T>();
//             }
//
//             if (!_instance._inititalized) {
//                 _instance.Initialized();
//                 _instance._inititalized = true;
//             }
//         }
//     }
//
//     private bool _inititalized;
//     protected virtual void Initialized(){}
//
//     public virtual void Awake() {
//         if (Application.isPlaying) {
//             DontDestroyOnLoad(this);
//         }
//
//         if (_instance != null) {
//             DestroyImmediate(this);
//         }
//     }
// }

/// <summary>
/// Return existed Object, don't create new one
/// </summary>
/// <typeparam name="T"></typeparam>

namespace CodeStringers.Framework {
    public abstract class ManualSingleton<T> : MonoBehaviour where T : ManualSingleton<T> {
        private static T _instance;

        public static T Instance => _instance;

        public virtual void Awake() {
            if (_instance != null) {
                DestroyImmediate(gameObject);
                return;
            }

            _instance = (T) (MonoBehaviour) this;
            if(_instance == null)
                Debug.LogError("Check init again" + typeof(T));
        }

        protected virtual void OnDestroy() {
            if (_instance == this)
                _instance = null;
        }
    }
}

