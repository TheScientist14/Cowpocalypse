// Use InputBindingComposite<TValue> as a base class for a composite that returns
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
[DisplayStringFormat("PinchComposite")]
public class PinchComposite : InputBindingComposite<float>
{
	// Each part binding is represented as a field of type int and annotated with
	// InputControlAttribute. Setting "layout" restricts the controls that
	// are made available for picking in the UI.
	//
	// On creation, the int value is set to an integer identifier for the binding
	// part. This identifier can read values from InputBindingCompositeContext.
	// See ReadValue() below.
	[InputControl(layout = "Value")]
	public int m_FirstFingerPositionValueId;

	[InputControl(layout = "Value")]
	public int m_SecondFingerPositionValueId;

	// on screen position, if negative, then not initialized
	private bool m_StartPinch = false;
	private float m_InitDistance;

	// This method computes the resulting input value of the composite based
	// on the input from its part bindings.
	public override float ReadValue(ref InputBindingCompositeContext iContext)
	{
		// ensure both fingers are touching the screen
		if(iContext.EvaluateMagnitude(m_FirstFingerPositionValueId) <= 0 || iContext.EvaluateMagnitude(m_SecondFingerPositionValueId) <= 0)
		{
			m_StartPinch = false;
			return 0;
		}

		Vector2 firstFingerPosition = iContext.ReadValue<Vector2, Vector2MagnitudeComparer>(m_FirstFingerPositionValueId);
		Vector2 secondFingerPosition = iContext.ReadValue<Vector2, Vector2MagnitudeComparer>(m_SecondFingerPositionValueId);

		// init pinch if not started
		if(!m_StartPinch)
		{
			m_StartPinch = true;
			m_InitDistance = Vector2.Distance(firstFingerPosition, secondFingerPosition);
			return 0;
		}

		return Vector2.Distance(firstFingerPosition, secondFingerPosition) - m_InitDistance;
	}

	// This method computes the current actuation of the binding as a whole.
	public override float EvaluateMagnitude(ref InputBindingCompositeContext iContext)
	{
		return (m_StartPinch ? 1 : 0);
	}

	static PinchComposite()
	{
		// Can give custom name or use default (type name with "Composite" clipped off).
		// Same composite can be registered multiple times with different names to introduce
		// aliases.
		//
		// NOTE: Registering from the static constructor using InitializeOnLoad and
		//       RuntimeInitializeOnLoadMethod is only one way. You can register the
		//       composite from wherever it works best for you. Note, however, that
		//       the registration has to take place before the composite is first used
		//       in a binding. Also, for the composite to show in the editor, it has
		//       to be registered from code that runs in edit mode.
		InputSystem.RegisterBindingComposite<PinchComposite>();
	}

	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
	} // Trigger static constructor.
}