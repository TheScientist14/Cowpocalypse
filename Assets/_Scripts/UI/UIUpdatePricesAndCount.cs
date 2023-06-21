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
        NewGridManager.instance.OnGridChanged.AddListener(UpdateUI);
    }

    private void UpdateUI()
    {
        StartCoroutine(WaitForDestroy());
    }

    private IEnumerator WaitForDestroy()
    {
        yield return new WaitForEndOfFrame();
        sellerCount.text = (ItemHandlerManager.instance.MaxShop - ItemHandlerManager.instance.ShopCount) + "/" + ItemHandlerManager.instance.MaxShop;
        machinePrice.text = ItemHandlerManager.instance.GetMachinePrice() + " $";
    }
}
