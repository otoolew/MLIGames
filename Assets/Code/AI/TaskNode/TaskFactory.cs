using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskFactory
{
    #region Variables
    public static IntVariable CreateIntVariable(string entryName)
    {
        IntVariable variable = ScriptableObject.CreateInstance<IntVariable>();
        variable.name = entryName;
        return variable;
    }
    public static FloatVariable CreateFloatVariable(string entryName)
    {
        FloatVariable variable = ScriptableObject.CreateInstance<FloatVariable>();
        variable.name = entryName;
        return variable;
    }
    public static BoolVariable CreateBoolEntry(string entryName)
    {
        BoolVariable variable = ScriptableObject.CreateInstance<BoolVariable>();
        variable.name = entryName;
        return variable;
    }
    public static VectorVariable CreateVectorEntry(string entryName)
    {
        VectorVariable variable = ScriptableObject.CreateInstance<VectorVariable>();
        variable.name = entryName;
        return variable;
    }
    #endregion

    #region Tasks

    #endregion
}
