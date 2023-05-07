using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private List<ItemStack> _stacks;
    [SerializeField] private List<StackPoints> _stackPoints = new List<StackPoints>();
    [SerializeField] private Vector3 _offset1 = new Vector3(-0.0430000015f,-0.474000007f,0.317999989f);
    [SerializeField] private Vector3 _offset2 = new Vector3(0.0430000015f,-0.474000007f,0.317999989f);
    private int _stackPointIndex;

    private void Update()
    {
        for (var i = 0; i < _stacks.Count; i++)
        {
            var offset = i == 0 ? _offset1 : _offset2;
            _stacks[i].transform.position = _stackPoints[_stackPointIndex].Points[i].transform
                .TransformPoint(_stackPoints[_stackPointIndex].Points[i].transform.localPosition + offset);
            _stacks[i].transform.rotation = _stackPoints[_stackPointIndex].Points[i].transform.rotation;
        }
    }

    public void SetStackPoint(int index)
    {
        _stackPointIndex = index;
    }

    [Serializable]
    private struct StackPoints
    {
        public List<Transform> Points;
    }
}