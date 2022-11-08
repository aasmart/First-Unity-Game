using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Projectiles.Script
{
    public class ProjectileAI : MonoBehaviour
    {
        // public Rigidbody2D rigidBody;
        [SerializeField] protected ParticleSystem collisionParticles;
        [SerializeField] protected AudioSource collisionSfx;
        [FormerlySerializedAs("setDeathParticleColor")] public bool setCollisionParticleColor = true;
        [SerializeField] protected int[] damageableEntities;
        [SerializeField] protected LayerMask collisionMask;
        protected Vector3 Direction;
        protected Vector3 DirectionNormal;
        [HideInInspector] public GameObject source;
        [HideInInspector] public float damage;
        [HideInInspector] public float velocity;
        [HideInInspector] public int penetrations;
        [HideInInspector] public float immunity;

        [SerializeField] private const int CollisionChecks = 2;
        protected RaycastHit2D[] _collisionResults = new RaycastHit2D[CollisionChecks];

        protected RaycastHit2D? CheckCollisions(Vector3 position, float distance)
        {
            Physics2D.RaycastNonAlloc(position, DirectionNormal, _collisionResults, distance);

            foreach (var hit in _collisionResults)
            {
                if (hit.transform is null || hit.collider.gameObject == source) continue;
                return hit;
            }

            return null;
        }

        protected void Collide(GameObject hit, Vector3 hitPosition)
        {
            // Check friendly fire
            if (source && hit.CompareTag(source.tag))
                return;

            var projectile = gameObject;
            projectile.transform.position = hitPosition;

            var hitColor = new Color(255, 255, 255);
            if (damageableEntities.Any(element => hit.gameObject.layer == element))
            {
                hitColor = LivingEntity.DoEntityCollision(hit, source, damage, immunity);
            }
            
            // Play particles
            PlayParticles(hitColor, hitPosition);
            
            // Audio
            if (collisionSfx)
            {
                PlayCollisionSfx();
            }

            // Kill the projectile
            if (ReachedMaxPenetrations())
                KillProjectile(projectile);
        }

        protected void PlayParticles(Color hitColor, Vector3 position)
        {
            var particles = Instantiate(collisionParticles);
            particles.transform.position = position;
            var particlesMain = particles.main;
            if(setCollisionParticleColor)
                particlesMain.startColor = hitColor;
            particles.Play();
        }

        protected void PlayCollisionSfx()
        {
            var sfx = Instantiate(collisionSfx);
            sfx.transform.position = transform.position;
            sfx.PlayOneShot(collisionSfx.clip, 1);
        }

        /// <summary>
        /// Determine if the projectile has reached the maximum amount of penetrations (equals zero). If not, it will decrement the current penetrations until it reaches zero.
        /// </summary>
        /// <returns>True if the number of penetrations equals zero</returns>
        protected bool ReachedMaxPenetrations()
        {
            if (penetrations < 0)
                return false;

            penetrations--;
            return penetrations <= 0;
        }

        protected static void KillProjectile(GameObject projectile)
        {
            projectile.SetActive(false);
            Destroy(projectile);
        }

        private void FixedUpdate()
        {
            var oldPos = transform.position;
            
            var hit = CheckCollisions(oldPos, velocity * Time.deltaTime);
            if(hit != null && hit.Value)
                Collide(hit.Value.transform.gameObject, hit.Value.point);
            
            transform.Translate(Direction * Time.deltaTime);
        }

        public void SetVelocityAndDirection(float velocity, Vector3 direction)
        {
            DirectionNormal = direction;
            this.velocity = velocity;
            Direction = velocity * direction;
        }

        protected void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, DirectionNormal);
        }
    }
}
