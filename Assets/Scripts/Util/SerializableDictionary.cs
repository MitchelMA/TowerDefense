using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    [Serializable]
    public class SerializableDictionary<TValue> : Dictionary<string, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct Dictstruct
        {
            public string name;
            public TValue value;
        };

        [SerializeField]
        private List<Dictstruct> _dict = new List<Dictstruct>();

        // Save the dictionary to the lists
        public void OnBeforeSerialize()
        {
            _dict.Clear();

            foreach(KeyValuePair<string, TValue> pair in this)
            {
                _dict.Add(new Dictstruct
                {
                    name = pair.Key,
                    value = pair.Value,
                });
            }
        }

        // Load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            var l = _dict.Count;
            for(int i = 0; i < l; i++)
            {
                var currentItem = _dict[i];

                if (ContainsKey(currentItem.name))
                {
                    currentItem.name = $"item{i}";
                }

                this.Add(currentItem.name, currentItem.value);
            }
        }
    }
}