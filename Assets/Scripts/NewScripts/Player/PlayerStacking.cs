using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct StackSpendingPrefab
{
    public StackableType Type;
    public GameObject Prefab;
}

public class PlayerStacking : MonoBehaviour
{
    static PlayerStacking thisInstance;
    [HideInInspector]
    public List<GameObject> Stack;
    public Transform StackableRoot;
    public Transform StackableRootL3;
    public StackSpendingPrefab[] SpendingPrefabs;
    [Header("Indication")]
    public OnScreenUI MaxStackInfo;
/*    public Text StackBottles;
    public Text StackCoats;
    public Text StackDiapers;
    public Text StackBurgers;*/

    // singleton behaviour
    public static PlayerStacking Instance
    {
        get
        {
            if (!thisInstance)
            {
                thisInstance = FindObjectOfType(typeof(PlayerStacking)) as PlayerStacking;

                if (!thisInstance)
                    Debug.LogError("No active PlayerStacking script found in scene.");
            }

            return thisInstance;
        }
    }

    public bool HasDisposableItems()
    {
        for (int z = 0; z < Stack.Count; z++)
            if (Stack[z].GetComponent<StackableObject>().Disposable)
                return true;

        return false;
    }


    public bool OnlyBabiesInStack ()
    {
        for (int z = 0; z < Stack.Count; z++)
            if (Stack[z].GetComponent<StackableObject>().Disposable)
                return false;

        return true;
    }

    public bool StackingAvailable()
    {
        int k = 0;
        for (int z = 0; z < Stack.Count; z++)
            if (Stack[z].GetComponent<StackableObject>().Type != StackableType.Baby)
            {
                k++;
                if (k >= GameManager.Instance.MaxStackSize) return false;
            }

        return true;
    }

    public void UpdateIndication()
    {
        if (!StackingAvailable())
        {
            MaxStackInfo.SetTarget(Stack[Stack.Count - 1].transform);
        }
        else
        MaxStackInfo.transform.localPosition = new Vector3(10000, 10000, 10000);

        //        int[] stackables = new int[(int)StackableType.Overall];

        /*        if (Stack.Count > 0)
                {
                    for (int z = 0; z < Stack.Count; z++)
                    {
                        Stack[z].TryGetComponent(out StackableObject so);
                        if (so != null)
                            stackables[(int)so.Type]++;
                    }
                }

                StackBottles.text = stackables[(int)StackableType.Bottle].ToString();
                StackDiapers.text = stackables[(int)StackableType.Diaper].ToString();
                StackCoats.text = stackables[(int)StackableType.Coat].ToString();
                StackBurgers.text = stackables[(int)StackableType.Food].ToString();*/
    }

    private void RecalculatePositions()
    {
        if (Stack.Count == 0)
            return;

        Vector3 local = Vector3.zero;
        for (int z = 0; z < Stack.Count; z++)
        {
            Stack[z].transform.localPosition = local;
            local.y += Stack[z].GetComponent<StackableObject>().Height;
        }
    }

    public void AddToStack (GameObject _stackable)
    {
        Stack.Add(_stackable);

        Transform stRoot = StackableRoot;
        if (GameManager.Instance.CreativeProMode)
            stRoot = StackableRootL3;

        _stackable.transform.parent = stRoot;
        _stackable.transform.rotation = Quaternion.identity;
        RecalculatePositions();

        EventController.TriggerEvent(EventController.EventTypes.StackingQuantityChanged, "");
        UpdateIndication();
        this.GetComponent<PlayerMovement>().SetMovementWithStack(true);
    }

    public bool HaveBabiesInStack(bool _readyToReturn)
    {
        for (int z = 0; z < Stack.Count; z++)
        {
            NPCBaby b = Stack[z].GetComponent<NPCBaby>();
            if ((b != null) && (b.ReadyToReturn == _readyToReturn))
                return true;
        }

        return false;
    }

    public void RecalculateStackAfterRemoving()
    {
        RecalculatePositions();
        UpdateIndication();
        if (Stack.Count == 0)
            this.GetComponent<PlayerMovement>().SetMovementWithStack(false);
    }

    public GameObject GetBabyFromStack()
    {
        int index = -1;

        for (int z = 0; z < Stack.Count; z++)
        {
            NPCBaby b = Stack[z].GetComponent<NPCBaby>();
            if (b != null)
            {
                index = z;
                break;
            }
        }

        if (index == -1)
            return null;

        GameObject result = Stack[index];
        Stack.RemoveAt(index);
        RecalculateStackAfterRemoving();

        EventController.TriggerEvent(EventController.EventTypes.BabiesInStackAmountChanged, "");
        return result;
    }

    public void RemoveFromStack (int _index, Transform _toObject)
    {
        for (int z = 0; z < SpendingPrefabs.Length; z++)
            if (SpendingPrefabs[z].Type == Stack[_index].GetComponent<StackableObject>().Type)
            {
                FlyingObjects.Instance.CreateEffect(SpendingPrefabs[z].Prefab, Stack[_index].transform, _toObject, 1, true);
                break;
            }

        Destroy(Stack[_index]);
        Stack.RemoveAt(_index);
        RecalculateStackAfterRemoving();
        UpdateIndication();

        EventController.TriggerEvent(EventController.EventTypes.StackingQuantityChanged, "");
    }

    public bool TryRemoveObjectOfType (StackableType _type, Transform _atTarget)
    {
        for (int z = Stack.Count - 1; z >= 0; z--)
            if (Stack[z].GetComponent<StackableObject>().Type == _type)
            {
                RemoveFromStack(z, _atTarget);
                UpdateIndication();
                return true;
            }

        UpdateIndication();
        return false;
    }

    private void Start()
    {
        Stack = new List<GameObject>();
    }
}
