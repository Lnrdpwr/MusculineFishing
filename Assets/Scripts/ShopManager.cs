using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Trinket[] _trinkets;
    [SerializeField] private GameObject _buyEffect;

    [Header("UI")]
    [SerializeField] private TMP_Text[] _priceTags;
    [SerializeField] private TMP_Text[] _trinketsNames;
    [SerializeField] private Button[] _buyButtons;
    [SerializeField] private Image[] _images;

    private Wallet _wallet;

    private void Start()
    {
        _wallet = Wallet.Instance;

        List<Trinket> trinkets = new List<Trinket>();

        for(int i = 0; i < 3; i++)
        {
            Trinket trinket;
            int index = i;

            do
            {
                trinket = _trinkets[Random.Range(0, _trinkets.Length)];
            } while (trinkets.Contains(trinket));

            trinkets.Add(trinket);

            _priceTags[i].text = $"{trinket.price}";

            _images[i].sprite = trinket.sprite;

            _trinketsNames[i].text = trinket.trinketName;

            _buyButtons[i].onClick.AddListener(()=> Buy(trinket.trinketName, trinket.price, _images[index], _images[index].transform));
        }
        
    }

    public void Buy(string item, int price, Image image, Transform imageTransform)
    {
        if (_wallet.GetMoney() < price) return;

        _wallet.UpdateWallet(-price);

        int boughtAmount = PlayerPrefs.GetInt(item, 0);
        PlayerPrefs.SetInt(item, boughtAmount+1);

        image.sprite = null;
        image.color = Color.clear;
        Instantiate(_buyEffect, imageTransform.position, Quaternion.identity);
    }

    public void Leave()
    {
        PlayerPrefs.SetInt("NewDay", 1);
        SceneManager.LoadScene("Fishing");
    }
}
