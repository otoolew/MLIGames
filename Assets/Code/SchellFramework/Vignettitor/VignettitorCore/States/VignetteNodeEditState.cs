//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using SG.Vignettitor.Graph.States;
using UnityEngine;

namespace SG.Vignettitor.VignettitorCore.States
{
    public class VignetteNodeEditState : EditNodeState
    {
        private readonly Vignettitor vignettitor;
        public VignetteNodeEditState(Vignettitor renderer)
            : base(renderer)
        {
            vignettitor = editor as Vignettitor;
        }

        public override void CommandDraw()
        {
            base.CommandDraw();
            if (GUILayout.Button("Hardwire"))
            {
                vignettitor.HardwirePath(vignettitor.allNodes[selection.AllSelected[0]]);
            }

            if (GUILayout.Button("Stop Editting"))
            {
                vignettitor.EnterSelectedState();
            }
        }
    }
}
