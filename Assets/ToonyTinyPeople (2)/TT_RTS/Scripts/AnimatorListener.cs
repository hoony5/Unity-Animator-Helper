using UnityEngine;
using UnityEngine.Events;

public class AnimatorListener : MonoBehaviour
{
    public UnityEvent OnStateEnter;
    public UnityEvent OnStateUpdate;
    public UnityEvent OnStateExit;
}