using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ToonInput : INetworkInput
{
    public enum Button
    {
        Weak = 0,
        Strong = 1,
        Dash = 2,
        Parry = 3,
    }

        public float HorizontalInput;
        public float VerticalInput;
    public NetworkButtons Buttons;
}
