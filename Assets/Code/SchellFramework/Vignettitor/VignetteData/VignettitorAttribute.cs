using System;

namespace SG.Vignettitor.VignetteData
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class VignettitorAttribute : Attribute
    {
        public Type vignetteGraphType;

        public VignettitorAttribute(Type vignetteGraphType)
        {   
            this.vignetteGraphType = vignetteGraphType;
        }

        public static bool IsVignettitorFor(Type vignettitorType, Type graphType)
        {
            if (!typeof(VignetteGraph).IsAssignableFrom(graphType))
                throw new ArgumentException("The type specified by 'graphType' must be a subtype of VignetteGraph.");

            object[] viewAttributes =
                vignettitorType.GetCustomAttributes(typeof(VignettitorAttribute), true);
            for (int a = 0; a < viewAttributes.Length; a++)
            {
                VignettitorAttribute va = viewAttributes[a] as VignettitorAttribute;

                if (!typeof(VignetteGraph).IsAssignableFrom(va.vignetteGraphType))
                    throw new ArgumentException("VignettitorAttribute used on class " + vignettitorType + " to specify a type (" + va.vignetteGraphType + ") that does not inherit from VignetteGraph.");

                if (va.vignetteGraphType == graphType)
	            {
                    return true;
	            }
            }
            return false;
        }
    }
}
