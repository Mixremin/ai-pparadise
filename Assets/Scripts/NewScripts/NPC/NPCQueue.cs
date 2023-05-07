using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCQueuesType
{
    Reception = 0,
}

public class NPCQueue : MonoBehaviour
{
    [SerializeField]
    private NPCQueuesType QueueType;
    public Transform[] QueuePlaces;
    private List<NPCMain> NPCs;
    public NPCQueuesType Type => QueueType;

    public bool CanEnqueue()
    {
        return NPCs.Count < QueuePlaces.Length;
    }

    public void EnterQueue(NPCMain _npc)
    {
        _npc.StartMovement(QueuePlaces[NPCs.Count].name);
        NPCs.Add(_npc);
    }

    public void ProgressQueue()
    {
        for (int z = 0; z < NPCs.Count; z++)
            NPCs[z].StartMovement(QueuePlaces[z].name);
    }

    public NPCMain RemoveFirstNPC ()
    {
        if (NPCs.Count == 0)
            return null;

        NPCMain result = NPCs[0];
        NPCs.RemoveAt(0);

        ProgressQueue();

        return result;
    }

    private void Start()
    {
        NPCs = new List<NPCMain>();
    }
}
