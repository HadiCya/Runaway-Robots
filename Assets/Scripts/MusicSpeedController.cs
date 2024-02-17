//UMGPT Response: In Unity3D, how can I have background music change speed depending on the amount of levels completed?

using UnityEngine;

public class MusicSpeedController : MonoBehaviour
{
    public float startingPitch = 0.85f; // normal speed
    public float pitchIncrement = 0.05f; // pitch increment after a certain number of levels
    public int levelThreshold = 2; // number of levels to complete before increasing speed
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = startingPitch; // Set initial audio pitch
    }

    public void UpdateMusicSpeed(int levelsCompleted)
    {
        if (audioSource != null)
        {
            // Adjust pitch based on the number of levels completed and level threshold
            audioSource.pitch = startingPitch + (pitchIncrement * (levelsCompleted / levelThreshold));

            // Optional: clamp the pitch to prevent it from becoming too high or too low
            audioSource.pitch = Mathf.Clamp(audioSource.pitch, 0.5f, 2.0f);
        }
    }
}