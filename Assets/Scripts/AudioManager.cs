using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("--- Audio Sources ---")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("--- Background Music ---")]
    [SerializeField] private AudioClip bgmClip;

    [Header("--- SFX Clips ---")]
    public AudioClip towerShoot;
    public AudioClip enemyDeath;
    public AudioClip menuOpen;
    public AudioClip menuClose;
    public AudioClip buyTower;
    public AudioClip sellTower;
    public AudioClip upgradeTower;
    public AudioClip notEnoughCoins;
    public AudioClip waveStart;
    public AudioClip lifeLost;
    public AudioClip gameWin;
    public AudioClip gameOver;
    public AudioClip enemyMove;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmClip != null && musicSource != null)
        {
            musicSource.clip = bgmClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Shortcut methods for common sounds
    public void PlayTowerShoot() => PlaySFX(towerShoot);
    public void PlayEnemyDeath() => PlaySFX(enemyDeath);
    public void PlayMenuOpen() => PlaySFX(menuOpen);
    public void PlayMenuClose() => PlaySFX(menuClose);
    public void PlayBuyTower() => PlaySFX(buyTower);
    public void PlaySellTower() => PlaySFX(sellTower);
    public void PlayUpgradeTower() => PlaySFX(upgradeTower);
    public void PlayNotEnoughCoins() => PlaySFX(notEnoughCoins);
    public void PlayWaveStart() => PlaySFX(waveStart);
    public void PlayLifeLost() => PlaySFX(lifeLost);
    public void PlayGameWin() => PlaySFX(gameWin);
    public void PlayGameOver() => PlaySFX(gameOver);
    public void PlayEnemyMove() => PlaySFX(enemyMove);
}
