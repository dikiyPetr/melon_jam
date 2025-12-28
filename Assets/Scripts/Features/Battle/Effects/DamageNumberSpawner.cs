using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private BattleConfig battleConfig;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1f, 0);

    public void SpawnDamageNumber(Vector3 position, int damage)
    {
        if (damageNumberPrefab == null) return;

        Vector3 spawnPosition = position + spawnOffset;
        GameObject damageNumberObj = Instantiate(damageNumberPrefab, spawnPosition, Quaternion.identity);

        DamageNumber damageNumber = damageNumberObj.GetComponent<DamageNumber>();
        if (damageNumber != null)
        {
            damageNumber.Initialize(damage, battleConfig);
        }
    }
}
