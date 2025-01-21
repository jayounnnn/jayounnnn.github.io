using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//VolumeComponentMenuForRenderPipeline
//Allows you to add commands to the Add Override popup menu on Volumes and
//specify for which render pipelines will be supported
[Serializable, VolumeComponentMenuForRenderPipeline("Custom/B&W Post-Processing", typeof(UniversalRenderPipeline))]
public class BlackAndWhitePostProcess : VolumeComponent, IPostProcessComponent
{
    public FloatParameter blendIntensity = new FloatParameter(1.0f);

    public bool IsActive() => true;
    public bool IsTileCompatible() => true;
}