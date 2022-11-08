using System;
using System.Linq;
using Items.Scripts;
using Unity.VisualScripting;
using UnityEngine;

namespace Projectiles.Script
{
    public class LaserBeamAI : ProjectileAI
    {
        public float laserLength;
        public Texture2D laserBody;

        private LineRenderer _lineRenderer;
        
        private LivingEntity _sourceLiving;
        private LivingEntity SourceLiving =>
            _sourceLiving != null ? _sourceLiving : _sourceLiving = source.GetComponent<LivingEntity>();

        private Vector3 _laserEndPoint;

        private void OnEnable()
        {
            /*_lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = .5f;
            _lineRenderer.endWidth = 0.5f;
            _lineRenderer.textureMode = LineTextureMode.Tile;
            _lineRenderer.material = laserBody;*/
        }

        protected new void Collide(GameObject hit, Vector3 hitPosition)
        {
            // Check friendly fire
            if (source && hit.CompareTag(source.tag))
                return;

            if (damageableEntities.Any(element => hit.gameObject.layer == element))
            {
                var hitColor = LivingEntity.DoEntityCollision(hit, source, damage, immunity);

                // Play particles
                PlayParticles(hitColor, hitPosition);
            }

            // Audio
            if (collisionSfx)
                PlayCollisionSfx();
        }

        private void Update()
        {
            if(!source || !source.activeSelf)
                Destroy(gameObject);
                
            if (!transform.parent)
            {
                gameObject.transform.SetParent(source.transform);
            }

            if (SourceLiving is null || !SourceLiving.IsCharging())
                Destroy(gameObject);
            
            DirectionNormal = (source.transform.rotation * Vector3.right).normalized;
            _laserEndPoint = DetermineLaserEnd();
            
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _laserEndPoint);
        }

        private void FixedUpdate()
        {
            // ignored
        }

        /// <summary>
        /// Calculates the end point of the laser using a raycast
        /// </summary>
        /// <returns>The calculated end point</returns>
        private Vector3 DetermineLaserEnd()
        {
            var laserEndPoint = DirectionNormal * laserLength;
            Physics2D.RaycastNonAlloc(transform.position, laserEndPoint, _collisionResults, collisionMask);
            foreach (var hit in _collisionResults)
            {
                if (hit.transform is null || hit.collider.gameObject == source) continue;
                Collide(hit.transform.gameObject, hit.point);
                return hit.point;
            }
            
            return laserEndPoint;
        }

        private new void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, _laserEndPoint - DirectionNormal);
        }
    }
}