using PathFinding;
using UnityEngine;

namespace Monsters
{
    public class SpawnPoint : MonoBehaviour
    {
        public BasePathNode startNode;

        public void SetupMonster(GameObject monster)
        {
            PathTraverser traverser = monster.GetComponent<PathTraverser>();

            traverser.TargetNode = startNode;
            monster.transform.position = transform.position;
        }
    }
}
