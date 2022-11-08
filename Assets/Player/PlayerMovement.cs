using System;
using System.Threading.Tasks;
using Enemies.Scripts.Pathfinding;
using Items.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerMovement : LivingEntity
    {
        [Header("Player Settings")]
        public float movementSmoothing;

        public float moveSpeed;
        public float runSpeedMultiplier;
        
        [SerializeField] private LayerMask notRespawnable;

        private float _yMove;
        private float _xMove;
        private bool _running;

        private Camera _camera;

        private void MovePlayer(float xMove, float yMove)
        {
            var velocity = rigidBody.velocity;
            var zero = Vector3.zero;

            var x = Mathf.Abs(xMove);
            var y = Mathf.Abs(yMove);

            // Average the directions for diagonal movements
            float angle = 0;
            if (Mathf.Abs(xMove) > 0 && Mathf.Abs(yMove) > 0)
                angle = Mathf.Abs(Mathf.Atan(yMove / xMove));
            else if (x > 0)
                angle = 0;
            else if (y > 0)
                angle = 1.57079f;

            rigidBody.velocity = Vector3.SmoothDamp(
                velocity,
                new Vector3(xMove * Mathf.Cos(angle), yMove * Mathf.Sin(angle)) * (moveSpeed * (_running ? runSpeedMultiplier : 1.0f)),
                ref zero,
                movementSmoothing * Time.deltaTime);
        }

        private void Look(Vector3 pos)
        {
            Vector3 playerPos = rigidBody.position;
            rigidBody.rotation = Helpers.AngleFloat(pos - playerPos) * (180.0f / Mathf.PI);
        }

        // Start is called before the first frame update
        private void Start()
        {
            _camera = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            CheckHealth();

            _yMove = Input.GetAxis("Vertical");
            _xMove = Input.GetAxis("Horizontal");
            _running = Input.GetKey(KeyCode.LeftShift);
            
            if (Input.GetKey(KeyCode.Mouse0))
                equippedItem.GetFunction().Use(KeyCode.Mouse0, this, rigidBody, Input.GetKeyDown(KeyCode.Mouse0));
            else if (Input.GetKey(KeyCode.Mouse1))
                equippedItem.GetFunction().Use(KeyCode.Mouse1, this, rigidBody, Input.GetKeyDown(KeyCode.Mouse1));
            else if(this.IsCharging())
                this.SetCharging(false);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Time.timeScale = Math.Abs(Time.timeScale - 1.0f) < 0.01f ? 0 : 1;
            }
            
            UpdateTimers();
            RegenerateHealth();
        }

        private void FixedUpdate()
        {
            MovePlayer(_xMove, _yMove);
            Look(_camera.ScreenToWorldPoint(Input.mousePosition));
        }

        protected override async void Kill()
        {
            gameObject.SetActive(false);
            PlayDeathParticles(transform.position, deathParticlesColor);
            await Task.Delay(2500);
            Respawn();
        }

        private void Respawn()
        {
            if (!Application.isPlaying)
                return;

            Vector3 pos;
            while (true)
            {
                pos = Random.insideUnitCircle * 15;

                // Make sure it's not being spawned inside another object
                if (Physics2D.Raycast(pos, Vector2.one, 0.01f, notRespawnable).collider)
                    continue;
                break;
            }

            CurrentHealth = maxHealth;
            gameObject.transform.position = pos;
            healthBar.Reset();
            gameObject.SetActive(true);
        }
    }
}
