using UnityEngine;

[CreateAssetMenu(menuName = "Game/AI/Task/Variable/Vector", fileName = "newVectorVar")]
public class VectorVariable : ScriptableObject, IVariable<Vector3>
{
    [SerializeField] private string varName;
    public string VariableName { get => varName; set => varName = value; }

    [SerializeField] private Vector3 value;
    public Vector3 Value { get => value; set => this.value = value; }

    public void SetValue(Vector3 value)
    {
        this.value = value;
    }
}
