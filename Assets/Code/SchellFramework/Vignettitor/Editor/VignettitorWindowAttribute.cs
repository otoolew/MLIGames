using SG.Core;
using SG.Vignettitor.Editor;
using SG.Vignettitor.VignetteData;
using System;
using System.Reflection;
using UnityEditor;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class VignettitorWindowAttribute : Attribute
{
    public Type vignettitorType;

    public VignettitorWindowAttribute(Type vignettitorType)
    {
        this.vignettitorType = vignettitorType;
    }

    public static VignettitorWindow GetVignettitorWindow(VignetteGraph graph)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            Type[] types = AssemblyUtility.GetTypes(assemblies[i]);

            for (int t = 0; t < types.Length; t++)
            {
                VignettitorWindowAttribute[] viewAttributes =
                    types[t].GetCustomAttributes(typeof(VignettitorWindowAttribute), true)
                    as VignettitorWindowAttribute[];
                for (int a = 0; a < viewAttributes.Length; a++)
                {
                    VignettitorWindowAttribute vwa = viewAttributes[a];
                    if (VignettitorAttribute.IsVignettitorFor(vwa.vignettitorType, graph.GetType()))
                    {
                        VignettitorWindow vw = EditorWindow.GetWindow(types[t]) as VignettitorWindow;
                        if (vw != null)
                            return vw;
                        return EditorWindow.GetWindow<VignettitorWindow>();
                    }
                }
            }
        }
        return EditorWindow.GetWindow<VignettitorWindow>();
    }
}
