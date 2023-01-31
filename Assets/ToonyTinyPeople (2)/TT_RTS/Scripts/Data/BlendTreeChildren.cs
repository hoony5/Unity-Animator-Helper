using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "newBlendTreeChildren", menuName = "ScriptableObject/Animator/BlendTreeChildren", order = 0)]
public class BlendTreeChildren : ScriptableObject
{
    public BlendTreeType blendTreeType;
    public string parameterAxisX;
    public string parameterAxisY;
    public BlendTreeClips[] motions;

}

[System.Serializable]
public class BlendTreeClips
{
    public AnimationClip clip;
    public Vector2 thresholds;
}