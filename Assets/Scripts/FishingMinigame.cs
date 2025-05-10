using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private DayManager _dayManager;                        // Менеджер дня
    [SerializeField] private FishCatchMinigame _previousMinigame;           // Предыдущая миниигра
    [SerializeField] private Animator _rodAnimator;                         // Аниматор удочки
    [SerializeField] private Animator _manAnimator;                         // Аниматор мужика
    [SerializeField] private float _fishMoveSpeed = 0.5f;                   // Скорость движения рыбы
    [SerializeField] private float _playerSliderRiseSpeed = 0.1f;           // Скорость подъёма слайдера при нажатии пробела
    [SerializeField] private float _playerSliderFallAcceleration = 0.05f;   // Ускорение падения слайдера
    [SerializeField] private float _catchThreshold = 0.1f;                  // Порог попадания по рыбе
    [SerializeField] private float _catchFillSpeed = 0.2f;                  // Скорость заполнения шкалы ловли
    [SerializeField] private float _catchDrainSpeed = 0.1f;                 // Скорость опустошения шкалы

    [Header("UI References")]
    [SerializeField] private Image _fillImage;                      // Image заполнения прогресса
    [SerializeField] private Gradient _gradient;                    // Градиент заполнения прогресса
    [SerializeField] private GameObject _minigameParent;            // Родительский объект со всеми объектами
    [SerializeField] private Slider _playerSlider;                  // Слайдер игрока
    [SerializeField] private Slider _fishSlider;                    // Слайдер с рыбой
    [SerializeField] private Slider _catchProgressBar;              // Шкала заполнения ловли

    [Header("Beauty")]
    [SerializeField] private CameraEffects _cameraEffects;          // Эффекты камеры
    [SerializeField] private Animator _patternAnimator;             // Для красивого перехода
    [SerializeField] private Animator _finishFishAnimator;          // РАЗЪЕБ
    [SerializeField] private AudioClip _finishFishClip;             // РАЗЪЕБ х2
    [SerializeField] private AudioSource _musicSource;              // Сурс с музыкой

    private AudioSource _audioSource;

    private float _fishTargetPos; // Текущая цель рыбы (для плавного движения)
    private float _playerSliderSpeed = 0f;
    private bool _isGameActive;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_isGameActive) return;

        UpdateFishPosition();
        UpdatePlayerSlider();
        UpdateCatchProgress();

        if (_catchProgressBar.value >= 1)
        {
            EndGame(true);
        }
        else if (_catchProgressBar.value <= 0)
        {
            EndGame(false);
        }
    }

    public void StartGame()
    {
        _cameraEffects.ZoomIn(0.5f);

        _isGameActive = true;
        _minigameParent.SetActive(true);
        _playerSlider.value = 0.5f;
        _catchProgressBar.value = 0.5f;
        SetRandomFishTarget();
    }

    public void EndGame(bool isSuccess)
    {
        _cameraEffects.ZoomOut(0.5f);

        _isGameActive = false;
        _rodAnimator.SetBool("isActive", false);
        _minigameParent.SetActive(false);

        if(!isSuccess)
        {
            _manAnimator.SetTrigger("Unluck");
            _previousMinigame.StartGame();
        }

        if (isSuccess)
        {
            _dayManager.StopTimer();

            _finishFishAnimator.SetTrigger("Appear");
            _audioSource.PlayOneShot(_finishFishClip);

            StartCoroutine(StopGameRoutine());
        }
    }

    private void UpdateFishPosition()
    {
        float currentFishPos = Mathf.Lerp(_fishSlider.value, _fishTargetPos, _fishMoveSpeed * Time.deltaTime);
        _fishSlider.value = currentFishPos;

        if (Mathf.Abs(_fishSlider.value - _fishTargetPos) < 0.05f)
        {
            SetRandomFishTarget();
        }
    }

    private void SetRandomFishTarget()
    {
        _fishTargetPos = Random.Range(_fishSlider.minValue, _fishSlider.maxValue);
    }

    private void UpdatePlayerSlider()
    {
        _playerSliderSpeed -= _playerSliderFallAcceleration * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            _playerSliderSpeed = _playerSliderRiseSpeed;
        }

        _playerSlider.value += _playerSliderSpeed * Time.deltaTime;
        _playerSlider.value = Mathf.Clamp(_playerSlider.value, _playerSlider.minValue, _playerSlider.maxValue);
    }

    private void UpdateCatchProgress()
    {
        float distance = Mathf.Abs(_fishSlider.value - _playerSlider.value);
        bool isFishInZone = distance < _catchThreshold;

        if (isFishInZone)
        {
            _catchProgressBar.value += _catchFillSpeed * Time.deltaTime;
        }
        else
        {
            _catchProgressBar.value -= _catchDrainSpeed * Time.deltaTime;
        }

        _catchProgressBar.value = Mathf.Clamp01(_catchProgressBar.value);
        _fillImage.color = _gradient.Evaluate(_catchProgressBar.value);
    }

    IEnumerator StopGameRoutine()
    {
        yield return new WaitWhile(()=> _audioSource.isPlaying);

        _patternAnimator.SetTrigger("Appear");

        yield return new WaitForSeconds(0.6f);

        SceneManager.LoadScene("Boxing");
    }
}