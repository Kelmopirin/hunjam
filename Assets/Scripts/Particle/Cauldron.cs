using UnityEngine;

public class Cauldron : MonoBehaviour
{
    public ParticleSystem particles;  // Assign in Inspector
    public float particleDuration = 2f; // seconds
    private AudioSource audioSource;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        if (particles == null) return;

        // Clear existing particles so it restarts fresh
        particles.Clear();

        // Re-enable emission in case it was disabled
        var emission = particles.emission;
        emission.enabled = true;

        // Play particles
        particles.Play();

        // Stop emitting new particles after duration, let existing ones fade
        Invoke(nameof(StopEmitting), particleDuration);

        audioSource.Play();
    }


    private void StopEmitting()
    {
        if (particles == null) return;

        var emission = particles.emission;
        emission.enabled = false; // stop spawning new particles
    }
}
