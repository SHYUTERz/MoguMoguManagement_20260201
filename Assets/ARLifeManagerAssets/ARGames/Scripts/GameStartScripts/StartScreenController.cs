using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject startPanel;       // スタートUI
    [SerializeField] private GameObject initialSetupPanel; // 初期設定UI

    [Header("Scene Names")]
    [SerializeField] private string homeSceneName = "Home";

    private void Start()
    {
        // 初期はスタート画面を表示、初期設定は隠す
        startPanel.SetActive(true);
        initialSetupPanel.SetActive(false);
    }

    // スタートボタン押下
    public void OnClickStart()
    {
        if (SaveManager.HasSave())
        {
            // DBはHomeで読むのでここでは何もしない
            SceneManager.LoadScene(homeSceneName);
            return;
        }

        // 新規ユーザー：初期設定UIへ
        startPanel.SetActive(false);
        initialSetupPanel.SetActive(true);
    }

    // 「ゲームデータ新規作成」押下
    public void OnClickCreateNewGame()
    {
        // テーブル作成して保存
        SaveManager.CreateNewGameDatabase();

        // Homeへ
        SceneManager.LoadScene(homeSceneName);
    }

    // 「以前バックアップされたゲームデータの読み込み」押下
    public void OnClickImportBackup()
    {
        // プロトタイプ案：
        // 1) まずは固定のインポートパス（例: persistentDataPath/Import/backup.mogumogu）から読み込む
        // 2) 後でNativeFilePicker等を入れて、ファイル選択できるようにする
        var imported = SaveManager.ImportBackupFromDefaultLocation();

        if (imported)
        {
            SceneManager.LoadScene(homeSceneName);
        }
        else
        {
            Debug.LogWarning("バックアップが見つからない、または拡張子が違います。");
        }
    }
}
