using UnityEngine;

namespace TP
{
    /// <summary>
    /// This is just a very minimal singleton helper class, facilitating easy access to the SoundManager class.
    /// Replace or customize it to your liking.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        /// <summary>
        /// The is persistence.
        /// </summary>
        public bool IsPersistence = false;

        /// <summary>
        /// The m instance.
        /// </summary>
        protected static T m_Instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                /*if (m_Instance == null)
                {
                    m_Instance = FindObjectOfType<T>();
                    if (m_Instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        m_Instance = obj.AddComponent<T>();
                    }
                }*/
                return m_Instance;
            }
        }

        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected virtual void Awake()
        {
            if (IsPersistence)
            {
                if (ReferenceEquals(m_Instance, null))
                {
                    m_Instance = this as T;

                    DontDestroyOnLoad(gameObject);
                }
                else if (!ReferenceEquals(m_Instance, this as T))
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                m_Instance = this as T;
            }
        }

        protected virtual void OnDestroy()
        {
            //m_Instance = null;
        }
    }

}