using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteButtonColor : MonoBehaviour
{
    [SerializeField]private Color mainColor;
    [SerializeField]private Color activeColor;
    
    // Start is called before the first frame update
    void Start()
    {
        BeltManager.instance.deleteMode.AddListener(changeColor);
    }

    private void changeColor(bool isDelete)
    {
        this.GetComponent<Image>().color = isDelete ? activeColor : mainColor;
    }
}
