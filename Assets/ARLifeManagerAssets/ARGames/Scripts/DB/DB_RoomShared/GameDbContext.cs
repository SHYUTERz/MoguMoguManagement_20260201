using SQLite4Unity3d;

/// <summary>
/// Global DB context for BaseScene lifetime:
/// - Initialize once when BaseScene loads
/// - Keep the SQLiteConnection open to avoid room-switch lag
/// </summary>
public static class GameDbContext
{
    public static SQLiteConnection Conn { get; private set; }
    public static GameDataCache Cache { get; } = new GameDataCache();

    public static bool IsReady => Conn != null;

    public static void Initialize(string dbPath)
    {
        if (Conn != null) return;

        Conn = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite);
        Conn.Execute("PRAGMA foreign_keys = ON;");
    }

    public static void Shutdown()
    {
        if (Conn == null) return;
        try { Conn.Close(); } catch { /* ignore */ }
        Conn = null;
    }
}
