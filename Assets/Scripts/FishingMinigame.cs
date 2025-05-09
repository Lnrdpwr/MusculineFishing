using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _fishMoveSpeed = 0.5f;                   // �������� �������� ����
    [SerializeField] private float _playerSliderRiseSpeed = 0.1f;           // �������� ������� �������� ��� ������� �������
    [SerializeField] private float _playerSliderFallAcceleration = 0.05f;   // ��������� ������� ��������
    [SerializeField] private float _catchThreshold = 0.1f;                  // ����� ��������� �� ����
    [SerializeField] private float _catchFillSpeed = 0.2f;                  // �������� ���������� ����� �����
    [SerializeField] private float _catchDrainSpeed = 0.1f;                 // �������� ����������� �����

    [Header("UI References")]
    [SerializeField] private GameObject _minigameParent;            // ������������ ������ �� ����� ���������
    [SerializeField] private Slider _playerSlider;                  // ������� ������
    [SerializeField] private Slider _fishSlider;                    // ������� � �����
    [SerializeField] private Slider _catchProgressBar;              // ����� ���������� �����

    private float _fishTargetPos; // ������� ���� ���� (��� �������� ��������)
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
            Debug.Log("���� �������!");
            EndGame(true);
        }
        else if (_catchProgressBar.value <= 0)
        {
            Debug.Log("���� ����!");
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