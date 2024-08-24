using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    [SerializeField] private TextMeshProUGUI totalDodgedBulletText;
    [SerializeField] private TextMeshProUGUI totalHitBulletText;
    [SerializeField] private TextMeshProUGUI totalHitNoteText;
    [SerializeField] private TextMeshProUGUI maxNoteComboText;
    [SerializeField] private TextMeshProUGUI totalDamageDealedText;

    private string title;
    private float suvivalTime;
    private int dodgedBullet;
    private int hitBullet;
    private int hitNote;
    private int maxCombo;
    private int damageDealed;

    private bool isPaused = false;
    private bool canPause = true;

    [SerializeField] private int timeBeforeAppear;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SongManager.Instance == null)
        {
            Debug.LogError("SongManager is missing in the scene");
            return;
        }
        SongManager.Instance.OnSongEnd += OnSongEnd;
        Player.Instance.GetComponent<HealthSystem>().OnPlayerDeath += OnPlayerDeath;
        HideEndPanel();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            SongManager.Instance.audioSource.UnPause();
        }
        else
        {
            Time.timeScale = 0;
            isPaused = true;
            SongManager.Instance.audioSource.Pause();
        }
    }

    private void OnSongEnd()
    {
        if (endPanel.activeSelf) return;
        if (FindObjectOfType<Enemy>() == null)
        {
            title = "Emeny Neutralized";
        }
        else
        {
            title = "Survivor's Path";
        }

        GameOver();
        Invoke("ShowEndPanel", timeBeforeAppear);
        canPause = false;
    }

    private void OnPlayerDeath()
    {
        title = "Game Over";
        GameOver();
        ShowEndPanel();

        canPause = false;
        isPaused = false;
        Pause();
    }

    private void ShowEndPanel()
    {
        endPanel.SetActive(true);
    }

    private void HideEndPanel()
    {
        endPanel.SetActive(false);
    }

    private void UpdateEndPanel()
    {
        titleText.text = title;
        survivalTimeText.text = suvivalTime.ToString("F2");
        totalDodgedBulletText.text = dodgedBullet.ToString();
        totalHitBulletText.text = hitBullet.ToString();
        totalHitNoteText.text = hitNote.ToString();
        maxNoteComboText.text = maxCombo.ToString();
        totalDamageDealedText.text = damageDealed.ToString();
    }

    public void GameOver()
    {
        suvivalTime = SongManager.Instance.audioSource.time;
        dodgedBullet = ScoreSystem.Instance.TotalDodgedBullets;
        hitBullet = ScoreSystem.Instance.TotalHitBullets;
        hitNote = ScoreSystem.Instance.TotalHitNotes;
        maxCombo = ScoreSystem.Instance.MaxHitNotesCombo;
        damageDealed = ScoreSystem.Instance.TotalDamageDealed;

        UpdateEndPanel();
    }

    private void OnDestroy()
    {
        SongManager.Instance.OnSongEnd -= OnSongEnd;
        if (Player.Instance != null)
        {
            Player.Instance.GetComponent<HealthSystem>().OnPlayerDeath -= OnPlayerDeath;
        }
    }
}
