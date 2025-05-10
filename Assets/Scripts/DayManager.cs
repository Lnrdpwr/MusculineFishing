using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private FishCatchMinigame _catchMinigame;
    [SerializeField] private FishingMinigame _fishingMinigame;
    [SerializeField] private int _dayTime;

    [Header("UI")]
    [SerializeField] private TMP_Text _dayTimer;

    [Header("Beauty")]
    [SerializeField] private Animator _patternAnimator;

    private int _timeLeft;

    private void Start()
    {
        if(PlayerPrefs.GetInt("NewDay", 1) == 1)
        {
            PlayerPrefs.SetInt("NewDay", 0);
            _timeLeft = _dayTime;
        }
        else
        {
            _timeLeft = PlayerPrefs.GetInt("DayTimeLeft", _dayTime);
        }

        _timeLeft = 60;

        StartCoroutine(TimerRoutine());
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("DayTimeLeft", _timeLeft);
    }

    IEnumerator TimerRoutine()
    {
        while(_timeLeft > 0)
        {
            _timeLeft--;
            _dayTimer.text = _timeLeft.ToString();

            yield return new WaitForSeconds(1);
        }

        int day = PlayerPrefs.GetInt("Day", 0);
        PlayerPrefs.SetInt("Day", day+1);

        _catchMinigame.StopGame();
        _fishingMinigame.EndGame(false);

        _patternAnimator.SetTrigger("Appear");

        yield return new WaitForSeconds(0.6f);

        SceneManager.LoadScene("Shop");
    }
}
