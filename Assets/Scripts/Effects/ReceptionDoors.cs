using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceptionDoors : MonoBehaviour
{
    private Animator thisAnim;

    private void OnOpenDoors(string _dummyParam)
    {
        thisAnim.SetTrigger("Open");
    }    

    void Start()
    {
        thisAnim = this.GetComponent<Animator>();
        EventController.AddListener(EventController.EventTypes.OpenReceptionDoors, OnOpenDoors);
    }
   
}
