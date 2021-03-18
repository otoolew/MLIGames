using UnityEngine;

namespace SG.Dynamics
{
    /// <summary>
    /// Basic ScriptableObject holding a single DynamicValue field.
    /// </summary>
    public class DynamicContainer : ScriptableObject
    {
        [SerializeField]
        private DynamicValue _container;
    }
}
