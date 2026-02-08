using System.IO;
using UnityEngine;

/// <summary>
/// セーブDBファイルの存在チェック / 新規作成 / インポートを扱う。
/// Start画面は「存在チェックだけ」を行い、DBの読み込みはHomeシーン側で実施する想定。
/// </summary>
public static class SaveManager
{
    public const string DbExtension = ".mogumogu";
    public const string DbFileName = "save" + DbExtension;

    public static string GetDbPath()
    {
        return Path.Combine(Application.persistentDataPath, DbFileName);
    }

    public static bool HasSave()
    {
        return File.Exists(GetDbPath());
    }

    /// <summary>
    /// 新規ゲームDBを生成する（tmpで生成→既存があればbak退避→置換）
    /// </summary>
    public static void CreateNewGameDatabase()
    {
        var dbPath = GetDbPath();
        var dir = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        var tempPath = dbPath + ".tmp";
        if (File.Exists(tempPath)) File.Delete(tempPath);

        using (File.Create(tempPath)) { }

        SchemaCreator.CreateAllTables(tempPath);

        if (File.Exists(dbPath))
        {
            File.Copy(dbPath, dbPath + ".bak", overwrite: true);
            File.Delete(dbPath);
        }

        File.Move(tempPath, dbPath);
        Debug.Log($"[SaveManager] New DB created: {dbPath}");
    }

    public static bool ImportBackup(string sourcePath)
    {
        if (!File.Exists(sourcePath)) return false;
        if (Path.GetExtension(sourcePath) != DbExtension) return false;

        var dbPath = GetDbPath();
        var dir = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        if (File.Exists(dbPath))
            File.Copy(dbPath, dbPath + ".bak", overwrite: true);

        File.Copy(sourcePath, dbPath, overwrite: true);
        Debug.Log($"[SaveManager] Imported backup to: {dbPath}");
        return true;
    }
}
