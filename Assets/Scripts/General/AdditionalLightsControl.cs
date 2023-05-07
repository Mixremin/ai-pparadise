using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalLightsControl : MonoBehaviour
{
    void Start()
    {
        GameObject[] lights = GameObject.FindGameObjectsWithTag("Additional Light");

        for (int z = 0; z < lights.Length; z++)
            lights[z].GetComponent<Light>().enabled = false;
    }
}
