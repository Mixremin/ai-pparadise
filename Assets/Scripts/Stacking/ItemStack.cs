using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemStack : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector3 spaceBetweenItems;
    [SerializeField] private float pickUpDuration = 1f;
    [SerializeField] private AnimationCurve pickUpYMovement;
    [SerializeField] private AnimationCurve throwYMovement;
    private readonly List<Transform> items = new List<Transform>();

    public IReadOnlyList<Transform> Items => items;

    public void PickUp(Transform _obj)
    {
        items.Add(_obj);
        StartCoroutine(MoveObjectToStack(_obj));
    }

    private void UpdateActionPoints()
    {

    }

    public void Throw(int _index, Transform _target, float _throwTimer)
    {
        var item = items[_index];
        items.RemoveAt(_index);
        item.SetParent(null);
        UpdateStackPositions();

        StartCoroutine(ThrowObject(item, _target, _throwTimer));
    }

    private IEnumerator MoveObjectToStack(Transform _obj)
    {
        var startPosition = _obj.position;
        var startRotation = _obj.rotation;
        var t = 0f;
        var index = items.Count - 1;

        while (t < 1f)
        {
            yield return null;
            t += Time.deltaTime / pickUpDuration;

            var targetPosition = CalculatePosition(index);
            var x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            var y = Mathf.Lerp(startPosition.y, targetPosition.y, pickUpYMovement.Evaluate(t));
            var z = Mathf.Lerp(startPosition.z, targetPosition.z, t);

            _obj.position = new Vector3(x, y, z);
            _obj.rotation = Quaternion.Lerp(startRotation, startPoint.rotation, t);
        }

        _obj.SetParent(startPoint);
    }

    private IEnumerator ThrowObject(Transform _obj, Transform _target, float _throwTimer)
    {
        var startPosition = _obj.position;
        var startRotation = _obj.rotation;
        var t = 0f;
        UpdateActionPoints();

/*        NPCMainScript npc = _obj.GetComponent<NPCMainScript>();
        if (npc != null)
            npc.CurrentTargetPoint = _target.GetComponent<ActionTargetPoint>();*/

        while (t < 1f)
        {
            yield return null;
            t += Time.deltaTime / _throwTimer;

            var targetPosition = _target.position;
            var x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            var y = Mathf.Lerp(startPosition.y, targetPosition.y, throwYMovement.Evaluate(t));
            var z = Mathf.Lerp(startPosition.z, targetPosition.z, t);

            _obj.position = new Vector3(x, y, z);
            _obj.rotation = Quaternion.Lerp(startRotation, _target.rotation, t);
        }

        EventController.TriggerEvent(EventController.EventTypes.NPCLandedAtActionPoint, _obj.name + ":" + _target.name);
    }

    private void UpdateStackPositions()
    {
        for (var i = 0; i < items.Count; i++)
        {
            items[i].position = CalculatePosition(i);
        }
    }

    private Vector3 CalculatePosition(int index)
    {
        return startPoint.TransformPoint(index * spaceBetweenItems);
    }
}