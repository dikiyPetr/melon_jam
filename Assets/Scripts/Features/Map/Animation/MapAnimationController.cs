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

    public void AnimateNodeReveal(GameObject nodeObject, Action onComplete = null)
    {
        StartCoroutine(NodeRevealCoroutine(nodeObject, onComplete));
    }

    public void AnimateConnectionReveal(LineRenderer lineRenderer, Action onComplete = null)
    {
        StartCoroutine(ConnectionRevealCoroutine(lineRenderer, onComplete));
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

    private IEnumerator NodeRevealCoroutine(GameObject nodeObject, Action onComplete)
    {
        float duration = _mapConfig != null ? _mapConfig.NodeRevealDuration : 0.3f;
        float elapsed = 0f;

        Vector3 originalScale = nodeObject.transform.localScale;
        nodeObject.transform.localScale = Vector3.zero;

        SpriteRenderer spriteRenderer = nodeObject.GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = SmoothStep(t);

            nodeObject.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, smoothT);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, smoothT);
            }

            yield return null;
        }

        nodeObject.transform.localScale = originalScale;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        onComplete?.Invoke();
    }

    private IEnumerator ConnectionRevealCoroutine(LineRenderer lineRenderer, Action onComplete)
    {
        float duration = _mapConfig != null ? _mapConfig.ConnectionRevealDuration : 0.2f;
        float elapsed = 0f;

        Vector3 startPos = lineRenderer.GetPosition(0);
        Vector3 endPos = lineRenderer.GetPosition(1);

        Color startColor = lineRenderer.startColor;
        Color transparent = new Color(startColor.r, startColor.g, startColor.b, 0f);

        lineRenderer.startColor = transparent;
        lineRenderer.endColor = transparent;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            lineRenderer.startColor = Color.Lerp(transparent, startColor, t);
            lineRenderer.endColor = Color.Lerp(transparent, lineRenderer.endColor, t);

            yield return null;
        }

        lineRenderer.startColor = startColor;
        lineRenderer.endColor = lineRenderer.endColor;

        onComplete?.Invoke();
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
