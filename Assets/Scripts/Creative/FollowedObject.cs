using System.Collections.Generic;
using UnityEngine;

namespace Creative
{
    public class FollowedObject : MonoBehaviour
    {
        public List<Frame> formerPositions = new List<Frame>();
        public int frameLimit;
        public float minimumDistance = 1;
        public float maxSpeed;
        public float maxRotationSpeed;
        private Vector3 formerPosition = Vector3.zero;
    
        private void Update()
        {
            if (formerPosition != transform.position)
            {
                formerPositions.Insert(0, new Frame
                {
                    Position = transform.position,
                    Rotation = transform.rotation
                });


                if (formerPositions.Count > frameLimit)
                    formerPositions.RemoveAt(formerPositions.Count - 1);
            }

            formerPosition = transform.position;
        }

        public void MoveFollower(FollowingObject follower)
        {
            if (formerPositions.Count <= follower.FrameDelay) return;

            var frame = formerPositions[follower.FrameDelay];
            var followerTransform = follower.transform;
            followerTransform.position = frame.Position;
            followerTransform.rotation = frame.Rotation;
        }

        [System.Serializable]
        public struct Frame
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }
    }
}