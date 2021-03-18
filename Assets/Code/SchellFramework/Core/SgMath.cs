//-----------------------------------------------------------------------------
//  Copyright © 2012 Schell Games, LLC. All Rights Reserved. 
//
//  Authors: Ryan Hipple, Jason Pratt
//  Date:   05/11/2012
//-----------------------------------------------------------------------------

using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Contains a collection of static methods to perform common math 
    /// operations.
    /// </summary>
    public static class SgMath
    {
        #region -- Scalar Methods ---------------------------------------------
        /// <summary>
        /// Gets an angle equivalent to the supplied angle but bound to the 
        /// range of 0 to 360. 
        /// </summary>
        /// <returns>The normalized angle.</returns>
        /// <param name='angle'>The input angle.</param>
        public static float GetNormalizedAngle(float angle)
        {
            return Modulo(angle, 360f);
        }

        /// <summary>
        /// Returns a factor to multiply current by to ease it toward target
        /// in a logarithmic fashion, such that current should be 99.9% of
        /// the way to target after duration seconds.
        /// 
        /// This is basically like doing an n% Lerp each frame, but in a
        /// frame rate independent manner.
        /// 
        /// It makes things smooth like butter.
        /// </summary>
        public static float GetSmoothApproach(float current, float target, float duration, float dt)
        {
            float remainingPortion = Mathf.Pow(0.001f, dt / duration);
            return Mathf.Lerp(current, target, 1f - remainingPortion);
        }

        /// <summary>
        /// This is like GetSmoothApproach, but does an additional lerp
        /// based on the portion argument from the smooth approach result
        /// to the final target.
        /// 
        /// This avoids an issue where GetSmoothApproach is being used over
        /// a specific duration, but the target is moving.  Normally in this
        /// situation, the result will never quite keep up with the target,
        /// causing a snap when the duration expires and the calling code
        /// just sets the value to the target.
        /// 
        /// The additional lerp prevents this snapping. A warning though, it
        /// also accelerates the rate at which the value approaches the target,
        /// causing the effective duration to be smaller than the requested
        /// duration parameter.
        /// </summary>
        public static float GetSmoothApproachWithLerp(float current, float target, float duration, float dt, float portion)
        {
            float remainingPortion = Mathf.Pow(0.001f, dt / duration);
            var smoothApproachResult = Mathf.Lerp(current, target, 1f - remainingPortion);
            return Mathf.Lerp(smoothApproachResult, target, portion);
        }

        /// <summary>
        /// Like GetSmoothApproach, but uses Mathf.LerpAngle.
        /// </summary>
        public static float GetSmoothApproachAngle(float currentAngle, float targetAngle, float duration, float dt)
        {
            float remainingPortion = Mathf.Pow(0.001f, dt / duration);
            return Mathf.LerpAngle(currentAngle, targetAngle, 1f - remainingPortion);
        }

        /// <summary>
        /// Adjusts a volume (sound pressure, or amplitude; for example a standard
        /// 0.0 to 1.0 volume parameter) given a (logarithmic) decibel offset
        /// of the sound pressure level (dB SPL).
        /// 
        /// Tips:
        /// -- +6 dB will double the volume, -6 dB will halve the volume.
        /// -- +10/-10 dB will roughly double/halve the perceived "loudness".
        /// 
        /// This is useful because our perception of loudness is more closely
        /// aligned with a logarithmic scale than a linear scale (it's a
        /// little more complicated than that).
        /// </summary>
        public static float AdjustVolume(float inputVolume, float decibelOffset)
        {
            // Calculation derived from standard sound pressure level formula:
            // dB = 20*log10( volume / reference volume )
            // http://en.wikipedia.org/wiki/Sound_pressure#Sound_pressure_level
            return inputVolume * Mathf.Pow(10f, decibelOffset / 20f);
        }

        /// <summary>
        /// Performs the canonical modulo operation, where the sign of the
        /// remainder is the same as the sign of the divisor.
        /// 
        /// The C# % operator, in contrast, performs the canonical remainder
        /// operation, where the sign of the remainder is the same as the
        /// sign of the dividend.
        /// 
        /// This version is useful when trying to wrap the index of an array.
        /// For instance, if you were implementing next and previous
        /// operations for an array, the following would work:
        /// 
        /// nextIndex = MathHelper.Modulo( index + 1, array.Length );
        /// prevIndex = MathHelper.Modulo( index - 1, array.Length );
        /// 
        /// But the following would NOT work:
        ///
        /// nextIndex = (index + 1) % array.Length;  // Okay
        /// prevIndex = (index - 1) % array.Length;  // Bad!  If index is 0, prevIndex will be -1 instead of array.Length - 1.
        /// 
        /// For more information, see http://en.wikipedia.org/wiki/Modulo_operation.
        /// </summary>
        public static int Modulo(int dividend, int divisor)
        {
            return ((dividend % divisor) + divisor) % divisor;
        }

        public static float Modulo(float dividend, float divisor)
        {
            return ((dividend % divisor) + divisor) % divisor;
        }

        /// <summary>
        /// Like Mathf.Approximately, but allows you to specify a custom
        /// margin of allowance.
        /// </summary>
        public static bool Approximately(float a, float b, float margin)
        {
            return Mathf.Abs(a - b) <= margin;
        }
        #endregion

        #region -- Vector3 Methods --------------------------------------------

        /// <summary>
        /// Creates a new Vector3 based on the x and y of a vector2 as well as 
        /// a specified z.
        /// </summary>
        /// <param name="vec2">
        /// The Vector2 that supplies the x and y component.
        /// </param>
        /// <param name="z">The z component of the result.</param>
        /// <returns>
        /// A Vector3 with an x and y component from the given Vector2 and a z 
        /// component from the specified float.
        /// </returns>
        public static Vector3 Vec2ToVec3(Vector2 vec2, float z)
        {
            return new Vector3(vec2.x, vec2.y, z);
        }

        /// <summary>
        /// Returns the result of Vector3.Angle signed based on a reference
        /// axis of rotation.
        /// 
        /// This makes the most sense if axis is parallel to the cross product
        /// of v1 and v2.  In this case, the result is positive when axis points
        /// the same direction as (v1 X v2), and negative when they point away
        /// from each other.
        /// 
        /// The method still works if axis and (v1 X v2) are not perfectly
        /// parallel, but the result becomes less and less intuitive as
        /// they become perpendicular.
        /// </summary>
        public static float GetSignedAngle(Vector3 v1, Vector3 v2, Vector3 axis)
        {
            // Get the unsigned angle.
            float angle = Vector3.Angle(v1, v2);

            // If v1 X v2 is facing away from axis, then the angle should be negative.
            if (Vector3.Dot(Vector3.Cross(v1, v2), axis) < 0f)
            {
                angle = -angle;
            }

            return angle;
        }

        /// <summary>
        /// Returns the angle to rotate v1 about axis such that v1, v2, and axis
        /// are coplanar, and v1 and v2 face the same direction when projected
        /// onto the plane defined by axis.
        /// 
        /// This lets you do things like have object 1 rotate around the up axis
        /// to "face" object 2, even if neither object 1 nor object 2 have their
        /// up axis aligned with the world's up axis.
        /// </summary>
        public static float GetAngleAroundAxis(Vector3 v1, Vector3 v2, Vector3 axis)
        {
            // Project both vectors onto the plane perpendicular to axis.
            Vector3 v1OnPlane = Vector3.Cross(Vector3.Cross(axis, v1), axis);
            Vector3 v2OnPlane = Vector3.Cross(Vector3.Cross(axis, v2), axis);

            // Return the signed angle.
            return GetSignedAngle(v1OnPlane, v2OnPlane, axis);
        }

        /// <summary>
        /// Like float GetSmoothApproach, but for Vector3s.
        /// </summary>
        public static Vector3 GetSmoothApproach(Vector3 current, Vector3 target, float duration, float dt)
        {
            float remainingPortion = Mathf.Pow(0.001f, dt / duration);
            return Vector3.Lerp(current, target, 1f - remainingPortion);
        }

        /// <summary>
        /// Like float GetSmoothApproachWithLerp, but for Vector3s.
        /// </summary>
        public static Vector3 GetSmoothApproachWithLerp(Vector3 current, Vector3 target, float duration, float dt, float portion)
        {
            float remainingPortion = Mathf.Pow(0.001f, dt / duration);
            var smoothApproachResult = Vector3.Lerp(current, target, 1f - remainingPortion);
            return Vector3.Lerp(smoothApproachResult, target, portion);
        }
        #endregion

        #region -- Vector2 Methods --------------------------------------------
        /// <summary> Returns the midpoint between two points. </summary>
        public static Vector2 GetMidPoint(Vector2 p1, Vector2 p2)
        {
            return (p1 + p2) * 0.5f;
        }

        /// <summary>
        /// Returns the signed angle between two vectors in degrees.
        /// 
        /// This method is superior to Vector2.Angle because it is signed,
        /// giving information about the direction of rotation on the
        /// cartesian plane.
        /// </summary>
        public static float GetAngle(Vector2 v1, Vector2 v2)
        {
            return Mathf.DeltaAngle(Mathf.Atan2(v1.y, v1.x) * Mathf.Rad2Deg, Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg);
        }
        #endregion

        #region -- 2D Collision -----------------------------------------------

        /// <summary>
        /// Adjusts the points of a line segment to be contained within a 
        /// rectangle. If this returns true, the start and end points will 
        /// represent the original line, constrained to the given rectangle.
        /// </summary>
        /// <param name="start">Line segment start point.</param>
        /// <param name="end">Line segment end point.</param>
        /// <param name="rect">Constraining rectangle.</param>
        /// <returns>
        /// True if any part of the line segment lies within the rectangle.
        /// </returns>
        public static bool ConstrainLineSegToRect(ref Vector2 start, ref Vector2 end, Rect rect)
        {
            bool intersects = false;
            bool startInside = rect.Contains(start);
            bool endInside = rect.Contains(end);

            if (startInside && endInside)
                return true;

            Vector2 tl = new Vector2(rect.xMin, rect.yMin);
            Vector2 tr = new Vector2(rect.xMax, rect.yMin);
            Vector2 bl = new Vector2(rect.xMin, rect.yMax);
            Vector2 br = new Vector2(rect.xMax, rect.yMax);

            Vector2 right = Vector2.zero;
            if (LineSegIntersect(start, end, tr, br, out right))
            {
                intersects = true;
                if (start.x > rect.xMax) start = right;
                else if (end.x > rect.xMax) end = right;
            }

            Vector2 left = Vector2.zero;
            if (LineSegIntersect(start, end, tl, bl, out left))
            {
                intersects = true;
                if (start.x < rect.xMin) start = left;
                else if (end.x < rect.xMin) end = left;
            }

            Vector2 top = Vector2.zero;
            if (LineSegIntersect(start, end, tl, tr, out top))
            {
                intersects = true;
                if (start.y < rect.yMin) start = top;
                else if (end.y < rect.yMin) end = top;
            }

            Vector2 bottom = Vector2.zero;
            if (LineSegIntersect(start, end, bl, br, out bottom))
            {
                intersects = true;
                if (start.y > rect.yMax) start = bottom;
                else if (end.y > rect.yMax) end = bottom;
            }

            if (!intersects)
                return false;

            return true;
        }

        /// <summary>
        /// Finds the intersection point between two line segments.
        /// </summary>
        /// <param name="aStart">Line segment A's start point.</param>
        /// <param name="aEnd">Line segment A's end point.</param>
        /// <param name="bStart">Line segment B's start point.</param>
        /// <param name="bEnd">Line segment B's end point.</param>
        /// <param name="intersection">
        /// Fills in an intersenction point if true is returned.
        /// </param>
        /// <returns>True if the line segments intersect.</returns>
        public static bool LineSegIntersect(Vector2 aStart, Vector2 aEnd,
            Vector2 bStart, Vector2 bEnd, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            Vector2 aDif = aEnd - aStart;
            Vector2 bDif = bEnd - bStart;
            float aDotbPerp = aDif.x * bDif.y - aDif.y * bDif.x;
            if (aDotbPerp == 0)
                return false;
            Vector2 c = bStart - aStart;
            float t = (c.x * bDif.y - c.y * bDif.x) / aDotbPerp;
            if (t < 0 || t > 1)
                return false;
            float u = (c.x * aDif.y - c.y * aDif.x) / aDotbPerp;
            if (u < 0 || u > 1)
                return false;
            intersection = aStart + t * aDif;
            return true;
        }

        #endregion -- 2D Collision --------------------------------------------

        #region -- Quaternion Methods -----------------------------------------
        /// <summary>
        /// Like float GetSmoothApproach, but for Quaternions.
        /// </summary>
        public static Quaternion GetSmoothApproach(Quaternion current, Quaternion target, float duration, float dt)
        {
            float remainingPortion = Mathf.Pow(0.001f, dt / duration);
            return Quaternion.Slerp(current, target, 1f - remainingPortion);
        }

        /// <summary>
        /// Like float GetSmoothApproachWithLerp, but for Quaternions.
        /// </summary>
        public static Quaternion GetSmoothApproachWithLerp(Quaternion current, Quaternion target, float duration, float dt, float portion)
        {
            float remainingPortion = Mathf.Pow(0.001f, dt / duration);
            var smoothApproachResult = Quaternion.Slerp(current, target, 1f - remainingPortion);
            return Quaternion.Slerp(smoothApproachResult, target, portion);
        }
        #endregion

        #region -- Float Functions --------------------------------------------

        /// <summary>Rounds the value to the nearest increment.</summary>
        /// <param name="value"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static float RoundTo(float value, float increment)
        {
            return Mathf.Round(value / increment) * (int)increment;
        }
        #endregion -- Float Functions -----------------------------------------
    }
}