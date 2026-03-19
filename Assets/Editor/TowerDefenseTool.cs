using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TowerDefenseTool : EditorWindow
{
    private Vector2 _scrollPos;
    private List<TowerData> _towerDataAssets = new List<TowerData>();
    private List<EnemyData> _enemyDataAssets = new List<EnemyData>();

    [MenuItem("Antigravity/Tower Defense Tool")]
    public static void ShowWindow()
    {
        GetWindow<TowerDefenseTool>("TD Tool");
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        GUILayout.Label("THIẾT LẬP GAME (SCENE SETUP)", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("1. Auto-Setup AudioManager", GUILayout.Height(30)))
        {
            SetupAudioManager();
        }
        if (GUILayout.Button("2. Kiểm Tra Lỗi Scene (Validator)", GUILayout.Height(30)))
        {
            ValidateScene();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        GUILayout.Label("QUẢN LÝ DỮ LIỆU (DATABASE)", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Tìm Tất Cả Tower & Enemy Data", GUILayout.Height(30)))
        {
            FindAssets();
        }

        if (_towerDataAssets.Count > 0)
        {
            EditorGUILayout.Space(5);
            GUILayout.Label("--- TRỤ (TOWERS) ---", EditorStyles.miniBoldLabel);
            foreach (var tower in _towerDataAssets)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(tower, typeof(TowerData), false);
                if (GUILayout.Button("Chọn", GUILayout.Width(50))) Selection.activeObject = tower;
                EditorGUILayout.EndHorizontal();
            }
        }

        if (_enemyDataAssets.Count > 0)
        {
            EditorGUILayout.Space(5);
            GUILayout.Label("--- QUÁI VẬT (ENEMIES) ---", EditorStyles.miniBoldLabel);
            foreach (var enemy in _enemyDataAssets)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(enemy, typeof(EnemyData), false);
                if (GUILayout.Button("Chọn", GUILayout.Width(50))) Selection.activeObject = enemy;
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        GUILayout.Label("XƯỞNG ÂM THANH (AUDIO WORKSHOP)", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Tạo Bộ Âm Thanh 8-bit (Shoot, Hit, Win...)", GUILayout.Height(30)))
        {
            GenerateAllSFX();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        GUILayout.Label("TIỆN ÍCH TEST (TEST UTILS)", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Thêm 1000 Tiền (Chỉ khi đang Play)", GUILayout.Height(30)))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(1000);
                Debug.Log("<color=yellow>Đã thêm 1000 tiền cho sếp!</color>");
            }
        }
        GUI.enabled = true;
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }

    private void SetupAudioManager()
    {
        GameObject amObj = GameObject.Find("AudioManager");
        if (amObj == null)
        {
            amObj = new GameObject("AudioManager");
            Undo.RegisterCreatedObjectUndo(amObj, "Create AudioManager");
            Debug.Log("<color=green>Đã tạo GameObject AudioManager mới!</color>");
        }

        AudioManager script = amObj.GetComponent<AudioManager>();
        if (script == null)
        {
            script = amObj.AddComponent<AudioManager>();
            Debug.Log("<color=green>Đã thêm script AudioManager!</color>");
        }

        // Create Music Source
        Transform musicT = amObj.transform.Find("MusicSource");
        if (musicT == null)
        {
            GameObject mObj = new GameObject("MusicSource");
            mObj.transform.SetParent(amObj.transform);
            script.musicSource = mObj.AddComponent<AudioSource>();
            script.musicSource.playOnAwake = false;
            Debug.Log("<color=green>Đã tạo MusicSource!</color>");
        }
        else
        {
            script.musicSource = musicT.GetComponent<AudioSource>();
        }

        // Create SFX Source
        Transform sfxT = amObj.transform.Find("SFXSource");
        if (sfxT == null)
        {
            GameObject sObj = new GameObject("SFXSource");
            sObj.transform.SetParent(amObj.transform);
            script.sfxSource = sObj.AddComponent<AudioSource>();
            script.sfxSource.playOnAwake = false;
            Debug.Log("<color=green>Đã tạo SFXSource!</color>");
        }
        else
        {
            script.sfxSource = sfxT.GetComponent<AudioSource>();
        }

        EditorUtility.SetDirty(script);
        Debug.Log("<b><color=cyan>AudioManager Setup Hoàn Tất!</color></b>");
    }

    private void ValidateScene()
    {
        bool hasError = false;

        if (GameObject.FindFirstObjectByType<Path>() == null)
        {
            Debug.LogError("[TD Tool] THIẾU: Không tìm thấy GameObject có script 'Path'!");
            hasError = true;
        }

        if (GameObject.FindFirstObjectByType<Spawner>() == null)
        {
            Debug.LogError("[TD Tool] THIẾU: Không tìm thấy GameObject có script 'Spawner'!");
            hasError = true;
        }

        if (GameObject.FindFirstObjectByType<UIController>() == null)
        {
            Debug.LogWarning("[TD Tool] CẢNH BÁO: Không tìm thấy UIController. Game có thể không có UI.");
        }

        if (GameObject.FindObjectsByType<Platform>(FindObjectsSortMode.None).Length == 0)
        {
            Debug.LogWarning("[TD Tool] CẢNH BÁO: Scene không có Platform nào để đặt trụ.");
        }

        if (!hasError)
        {
            Debug.Log("<color=green>[TD Tool] Scene có vẻ ổn! Không tìm thấy lỗi nghiêm trọng.</color>");
        }
    }

    private void FindAssets()
    {
        _towerDataAssets.Clear();
        _enemyDataAssets.Clear();

        string[] towerGuids = AssetDatabase.FindAssets("t:TowerData");
        foreach (string guid in towerGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            _towerDataAssets.Add(AssetDatabase.LoadAssetAtPath<TowerData>(path));
        }

        string[] enemyGuids = AssetDatabase.FindAssets("t:EnemyData");
        foreach (string guid in enemyGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            _enemyDataAssets.Add(AssetDatabase.LoadAssetAtPath<EnemyData>(path));
        }

        Debug.Log($"<color=cyan>Đã tìm thấy {_towerDataAssets.Count} TowerData và {_enemyDataAssets.Count} EnemyData.</color>");
    }

    private void GenerateAllSFX()
    {
        string dirPath = System.IO.Path.Combine(Application.dataPath, "Audio/Generated");
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        SaveAndRefresh("Shoot.wav", ProceduralAudio.GenerateShoot(), dirPath);
        SaveAndRefresh("Death.wav", ProceduralAudio.GenerateDeath(), dirPath);
        SaveAndRefresh("Click.wav", ProceduralAudio.GenerateClick(), dirPath);
        SaveAndRefresh("Win.wav", ProceduralAudio.GenerateWin(), dirPath);
        SaveAndRefresh("Lose.wav", ProceduralAudio.GenerateLose(), dirPath);
        SaveAndRefresh("Coin.wav", ProceduralAudio.GenerateCoin(), dirPath);
        SaveAndRefresh("Spend.wav", ProceduralAudio.GenerateSpend(), dirPath);

        AssetDatabase.Refresh();
        Debug.Log("<b><color=green>[TD Tool] Sản xuất âm thanh hoàn tất! Kiểm tra Assets/Audio/Generated nhé sếp!</color></b>");
    }

    private void SaveAndRefresh(string fileName, byte[] bytes, string dir)
    {
        string fullPath = System.IO.Path.Combine(dir, fileName);
        File.WriteAllBytes(fullPath, bytes);
        Debug.Log($"<color=white>Đã tạo: {fileName}</color>");
    }
}
