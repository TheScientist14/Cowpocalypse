/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

// Interaction which performs when you hold and drag a button.
public class DragInteraction : IInputInteraction<Vector2>
{
    private bool m_IsDragging = false;
    private Vector2 m_InitPos;

    void Process(ref InputInteractionContext context)
    {
        if(context.timerHasExpired)
        {
            context.Canceled();
            return;
        }

        switch(context.phase)
        {
            case InputActionPhase.Started:

                context.Started();
                break;
            case InputActionPhase.Performed:
                context.Performed();
                break;

            case InputActionPhase.Started:
                if(context.control.ReadValue<Vector2>() == -1)
                    
                break;
        }
    }

    // Unlike processors, Interactions can be stateful, meaning that you can keep a
    // local state that mutates over time as input is received. The system might
    // invoke the Reset() method to ask Interactions to reset to the local state
    // at certain points.
    void Reset()
    {
        m_IsDragging = false;
    }

    static DragInteraction()
    {
        InputSystem.RegisterInteraction<DragInteraction>();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init() { }
}*/