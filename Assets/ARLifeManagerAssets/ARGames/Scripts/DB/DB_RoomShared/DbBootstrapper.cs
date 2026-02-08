using UnityEngine;

/// <summary>
/// Put this on a GameObject in BaseScene.
/// It opens the DB once and (optionally) loads initial cache.
/// </summary>
public class DbBootstrapper : MonoBehaviour
{
    [SerializeField] private bool reloadCacheOnStart = true;

    private void Awake()
    {
        // Adjust these method names to your SaveManager if needed.
        string dbPath = SaveManager.GetDbPath();

        GameDbContext.Initialize(dbPath);

        if (reloadCacheOnStart)
            GameDbContext.Cache.ReloadAll(GameDbContext.Conn);
    }

    private void OnDestroy()
    {
        // If you ever leave BaseScene and want to close DB, uncomment:
        // GameDbContext.Shutdown();
    }
}
