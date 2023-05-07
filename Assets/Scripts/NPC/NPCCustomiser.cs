using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MaterialLocation
{
    public SkinnedMeshRenderer Renderer;
    public int Index;
}

public class NPCCustomiser : MonoBehaviour
{
    public MaterialLocation Hair;
    public MaterialLocation Skin;
    public MaterialLocation Lips;
    public MaterialLocation Outfit1;
    public MaterialLocation Outfit2;
    public MaterialLocation Shoes;
    [HideInInspector]
    public Color SetSkinClr;
    public Transform ChildAttachPt;
    public SkinnedMeshRenderer BlendShapeContainer;

    public void AssignColourToRenderer (Color _clr, MaterialLocation _location)
    {
        Material[] mats;

        mats = _location.Renderer.materials;
        mats[_location.Index].color = _clr;
        _location.Renderer.materials = mats;
    }

    private Color GetLipsColour (Color _skin)
    {
        Color.RGBToHSV(_skin, out float h, out float s, out float v);
        h -= 0.025f;
        if (h < 0) h += 1;
        s += 0.05f;
        v -= 0.05f;

        Color lipsClr = Color.HSVToRGB (h, s, v);
        lipsClr.a = 1;
        
        return lipsClr;
    }

    public void ApplyIdentity (NPCIdentity _identity)
    {
        Color lipsClr = GetLipsColour(_identity.Skin);

        AssignColourToRenderer(_identity.Hair, Hair);
        AssignColourToRenderer(_identity.Skin, Skin);
        AssignColourToRenderer(lipsClr, Lips);
        AssignColourToRenderer(_identity.Outfit1, Outfit1);
        AssignColourToRenderer(_identity.Outfit2, Outfit2);
        AssignColourToRenderer(_identity.Shoes, Shoes);
        SetSkinClr = _identity.Skin;
    }
}
