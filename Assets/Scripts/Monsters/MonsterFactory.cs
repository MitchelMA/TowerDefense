using System;
using UnityEngine;
using Util;

namespace Monsters
{
    public class MonsterFactory : GenericSingleton<MonsterFactory>
    {
        [SerializeField] private SerializableDictionary<GameObject> monsters;

        public bool CreateMonster(BaseMonster.MonsterType type, out GameObject monster)
        {
            foreach (var (key, value) in monsters)
            {
                if(value == null 
                   ||!Enum.TryParse(key, out BaseMonster.MonsterType parseType)
                   || parseType != type)
                    continue;
                
                monster = value;
                return true;
            }
            
            Debug.LogError($"MonsterType of Type {type} could not be found", this);
            monster = default;
            return false;
        }

        public bool CreateMonster(int type, out GameObject monster)
        {
            return CreateMonster((BaseMonster.MonsterType) type, out monster);
        }
    }
}