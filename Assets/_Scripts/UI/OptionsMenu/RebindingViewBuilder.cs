using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindingViewBuilder : MonoBehaviour
{
    [SerializeField] Transform m_RebindersContainer;
    [SerializeField] GameObject m_RebinderPrefab;

    // Start is called before the first frame update
    void Start()
    {
        InputsActions inputsActions = InputMaster.instance.InputAction;
        foreach(InputAction inputAction in inputsActions)
        {
            if(inputAction == null)
                continue;

            for(int bindingIdx = 0; bindingIdx < inputAction.bindings.Count; bindingIdx++)
            {
                InputBinding inputBinding = inputAction.bindings[bindingIdx];
                if(inputBinding == null || inputBinding.groups == null)
                    continue;

                if(!inputBinding.groups.Contains("Rebindable") || inputBinding.isComposite)
                    continue;

                Rebinder rebinder = Instantiate(m_RebinderPrefab, m_RebindersContainer).GetComponentInChildren<Rebinder>();
                if(rebinder == null)
                {
                    Debug.LogError("No Rebinder component in prefab");
                    Destroy(m_RebinderPrefab.gameObject);
                    continue;
                }
                rebinder.SetInputAction(inputAction, bindingIdx);
            }
        }
    }
}
