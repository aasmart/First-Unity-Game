using System;
using System.Threading.Tasks;
using Projectiles.Script;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items.Scripts
{
    [CreateAssetMenu(menuName = "Create Items/Projectile Item", order = 2)]
    public class ProjectileItem : ItemFunction
    {
        [Header("Projectiles")]
        public Projectile projectile;
        public float shootVelocity;
        public Vector3 shootOffset;
        public int numProjectiles;

        [Tooltip("The spread, in degrees, of the projectile from -spread to spread")]
        public float spread;
        
        /*[Tooltip("The space needed in front of ")]
        public float shootClearance;*/

        public float projectileSpawnDelay;
        public float recoilForce;

        private const double Tolerance = 0.01f;
        
        private float _shootOffsetMagnitude;

        private float ShootOffsetMagnitude
        {
            get
            {
                if (!(Math.Abs(shootOffset.sqrMagnitude - _shootOffsetMagnitude * _shootOffsetMagnitude) > Tolerance))
                    return _shootOffsetMagnitude;
                
                _shootOffsetMagnitude = shootOffset.magnitude;
                _shootOffsetAngle = Mathf.Atan2(shootOffset.y, shootOffset.x);

                return _shootOffsetMagnitude;
            }
        }

        private float _shootOffsetAngle;

        private float ShootOffsetAngle
        {
            get
            {
                if (!(Math.Abs(shootOffset.sqrMagnitude - _shootOffsetMagnitude * _shootOffsetMagnitude) > Tolerance))
                    return _shootOffsetAngle;
                
                _shootOffsetMagnitude = shootOffset.magnitude;
                _shootOffsetAngle = Mathf.Atan2(shootOffset.y, shootOffset.x);

                return _shootOffsetAngle;
            }
        }

        protected override async Task LeftUse(LivingEntity parent, Rigidbody2D parentRigidBody)
        {
            parent.SetCharging(isChargeItem);
            
            var transform = parent.transform;

            // Spawn the projectile(s)
            for (var i = 1; i <= numProjectiles; i++)
            {
                if (!Application.isPlaying || !transform || !parent.gameObject.activeSelf)
                    return;
                
                /*var direction = CalculateDirection(position);*/
                var direction = (transform.rotation * Vector3.right).normalized;

                // Angle of the direction
                var angle = Helpers.AngleFloat(direction);

                var offset = CalculateOffsetPosition(angle);

                // Calculate the projectile spread
                var tempDirection = direction;
                if (spread != 0)
                {
                    var getSpread = Random.Range(-spread, spread);
                    tempDirection = Helpers.RotateVector(direction, getSpread * Mathf.Deg2Rad);
                }

                // Spawn the projectiles
                projectile.Spawn(
                    transform.position + offset,
                    tempDirection,
                    shootVelocity,
                    parent.gameObject,
                    damage);
                
                // Add recoil force
                parentRigidBody.AddForce(-direction * recoilForce, ForceMode2D.Impulse);

                await Task.Delay((int)(projectileSpawnDelay * 1000));
            }
        }

        protected override async Task RightUse(LivingEntity parent, Rigidbody2D parentRigidBody)
        {
            await Task.CompletedTask;
        }
        
        protected Vector3 CalculateOffsetPosition(float initialAngle)
        {
            // Calculate the angle of the firing offset
            var offsetAngle = initialAngle + ShootOffsetAngle;

            // Calculate the offset position
            return ShootOffsetMagnitude * new Vector3(Mathf.Cos(offsetAngle), Mathf.Sin(offsetAngle));
        }
    }
}