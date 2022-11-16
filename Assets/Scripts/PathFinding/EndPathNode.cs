using Health;
using Monsters;
using UnityEngine;

namespace PathFinding
{
    public class EndPathNode : BasePathNode
    {
        public override void CallBack(PathTraverser traverser)
        {
            // get the monster-data from the traverser
            BaseMonster monsterData = traverser.GetComponent<BaseMonster>();
            HealthController.Instance.Deplete(monsterData.baseStats.damage);
            Destroy(traverser.gameObject);
        }
    }
}
