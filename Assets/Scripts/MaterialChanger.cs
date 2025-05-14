using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [Header("대상 오브젝트")]
    public List<Renderer> targetRenderers;

    public void SetMaterial(Material mat)
    {
        if (targetRenderers != null && mat != null)
        {
            foreach(Renderer renderer in targetRenderers){
                renderer.material = mat;
            }
        }
    }
}
