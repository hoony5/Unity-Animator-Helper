
[System.Serializable]
public class TransitionSetting
{
    public string targetStateNamePart;
    public TransitionConditionSetting[] ConditionSettings;
    public bool hasExitTime;
    public float exitTime;
    public float transitionDuration;
}