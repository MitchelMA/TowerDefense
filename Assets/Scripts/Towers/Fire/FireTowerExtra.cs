using UnityEngine;

namespace Towers.Fire
{
    // Additions to the Fire Tower in this file
    public partial class FireTower
    {
        protected override void Start()
        {
            // Call the base class' Start method
            base.Start();
            
            // Set the corresponding type
            Type = TowerType.Fire;
            
            Debug.Log($"Type: {Type}");
        }
    }
}