using UnityEngine;


/// <summary>
/// Generic singleton base class.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static bool applicationIsQuitting = false;

    /// <summary>
    /// returns instance of T.
    /// </summary>
    public static T instance
    {
        get
        {

            if (applicationIsQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;

                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<T>();
                    _instance.gameObject.name = _instance.GetType().Name;
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// returns true if instance has not been destroyed.
    /// </summary>
    public static bool HasInstance
    {
        get
        {
            return !IsDestroyed;
        }
    }

    /// <summary>
    /// returns true if instance is not null.
    /// </summary>
    public static bool IsDestroyed
    {
        get
        {
            return _instance != null;
        }
    }

}