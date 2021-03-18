using SG.Vignettitor.Graph.States;
using UnityEngine;

//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

namespace SG.Vignettitor.VignettitorCore.States
{
    public class VignetteConnectionEditState : ConnectionEditState
    {
        public VignetteConnectionEditState(Vignettitor editor, int node, int outputIndex)
            : base(editor, node, outputIndex)
        {}

        public override void CommandDraw()
        {
            base.CommandDraw();
            if (GUILayout.Button("Toggle Hardwire"))
            {
                Vignettitor vignettitor = editor as Vignettitor;
                if (vignettitor != null)
                    vignettitor.ToggleHardwire(Node, OutputIndex);
            }
        }
    }
}
