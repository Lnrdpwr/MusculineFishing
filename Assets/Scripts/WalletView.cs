using TMPro;
using UnityEngine;

public class WalletView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private Wallet wallet;

    private void OnEnable()
    {
        wallet = Wallet.Instance;
        wallet.onMoneyChange += ChangeText;
        ChangeText();
    }

    private void OnDisable()
    {
        wallet.onMoneyChange -= ChangeText;
    }

    private void ChangeText()
    {
        int money = wallet.GetMoney();
        _text.text = $"{money}$";
    }
}
