using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Projectiles.Script
{
    public class HomingProjectileAI : ProjectileAI
    {
        public float trackDistance;
        public float turnSmoothing;
        public float trackDelay;
        
        private GameObject _target;
        private GameObject Target => _target && _target.activeSelf ? _target : _target = FindTarget();
        
        private Vector3 _refVelocity = Vector3.zero;

        private void Update()
        {
            if (trackDelay > 0)
                trackDelay -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (trackDelay > 0)
                return;
            
            var oldPos = transform.position;

            var hit = CheckCollisions(oldPos, velocity * Time.deltaTime);
            if(hit != null && hit.Value)
                Collide(hit.Value.transform.gameObject, hit.Value.point);
            
            if (Target)
            {
                var targetDirection = (Target.transform.position - oldPos).normalized;
                var directionNew = Vector3.SmoothDamp(DirectionNormal, targetDirection, ref _refVelocity, turnSmoothing);
                DirectionNormal = directionNew;
                Direction = DirectionNormal * velocity;
            }

            transform.Translate(Direction * Time.deltaTime);
        }

        private GameObject FindTarget()
        {
            var entities = FindObjectsOfType(typeof(LivingEntity), false);

            var projectilePos = transform.position;
            GameObject closestEntity = null;
            var closestDist = trackDistance;
            foreach (var entityObj in entities)
            {
                try
                {
                    var entity = entityObj.GameObject();
                    if (entity.activeSelf && (entity.transform.position - projectilePos).sqrMagnitude <= closestDist * closestDist && !entity.CompareTag(source.tag) && HasLineOfSight(entity))
                    {
                        closestEntity = entity;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            
            return closestEntity;
        }
        
        private bool HasLineOfSight(GameObject entity)
        {
            // Creates a linecast between the GameObject's current position and the player's position
            return !Physics2D.Linecast(
                transform.position, 
                entity.transform.position, 
                collisionMask).collider;
        }
    }
}
