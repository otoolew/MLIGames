using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newAIBoard", menuName = "Game/AI/AIBoard")]
public class AIBoard : ScriptableObject
{
    [SerializeField] private AIController assignedController;
    public AIController AssignedController { get => assignedController; set => assignedController = value; }

    [SerializeField] private string boardTitle;
    public string BoardTitle { get => boardTitle; set => boardTitle = value; }

    [SerializeField] private string boardDescription;
    public string BoardDescription { get => boardDescription; set => boardDescription = value; }

    [SerializeField] private Dictionary<string, EntryVariable> variables;
    public Dictionary<string, EntryVariable> Variables { get => variables; set => variables = value; }

    [SerializeField] private List<string> variableList;
    public List<string> VariableList { get => variableList; set => variableList = value; }

    [SerializeField] private List<TaskNode> taskList;
    public List<TaskNode> TaskList { get => taskList; set => taskList = value; }

    public void Init(AIController controller)
    {
        assignedController = controller;
        foreach (TaskNode taskNode in TaskList)
        {
            foreach (EntryVariable variable in taskNode.VariableList)
            {
                AddEntryVariable(variable);
            }
        }
    }

    private void AddEntryVariable(EntryVariable entryVariable)
    {
        if (!variables.ContainsKey(entryVariable.EntryName))
        {
            variables.Add(entryVariable.EntryName, entryVariable);
        }
    }
    private void RemoveEntryVariable(EntryVariable entryVariable)
    {
        if (variables.ContainsKey(entryVariable.EntryName))
        {
            variables.Remove(entryVariable.EntryName);
        }
    }

    #region Inspector
    //public void OnBeforeSerialize()
    //{
    //    variableList.Clear();

    //    foreach (var kvp in variables)
    //    {
    //        variableList.Add(kvp.Value.ToString());
    //    }
    //}
    //public void OnAfterDeserialize()
    //{
    //    //StateStack = new Stack<T>();

    //    for (int i = variableList.Count; i >= 0; i--)
    //        StateStack.Push(StateList[i]);
    //}
    #endregion
}
