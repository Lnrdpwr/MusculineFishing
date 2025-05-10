using UnityEngine;
using System.Collections;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] private float zoomScale = 0.95f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float maxMagnitude = 0.2f;
    [SerializeField] private AnimationCurve dampeningCurve;
    [SerializeField] private AnimationCurve zoomCurve;

    private Vector3 originalPosition;

    private Coroutine _zoomRoutine;

    private Camera _camera;
    private float _defaultCameraSize;

    private bool isShaking = false;


    void Awake()
    {
        originalPosition = transform.localPosition;
        _camera = GetComponent<Camera>();
        _defaultCameraSize = _camera.orthographicSize;
    }

    public void Shake()
    {
        if (!isShaking)
        {
            StartCoroutine(DoShake());
        }
    }

    public void ZoomIn(float zoomTime)
    {
        if(_zoomRoutine != null)
        {
            StopCoroutine(_zoomRoutine);
        }

        _zoomRoutine = StartCoroutine(ZoomInRoutine(zoomTime));
    }

    public void ZoomOut(float zoomTime)
    {
        if (_zoomRoutine != null)
        {
            StopCoroutine(_zoomRoutine);
        }

        _zoomRoutine = StartCoroutine(ZoomOutRoutine(zoomTime));
    }

    IEnumerator ZoomInRoutine(float zoomTime)
    {
        for (float i = 0; i < zoomTime; i += Time.deltaTime)
        {
            _camera.orthographicSize = _defaultCameraSize * Mathf.Lerp(1, zoomScale, zoomCurve.Evaluate(i/zoomTime));

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ZoomOutRoutine(float zoomTime)
    {
        float previousSize = _camera.orthographicSize;

        for (float i = 0; i < zoomTime; i += Time.deltaTime)
        {
            _camera.orthographicSize = Mathf.Lerp(previousSize, _defaultCameraSize, zoomCurve.Evaluate(i / zoomTime));

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DoShake()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float currentMagnitude = maxMagnitude * dampeningCurve.Evaluate(progress);

            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isShaking = false;
    }
}