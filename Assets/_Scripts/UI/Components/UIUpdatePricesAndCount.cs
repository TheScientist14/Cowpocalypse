using System.Collections;
using TMPro;
using UnityEngine;

public class UIUpdatePricesAndCount : MonoBehaviour
{
	[SerializeField] TMP_Text m_MachinePriceTxt;
	[SerializeField] TMP_Text m_SellerCountTxt;
	[SerializeField] TMP_Text m_SpawnerPriceTxt;

	// Start is called before the first frame update
	void Start()
	{
		UpdateUI();
		GridManager.instance.OnGridChanged.AddListener(UpdateUI);
	}

	private void UpdateUI()
	{
		StartCoroutine(WaitForDestroy());
	}

	private IEnumerator WaitForDestroy()
	{
		yield return new WaitForEndOfFrame();
		m_MachinePriceTxt.text = ItemHandlerManager.instance.GetMachinePrice() + "$";
		m_SellerCountTxt.text = (ItemHandlerManager.instance.GetMaxNbShop() - Seller.GetCount()) + "/" + ItemHandlerManager.instance.GetMaxNbShop();
		m_SpawnerPriceTxt.text = ItemHandlerManager.instance.GetSpawnerPrice() + "$";
	}
}
