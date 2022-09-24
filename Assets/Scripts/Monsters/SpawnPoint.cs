using PathFinding;
using UnityEngine;

namespace Monsters
{
    public class SpawnPoint : MonoBehaviour
    {
        public BasePathNode startNode;

        public void SetupMonster(GameObject monster, float difficultyMultiplier)
        {
            PathTraverser traverser = monster.GetComponent<PathTraverser>();
            BaseMonster monsterData = monster.GetComponent<BaseMonster>();

            traverser.TargetNode = startNode;
            monster.transform.position = transform.position;
            // apply the multiplier
            monsterData.baseStats.hp = (int)(monsterData.baseStats.hp * difficultyMultiplier);
            monsterData.baseStats.speed *= difficultyMultiplier;
            monsterData.baseStats.damage = (int)(monsterData.baseStats.damage * difficultyMultiplier);
        }
    }
}
