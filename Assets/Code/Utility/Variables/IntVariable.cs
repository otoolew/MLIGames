using UnityEngine;

[CreateAssetMenu(menuName = "Game/AI/Variable/Int", fileName = "newIntVar")]
public class IntVariable : ScriptableObject, IVariable<int>
{
    [SerializeField] private string varName;
    public string VariableName { get => varName; set => varName = value; }

    [SerializeField] private int value;
    public int Value { get => value; set => this.value = value; }

    public void SetValue(int value)
    {
        this.value = value;
    }
}
