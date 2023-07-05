using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlaceItemHandlerButton : MonoBehaviour
{
    [SerializeField] GameObject m_ItemHandlerToPlacePrefab;
    [SerializeField] Color m_ActiveColor = Color.green;
    [SerializeField] Color m_InactiveColor = Color.white;
    private Button m_Button;

    // Start is called before the first frame update
    void Start()
    {
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(TogglePlaceMachine);
        ItemHandlerManager.instance.OnBeltModeChange.AddListener(UpdateColor);
    }

    void UpdateColor()
    {
        if(m_Button == null)
        {
            Debug.LogError("Button required on " + gameObject.name);
            return;
        }

        ColorBlock colorBlock = m_Button.colors;
        if(ItemHandlerManager.instance.IsPlacingMachine(m_ItemHandlerToPlacePrefab))
            colorBlock.normalColor = m_ActiveColor;
        else
            colorBlock.normalColor = m_InactiveColor;
        colorBlock.selectedColor = colorBlock.normalColor;
        m_Button.colors = colorBlock;
    }

    void TogglePlaceMachine()
    {
        if(ItemHandlerManager.instance.IsPlacingMachine(m_ItemHandlerToPlacePrefab))
            ItemHandlerManager.instance.EndPlaceMachine();
        else
            ItemHandlerManager.instance.StartPlaceMachine(m_ItemHandlerToPlacePrefab);
    }
}
