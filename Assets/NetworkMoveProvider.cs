using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkMoveProvider : ActionBasedContinuousMoveProvider
{
    private bool disableInput;

    public bool DisableInput { get; set; }

    protected override Vector2 ReadInput()
    {
        if (disableInput) return Vector2.zero;

        return base.ReadInput();
    }
}
