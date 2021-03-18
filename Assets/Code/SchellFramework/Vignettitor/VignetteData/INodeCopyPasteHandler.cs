// ----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   01/23/2017
// ----------------------------------------------------------------------------

using SG.Vignettitor.VignettitorCore;

namespace SG.Vignettitor.VignetteData
{
    /// <summary>
    /// Used on a VignetteNode if that node wants to do any special handling 
    /// when it is copied or pasted.
    /// </summary>
    public interface INodeCopyPasteHandler
    {
        /// <summary>
        /// Called on the source node before a clone is made. Any data changes 
        /// will be present on the source and the copy in the clipboard.
        /// </summary>
        void OnWillCopy();

        /// <summary>
        /// Called on the copy of the source after it was created. Changes here 
        /// will not affect the source.
        /// </summary>
        void OnDidCopy();

        /// <summary>
        /// Called after a node is pasted.
        /// </summary>
        /// <param name="source">
        /// The data controller that the source node was copied from.
        /// </param>
        /// <param name="destination">
        /// The data controller that the copy is being placed into.
        /// </param>
        void OnDidPaste(VignettitorDataController source, VignettitorDataController destination);
    }
}