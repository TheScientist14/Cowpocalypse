// Use InputBindingComposite<TValue> as a base class for a composite that returns
// values of type TValue.
// NOTE: It is possible to define a composite that returns different kinds of values
//       but doing so requires deriving directly from InputBindingComposite.
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using UnityEngine.InputSystem.Controls;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad] // Automatically register in editor.
#endif
// Determine how GetBindingDisplayString() formats the composite by applying
// the  DisplayStringFormat attribute.
[DisplayStringFormat("DragInteraction")]
// source : https://forum.unity.com/threads/implement-a-mouse-drag-composite.807906/
public class MouseDragInteraction : IInputInteraction
{
    static MouseDragInteraction()
    {
        InputSystem.RegisterInteraction<MouseDragInteraction>();
    }

    public void Reset()
    {
    }

    public void Process(ref InputInteractionContext context)
    {
        if(context.timerHasExpired)
        {
            context.Performed();
            return;
        }

        var phase = context.phase;

        switch(phase)
        {
            case InputActionPhase.Disabled:
                break;
            case InputActionPhase.Waiting:
                if(context.ControlIsActuated())
                {
                    context.Started();
                    context.SetTimeout(float.PositiveInfinity);
                }

                break;
            case InputActionPhase.Started:
                context.PerformedAndStayPerformed();
                break;
            case InputActionPhase.Performed:
                if(context.ControlIsActuated())
                {
                    context.PerformedAndStayPerformed();
                }
                else if(!((ButtonControl)context.action.controls[0]).isPressed)
                {
                    context.Canceled();
                }

                break;
            case InputActionPhase.Canceled:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
    }
}