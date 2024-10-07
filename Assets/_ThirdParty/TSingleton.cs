using UnityEngine;

namespace TAGUNI
{
	public class TSingleton<T> : object where T : new()
    {
        protected static T _instance;
        private static object _lock = new object();

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
					{
                        if (_instance == null)
                            _instance = new T();
                        
                        return _instance;
                    }
                }
                else
                    return _instance;
            }
        }

        public virtual void Init() { }
        internal virtual void InternalInit() { }

    }

	public class TSingletonWithMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        private static object _lock = new object();

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
						{                     
                            _instance = (T)FindObjectOfType(typeof(T));

                            if (FindObjectsOfType(typeof(T)).Length > 1)
                            {
                                //Debug.LogError("[Singleton] Something went really wrong " +
                                //    " - there should never be more than 1 singleton!" +
                                //    " Reopenning the scene might fix it.");
                                return _instance;
                            }

                            if (_instance == null)
                            {
                                GameObject singleton = new GameObject();
                                _instance = singleton.AddComponent<T>();
                                singleton.name = "[" + typeof(T).ToString() + "]";

                                if (Application.isPlaying)
                                {
                                    DontDestroyOnLoad(singleton);
                                }
                            }
                            else
                            {
                                //Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);
                            }
                        }

                        return _instance;
                    }
                }
                else
                {
                    return _instance;
                }
            }
        }

        public virtual void Init() { }
        internal virtual void InternalInit() { }

    }
}