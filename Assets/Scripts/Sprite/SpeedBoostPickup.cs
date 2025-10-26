using UnityEngine;

public class SpeedBoostPickup : MonoBehaviour
{
    public float slowMultiplier = 0.4f;   // 0.4 = 60% slower
    public float slowDuration = 3f;
    public float energyLoss = 20f;        // how much energy to remove
    public AudioSource slipSound;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // Play slip sound
            if (slipSound != null)
                slipSound.Play();

            // Slow player
            player.StartSpeedBoost(slowMultiplier, slowDuration);

            // ✅ Reduce energy
            player.player.ReduceEnergy(energyLoss);
        }
    }
}
