using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "NewTrailConfig", menuName = "ScriptableObject/TrailConfig")]
public class TrailConfig : ScriptableObject
{
    public float time = 5;
    public float minVertDistance = 0.1f;
    public AnimationCurve width;
    public Gradient color;
    public Material material;

    public int cornerVerices = 0;
    public int endCapVerices = 0;

    public ShadowCastingMode castShadow = ShadowCastingMode.Off;
    //public bool lightProbes = false;
}
