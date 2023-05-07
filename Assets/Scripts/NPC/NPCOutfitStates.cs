using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OutfitState
{
    public GameObject Item;
    public int State;
}

public class NPCOutfitStates : MonoBehaviour
{
    public int DefaultState;
    public OutfitState[] Items;
    public int CurrentState { get; private set; }

    public void SetState (int _state)
    {
        if ((Items == null) || (Items.Length == 0))
            return;

        CurrentState = _state;

        for (int z = 0; z < Items.Length; z++)
            Items[z].Item.SetActive(false);

        for (int z = 0; z < Items.Length; z++)
            if (Items[z].State == _state)
                Items[z].Item.SetActive(true);
    }

    void Start()
    {
        SetState(DefaultState);
    }
}
