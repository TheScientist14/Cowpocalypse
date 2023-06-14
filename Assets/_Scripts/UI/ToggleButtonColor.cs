using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonColor : MonoBehaviour
{
    [SerializeField]private Color mainColor;
    [SerializeField]private Color activeColor;
    
    public void ToggleColor()
    {
        GetComponent<Image>().color = (GetComponent<Image>().color == mainColor) ? activeColor : mainColor;
    }
}
