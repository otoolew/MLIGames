// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Russell Ball
//
//  Created: 5/10/2016 11:36:04 AM
// -----------------------------------------------------------------------------

using System;
using UnityEngine.Events;

namespace SG.Core
{
    [Serializable]
    public class UnityBoolEvent : UnityEvent<bool> { }

    [Serializable]
    public class UnityByteEvent : UnityEvent<byte> { }

    [Serializable]
    public class UnitySbyteEvent : UnityEvent<sbyte> { }

    [Serializable]
    public class UnityShortEvent : UnityEvent<short> { }

    [Serializable]
    public class UnityUshortEvent : UnityEvent<ushort> { }

    [Serializable]
    public class UnityIntEvent: UnityEvent<int>{ }

    [Serializable]
    public class UnityUintEvent : UnityEvent<uint> { }

    [Serializable]
    public class UnityLongEvent : UnityEvent<long> { }

    [Serializable]
    public class UnityUlongEvent : UnityEvent<ulong> { }

    [Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }

    [Serializable]
    public class UnityDoubleEvent : UnityEvent<double> { }

    [Serializable]
    public class UnityCharEvent : UnityEvent<char> { }

    [Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
}
