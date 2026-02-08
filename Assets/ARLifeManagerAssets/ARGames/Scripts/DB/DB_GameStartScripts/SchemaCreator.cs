using System;
using SQLite4Unity3d;
using UnityEngine;

/// <summary>
/// TSV（列定義 + sql列）を読み込み、sql列に入っているSQL「1文」だけを順番に実行してスキーマを作る。
///
/// 前提（重要）
/// - TSVは Resources から TextAsset として読み込む（配置: Assets/Resources/SchemaTSV/*.tsv）
/// - TSVは「テーブルごとに1ファイル」
/// - TSVの1行目はヘッダー。必ず "sql" 列を含むこと
/// - "sql" 列は「1ファイルにつき1文のみ」
///   - 通常は、先頭データ行（ordinal=1）の sql セルだけにSQLを入れる
/// - SQL末尾に ';' を付けない（付いていてもSQLiteは通ることが多いが、運用ルールとして禁止）
///
/// 目的
/// - “列定義はTSVで見える化”しつつ、実装は “sql列を実行するだけ” にしてコードを短く保つ。
/// </summary>
public static class SchemaCreator
{
    /// <summary>
    /// Resources/SchemaTSV/ 配下の拡張子なしパス（実行順が命）
    /// ※TSVを増やすときは、ここに追加するだけでOK
    /// </summary>
    private static readonly string[] ScriptPaths =
    {
        "SchemaTSV/Schema_Version/Schema_Version__schema_version",
        "SchemaTSV/Character_Info/Character_Info__type_table",
        "SchemaTSV/Character_Info/Character_Info__variety_table",
        "SchemaTSV/Character_Info/Character_Info__personality_table",
        "SchemaTSV/Character_Info/Character_Info__gs_table",
        "SchemaTSV/Character_Info/Character_Info__ds_table",
        "SchemaTSV/Character_Info/Character_Info__state_table",
        "SchemaTSV/Character_Info/Character_Info__pet_table",
        "SchemaTSV/Character_Info/Character_Info__icon_table",
        "SchemaTSV/User_Info/User_Info__userInfo_table",
        "SchemaTSV/User_Info/User_Info__seed_userInfo",
    };

    /// <summary>
    /// DBファイル（dbFilePath）に対して、TSVに書かれたSQLを順番に実行する。
    /// 新規DB生成のタイミングで呼ばれる想定（SaveManager.CreateNewGameDatabase() など）。
    /// </summary>
    public static void CreateAllTables(string dbFilePath)
    {
        // ここでDBを開く（無ければ作られる）
        var flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create;
        using var conn = new SQLiteConnection(dbFilePath, flags);

        // 外部キー制約を使う（CREATE文にFOREIGN KEYを書いた場合に効く）
        conn.Execute("PRAGMA foreign_keys = ON;");

        Debug.Log($"[SchemaCreator] Start applying schema TSVs. dbPath={dbFilePath}");
        Debug.Log($"[SchemaCreator] TSV count = {ScriptPaths.Length}");

        for (int i = 0; i < ScriptPaths.Length; i++)
        {
            var path = ScriptPaths[i];

            // 1) TSVファイルをResourcesからロード
            //    Resources/SchemaTSV/xxxx.tsv なら "SchemaTSV/xxxx" を渡す
            var tsvAsset = Resources.Load<TextAsset>(path);

            if (tsvAsset == null)
            {
                // ここで落ちる場合はパスの指定ミスか、Resources配置ミスの可能性が高い
                Debug.LogError($"[SchemaCreator] TSV not found in Resources: {path}");
                throw new Exception($"TSV not found in Resources: {path}");
            }

            // 2) TSVの "sql" 列から、最初に見つかった非空セルを取り出す
            //    （運用ルール: 1ファイルにつき1文のみ）
            var sql = ExtractSqlFromTsv(tsvAsset.text, out var tableName);

            if (string.IsNullOrWhiteSpace(sql))
            {
                Debug.LogError($"[SchemaCreator] SQL column is empty. path={path}, table={tableName}");
                throw new Exception($"SQL column is empty in TSV: {path}");
            }

            // 3) 実行ログ（どのTSVのSQLを実行しているか、後から追えるように）
            Debug.Log($"[SchemaCreator] ({i + 1}/{ScriptPaths.Length}) Apply: path={path}, table={tableName}");

            // 4) SQL実行
            //    失敗した場合、"どのSQLで落ちたか" が最重要なので SQL全文もログ出しする
            try
            {
                Debug.Log($"[SchemaCreator] SQL => {sql}");
                conn.Execute(sql);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SchemaCreator] SQL execution failed.\npath={path}\ntable={tableName}\nSQL={sql}\nException={e}");
                throw;
            }
        }

        Debug.Log("[SchemaCreator] Schema apply completed successfully.");
    }

    /// <summary>
    /// TSVの "sql" 列から、最初に見つかった非空文字列を返す。
    ///
    /// 戻り値:
    /// - sql: 実行するSQL（1文）
    /// - tableName: TSVの "table" 列から取得（ログ用）
    ///
    /// 仕様:
    /// - ヘッダー行（1行目）から "sql" 列の位置を特定
    /// - 2行目以降を走査して、最初に見つかった非空 sql を採用
    /// </summary>
    private static string ExtractSqlFromTsv(string tsv, out string tableName)
    {
        tableName = "(unknown)";

        // 改行コード差（Windows/Unix）を吸収
        var lines = tsv.Replace("\r\n", "\n").Split('\n');
        if (lines.Length == 0) return "";

        // ヘッダー行
        var header = lines[0].Split('\t');

        // "sql" 列が無いTSVは規約違反なので即エラー
        int sqlIndex = Array.IndexOf(header, "sql");
        if (sqlIndex < 0) throw new Exception("TSV header must include 'sql' column.");

        // table列は「ログ用」。無くても動くが、あると便利
        int tableIndex = Array.IndexOf(header, "table");

        // 2行目以降（データ行）から探す
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cols = line.Split('\t');

            // table名（ログ用）を拾えるなら拾っておく
            if (tableIndex >= 0 && cols.Length > tableIndex && !string.IsNullOrWhiteSpace(cols[tableIndex]))
                tableName = cols[tableIndex].Trim();

            // sql列
            if (cols.Length <= sqlIndex) continue;

            var candidate = cols[sqlIndex].Trim();
            if (!string.IsNullOrEmpty(candidate))
                return candidate;
        }

        return "";
    }
}
