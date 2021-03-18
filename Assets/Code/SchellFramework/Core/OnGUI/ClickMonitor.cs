//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   01/28/2015
//-----------------------------------------------------------------------------

using UnityEngine;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// ClickMonitor evaluates and tracks if input events could be considered
    /// a tap or click.
    /// 
    /// Tracks input to identify if a valid click or tap is occurring. Usage is 
    /// to create a new instance, specifying thresholds and then call Down when 
    /// a mouse down or press occurs and Up when there is a release. If Up 
    /// returns 1 or 2, a valid input event has occurred.
    /// </summary>
    public class ClickMonitor
    {
        /// <summary>
        /// Create a new click monitor that is used to identify if a down and 
        /// up event should be treated as a click or tap.
        /// </summary>
        /// <param name="maxClickInterval">
        /// Maximum amount of time between down and up events for an input to 
        /// be considered a click.
        /// </param>
        /// <param name="maxClickTravel">
        /// Maximum amount that an input may travel between down and up events
        /// for an input to be considered a click.
        /// </param>
        public ClickMonitor(float maxClickInterval, float maxClickTravel)
        {
            _maxInterval = maxClickInterval;
            _maxTravel = maxClickTravel;
        }

        /// <summary>
        /// Record a mouse/finger down event position and time.
        /// </summary>
        /// <param name="position">Where event occurred</param>
        public void Down(Vector2 position)
        {
            _downTime = Time.realtimeSinceStartup;
            _downPos = position;
        }

        /// <summary>
        /// Record a mouse/finger release and indicate if it should count as a 
        /// click or tap.
        /// </summary>
        /// <param name="position">Where the up event occurred.</param>
        /// <returns>
        /// The amount of clicks counted: 0 if there was no valid click, 
        /// 1 if it is a single click, or 2 if this was a double click.
        /// </returns>
        public int Up(Vector2 position)
        {
            int result = 0;
            if (_clickCount == 0)
            {
                if (Time.realtimeSinceStartup - _downTime < _maxInterval
                   && Vector2.Distance(_downPos, position) < _maxTravel)
                {
                    _clickCount = 1;
                    _upTime = Time.realtimeSinceStartup;
                    result = _clickCount;
                }
            }
            else
            {
                if (Time.realtimeSinceStartup - _upTime < _maxInterval
                   && Vector2.Distance(_downPos, position) < _maxTravel)
                {
                    result = 2;
                }
                _upTime = 0.0f;
                _clickCount = 0;
            }

            return result;
        }

        /// <summary>
        /// Maximum amount of time between down and up events for an input to 
        /// be considered a click.
        /// </summary>
        private readonly float _maxInterval;

        /// <summary>
        /// Maximum amount that an input may travel between down and up events
        /// for an input to be considered a click.
        /// </summary>
        private readonly float _maxTravel;

        /// <summary>
        /// Time when the previous click was released.
        /// </summary>
        private float _upTime;

        /// <summary>Time the click has started.</summary>
        private float _downTime;

        /// <summary>Where the last down event occurred.</summary>
        private Vector2 _downPos;

        /// <summary>
        /// How many clicks were detected in the current sequence.
        /// </summary>
        private int _clickCount;
    }
}
