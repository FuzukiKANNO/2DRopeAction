using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }

    public void FadeOutBGM(float duration)
    {
        StartCoroutine(FadeOut(duration));
    }

    System.Collections.IEnumerator FadeOut(float duration)
    {
        float startVolume = audioSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
