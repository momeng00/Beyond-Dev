using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class ParallaxSkybox : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField]
    private ParallaxObject[] parallaxes
        = new ParallaxObject[]
        {
            new ParallaxObject(){ name = "_OffsetBack",offset = 0.001f,target = Vector3.zero },
            new ParallaxObject(){ name = "_OffsetBetween",offset = 0.005f,target = Vector3.zero },
            new ParallaxObject(){ name = "_OffsetFore",offset = 0.009f,target = Vector3.zero }
        };
    private void Reset()
    {
        followTarget = Camera.main.transform;
        parallaxes = new ParallaxObject[]
        {
            new ParallaxObject(){ name = "_OffsetBack",offset = 0.001f,target = Vector3.zero },
            new ParallaxObject(){ name = "_OffsetBetween",offset = 0.005f,target = Vector3.zero },
            new ParallaxObject(){ name = "_OffsetFore",offset = 0.009f,target = Vector3.zero }
        };
    }
    private float offset;
    private void Update()
    {
        for(int i = 0; i < parallaxes.Length;i++)
        {
            offset = new Vector3((followTarget.position - parallaxes[i].target).x * parallaxes[i].offset, 0).x;
            parallaxes[i].target = followTarget.position;
            if(!Mathf.Approximately(offset,0))
            {
                RenderSettings.skybox.SetFloat(parallaxes[i].name, RenderSettings.skybox.GetFloat(parallaxes[i].name) + offset);
            }
        }
    }
    [System.Serializable]
    public struct ParallaxObject
    {
        public string name;
        public float offset;
        public Vector3 target;
    }
}
