using UnityEngine;

[CreateAssetMenu(fileName = "newAnimatorUtilData", menuName = "ScriptableObject/Animator/AnimatorUtilData", order = 0)]
public class AnimatorUtilData : ScriptableObject
{
    public TransitionSetting[] settings;
}