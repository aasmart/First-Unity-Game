using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class FollowCam : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        public Vector3 offset;
        public float followSpeed;

        // Start is called before the first frame update
        private void Start()
        {
            Helpers.Camera.transform.position = target.transform.position;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            var targetPos = target.transform.position + offset;
            Helpers.Camera.transform.position =
                Vector3.Lerp(Helpers.Camera.transform.position + offset, targetPos + offset, followSpeed);
        }
    }
}
