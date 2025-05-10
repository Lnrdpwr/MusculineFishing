using System.Collections;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private int _tutorialIndex;
    [SerializeField] private KeyCode _keyCode;
    [SerializeField] private bool _isKeyboard;

    private TMP_Text _text;

    private void Start()
    {
        _text = GetComponent<TMP_Text>();

        if(PlayerPrefs.GetInt($"Tutorial{_tutorialIndex}", 0) == 1)
        {
            _text.alpha = 0;
        }
        else
        {
            StartCoroutine(WaitForInputRoutine());
        }
    }

    IEnumerator WaitForInputRoutine()
    {
        if (_isKeyboard)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(_keyCode));
        }
        else
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        for(float i = 1; i > 0; i -= Time.deltaTime)
        {
            _text.alpha = i;

            yield return new WaitForEndOfFrame();
        }

        _text.alpha = 0;
        PlayerPrefs.SetInt($"Tutorial{_tutorialIndex}", 1);
    }
}
