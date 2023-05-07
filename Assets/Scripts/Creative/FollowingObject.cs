using UnityEngine;

namespace Creative
{
    public class FollowingObject : MonoBehaviour
    {
        public FollowedObject Target;
        public int FrameDelay;

        private void Update()
        {
            Target.MoveFollower(this);
        }
    }
}