using System;
using System.Collections.Generic;
using Enemies.Scripts.Pathfinding;
using Items.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Task = System.Threading.Tasks.Task;

namespace Enemies.Scripts
{
    public class Enemy : LivingEntity
    {
        [Header("Enemy Settings")]
        public float turnSpeed;
        public float moveSpeed;
        public float followRange;
        public float maxAttackDistance;
        public float minAttackDistance;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private Canvas healthBarCanvas;
        [SerializeField] private float healthBarOffset;

        // Targeting
        private GameObject _target;
        private Rigidbody2D _targetRigidBody;
        private GameObject Target => _target ? _target : _target = GameObject.FindGameObjectWithTag("Player");
        private Rigidbody2D TargetRigidBody => _targetRigidBody ? _targetRigidBody : _targetRigidBody = Target.GetComponent<Rigidbody2D>();
        
        // Pathfinding
        private AStarPathfinder _pathfinder;
        private AStarPathfinder PathfindManager => _pathfinder ? _pathfinder : _pathfinder = GameObject.FindGameObjectWithTag("PathfindManager").GetComponent<AStarPathfinder>();
        
        private List<Node> _path;
        private int _targetIndex;
        private Vector3 _wayPoint;
        private Vector3 _targetPosOld;

        private bool _hasSpawned;

        private void OnDrawGizmos()
        {
            AStarPathfinder.DrawPath(_path);
        }

        public async void OnEnable()
        {
            var vec = Vector3.zero;
            transform.localScale = Vector3.zero;
            while (transform && (Vector3.one - transform.localScale).sqrMagnitude > .01f)
            {
                transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one,ref vec, 0.15f);
                await Task.Delay(1);
            }
            
            if(!gameObject)
                return;

            transform.localScale = Vector3.one;
            _hasSpawned = true;
            DoEntityCollision += GameObjectEntityCollision;
        }

        private void Update()
        {
            CheckHealth();
            healthBarCanvas.enabled = CombatTimer > 0.0f;
            
            if (!_hasSpawned)
                return;

            if (CanFire())
            {
                equippedItem.GetFunction().Use(KeyCode.Mouse0, this, rigidBody, Input.GetKeyDown(KeyCode.Mouse0));
            }

            UpdateTimers();
            RegenerateHealth();

            // Pathfinding stuff
            if ((CanFire() && InRange(minAttackDistance)) || !InRange(followRange) || !_target.activeSelf)
            {
                _path = null;
                return;
            }
                
            if (_path != null/* && PathfindManager.GetGrid().GetNode(_targetPosOld) == PathfindManager.GetGrid().GetNode(Player.transform.position) */&& _targetIndex <= _path.Count)
            {
                FollowPath();
            }
            else
            {
                _path = PathfindManager.GetEntityPath(transform.position, Target.transform.position, gameObject);
                if(_path is null)
                    return;
                _wayPoint = _path[0].WorldPos;
                _targetIndex = 0;
            }
            
            _targetPosOld = Target.transform.position;
        }

        private void FixedUpdate()
        {
            if (!Target)
                return;
            
            var targetPos = Target.transform.position;
            var position = targetPos;
            if (equippedItem.GetFunction() is ProjectileItem)
            { 
                position = (Vector2)targetPos + TargetRigidBody.velocity * ((targetPos - transform.position).magnitude / ((ProjectileItem)equippedItem.GetFunction()).shootVelocity);
            }

            if(Target && HasLineOfSight()) LookPosition(position);
        }

        private void LateUpdate()
        {
            var position = transform.position;
            var canvasTransform = healthBarCanvas.transform;

            healthBarCanvas.transform.rotation = Quaternion.Euler(0.0f, 0.0f, position.z * -1.0f);
            healthBarCanvas.transform.position =
                 new Vector3(
                     healthBarOffset * Mathf.Sin(position.z), 
                     healthBarOffset * Mathf.Cos(position.z), 
                     canvasTransform.position.z)
                 + position;
        }

        private void LookPosition(Vector3 lookPos)
        {
            Vector3 pos = rigidBody.position;
            transform.rotation = Quaternion.Lerp(
                transform.rotation, 
                Quaternion.Euler(new Vector3(0, 0, Helpers.AngleFloat(lookPos - pos) * Mathf.Rad2Deg)), 
                turnSpeed * Time.deltaTime);
        }
        
        private bool CanFire()
        {
            return Target && Target.activeSelf && HasLineOfSight() && InRange(maxAttackDistance);
        }

        private bool HasLineOfSight()
        {
            // Creates a linecast between the GameObject's current position and the player's position
            return !Physics2D.Linecast(
                gameObject.transform.position, 
                Target.transform.position,
                collisionMask).collider;
        }

        private bool InRange(float range)
        {
            return Target && (Target.transform.position - transform.position).sqrMagnitude < range * range;
        }
        

        protected override void Kill()
        {
            var obj = gameObject;
            var pos = transform.position;
            PlayDeathParticles(pos, deathParticlesColor);
            obj.SetActive(false);
            Destroy(obj);
        }

        public float GetAttackRange()
        {
            return maxAttackDistance;
        }

        private async void FollowPath()
        {
            var pos = transform.position;
            /*var grid = PathfindManager.GetGrid();*/
            
            if (/*grid.GetNode(_wayPoint) == grid.GetNode(pos)*/(_wayPoint - pos).sqrMagnitude < 0.05f)
            {
                _targetIndex++;
                if (_targetIndex >= _path.Count)
                {
                    return;
                }
                _wayPoint = _path[_targetIndex].WorldPos;
            }
            if(!CanFire())
                LookPosition(_wayPoint);
            transform.position += (_wayPoint - pos).normalized * (moveSpeed * Time.deltaTime);

            /*_path = pathfinder.GetEntityPath(transform.position, Player.transform.position);*/
            await Task.CompletedTask;
        }
    }
}
