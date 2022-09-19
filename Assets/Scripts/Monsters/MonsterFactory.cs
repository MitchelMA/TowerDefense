using System;
using UnityEngine;

namespace Monsters
{
    public class MonsterFactory : MonoBehaviour
    {
        [Serializable]
        private struct FactoryEntry
        {
            public BaseMonster.MonsterType type;
            public GameObject monster;
        }

        [SerializeField] private FactoryEntry[] monsters = new FactoryEntry[3];

        public bool CreateMonster(BaseMonster.MonsterType type, out GameObject monster)
        {
            foreach (FactoryEntry entry in monsters.AsSpan())
            {
                if (!entry.type.Equals(type) || entry.monster is null)
                    continue;

                monster = entry.monster;
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
