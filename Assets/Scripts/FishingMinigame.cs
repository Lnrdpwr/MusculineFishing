using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _fishMoveSpeed = 0.5f;                   // Скорость движения рыбы
    [SerializeField] private float _playerSliderRiseSpeed = 0.1f;           // Скорость подъёма слайдера при нажатии пробела
    [SerializeField] private float _playerSliderFallAcceleration = 0.05f;   // Ускорение падения слайдера
    [SerializeField] private float _catchThreshold = 0.1f;                  // Порог попадания по рыбе
    [SerializeField] private float _catchFillSpeed = 0.2f;                  // Скорость заполнения шкалы ловли
    [SerializeField] private float _catchDrainSpeed = 0.1f;                 // Скорость опустошения шкалы

    [Header("UI References")]
    [SerializeField] private GameObject _minigameParent;            // Родительский объект со всеми объектами
    [SerializeField] private Slider _playerSlider;                  // Слайдер игрока
    [SerializeField] private Slider _fishSlider;                    // Слайдер с рыбой
    [SerializeField] private Slider _catchProgressBar;              // Шкала заполнения ловли

    private float _fishTargetPos; // Текущая цель рыбы (для плавного движения)
    private float _playerSliderSpeed = 0f;
    private bool _isGameActive;

    private void Update()
    {
        if (!_isGameActive) return;

        UpdateFishPosition();
        UpdatePlayerSlider();
        UpdateCatchProgress();

        if (_catchProgressBar.value >= 1)
        {
            Debug.Log("Рыба поймана!");
            EndGame(true);
        }
        else if (_catchProgressBar.value <= 0)
        {
            Debug.Log("Рыба ушла!");
            EndGame(false);
        }
    }

    public void StartGame()
    {
        _isGameActive = true;
        _minigameParent.SetActive(true);
        _playerSlider.value = 0.5f;
        _catchProgressBar.value = 0.5f;
        SetRandomFishTarget();
    }

    private void EndGame(bool isSuccess)
    {
        _isGameActive = false;
        _minigameParent.SetActive(false);
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
    }
}