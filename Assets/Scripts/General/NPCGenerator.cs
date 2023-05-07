using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkinColours
{
    Error = -1,
    White = 0,
    Black = 1,
    Mixed = 2
}

public enum NPCType
{
    Error = -1,
    Female = 0,
    FemalePregnant = 1,
    Male = 2,
    Baby = 3
}

[System.Serializable]
public struct CharacterRandomSkin
{
    public SkinColours Colour;
    public Gradient SkinGradient;
    public float Probability;
}

[System.Serializable]
public struct NPCSettings
{
    public GameObject Template;
    public int[] SkinTextures;
    public int[] RandomisedOutfitTextures;
    public int[] RandomisedShoeTextures;
}

[System.Serializable]
public struct NPCIdentity
{
    public NPCType Type;
    public int ModelSubIndex;
    public SkinColours SkinColour;
    public Color Skin;
    public Color Hair;
    public Color Outfit1;
    public Color Outfit2;
    public Color Shoes;
}

[System.Serializable]
public struct NPCTemplate
{
    public NPCType Type;
    public GameObject[] Templates;
}

public class NPCGenerator : MonoBehaviour
{
    static NPCGenerator npcgInstance;
    public CharacterRandomSkin[] SkinTypes;
    public Gradient HairColours;
    public Gradient OutfitColours1;
    public Gradient OutfitColours2;
    public Gradient ShoeColours;
    public Transform ParentContainer;

    public NPCTemplate[] NPCTemplates;
    
    // singleton behaviour
    public static NPCGenerator Instance
    {
        get
        {
            if (!npcgInstance)
            {
                npcgInstance = FindObjectOfType(typeof(NPCGenerator)) as NPCGenerator;

                if (npcgInstance == null)
                    Debug.LogError("No active NPCGenerator script found in scene.");
            }

            return npcgInstance;
        }
    }

    private SkinColours GenerateRandomSkinColour (out Color _clr)
    {
        float maxV = 0;

        for (int z = 0; z < SkinTypes.Length; z++)
            maxV += SkinTypes[z].Probability;

        float st = Random.Range(0, maxV);
        float a = 0;

        for (int z = 0; z < SkinTypes.Length; z++)
            if ((st >= a) && (st <= a + SkinTypes[z].Probability)) 
            {
                _clr = SkinTypes[z].SkinGradient.Evaluate (Random.Range(0f,1f));
                return SkinTypes[z].Colour; 
            }
            else
                a += SkinTypes[z].Probability;

        _clr = SkinTypes[0].SkinGradient.Evaluate(Random.Range(0f, 1f));
        return SkinTypes[0].Colour;
    }

    private SkinColours MixColours (SkinColours _skin1, SkinColours _skin2)
    {
        if (_skin1 == SkinColours.White)
        {
            if (_skin2 == SkinColours.White)
                return SkinColours.White;
            if (_skin2 == SkinColours.Mixed)
                return SkinColours.Mixed;
            if (_skin2 == SkinColours.Black)
                return SkinColours.Mixed;
        }

        if (_skin1 == SkinColours.Black)
        {
            if (_skin2 == SkinColours.White)
                return SkinColours.Mixed;
            if (_skin2 == SkinColours.Mixed)
                return SkinColours.Black;
            if (_skin2 == SkinColours.Black)
                return SkinColours.Black;
        }

        if (_skin1 == SkinColours.Mixed)
        {
            if (_skin2 == SkinColours.White)
                return SkinColours.Mixed;
            if (_skin2 == SkinColours.Mixed)
                return SkinColours.Mixed;
            if (_skin2 == SkinColours.Black)
                return SkinColours.Black;
        }

        return SkinColours.White;
    }

    // random NPC
    private NPCIdentity GenerateRandom (NPCType _type)
    {
        NPCIdentity identity = new NPCIdentity();
        identity.SkinColour = SkinColours.Error;

        int max = -1;

        for (int z = 0; z < NPCTemplates.Length; z++)
            if (NPCTemplates[z].Type == _type)
            {
                max = NPCTemplates[z].Templates.Length;
                break;
            }

        if (max == -1)
            return identity;

        int state = System.DateTime.Now.Millisecond;
        identity.SkinColour = GenerateRandomSkinColour(out Color clr);
        identity.Skin = clr;
        identity.ModelSubIndex = Random.Range(0, max);
        identity.Type = _type;
        identity.Hair = HairColours.Evaluate(Random.Range(0f, 1f));
        identity.Outfit1 = OutfitColours1.Evaluate(Random.Range(0f, 1f));
        identity.Outfit2 = OutfitColours2.Evaluate(Random.Range(0f, 1f));
        identity.Shoes = ShoeColours.Evaluate(Random.Range(0f, 1f));

        return identity;
    }

    // random NPC of selected skin colour
    private NPCIdentity GenerateRandom(NPCType _type, SkinColours _colour)
    {
        NPCIdentity identity = GenerateRandom(_type);
        identity.SkinColour = _colour;

        Gradient grad = SkinTypes[0].SkinGradient;
        for (int z = 0; z < SkinTypes.Length; z++)
            if (SkinTypes[z].Colour == _colour)
                grad = SkinTypes[z].SkinGradient;

        identity.Skin = grad.Evaluate(Random.Range(0f, 1f));

        return identity;
    }

    // main npc-generating method
    public GameObject CreateNPC (string _name, NPCType _type, SkinColours _colour)
    {
        GameObject result = new GameObject();
        result.transform.parent = ParentContainer;
        result.name = _name;
        result.transform.position = Vector3.zero;
        result.transform.rotation = Quaternion.identity;

        // movement and main behaviour scripts
        result.AddComponent<NPCMovements>();
        result.AddComponent<NPCMain>();

        GameObject npcContainer = new GameObject();
        npcContainer.transform.parent = result.transform;
        npcContainer.name = "NPC Container";
        RandomNPC rnd = npcContainer.AddComponent<RandomNPC>();
        rnd.Initiate(GenerateRandom(_type, _colour));
        npcContainer.transform.localPosition = Vector3.zero;

        return result;
    }

    // npc-generating method with random skin colour
    public GameObject CreateNPC (string _name, NPCType _type)
    {
        SkinColours rsc = GenerateRandomSkinColour(out Color _dummy);
        return CreateNPC(_name, _type, rsc);
    }
}
