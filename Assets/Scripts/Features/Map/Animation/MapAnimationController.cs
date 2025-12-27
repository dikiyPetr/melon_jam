using System;
using System.Collections;
using UnityEngine;

public class MapAnimationController : MonoBehaviour
{
    [SerializeField] private MapConfig _mapConfig;

    public event Action OnPlayerMovementComplete;

    public void AnimatePlayerMovement(Transform playerIcon, Vector3 fromPosition, Vector3 toPosition, Action onComplete = null)
    {
        StartCoroutine(PlayerMovementCoroutine(playerIcon, fromPosition, toPosition, onComplete));
    }

    private IEnumerator PlayerMovementCoroutine(Transform playerIcon, Vector3 fromPosition, Vector3 toPosition, Action onComplete)
    {
        float duration = _mapConfig != null ? _mapConfig.PlayerMovementDuration : 0.7f;
        float elapsed = 0f;

        toPosition.z = fromPosition.z;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = SmoothStep(t);

            playerIcon.position = Vector3.Lerp(fromPosition, toPosition, smoothT);

            yield return null;
        }

        playerIcon.position = toPosition;
        onComplete?.Invoke();
        OnPlayerMovementComplete?.Invoke();
    }

    public void AnimateConnectionVisited(LineRenderer lineRenderer, Color targetColor, Action onComplete = null)
    {
        StartCoroutine(ConnectionVisitedCoroutine(lineRenderer, targetColor, onComplete));
    }

    private IEnumerator ConnectionVisitedCoroutine(LineRenderer lineRenderer, Color targetColor, Action onComplete)
    {
        if (lineRenderer == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        float duration = 0.3f;
        float elapsed = 0f;

        Color startColor = lineRenderer.startColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Color currentColor = Color.Lerp(startColor, targetColor, t);
            lineRenderer.startColor = currentColor;
            lineRenderer.endColor = currentColor;

            yield return null;
        }

        lineRenderer.startColor = targetColor;
        lineRenderer.endColor = targetColor;

        onComplete?.Invoke();
    }

    private float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }
}
