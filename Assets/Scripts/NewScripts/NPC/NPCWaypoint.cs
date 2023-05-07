using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaypointType
{
    Standard = 0,
    Sequential = 1,
    ReachSofa = 2,
    UseObject = 3,
    NextTile = 4,
    TryGroupedObject = 5,
    LastWaypoint = 6,
    TakeChild = 7
}

public class NPCWaypoint : MonoBehaviour
{
    public WaypointType WptType;
    [Header("Sequential Type Options")]
    public NPCWaypoint NextPoint;
    [Header("SofaReach Type Options")]
    public NPCSofa Sofa;
    [Header("Object Usage Type Options")]
    public UsableObject Object;
    [Header("NextTile Type Options")]
    public LevelTile Tile;
    public NPCWaypoint NextPointIfOpen;
    public NPCWaypoint NextPointIfClosed;
    [Header("Try Grouped Object Options")]
    public UsableObjectGroup ObjectGroup;
    public NPCWaypoint FallbackOption;
    [Header("Take Child Options")]
    public GameObject AttachPoint;
    public ActionZone ActionZone;
    [HideInInspector] public GameObject UsedByNPC;

    private void Start()
    {
        UsedByNPC = null;

        if (WptType == WaypointType.ReachSofa)
            Sofa.ApproachPoint = this;
    }
}
