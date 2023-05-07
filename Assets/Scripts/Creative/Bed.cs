using System.Collections;
using UnityEngine;

namespace Creative
{
    public class Bed : MonoBehaviour
    {
        private static readonly int LayDown = Animator.StringToHash("LayDown");

        [SerializeField] private bool _instaMove;
        [SerializeField] private Transform _womanPoint;
        [SerializeField] private AnimationCurve _xMovement;
        [SerializeField] private AnimationCurve _yMovement;

        public void LayWomanDown(Animator woman)
        {
            woman.SetTrigger(LayDown);
            if (_instaMove)
            {
                woman.transform.SetParent(transform);
                woman.transform.position = _womanPoint.position;
                woman.transform.rotation = _womanPoint.rotation;
            }
            else
            {
                StartCoroutine(MoveWomanToBed(woman.transform));
                StartCoroutine(RotateWomanToBed(woman.transform));
            }
        }

        private IEnumerator RotateWomanToBed(Transform woman)
        {
            var t = 0f;
            var startRotation = woman.rotation;
            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                woman.rotation = Quaternion.Lerp(startRotation, _womanPoint.rotation, t);
                yield return null;
            }
        }

        private IEnumerator MoveWomanToBed(Transform woman)
        {
            var t = 0f;
            var startPosition = woman.position;

            woman.SetParent(transform);
            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                var x = Mathf.Lerp(startPosition.x, _womanPoint.position.x, _xMovement.Evaluate(t));
                var y = Mathf.Lerp(startPosition.y, _womanPoint.position.y, _yMovement.Evaluate(t));
                var z = Mathf.Lerp(startPosition.z, _womanPoint.position.z, t);
                woman.position = new Vector3(x, y, z);
                yield return null;
            }
        }
    }
}