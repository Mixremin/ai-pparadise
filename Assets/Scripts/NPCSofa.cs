using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSofa : MonoBehaviour
{
    [SerializeField] private Transform[] AttachPoints;
    public List<GameObject> NPCs;
    public Transform OriginalNPCParent { get; private set; }
    [HideInInspector]
    public NPCWaypoint ApproachPoint;
    [HideInInspector]
    public UsableObjectGroup AssociatedGroup;

    private readonly Vector3 POSITION_OFFSET = new Vector3 (0.0f, 0.25f, 0.2f);

    public void PlaceNPC (GameObject _npc)
    {
        if (OriginalNPCParent == null)
            OriginalNPCParent = _npc.transform.parent;

        NPCs.Add(_npc);
        _npc.GetComponent<NPCMain>().thisSofa = this;

        int max = 0;
        int setPoint = 0;
        bool placed = false;
        do
        {
            max++;

            for (int z = 0; z < AttachPoints.Length; z++)
            {
                if (AttachPoints[z].childCount < max)
                {
                    setPoint = z;
                    placed = true;
                    break;
                }
            }
        } while (!placed);

        _npc.transform.parent = AttachPoints[setPoint];

        Vector3 offset = (max-1) * POSITION_OFFSET;

        _npc.transform.localPosition = Vector3.zero + offset;
        _npc.transform.localRotation = Quaternion.identity;
    }

    public void RemoveNPCFromList (string _name)
    {
        int index = -1;

        for (int z = 0; z < NPCs.Count; z++)
            if (NPCs[z].name == _name)
            {
                index = z;
                break;
            }

        if (index == -1)
            return;

        NPCs.RemoveAt(index);
    }

    public NPCMain EjectLastNPC ()
    {
        if (NPCs.Count == 0)
            return null;

        NPCMain result = NPCs[NPCs.Count - 1].GetComponent<NPCMain>();
        result.thisSofa = null;
        NPCs.RemoveAt(NPCs.Count - 1);

        result.transform.parent = OriginalNPCParent;
        result.thisAnim.SetBool("Sitting", false);
        result.transform.SetPositionAndRotation(ApproachPoint.transform.position, ApproachPoint.transform.rotation);
        result.StopGroupPolling();
        return result;
    }

    void Start()
    {
        NPCs = new List<GameObject>();
    }
}
