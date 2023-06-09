/*// Use InputBindingComposite<TValue> as a base class for a composite that returns
// values of type TValue.
// NOTE: It is possible to define a composite that returns different kinds of values
//       but doing so requires deriving directly from InputBindingComposite.
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad] // Automatically register in editor.
#endif

// Determine how GetBindingDisplayString() formats the composite by applying
// the  DisplayStringFormat attribute.
[DisplayStringFormat("DragComposite")]
// source : https://forum.unity.com/threads/implement-a-mouse-drag-composite.807906/
public class DragComposite : InputBindingComposite<Vector2>
{

    [InputControl(layout = "Button")]
    public int Button;

    [InputControl(layout = "Value")]
    public int CurrentPos;

    private bool isDragging = false;
    private Vector2 oldPos;

    public override Vector2 ReadValue(ref InputBindingCompositeContext context)
    {
        Vector2 curPos = context.ReadValue<Vector2, Vector2MagnitudeComparer>(CurrentPos);
        if(!isDragging)
        {
            oldPos = curPos;
            isDragging = true;
            Debug.Log("init drag : " + curPos);
            return Vector2.zero;
        }

        Debug.Log("drag : " + (curPos - oldPos).magnitude);
        return curPos - oldPos;
    }

    public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
    {
        float magnitude = context.EvaluateMagnitude(Button);
        Debug.Log(magnitude);
        if(magnitude <= 0)
        {
            isDragging = false;
            return 0;
        }

        return magnitude;
    }

    static DragComposite()
    {
        InputSystem.RegisterBindingComposite<DragComposite>();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init() { }
}*/