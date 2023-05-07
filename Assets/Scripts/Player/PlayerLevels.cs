using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevels : MonoBehaviour
{
    public GameObject CurrentModel { get; private set; }
    public int Level { get; private set; }
//    [SerializeField] private PlayerModel playerModel;
    private Transform thisTr;

    private const int MAX_LEVEL = 3;

    private void SetLevelModel(int _level)
    {
        for (int z = 0; z < thisTr.GetChild(0).childCount; z++)
        {
            thisTr.GetChild(0).GetChild(z).gameObject.SetActive(z == (_level - 1));
            if (z == (_level - 1))
            {
                CurrentModel = thisTr.GetChild(0).GetChild(z).gameObject;
//                if (playerModel)
//                   playerModel.SetStackPoint(z);
            }
        }
    }

    private void OnLevelUp(string _dummyParam)
    {
        if (Level == MAX_LEVEL) return;

        Level++;
        SetLevelModel(Level);
    }

    void Start()
    {
        Level = GameManager.Instance.StartingPlayerLevel;
        thisTr = this.transform;
        SetLevelModel(Level);
       EventController.AddListener(EventController.EventTypes.PlayerLevelUp, OnLevelUp);
    }
}