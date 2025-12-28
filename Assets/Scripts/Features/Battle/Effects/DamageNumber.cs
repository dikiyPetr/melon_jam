using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float fadeStartDelay = 0.5f;

    private TextMeshPro textMesh;
    private Color originalColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            originalColor = textMesh.color;
        }
    }

    public void Initialize(int damage, BattleConfig config = null)
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        textMesh.text = damage.ToString();

        if (config != null)
        {
            lifetime = config.damageNumberLifetime;
            moveSpeed = config.damageNumberMoveSpeed;
            fadeStartDelay = config.damageNumberFadeDelay;
        }

        originalColor = textMesh.color;
        StartCoroutine(AnimateCoroutine());
    }

    private IEnumerator AnimateCoroutine()
    {
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < lifetime)
        {
            transform.position = startPosition + Vector3.up * (moveSpeed * elapsed);

            if (elapsed > fadeStartDelay)
            {
                float fadeProgress = (elapsed - fadeStartDelay) / (lifetime - fadeStartDelay);
                Color color = originalColor;
                color.a = Mathf.Lerp(1f, 0f, fadeProgress);
                textMesh.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
