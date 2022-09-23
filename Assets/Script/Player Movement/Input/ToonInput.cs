using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ToonInput : INetworkInput
{
    public enum Button
    {
        Stab = 0,
        Dash = 1,
    }

        public float HorizontalInput;
        public float VerticalInput;
    public NetworkButtons Buttons;
}
