using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StackableType
{
    Coat = 0,
    Food = 1,
    Bottle = 2,
    Diaper = 3,
    Baby = 4,
    Overall = 5
}

public class StackableObject : MonoBehaviour
{
    [SerializeField]
    private StackableType ObjectType;
    public StackableType Type => ObjectType;
    [SerializeField]
    private float ObjectHeight = 0;
    public float Height => ObjectHeight;
    [SerializeField]
    private bool DisposableObject;
    [SerializeField]
    private bool OverrideQuantity;
    public bool Disposable => DisposableObject;

    public void SetHeight (float _hgt)
    {
        ObjectHeight = _hgt;
    }
}
