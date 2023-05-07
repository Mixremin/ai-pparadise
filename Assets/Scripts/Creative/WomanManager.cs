using System.Collections.Generic;
using UnityEngine;

namespace Creative
{
    public class WomanManager : MonoBehaviour
    {
        [SerializeField] private List<Animator> _women;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Bed bed)) return;

            var index = _women.Count - 1;
            bed.LayWomanDown(_women[index]);
            _women.RemoveAt(index);
        }
    }
}