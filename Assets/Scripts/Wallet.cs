using UnityEngine;

public class Wallet : MonoBehaviour
{
    public delegate void WalletDelegate();
    public WalletDelegate onMoneyChange;

    private int _money;

    internal static Wallet Instance;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void UpdateWallet(int changeAmount)
    {
        _money = PlayerPrefs.GetInt("Money", 0);

        if(_money + changeAmount < 0)
        {
            return;
        }

        _money += changeAmount;
        PlayerPrefs.SetInt("Money", _money);

        onMoneyChange?.Invoke();
    }

    public int GetMoney()
    {
        _money = PlayerPrefs.GetInt("Money", 0);
        return _money;
    }
}
