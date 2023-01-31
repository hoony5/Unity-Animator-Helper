using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "newAnimatorSettings", menuName = "ScriptableObject/Animator/Settings", order = 0)]
public class AnimatorSettings : ScriptableObject
{
    public string stateNamePart;
    public AnimatorController[] controllers;
    public bool isAnyState;
    public bool isBlendTree;
    public bool hasSpeedParameter;
    public string speedParameterName;
    public AnimatorUtilData transitionData;
    public BlendTreeChildren blendTreeData;
}