using System.Collections;
using System.Collections.Generic;
using Enemies.Scripts;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Enemy))]
    public class EnemyHandles : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            // Get the target and attempt to convert it to an Enemy
            var selected = target as Enemy;
            if (!selected) return;

            var transform = selected.transform;
            var pos = transform.position;
            var forwards = transform.forward;

            Handles.color = Color.white;
            Handles.DrawWireDisc(pos, forwards, selected.maxAttackDistance);
            // Handles.DrawSolidArc(pos, forwards);

            Handles.color = Color.red;
            var handleEndPos = pos + Vector3.down * selected.maxAttackDistance;
            selected.maxAttackDistance =
                Handles.ScaleValueHandle(
                    selected.maxAttackDistance, handleEndPos
                    , Quaternion.LookRotation(Vector3.forward),
                    2,
                    Handles.SphereHandleCap,
                    0.5f);
        }
    }
}
