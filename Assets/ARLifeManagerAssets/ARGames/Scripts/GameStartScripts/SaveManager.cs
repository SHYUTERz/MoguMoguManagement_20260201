using System;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    // アプリ専用拡張子
    public const string DbExtension = ".mogumogu";
    public const string DbFileName = "save" + DbExtension;

    // バックアップ読み込み（プロトタイプ用の固定場所）
    // 例：スマホならこのフォルダに置いたものを拾う（後でファイルピッカーに差し替え）
    public const string ImportFolderName = "Import";
    public const string ImportFileName = "backup" + DbExtension;

    public static string GetDbPath()
    {
        return Path.Combine(Application.persistentDataPath, DbFileName);
    }

    public static bool HasSave()
    {
        return File.Exists(GetDbPath());
    }

    public static void CreateNewGameDatabase()
    {
        var dbPath = GetDbPath();
        var dir = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        // 破損対策：一時ファイルに作ってから置き換える
        var tempPath = dbPath + ".tmp";

        if (File.Exists(tempPath)) File.Delete(tempPath);

        // 1) 空ファイル作成
        using (File.Create(tempPath)) { }

        // 2) SQLiteで接続してテーブル作成
        // ※ここはあなたのSQLite実装に合わせて差し替え
        SchemaCreator.CreateAllTables(tempPath);

        // 3) 既存があれば退避して置換
        if (File.Exists(dbPath))
        {
            var bakPath = dbPath + ".bak";
            File.Copy(dbPath, bakPath, overwrite: true);
            File.Delete(dbPath);
        }

        File.Move(tempPath, dbPath);

        Debug.Log($"New DB created: {dbPath}");
    }

    public static bool ImportBackupFromDefaultLocation()
    {
        var importPath = Path.Combine(Application.persistentDataPath, ImportFolderName, ImportFileName);
        return ImportBackup(importPath);
    }

    public static bool ImportBackup(string sourcePath)
    {
        if (!File.Exists(sourcePath)) return false;
        if (Path.GetExtension(sourcePath) != DbExtension) return false;

        var dbPath = GetDbPath();
        var dir = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        // 既存があればバックアップ
        if (File.Exists(dbPath))
        {
            File.Copy(dbPath, dbPath + ".bak", overwrite: true);
        }

        File.Copy(sourcePath, dbPath, overwrite: true);
        Debug.Log($"Imported backup to: {dbPath}");
        return true;
    }
}