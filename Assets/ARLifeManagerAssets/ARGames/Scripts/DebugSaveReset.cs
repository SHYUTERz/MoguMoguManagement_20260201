using System.IO;
using UnityEngine;

public class DebugSaveReset : MonoBehaviour
{
    // SaveManager と同じ定義に合わせる（あなたの設定に合わせて変更OK）
    private const string DbExtension = ".mogumogu";
    private const string DbFileName = "save" + DbExtension;

    // 追加で消したい派生ファイル（.bak や .tmp）も消す
    private static readonly string[] ExtraSuffixes = { ".bak", ".tmp", ".tmp-shm", ".tmp-wal", "-shm", "-wal" };

    /// <summary>
    /// UIボタンから呼ぶ用：セーブDBを完全削除
    /// </summary>
    public void DeleteSaveDb()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        var dbPath = Path.Combine(Application.persistentDataPath, DbFileName);

        // 本体
        DeleteFileIfExists(dbPath);

        // ありがちな派生ファイルも削除（SQLiteのwal/shm、バックアップなど）
        foreach (var suf in ExtraSuffixes)
        {
            DeleteFileIfExists(dbPath + suf);
        }

        // 念のため persistentDataPath 直下に残ったmogumoguを掃除したい場合（任意）
        // DeleteAllMogumoguInPersistentDataPath();

        Debug.Log($"[DebugSaveReset] Deleted save db: {dbPath}");
#else
        Debug.LogWarning("[DebugSaveReset] Disabled in non-development builds.");
#endif
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private static void DeleteFileIfExists(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException e)
        {
            Debug.LogWarning($"[DebugSaveReset] Failed to delete: {path}\n{e}");
        }
    }

    // もし「save以外のmogumogu」も全部消したいなら使う
    private static void DeleteAllMogumoguInPersistentDataPath()
    {
        try
        {
            var files = Directory.GetFiles(Application.persistentDataPath, "*" + DbExtension, SearchOption.TopDirectoryOnly);
            foreach (var f in files) File.Delete(f);
        }
        catch (IOException e)
        {
            Debug.LogWarning($"[DebugSaveReset] Failed to delete all mogumogu files.\n{e}");
        }
    }
#endif
}
