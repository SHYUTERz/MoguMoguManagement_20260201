using SQLite4Unity3d;

public sealed class SqlExecutor
{
    private readonly string _dbPath;

    public SqlExecutor(string dbPath)
    {
        _dbPath = dbPath;
    }

    public void Execute(string sql)
    {
        // Create も含める（新規作成時に必要）
        var flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create;

        using (var conn = new SQLiteConnection(_dbPath, flags))
        {
            conn.Execute(sql);
        }
    }
}