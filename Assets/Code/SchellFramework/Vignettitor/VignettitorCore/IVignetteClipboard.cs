//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/09/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Vignettitor.Graph;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.VignettitorCore
{
    /// <summary>
    /// Defines the data generated from a paste operation
    /// </summary>
    public class VignettePasteResult
    {
        /// <summary>New nodes that were pasted.</summary>
        public List<VignetteNode> Nodes = new List<VignetteNode>();

        /// <summary>Target positions of newly pasted nodes.</summary>
        public List<Vector2> Positions = new List<Vector2>();

        /// <summary> Annotations that were created in the paste. </summary>
        public List<Annotation> Annotations = new List<Annotation>();
    }

    public interface IVignetteClipboard
    {
        void DrawClipboardPreview();

        bool HasPasteData();

        /// <summary>Copy the given nodes to the clipboard.</summary>
        /// <param name="nodes">Nodes to copy.</param>
        /// <param name="positions">Positions of the nodes to copy.</param>
        /// <param name="annotations">Annotations to copy.</param>
        /// <param name="dataController">
        /// The data controller that may be used to access vignette context.
        /// </param>
        void Copy(List<VignetteNode> nodes, List<Vector2> positions, List<Annotation> annotations, VignettitorDataController dataController);

        /// <summary>
        /// Paste the nodes from the last copy operation, saving them into the 
        /// specified data controller.
        /// </summary>
        /// <param name="dataController">
        /// Data controller used to save the nodes that are being pasted.
        /// </param>
        VignettePasteResult Paste(VignettitorDataController dataController);

        /// <summary>
        /// Duplicate the given nodes into the given data controller.
        /// </summary>
        /// <param name="nodes">Nodes to duplicate.</param>
        /// <param name="positions">Positions of the nodes to copy.</param>
        /// <param name="annotations">Annotations to duplicate.</param>
        /// <param name="dataController">
        /// Data controller used to save the nodes that are being duplicated.
        /// </param>        
        VignettePasteResult Duplicate(List<VignetteNode> nodes, List<Vector2> positions, List<Annotation> annotations, VignettitorDataController dataController);
    }
}
