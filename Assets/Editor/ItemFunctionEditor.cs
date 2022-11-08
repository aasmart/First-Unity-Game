using Items.Scripts;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ItemFunction))]
    public class ItemFunctionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // If we call base the default inspector will get drawn too.
            // Remove this line if you don't want that to happen.
            //base.OnInspectorGUI();

            var itemFunction = target as ItemFunction;
            if(!itemFunction)
                return;

            if (itemFunction.isChargeItem)
            {
                itemFunction.chargeTime = EditorGUILayout.FloatField ("Total Charge Time:", itemFunction.chargeTime);
            }
        }
    }
}
