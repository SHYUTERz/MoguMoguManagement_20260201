using SQLite4Unity3d;

public sealed class SimpleSqlExecutor
{
    private readonly string _dbPath;
    public SimpleSqlExecutor(string dbPath) => _dbPath = dbPath;

    public void Execute(string sql)
    {
        var flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create;
        using var conn = new SQLiteConnection(_dbPath, flags);
        conn.Execute(sql);
    }
}

public static class SchemaCreator
{
    public static void CreateAllTables(string dbFilePath)
    {
        var exec = new SimpleSqlExecutor(dbFilePath);

        exec.Execute("PRAGMA foreign_keys = ON;");

        // ===== userInfo_table =====
        // user_id: INTEGER PK AUTOINCREMENT
        // user_gamemode: 0=育成,1=食品管理
        // user_growing_pet: pet_table.pet_id を参照
        // user_wallet: 所持金
        // user_coin: ガチャコイン
        exec.Execute(@"
CREATE TABLE IF NOT EXISTS userInfo_table (
    user_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    user_gamemode INTEGER NOT NULL,
    user_growing_pet INTEGER NOT NULL,
    user_wallet INTEGER NOT NULL,
    user_coin INTEGER NOT NULL,
    app_last_closed_at INTEGER NOT NULL,
    app_last_opened_at INTEGER NOT NULL
);");

        // ===== master tables =====
        exec.Execute(@"
CREATE TABLE IF NOT EXISTS type_table (
    type_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    type_name TEXT NOT NULL
);");

        exec.Execute(@"
CREATE TABLE IF NOT EXISTS variety_table (
    variety_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    variety_name TEXT NOT NULL
);");

        exec.Execute(@"
CREATE TABLE IF NOT EXISTS personality_table (
    personality_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    personality_name TEXT NOT NULL
);");

        exec.Execute(@"
CREATE TABLE IF NOT EXISTS gs_table (
    gs_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    gs_name TEXT NOT NULL
);");

        exec.Execute(@"
CREATE TABLE IF NOT EXISTS ds_table (
    ds_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    ds_name TEXT NOT NULL
);");

        // ===== state_table =====
        // state_id: PK
        // state_moisture..state_decay: INTEGER NOT NULL
        exec.Execute(@"
CREATE TABLE IF NOT EXISTS state_table (
    state_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    state_moisture INTEGER NOT NULL,
    state_nutrients INTEGER NOT NULL,
    state_sunlight INTEGER NOT NULL,
    state_pest INTEGER NOT NULL,
    state_health INTEGER NOT NULL,
    state_stress INTEGER NOT NULL,
    state_decay INTEGER NOT NULL
);");

        // ===== pet_table =====
        // pet_dob: UNIX timestamp (INTEGER)
        // pet_pl: 前世pet_id。先祖は0
        // pet_icon/type/variety/personality/state/gs/ds は各テーブルのId
        exec.Execute(@"
CREATE TABLE IF NOT EXISTS pet_table (
    pet_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    pet_icon INTEGER NOT NULL,
    pet_name TEXT NOT NULL,
    pet_type INTEGER NOT NULL,
    pet_variety INTEGER NOT NULL,
    pet_personality INTEGER NOT NULL,
    pet_dob INTEGER NOT NULL,
    pet_age INTEGER NOT NULL,
    pet_state INTEGER NOT NULL,
    pet_tr INTEGER NOT NULL,
    pet_gs INTEGER NOT NULL,
    pet_ds INTEGER NOT NULL,
    pet_pl INTEGER NOT NULL
);");

        // ===== icon_table =====
        // icon_image: BLOB
        exec.Execute(@"
CREATE TABLE IF NOT EXISTS icon_table (
    icon_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    icon_image BLOB NOT NULL,
    pet_id INTEGER NOT NULL
);");

        // --- 外部キー制約（PDF上は「外部キー」指定があるので、ここで追加） ---
        // SQLiteでは CREATE TABLE に直接書くのが一般的だけど、
        // 既存テーブルがある場合 ALTER TABLE で追加できないため、
        // プロトタイプでは「論理的に参照する」運用でもOK。
        // ただし、最初からFKを効かせたいなら、上のCREATE文に FOREIGN KEY(...) を書き込む形へ移行してください。

        // スキーマバージョン（任意：移行管理）
        exec.Execute(@"
CREATE TABLE IF NOT EXISTS schema_version (
    id INTEGER PRIMARY KEY,
    version INTEGER NOT NULL,
    applied_at TEXT
);");
        exec.Execute("INSERT OR REPLACE INTO schema_version (id, version, applied_at) VALUES (1, 1, datetime('now'));");

        SeedInitialUserInfo(dbFilePath);
    }

    /// <summary>
    /// userInfo_table が空なら初期レコードを1件作成する
    /// </summary>
    public static void SeedInitialUserInfo(string dbFilePath)
    {
        var flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create;
        using var conn = new SQLiteConnection(dbFilePath, flags);

        // すでにレコードがあるなら何もしない
        var count = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM userInfo_table;");
        if (count > 0) return;

        // 初期値（暫定）
        // user_gamemode: 0=育成（一旦、育成モードで設定しておく）
        // user_growing_pet: 0（まだキャラ未作成のため）
        // user_wallet: 500（初期の所持金）
        // user_coin: 0
        // app_last_closed_at: 0（最後に閉じた日時）
        // app_last_opened_at: 0（現在日時）
        conn.Execute(@"
INSERT INTO userInfo_table
(user_gamemode, user_growing_pet, user_wallet, user_coin, app_last_closed_at, app_last_opened_at)
VALUES
(0, 0, 0, 0, 0, 0);
");
    }
}