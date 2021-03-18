using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/AI/Variable/String", fileName = "newStringVar")]

public class StringVariable : ScriptableObject, IVariable<string>
{
    [SerializeField] private string varName;
    public string VariableName { get => varName; set => varName = value; }

    [SerializeField] private string value;
    public string Value { get => value; set => this.value = value; }

    public void SetValue(string value)
    {
        this.value = value;
    }
}
