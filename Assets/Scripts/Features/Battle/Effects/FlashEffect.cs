using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlashEffect : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private Color flashColor = Color.white;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Material flashMaterial;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;

        flashMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float flashInterval = flashDuration / (flashCount * 2);

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.color = Color.white;
        flashCoroutine = null;
    }

    private void OnDestroy()
    {
        if (flashMaterial != null)
        {
            Destroy(flashMaterial);
        }
    }
}
