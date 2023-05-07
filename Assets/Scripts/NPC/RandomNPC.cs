using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNPC : MonoBehaviour
{
    public Color ExactColour { get; private set; }
    NPCIdentity identity;
    
    GameObject charModel;

    public void Initiate (NPCIdentity _identity)
    {
        identity = _identity;
        GameObject template = null;

        for (int z = 0; z < NPCGenerator.Instance.NPCTemplates.Length; z++)
            if (NPCGenerator.Instance.NPCTemplates[z].Type == identity.Type)
            {
                template = NPCGenerator.Instance.NPCTemplates[z].Templates[identity.ModelSubIndex];
                break;
            }

        if (template == null)
            return;

        charModel = GameObject.Instantiate(template, this.transform);
        charModel.transform.localScale = Vector3.one;
        charModel.transform.localPosition = Vector3.zero;
        charModel.transform.localRotation = Quaternion.identity;
        charModel.name = "Character Model";
        charModel.GetComponent<NPCCustomiser>().ApplyIdentity(identity);
        ExactColour = identity.Skin;
    }
}
