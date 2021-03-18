using UnityEngine;

[CreateAssetMenu(menuName = "Game/AI/Variable/Bool", fileName = "newBoolVar")]
public class BoolVariable : ScriptableObject, IVariable<bool>
{
    [SerializeField] private string varName;
    public string VariableName { get => varName; set => varName = value; }

    [SerializeField] private bool value;
    public bool Value { get => value; set => this.value = value; }

    public void SetValue(bool value)
    {
        this.value = value;
    }
}
