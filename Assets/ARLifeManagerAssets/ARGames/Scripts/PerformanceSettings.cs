using UnityEngine;

public class PerformanceSettings : MonoBehaviour
{
    [SerializeField] int targetFps = 60;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFps;
    }
}