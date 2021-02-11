using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedDelegateStack : StackCollection<ScriptedDelegate>
{
    [SerializeField] private Stack<ScriptedDelegate> stateStack;
    public override Stack<ScriptedDelegate> StateStack => stateStack;

    [SerializeField] private List<ScriptedDelegate> stateList;
    public override List<ScriptedDelegate> StateList => stateList;

    private void OnEnable()
    {
        stateStack = new Stack<ScriptedDelegate>();
        stateList = new List<ScriptedDelegate>();
    }
    public static ScriptedDelegateStack Create()
    {
        return CreateInstance<ScriptedDelegateStack>();
    }


    #region Editor
    public override void OnBeforeSerialize()
    {
        StateList.Clear();

        foreach (var kvp in StateStack)
        {
            StateList.Add(kvp);
        }
    }
    public override void OnAfterDeserialize()
    {
        stateStack = new Stack<ScriptedDelegate>();

        for (int i = StateList.Count; i >= 0; i--)
            stateStack.Push(StateList[i]);
    }
    #endregion
}
