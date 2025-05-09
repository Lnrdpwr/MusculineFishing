using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishCatchMinigame : MonoBehaviour
{
    [Header("Minigame Settings")]
    [SerializeField] private Animator _rodAnimator;         // �������� ������
    [SerializeField] private Animator _bubbleAnimator;      // �������� �������
    [SerializeField] private Animator _manAnimator;         // �������� ������
    [SerializeField] private FishingMinigame _minigame;     // ��������� ��������
    [SerializeField] private float _effectTimer;            // ����� ������������� �������
    [SerializeField] private float _minTimieBetweenEffects; // ����������� ����� �� ������ ��������� �������
    [SerializeField] private float _maxTimieBetweenEffects; // ������������ ����� �� ������ ��������� �������
    [SerializeField] private float _catchCooldown;          // ����� �� ���������� ������� �������

    private bool _canCatch = false;
    private bool _isOnCooldown = false;
    private bool _isGameActive = true;

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!_isGameActive) return;

        if (Input.GetKeyDown(KeyCode.Space) && !_isOnCooldown)
        {
            if(_canCatch)
            {
                _minigame.StartGame();
                _bubbleAnimator.SetBool("isActive", false);
                _isGameActive = false;
            }
            else
            {
                _isOnCooldown = true;
                _manAnimator.SetTrigger("Unluck");
                Invoke(nameof(ResetCooldown), _catchCooldown);
            }
        }
    }

    private void SpawnParticle()
    {
        if (!_isGameActive) return;

        _canCatch = true;

        float timeToNextEffect = Random.Range(_minTimieBetweenEffects, _maxTimieBetweenEffects);

        _rodAnimator.SetBool("isActive", true);
        _bubbleAnimator.SetBool("isActive", true);

        Invoke(nameof(SpawnParticle), timeToNextEffect);
        Invoke(nameof(StopCatching), _effectTimer);
    }

    private void StopCatching()
    {
        if (!_isGameActive) return;

        _canCatch = false;

        _rodAnimator.SetBool("isActive", false);
        _bubbleAnimator.SetBool("isActive", false);
    }

    private void ResetCooldown()
    {
        _isOnCooldown = false;
    }

    public void StartGame()
    {
        _canCatch = false;
        _isOnCooldown = false;
        _isGameActive = true;

        float timeToNextEffect = Random.Range(_minTimieBetweenEffects, _maxTimieBetweenEffects);

        Invoke(nameof(SpawnParticle), timeToNextEffect);
    }
}
