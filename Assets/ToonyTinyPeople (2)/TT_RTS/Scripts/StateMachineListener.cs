using UnityEngine;

public class StateMachineListener : StateMachineBehaviour
{
    private AnimatorListener _listener;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.TryGetComponent(out AnimatorListener comp)) return;
        _listener ??= comp;
        
        _listener.OnStateEnter?.Invoke();

    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ReferenceEquals(_listener, null)) return;
        
        _listener.OnStateUpdate?.Invoke();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ReferenceEquals(_listener, null)) return;
        
        _listener.OnStateExit?.Invoke();
    }
}
