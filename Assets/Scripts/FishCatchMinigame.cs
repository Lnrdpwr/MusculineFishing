using TMPro;
using UnityEngine;

public class FishCatchMinigame : MonoBehaviour
{
    [Header("Minigame Settings")]
    [SerializeField] private GameObject _effect;            // Эффект блесток
    [SerializeField] private Transform _effectSpawn;
    [SerializeField] private FishingMinigame _minigame;     // Следующая миниигра
    [SerializeField] private float _effectTimer;            // Время существования эффекта
    [SerializeField] private float _minTimieBetweenEffects; // Минимальное время до спавна следующих блесток
    [SerializeField] private float _maxTimieBetweenEffects; // Максимальное время до спавна следующих блесток
    [SerializeField] private float _catchCooldown;          // Время до следующего нажатия пробела

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_canCatch && !_isOnCooldown)
            {
                _minigame.StartGame();
                _isGameActive = false;
            }
            else
            {
                _isOnCooldown = true;
                Invoke(nameof(ResetCooldown), _catchCooldown);
            }
        }
    }

    private void SpawnParticle()
    {
        if (!_isGameActive) return;

        Instantiate(_effect, _effectSpawn.position, Quaternion.identity);
        _canCatch = true;

        float timeToNextEffect = Random.Range(_minTimieBetweenEffects, _maxTimieBetweenEffects);

        Invoke(nameof(SpawnParticle), timeToNextEffect);
        Invoke(nameof(StopCatching), _effectTimer);
    }

    private void StopCatching()
    {
        _canCatch = false;
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
