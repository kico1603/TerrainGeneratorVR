using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neuro
{
    public class SetHeightCenterEye : MonoBehaviour
    {
        public Transform centerEye;
        public float followSpeed = 2.0f; // Velocidad de seguimiento
        Vector3 desiredPosition;

        public float offSetCenterEye = -1f;

        float referenceHeight;

        private void Start()
        {
            desiredPosition = new Vector3();
            referenceHeight = centerEye.position.y;
        }

        private void Update()
        {
            FollowCenterEyeSmoothly();
        }

        public void FollowCenterEyeSmoothly()
        {
            desiredPosition = new Vector3(transform.position.x, centerEye.position.y + offSetCenterEye, transform.position.z);

            if (centerEye != null)
            {
                if (desiredPosition.y <= referenceHeight)
                {
                    desiredPosition = new Vector3(transform.position.x, referenceHeight, transform.position.z);
                }

                transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            }

        }
    }
}
