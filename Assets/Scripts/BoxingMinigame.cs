using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxingMinigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject _sparkleEffect;         // Эффект Блесток
    [SerializeField] private GameObject _hitEffect;             // Эффект удара
    [SerializeField] private GameObject _punchHitbox;           // Хитбокс по которому надо ударить
    [SerializeField] private Collider2D _fishCollider;          // Коллайдер рыбы
    [SerializeField] private float _minTimeBetweenEffects;      // Минимальное время до спавна
    [SerializeField] private float _maxTimeBetweenEffects;      // Максимальное время до спавна
    [SerializeField] private float _effectExistTime;            // Время существования эффекта
    [SerializeField] private float _specialFishSpawnChance;     // Шанс спавна СПЕЦИАЛЬНОЙ рыбы
    [SerializeField] private float _boxingTime;                 // Время на избиение
    [SerializeField] private int _fishMaxAgility;               // Максимальная выносливость рыбы

    [Header("UI")]
    [SerializeField] private TMP_Text _countdownTimer;          // Таймер обратного отсчета до миниигры
    [SerializeField] private TMP_Text _boxingTimer;             // Таймер боксинга
    [SerializeField] private Image _fishHP;

    private GameObject _currentPunchHitbox;                     // Текущий хитбокс по которому надо ударить

    private Vector2 _bottomLeft;
    private Vector2 _topRight;
    
    private bool _isGameActive = false;
    private int _fishAgility;

    private void Start()
    {
        _fishAgility = _fishMaxAgility;

        _bottomLeft = new Vector2(_fishCollider.bounds.min.x, _fishCollider.bounds.min.y);
        _topRight = new Vector2(_fishCollider.bounds.max.x, _fishCollider.bounds.max.y);

        StartCoroutine(CountdownRoutine());
    }



    private void SpawnParticle()
    {
        if (!_isGameActive) return;

        Vector2 spawnPos;

        do
        {
            spawnPos = new Vector2(Random.Range(_bottomLeft.x, _topRight.x), Random.Range(_bottomLeft.y, _topRight.y));
        } while (!_fishCollider.OverlapPoint(spawnPos));
        
        Instantiate(_sparkleEffect, spawnPos, Quaternion.identity);

        _currentPunchHitbox = Instantiate(_punchHitbox, spawnPos, Quaternion.identity);
        _currentPunchHitbox.GetComponent<PunchHitbox>().Construct(this);
    }

    public void Hit()
    {
        if (!_isGameActive) return;

        _fishAgility--;

        if(_fishAgility <= 0)
        {
            StopGame();
        }

        Instantiate(_hitEffect, _currentPunchHitbox.transform.position, Quaternion.identity);

        Destroy(_currentPunchHitbox);
    }

    private void StopGame()
    {
        _isGameActive = false;
    }

    IEnumerator CountdownRoutine()
    {
        for(float i = 3; i >= 0; i--)
        {
            _countdownTimer.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        _countdownTimer.alpha = 0;

        _isGameActive = true;

        float spawnTime = Random.Range(_minTimeBetweenEffects, _maxTimeBetweenEffects);
        Invoke(nameof(SpawnParticle), spawnTime);

        StartCoroutine(BoxingTimerRoutine());
    }

    IEnumerator BoxingTimerRoutine()
    {
        for(float i = _boxingTime; i > 0; i -= Time.deltaTime)
        {
            float seconds = Mathf.FloorToInt(i);
            float miliseconds = Mathf.FloorToInt(i % 1);

            _boxingTimer.text = string.Format("{0:00}:{1:00}", seconds, miliseconds);

            yield return new WaitForEndOfFrame();
        }

        _boxingTimer.text = "00:00";

        StopGame();
    }
}
