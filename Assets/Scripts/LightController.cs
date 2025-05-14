using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private List<Light> lights;
    private void Start()
    {
        lights = new List<Light>();
        int lightnum = transform.childCount;
        for(int i = 0 ; i<lightnum ; i++){
            lights.Add(transform.GetChild(i).gameObject.GetComponent<Light>());
        }
    }

    public void SetLight(float mul)
    {
        foreach(Light light in lights)
        {
            light.intensity = light.intensity*mul;
        }
    }
}
