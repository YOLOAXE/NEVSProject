using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapInfo
{
    [SerializeField] private GameObject[] allObjects = null;
    [Header("effect")]
    [SerializeField] private bool ChangeSkybox = false;
    [SerializeField] private Material skybox1 = null;
    [SerializeField] private Material skybox2 = null;
    [SerializeField] private Terrain t = null;

    public void LoadMap(bool state)
    {
        foreach(GameObject o in this.allObjects)
        {
            o.SetActive(state);
        }
        if (ChangeSkybox)
        {
            if (state)
            {
                RenderSettings.skybox = this.skybox1;
                Terrain.activeTerrain.detailObjectDistance = 80;
            }
            else
            {
                RenderSettings.skybox = this.skybox2;
                Terrain.activeTerrain.detailObjectDistance = 0;
            }
        }
    }

    public bool GetChangeS()
    {
        return this.ChangeSkybox;
    }
}
public class LoadMap : MonoBehaviour
{
    [SerializeField] private MapInfo[] mi = null;
    [SerializeField] private PostProcessVolume PPVE = null;
    [SerializeField] private PostProcessVolume PPVI = null;

    public void map(bool state,int indice)
    {
        mi[indice].LoadMap(state);
        if (state && mi[indice].GetChangeS())
        {
            StartCoroutine(AplyPPV(5.0f, PPVE));
        }
        else
        {
            StartCoroutine(AplyPPV(5.0f, PPVI));
        }
    }

    IEnumerator AplyPPV(float time, PostProcessVolume p)
    {
        float varTime = 0.0f;
        while (varTime < time)
        {
            varTime += Time.deltaTime;
            p.weight = (varTime / time);
            yield return null;
        }

    }
}
