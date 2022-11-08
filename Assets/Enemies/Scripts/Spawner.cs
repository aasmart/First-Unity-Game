using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Scripts
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Enemy spawn;
        [SerializeField] private float spawnRate;
        [SerializeField] private float spawnRadius;
        [SerializeField] private LayerMask notSpawnable;
        private float _currentSpawnTime;

        private void Awake()
        {
            _currentSpawnTime = spawnRate;
        }

        // Update is called once per frame
        private void Update()
        {
            while (_currentSpawnTime <= 0)
            {
                // Get the spawn position
                var pos = transform.position;
                GetSpawnPosition(ref pos);

                // Make sure it's not being spawned inside another object
                if (Physics2D.Raycast(pos, Vector2.one, 0.01f, notSpawnable).collider)
                    continue;
                
                // Create the enemy
                Instantiate(spawn, pos, Quaternion.identity);

                // Reset the timer
                _currentSpawnTime = spawnRate;
            }

            _currentSpawnTime -= Time.deltaTime;
        }

        private void GetSpawnPosition(ref Vector3 position)
        {
            position.z = 0;
            position += (Vector3) (Random.insideUnitCircle * spawnRadius);
        }
    }
}
