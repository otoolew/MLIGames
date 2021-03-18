// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Max Golden
//
//  Created: 12/9/2016 10:39 AM
// -----------------------------------------------------------------------------

using UnityEngine;

namespace SG.Core.Templating
{
    /// <summary>
    /// ScriptableObject allowing manipulating templates as .asset files
    /// </summary>
    [CreateAssetMenu(fileName = "NewTextTemplate.asset", menuName = "Data/Text Template")]
    public class TextTemplate : ScriptableObject
    {
        public string text = "";

        /// <summary>
        /// Returns a new template engine initialized to this file's text
        /// </summary>
        public TemplateEngine GetTemplateEngine()
        {
            return new TemplateEngine(this.text);
        }
    }
}
