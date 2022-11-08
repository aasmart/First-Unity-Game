using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Projectiles.Script;

namespace Projectiles.Script
{
    [CreateAssetMenu(menuName = "Create Projectiles/Basic Projectile", order = 1)]
    public class Projectile : ScriptableObject
    {
        [SerializeField] private ProjectileAI projectile;
        public float duration;
        public float damage;
        public int pierce = 1;
        public float giveImmunityAmount;

        public GameObject Spawn(Vector3 position, Vector3 shootDirection, float shootVelocity, GameObject source, float damage)
        {
            var proj = Instantiate(projectile);
            proj.source = source;
            proj.damage = this.damage + damage;
            proj.SetVelocityAndDirection(shootVelocity, shootDirection);
            proj.penetrations = pierce;
            proj.immunity = giveImmunityAmount;
            proj.transform.SetPositionAndRotation(position, Quaternion.identity);
            KillProjectile(proj.gameObject);

            return proj.gameObject;
        }

        private async void KillProjectile(Object proj)
        {
            if (duration < 0)
                return;
            await Task.Delay((int) (duration * 1000));
            if (!Application.isPlaying)
                return;
            Destroy(proj, duration);
        }
    }
}
