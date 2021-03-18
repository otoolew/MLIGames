//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/03/2014
//-----------------------------------------------------------------------------

namespace SG.Vignettitor.Graph.NodeViews
{
    /// <summary>
    /// Base Node View for views that display editable versions of a node. 
    /// This is typically accessed by double clicking on a node. Implementing 
    /// Nodes may be in an editor assemble and access EditorGUI functionality.
    /// </summary>
    public abstract class NodeEditView : NodeView
    {}
}
