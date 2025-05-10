using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoxingMinigame : MonoBehaviour
{
    public delegate void BoxingDelegate();
    public BoxingDelegate onVictory;
    public BoxingDelegate onHit;

    [Header("Settings")]
    [SerializeField] private CameraEffects _cameraEffects;          // СЕЙЧАС БУДЕТ ТРЯСКА
    [SerializeField] private GameObject _hitEffect;             // Эффект удара
    [SerializeField] private GameObject _punchHitbox;           // Хитбокс по которому надо ударить
    [SerializeField] private Collider2D _fishCollider;          // Коллайдер рыбы
    [SerializeField] private float _minTimeBetweenEffects;      // Минимальное время до спавна
    [SerializeField] private float _maxTimeBetweenEffects;      // Максимальное время до спавна
    [SerializeField] private float _effectExistTime;            // Время существования эффекта
    [SerializeField] private float _boxingTime;                 // Время на избиение
    [SerializeField] private int _fishMaxHealth;               // Максимальная выносливость рыбы

    [Header("UI")]
    [SerializeField] private TMP_Text _countdownTimer;          // Таймер обратного отсчета до миниигры
    [SerializeField] private TMP_Text _boxingTimer;             // Таймер боксинга
    [SerializeField] private Image _fishHPBar;                     // HP Рыбы
    [SerializeField] private Gradient _fishHPGradient;          // Градиент хп бара
    [SerializeField] private AnimationCurve _changeCurve;       // Кривая изменения хп бара

    [Header("Fish stuff")]
    [SerializeField] private Transform _fishSpawnPos;           // Где спавнить рыбу
    [SerializeField] private float _specialFishSpawnChance;     // Шанс спавна СПЕЦИАЛЬНОЙ рыбы
    [SerializeField] private GameObject[] _defaultFishes;       // Нормисы
    [SerializeField] private GameObject[] _rareFishes;          // Рарные
 
    [Header("Beauty")]
    [SerializeField] private Animator _patternAnimator;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioClip _KOClip;
    [SerializeField] private Animator _KOAnimator;
    [SerializeField] private AudioClip _musicIntro;

    private GameObject _currentPunchHitbox;                     // Текущий хитбокс по которому надо ударить

    private AudioSource _audioSource;

    private FishScriptableObject _fishData;

    private Coroutine _barChangeRoutine;
    private Coroutine _boxingTimerRoutine;

    private Vector2 _bottomLeft;
    private Vector2 _topRight;
    
    private bool _isGameActive = false;
    private int _fishHealth;

    private void Start()
    {
        Fish fish;

        if(Random.Range(0, 1f) < _specialFishSpawnChance)
        {
            int index = Random.Range(0, _rareFishes.Length);
            fish = Instantiate(_rareFishes[index], _fishSpawnPos).GetComponent<Fish>();
        }
        else
        {
            int index = Random.Range(0, _defaultFishes.Length);
            fish = Instantiate(_defaultFishes[index], _fishSpawnPos).GetComponent<Fish>();
        }

        _fishData = fish.GetData();

        _fishMaxHealth = _fishData.fishHealth;
        _fishHealth = _fishData.fishHealth;
        _minTimeBetweenEffects = _fishData.minTimeBetweenPunches;
        _maxTimeBetweenEffects = _fishData.maxTimeBetweenPunches;
        _boxingTime = _fishData.fishTimer;
        _fishCollider = fish.gameObject.GetComponent<Collider2D>();

        _boxingTimer.text = $"{_boxingTime}:00";

        _audioSource = GetComponent<AudioSource>();

        _bottomLeft = new Vector2(_fishCollider.bounds.min.x, _fishCollider.bounds.min.y);
        _topRight = new Vector2(_fishCollider.bounds.max.x, _fishCollider.bounds.max.y);

        _audioSource.volume = _musicSource.volume;
        _audioSource.PlayOneShot(_musicIntro);
        _musicSource.PlayDelayed(_musicIntro.length + 0.25f);

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

        _currentPunchHitbox = Instantiate(_punchHitbox, spawnPos, Quaternion.identity);
        _currentPunchHitbox.GetComponent<PunchHitbox>().Construct(this);

        float spawnTime = Random.Range(_minTimeBetweenEffects, _maxTimeBetweenEffects);
        Invoke(nameof(SpawnParticle), spawnTime);
    }

    public void Hit()
    {
        if (!_isGameActive) return;

        onHit?.Invoke();

        _cameraEffects.Shake();
        Instantiate(_hitEffect, _currentPunchHitbox.transform.position, Quaternion.identity);

        _fishHealth--;

        if (_barChangeRoutine != null)
        {
            StopCoroutine(_barChangeRoutine);
        }
        _barChangeRoutine = StartCoroutine(ChangeHPBarRoutine());

        if(_fishHealth <= 0)
        {
            StopGame(true);
        }

        Destroy(_currentPunchHitbox);
    }

    private void StopGame(bool isSuccessful)
    {
        StopCoroutine(_boxingTimerRoutine);
        StartCoroutine(StopGameRoutine());

        _cameraEffects.ZoomOut(0.25f);

        _isGameActive = false;

        if (isSuccessful)
        {
            Wallet.Instance.UpdateWallet(_fishData.fishReward);
            onVictory?.Invoke();
            _KOAnimator.SetTrigger("Activate");
            _audioSource.PlayOneShot(_KOClip);
        }
    }

    IEnumerator StopGameRoutine()
    {
        float previousVolume = _musicSource.volume;

        for(float i = 0; i < 1; i += Time.deltaTime)
        {
            _musicSource.volume = Mathf.Lerp(previousVolume, 0, i);

            yield return new WaitForEndOfFrame();
        }

        _musicSource.volume = 0;

        _patternAnimator.SetTrigger("Appear");

        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene("Fishing");
    }

    IEnumerator CountdownRoutine()
    {
        for (float i = 3; i >= 0; i--)
        {
            _countdownTimer.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        _countdownTimer.alpha = 0;

        _isGameActive = true;

        float spawnTime = Random.Range(_minTimeBetweenEffects, _maxTimeBetweenEffects);
        Invoke(nameof(SpawnParticle), spawnTime);

        _cameraEffects.ZoomIn(_boxingTime);

        _boxingTimerRoutine = StartCoroutine(BoxingTimerRoutine());
    }

    IEnumerator BoxingTimerRoutine()
    {
        _audioSource.volume = 0.6f;

        float defaultFontSize = _boxingTimer.fontSize;

        for (float i = _boxingTime; i > 0; i -= Time.deltaTime)
        {
            float seconds = Mathf.FloorToInt(i % 60);
            float miliseconds = Mathf.FloorToInt(i * 100 % 100);

            _boxingTimer.text = string.Format("{0:0}.{1:00}", seconds, miliseconds);
            _boxingTimer.color = Color.Lerp(Color.red, Color.white, i/_boxingTime);
            _boxingTimer.fontSize = defaultFontSize + 40 * Mathf.Lerp(1, 0, i/_boxingTime);

            yield return new WaitForEndOfFrame();
        }

        _boxingTimer.text = "0.00";

        StopGame(false);
    }
    IEnumerator ChangeHPBarRoutine()
    {
        float newHP = _fishHealth;
        float lastHP = _fishHealth + 1;

        for(float i = 0; i < 0.25f; i += Time.deltaTime)
        {
            float currentFill = Mathf.Lerp(lastHP, newHP, _changeCurve.Evaluate(i/0.25f));

            _fishHPBar.fillAmount = currentFill / _fishMaxHealth;
            _fishHPBar.color = _fishHPGradient.Evaluate(_fishHPBar.fillAmount);

            yield return new WaitForEndOfFrame();
        }

        _fishHPBar.fillAmount = newHP/_fishMaxHealth;
    }
}
