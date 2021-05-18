using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public enum effectType
{ 
Twirl =0,Wave =1
}
[Serializable]
public sealed class EffectTypeParameter : VolumeParameter<effectType> { };

[VolumeComponentMenu("Limitless Glitch/TwirlEffefctURP")]
public class LimitlessTwirlEffefctURP : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
    public EffectTypeParameter m_EffectType = new EffectTypeParameter();
    [Tooltip("seed x")]
    public ClampedFloatParameter amount = new ClampedFloatParameter(127.1f, -2f, 200f);
        [Tooltip("seed x")]
    public ClampedFloatParameter speed = new ClampedFloatParameter(127.1f, -2f, 200f);
        [Tooltip("seed x")]
    public ClampedFloatParameter verticleDensity = new ClampedFloatParameter(127.1f, -2f, 200f);       
    [Tooltip("seed x")]
    public ClampedFloatParameter effectRadius = new ClampedFloatParameter(127.1f, -2f, 200f);
    public Material effectMaterial;

    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}