using TMPro;
using UnityEngine;

public class WalletUI : MonoBehaviour
{
    [SerializeField] Wallet _wallet;
    [SerializeField] TextMeshProUGUI _walletText;

    void OnEnable()
    {
        _wallet.MoneyChanged += OnMoneyChanged;
    }

    void Start()
    {
        _walletText.text = $"$ {_wallet.Money}";
    }

    void OnDisable()
    {
        _wallet.MoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged(int newValue)
    {
        _walletText.text = $"$ {newValue}";
    }
}