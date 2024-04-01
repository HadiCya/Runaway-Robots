//UMGPT Response: In Unity3D, how can I have background music change speed depending on the amount of levels completed?

using UnityEngine;

public class MusicSpeedController : MonoBehaviour
{
    private AudioSource audioSource;

    public float startingPitch = 0.75f; // Starting slower than the normal speed
    public float pitchMultiplier = 1.02f; // Multiplier for exponential growth (greater than 1)
    public int levelThreshold = 2; // Increase pitch after every 'levelThreshold' levels 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = startingPitch; // Set initial audio pitch
    }

    public void UpdateMusicSpeed(int levelsCompleted)
    {
        // Calculate the exponent based on the number of thresholds passed.
        int thresholdsPassed = levelsCompleted / levelThreshold;

        // Calculate new pitch with exponential growth based on thresholds passed.
        float newPitch = startingPitch * Mathf.Pow(pitchMultiplier, thresholdsPassed);

        // Clamp the new pitch value to prevent it from going too high or too low.
        audioSource.pitch = Mathf.Clamp(newPitch, 0.5f, 2.0f);
    }
}