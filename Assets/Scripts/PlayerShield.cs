using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject shieldEffect;   // The shield VFX GameObject
    public float shieldDuration = 5f; // How long shield stays active
    public float flickerTime = 2f;    // When to start flickering before ending

    [Header("Sound Effects")]
    public AudioSource audioSource;   // To play sound
    public AudioClip shieldActivateSound;
    public AudioClip shieldDeactivateSound;

    private bool isShieldActive = false;
    private int lastShieldScore = 0;

    private Coroutine shieldTimerCoroutine = null;
    private Coroutine flickerCoroutine = null;

    public bool IsShieldActive()
    {
        return isShieldActive;
     }

    // -----------------------------
    // 🔹 Call this from your score system
    // -----------------------------
    public void CheckShieldByScore(int score)
    {
        // Activate shield every +2 score
        if (score % 2 == 0 && score > lastShieldScore && !isShieldActive)
        {
            lastShieldScore = score;
            ActivateShield();
            Debug.Log($"🛡️ SHIELD ACTIVATED at score {score}!");

        }
        else
        {
            Debug.Log($"❌ Shield skipped: score={score}, last={lastShieldScore}, active={isShieldActive}");
        }
    }

    // -----------------------------
    // 🔹 Activates the shield
    // // -----------------------------

    void ActivateShield(bool refresh = false)
    {
        // If shield already active and not a refresh request, do nothing
        if (isShieldActive && !refresh)
            return;

        // If we're refreshing while active, stop existing coroutines so we restart the timer cleanly
        if (isShieldActive && refresh)
        {
            if (shieldTimerCoroutine != null) StopCoroutine(shieldTimerCoroutine);
            if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
        }

        isShieldActive = true;
        shieldEffect?.SetActive(true);

        // Play activate sound
        if (audioSource && shieldActivateSound)
            audioSource.PlayOneShot(shieldActivateSound);

        // Start the shield timer coroutine and keep a handle
        shieldTimerCoroutine = StartCoroutine(ShieldTimer());
    }

    // -----------------------------
    // 🔹 Timer for shield
    // -----------------------------
    
       IEnumerator ShieldTimer()
    {
        // Wait until flicker time starts
        float timeBeforeFlicker = Mathf.Max(0f, shieldDuration - flickerTime);
        yield return new WaitForSeconds(timeBeforeFlicker);

        // Start flickering
        flickerCoroutine = StartCoroutine(FlickerEffect());

        // Wait remaining time then deactivate
        yield return new WaitForSeconds(flickerTime);

        DeactivateShield();
    }


    // -----------------------------
    // 🔹 Flicker effect
    // -----------------------------

    IEnumerator FlickerEffect()
    {
        float flickerSpeed = 0.2f; // speed of flicker
        // prefer SpriteRenderer or Renderer depending on your object — try both safely
        Renderer shieldRenderer = shieldEffect ? shieldEffect.GetComponent<Renderer>() : null;
        SpriteRenderer spriteR = shieldEffect ? shieldEffect.GetComponent<SpriteRenderer>() : null;

        while (isShieldActive)
        {
            if (shieldRenderer)
                shieldRenderer.enabled = !shieldRenderer.enabled;
            if (spriteR)
                spriteR.enabled = !spriteR.enabled;

            yield return new WaitForSeconds(flickerSpeed);
        }

        // Make sure it's visible again before turning off
        if (shieldRenderer) shieldRenderer.enabled = true;
        if (spriteR) spriteR.enabled = true;
    }

    // -----------------------------
    // 🔹 Deactivate shield
    // -----------------------------
    void DeactivateShield()
    {
        isShieldActive = false;
        shieldEffect.SetActive(false);

        // Play deactivate sound
        if (audioSource && shieldDeactivateSound)
            audioSource.PlayOneShot(shieldDeactivateSound);

        // clear coroutine handles
        shieldTimerCoroutine = null;
        flickerCoroutine = null;
    }

    // -----------------------------
    // 🔹 Protect player on collision
    // -----------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isShieldActive)
        {
            Debug.Log("Shield absorbed the hit!");
            return; // Ignore collision
        }

        // Normal game over logic here
        Debug.Log("Game Over!");
        // GameOver();
    }
}
