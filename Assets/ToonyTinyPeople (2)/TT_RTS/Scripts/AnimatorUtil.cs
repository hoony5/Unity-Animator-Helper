using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public static class StringUtil
{
    public static string StartWithLowerChar(this string value)
    {
        Span<char> ROValue = new Span<char>(value.ToCharArray());

        ROValue[0] = char.ToLower(ROValue[0]);

        return ROValue.ToString();
    }
}
public class AnimatorUtil : MonoBehaviour
{
    // Animator Transition Setting
    [Space(20)] public List<ParameterData> ParameterDatas;
    [Space(20)] public List<AnimatorSettings> SettingsList;

    // parameter Info
    public string parameterResetSnippet;
    public string parameterNameSnippet;
    public List<string> triggers = new List<string>(32);
    public List<string> booleans = new List<string>(32);

    private void DeleteParameters()
    {
        foreach (AnimatorSettings settings in SettingsList)
        { 
            foreach (AnimatorController controller in settings.controllers)
            {
                for (int i = 0; i < controller.parameters.Length; i++)
                    controller.RemoveParameter(i);
            }
        }
    }

    private void AddParameters(ParameterData parameterData)
    {
        foreach (AnimatorSettings settings in SettingsList)
        {
            foreach (AnimatorController controller in settings.controllers)
            {
                bool exist = Array.Exists(controller.parameters, item => item.name == parameterData.parameterName);
                    
                if(exist) continue;
                controller.AddParameter(parameterData.parameterName, parameterData.parameterType);
            }
        }
    }
    public void SetParameters()
    {
        DeleteParameters();
        
        foreach (ParameterData parameterData in ParameterDatas)
        {
            AddParameters(parameterData);
        }
    }

    private void SaveParametersForWriteSnippet()
    {
        triggers.Clear();
        booleans.Clear();

        foreach (ParameterData parameterData in ParameterDatas)
        {
            if (parameterData.parameterType is AnimatorControllerParameterType.Trigger &&
                !triggers.Contains(parameterData.parameterName))
                triggers.Add(parameterData.parameterName);

            if (parameterData.parameterType is AnimatorControllerParameterType.Bool &&
                !booleans.Contains(parameterData.parameterName))
                booleans.Add(parameterData.parameterName);
        }
    }
    public string WriteSnippetSetParameter()
    {
        SaveParametersForWriteSnippet();

        string ret = string.Empty;
        ret = "// name\n";
        foreach (string boolean in booleans)
        {
            ret += $" string _{boolean.StartWithLowerChar()} = \"{boolean}\";\n";
        }
        foreach (string trigger in triggers)
        {
            ret += $" string _{trigger.StartWithLowerChar()} = \"{trigger}\";\n";
        }
        ret += "\n";
        return ret;
    }
    public string WriteSnippetResetParameterMethod()
    {
        string ret = string.Empty;
        ret += "//Method\n";
        ret += "private void ResetParmeters()\n{\n\t";
        foreach (string boolean in booleans)
        {
            ret += $"animator.SetBool(_{boolean.StartWithLowerChar()}, false);\n\t";
        }
        ret += "\n\t";
        for (int index = 0; index < triggers.Count; index++)
        {
            string trigger = triggers[index];
            ret += $"animator.ResetTrigger(_{trigger.StartWithLowerChar()});";
            if(index != triggers.Count - 1)
                ret += "\n\t";
        }
        ret += "\n}";

        return ret;
    }
    public void SetTransition()
    {
       SetTransitions(0);
    }

    private BlendTree CreateBlendTree(AnimatorController controller,AnimatorStateMachine stateMachine, AnimatorSettings settings)
    {
        AnimatorState blendTreeInController =
            controller.CreateBlendTreeInController(settings.stateNamePart, out BlendTree tree);

        tree = SetBlendTreeParameters(tree, settings);

        // 축 맵핑 설정
        tree = MappingParameters(tree, settings);
       
        // 속도 제어 설정
        blendTreeInController = SetSpeedParameter(blendTreeInController, settings);
        
        // 블렌드 트리 트랜지션 설정 값 연동
        CombineTransition(stateMachine, blendTreeInController, settings.transitionData,
            settings.isAnyState);

        return tree;
    }

    private BlendTree SetBlendTreeParameters(BlendTree tree,AnimatorSettings settings)
    {
        tree.name = settings.stateNamePart;
        tree.blendType = settings.blendTreeData.blendTreeType;

        // 블렌드 트리 값 설정
        if (settings.blendTreeData.blendTreeType is BlendTreeType.Simple1D)
            tree.blendParameter = settings.blendTreeData.parameterAxisX;
        if (settings.blendTreeData.blendTreeType is BlendTreeType.FreeformCartesian2D
            or BlendTreeType.FreeformDirectional2D or BlendTreeType.SimpleDirectional2D)
            tree.blendParameterY = settings.blendTreeData.parameterAxisY;

        return tree;
    }

    private BlendTree MappingParameters(BlendTree tree,AnimatorSettings settings)
    {
        if (settings.blendTreeData.motions.Length > 0)
        {
            tree.useAutomaticThresholds = false;
            foreach (BlendTreeClips info in settings.blendTreeData.motions)
            {
                if (settings.blendTreeData.blendTreeType is BlendTreeType.Simple1D)
                    tree.AddChild(info.clip, info.thresholds.x);
                else
                    tree.AddChild(info.clip, info.thresholds);
            }
        }

        return tree;
    }

    private AnimatorState SetSpeedParameter(AnimatorState blendTreeInController, AnimatorSettings settings)
    {
        
        if (string.IsNullOrEmpty(settings.speedParameterName))
        {
            blendTreeInController.speedParameterActive = settings.hasSpeedParameter;
            blendTreeInController.speedParameter = settings.speedParameterName;
        }

        return blendTreeInController;
    }

    private void RemoveAllTransitions(int layerIndex = 0)
    {
        foreach (AnimatorSettings settings in SettingsList)
        {
            RemoveAllTransitionsInternal(layerIndex, settings);
        }
    }

    private void RemoveAllTransitionsInternal(int layerIndex, AnimatorSettings settings)
    {
        foreach (AnimatorController controller in settings.controllers)
        {
          
            AnimatorStateMachine stateMachine = controller.layers[layerIndex].stateMachine;
            RemoveAnyStateTransitions(stateMachine);
            RemoveNormalStateTransitions(stateMachine);
        }
    }

    private void RemoveAnyStateTransitions(AnimatorStateMachine stateMachine)
    {
        // remove any transition
        foreach (AnimatorStateTransition transition in stateMachine.anyStateTransitions)
        {
            stateMachine.RemoveAnyStateTransition(transition);
        }
    }

    private void RemoveNormalStateTransitions(AnimatorStateMachine stateMachine)
    {
        // other transition
        foreach (ChildAnimatorState state in stateMachine.states)
        {
            foreach (AnimatorStateTransition transition in state.state.transitions)
            {
                state.state.RemoveTransition(transition);
            }
        }
    }
    public void SetTransitions(int layerIndex = 0)
    {
        foreach (AnimatorSettings settings in SettingsList)
        {
            foreach (AnimatorController controller in settings.controllers)
            {
                AnimatorStateMachine stateMachine = controller.layers[layerIndex].stateMachine;
                
                bool existsBlendTree = Array.Exists(controller.layers[0].stateMachine.states,
                    i => i.state.name.Contains(settings.stateNamePart));
                if (settings.isBlendTree && !existsBlendTree)
                {
                   BlendTree tree = CreateBlendTree(controller, stateMachine, settings);
                }

                AnimatorState current = GetAnimatorState(stateMachine, settings.stateNamePart);
                
                if (current is null) continue;

                // 속도 제어
                SetSpeedParameter(settings, controller, layerIndex);

                // 스테이트 머신 콜백
                AddBehaviour(controller, layerIndex);

                // 일반 스테이트 트랜지션 설정 값 연동
                CombineTransition(stateMachine, current, settings.transitionData, settings.isAnyState);
            }
        }
    }

    private void SetSpeedParameter(AnimatorSettings settings, AnimatorController controller, int layerIndex = 0)
    {
        foreach (ChildAnimatorState state in controller.layers[layerIndex].stateMachine.states)
        {
            if (!state.state.name.Contains(settings.stateNamePart))continue; 
            
            state.state.speedParameterActive = settings.hasSpeedParameter;
            state.state.speedParameter = settings.hasSpeedParameter ? settings.speedParameterName : "";
        }
    }
    private void AddBehaviour(AnimatorController controller, int layerIndex = 0)
    {
        foreach (ChildAnimatorState state in controller.layers[layerIndex].stateMachine.states)
        {
            if (state.state.transitions.Length == 0 || state.state.behaviours.Length != 0)continue;

            state.state.AddStateMachineBehaviour(typeof(StateMachineListener));
        }
        
        foreach (AnimatorStateTransition transition in controller.layers[layerIndex].stateMachine.anyStateTransitions)
        {
            if (transition.destinationState.behaviours.Length != 0)continue;

            transition.destinationState.AddStateMachineBehaviour(typeof(StateMachineListener));
        }
    }

    private void CombineTransition(AnimatorStateMachine stateMachine,AnimatorState state, AnimatorUtilData transitionData, bool isAnyState)
    {
        foreach (TransitionSetting setting in transitionData.settings)
        {
            // 목적지 가져오기
            AnimatorState next = GetAnimatorState(stateMachine, setting.targetStateNamePart);

            // exit일 경우
            if(setting.targetStateNamePart.Contains("exit"))
            {
                state.AddExitTransition();
            }

            if(next is not null)
            {
                // AnyState를 사용하는가 아닌가. 
                AnimatorStateTransition stateTransition = isAnyState && state.name.Contains(setting.targetStateNamePart)
                    ? stateMachine.AddAnyStateTransition(state)
                    : state.AddTransition(next);
                // 조건 설정
                stateTransition.hasExitTime = setting.hasExitTime;
                stateTransition.duration = setting.transitionDuration;
                stateTransition.exitTime = setting.exitTime;
                AddConditions(stateTransition, setting);
            }
        }
    }
    
    private AnimatorState GetAnimatorState(AnimatorStateMachine stateMachine, string targetName)
    {
        foreach (ChildAnimatorState childAnimatorClip in stateMachine.states)
        {
            if (childAnimatorClip.state.name.Contains(targetName)) return childAnimatorClip.state;
        }
        return null;
    }
    
    private void AddConditions(AnimatorStateTransition current, TransitionSetting setting)
    {
        foreach (TransitionConditionSetting condition in setting.ConditionSettings)
        {
            current.AddCondition(condition.mode, condition.parameterBase.threshold, condition.parameterBase.name);
        }
    }
}
