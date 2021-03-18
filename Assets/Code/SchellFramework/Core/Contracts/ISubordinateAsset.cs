// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//  Contact: Ryan Hipple
//  Created: 10/19/2015 11:07:27 PM
// ------------------------------------------------------------------------------

namespace SG.Core.Contracts
{
    /// <summary>
    /// Using ISubordinateAsset indicates that a data class instance is a 
    /// subordinate of some other data, called the superior data. 
    /// 
    /// Subordinate asset data is serialized to disk as a sub-asset of the 
    /// superior asset. If the superior asset is already a sub-asset, they
    /// will appear as siblings in the project view.
    /// 
    /// If the superior data is cloned, the subordinate asset will also be 
    /// cloned (and the superior data clone will point to the new subordinate 
    /// asset clone). If the superior data is deleted, the subordinate asset 
    /// is deleted.
    /// 
    /// An example usage is LocalizedText. Whenever duplicating an asset with 
    /// LocalizedText reference, it is important to duplicate the LocalizedText
    /// asset as well to avoid linking multiple things to the same text 
    /// instance. Similarly, when deleting an object referencing LocalizedText,
    /// the LocalizedText will also be deleted.
    /// 
    /// Any classs implementing ISubordinateAsset should inherit from 
    /// ScriptableObject since it is meant to be serialized to disk as its 
    /// own asset.
    /// </summary>
    public interface ISubordinateAsset
    {
        /// <summary>
        /// Make a deep copy of the asset for serialization.
        /// </summary>
        /// <returns>A copy of the asset.</returns>
        ISubordinateAsset DeepCopy();
    }
}
