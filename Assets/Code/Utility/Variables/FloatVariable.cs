using UnityEngine;

[CreateAssetMenu(menuName = "Game/AI/Variable/Float", fileName = "newFloatVar")]
public class FloatVariable : ScriptableObject, IVariable<float>
{
    [SerializeField] private string varName;
    public string VariableName { get => varName; set => varName = value; }

    [SerializeField] private float value;
    public float Value { get => value; set => this.value = value; }

    public void SetValue(float value)
    {
        this.value = value;
    }
}
