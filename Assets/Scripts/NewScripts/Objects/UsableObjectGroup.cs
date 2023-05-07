using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UsableObjectGroupType
{
    Dressers = 0,
    Echo = 1,
    Chairs = 2,
    WardBeds = 3
}

public class UsableObjectGroup : MonoBehaviour
{
    [SerializeField]
    private UsableObjectGroupType GroupType;
    [SerializeField]
    private UsableObject[] ObjectsInGroup;
    public NPCSofa AssociatedSofa;
    public UsableObjectGroupType Type => GroupType;

    public UsableObject GetReadyObject()
    {
        if (ObjectsInGroup.Length == 0)
            return null;

        List<UsableObject> list = new List<UsableObject>();
        for (int z = 0; z < ObjectsInGroup.Length; z++)
            if (ObjectsInGroup[z].IsReady())
                list.Add(ObjectsInGroup[z]);

        if (list.Count == 0)
            return null;

//        Debug.Log("Objects ready in group " + this.name + ": " + list.Count.ToString());

        Random.InitState(System.DateTime.Now.Millisecond);
        int k = Random.Range(0, list.Count);
        return list[k];
    }

    public NPCMain GetNewNPCFromSofa (UsableObject _toObject)
    {
        if (AssociatedSofa.NPCs.Count == 0)
            return null;

        NPCMain npc = AssociatedSofa.EjectLastNPC();
        npc.StopGroupPolling();
        npc.StartMovement(_toObject.ApproachWptSofa);
        _toObject.isOccupied = false;
        _toObject.UsedByNPC = npc;
        return npc;
    }

    private void Start()
    {
        for (int z = 0; z < ObjectsInGroup.Length; z++)
            ObjectsInGroup[z].PartOfGroup = this;

        AssociatedSofa.AssociatedGroup = this;
    }
}
