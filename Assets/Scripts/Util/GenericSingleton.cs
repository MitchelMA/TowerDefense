using UnityEngine;

namespace Util
{
    public class GenericSingleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<T>();
                if (Instance != null) return _instance;

                GameObject container = new GameObject(typeof(T).Name);
                _instance = container.AddComponent<T>();

                return _instance;
            }
        }
    }
}