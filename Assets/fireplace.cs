using UnityEngine;
using System.Collections;

public class FireplaceTrigger : MonoBehaviour
{
    [Header("Fire Settings")]
    public GameObject fireEffectPrefab;    // Fire particle prefab
    public GameObject smokeEffectPrefab;   // Smoke particle prefab
    public float burnDuration = 8f;        // How long before the barrel burns out
    public Color burnColor = new Color(1f, 0.4f, 0f);  // Bright orange glow

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Barrel"))
        {
            // Prevent multiple ignitions
            if (other.transform.Find("Fire(Clone)") != null)
                return;

            // Spawn fire effect (use FirePoint if available)
            Transform firePoint = other.transform.Find("FirePoint");
            Vector3 spawnPos = firePoint ? firePoint.position : other.transform.position + Vector3.up * 0.5f;

            if (fireEffectPrefab != null)
            {
                Instantiate(fireEffectPrefab, spawnPos, Quaternion.identity, other.transform);
            }

            // Start burn process
            StartCoroutine(BurnAndSmoke(other.gameObject));
        }
    }

    private IEnumerator BurnAndSmoke(GameObject barrel)
    {
        Renderer rend = barrel.GetComponentInChildren<Renderer>();

        if (rend == null)
            yield break;

        // Create a unique material instance so we don't affect other barrels
        rend.material = new Material(rend.material);

        // Determine which property to use based on shader type
        Color startColor = Color.white;
        if (rend.material.HasProperty("_BaseColor"))
            startColor = rend.material.GetColor("_BaseColor");
        else if (rend.material.HasProperty("_Color"))
            startColor = rend.material.GetColor("_Color");

        float timer = 0f;

        while (timer < burnDuration)
        {
            timer += Time.deltaTime;
            float t = timer / burnDuration;

            Color newColor = Color.Lerp(startColor, burnColor, t);

            // Apply color depending on shader
            if (rend.material.HasProperty("_BaseColor"))
                rend.material.SetColor("_BaseColor", newColor);
            if (rend.material.HasProperty("_Color"))
                rend.material.SetColor("_Color", newColor);

            // Optional: emission glow effect
            if (rend.material.HasProperty("_EmissionColor"))
            {
                Color emission = Color.Lerp(Color.black, burnColor * 4f, t);
                rend.material.SetColor("_EmissionColor", emission);
            }

            yield return null;
        }

        // Spawn white smoke at the end
        if (smokeEffectPrefab != null)
        {
            Instantiate(
                smokeEffectPrefab,
                barrel.transform.position + Vector3.up * 0.5f,
                Quaternion.identity
            );
        }

        // Destroy the barrel
        Destroy(barrel);
    }
}
