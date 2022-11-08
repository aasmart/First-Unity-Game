using System.Collections;
using System.Collections.Generic;
using Items.Scripts;
using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(menuName = "Create Items/Inventory Item", order = 1)]
    public class InventoryItem : ScriptableObject
    {
        public string itemName;
        [SerializeField] private ItemFunction function;

        public ItemFunction GetFunction()
        {
            return function;
        }
    }
}
