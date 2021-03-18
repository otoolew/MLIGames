// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 5/12/2016 1:46:45 PM
// ------------------------------------------------------------------------------

using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Extension methods for Coroutines. It is considered best practice to always have a reference available
    /// to any coroutines being run so they can be stopped by reference, rather than by string name.
    /// </summary>
    public static class SgCoroutine
    {
        public static Coroutine ReplaceCoroutine([NotNull] this MonoBehaviour mb, [CanBeNull] ref Coroutine toHalt, [NotNull] IEnumerator toStart)
        {
            if (toHalt != null)
                mb.StopCoroutine(toHalt);
            toHalt = mb.StartCoroutine(toStart);
            return toHalt;
        }

        public static void HaltCoroutine([NotNull] this MonoBehaviour mb, [CanBeNull] ref Coroutine toHalt)
        {
            if (toHalt == null)
                return;
            mb.StopCoroutine(toHalt);
            toHalt = null;
        }
    }
}
