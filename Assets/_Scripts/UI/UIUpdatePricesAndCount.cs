using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UIUpdatePricesAndCount : MonoBehaviour
{
    [SerializeField] private TMP_Text sellerCount;
    [SerializeField] private TMP_Text machinePrice;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
        GridManager.instance.onGridChanged.AddListener(UpdateUI);
    }

    private void UpdateUI()
    {
        StartCoroutine(WaitForDestroy());
    }

    private IEnumerator WaitForDestroy()
    {
        yield return new WaitForEndOfFrame();
        sellerCount.text = (BeltManager.instance.MaxShop - BeltManager.instance.ShopCount) + "/" + BeltManager.instance.MaxShop;
        machinePrice.text = BeltManager.instance.GetMachinePrice() + " $";
    }
}
