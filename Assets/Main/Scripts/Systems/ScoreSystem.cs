using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance;

    // UI
    [SerializeField] private TMPro.TextMeshProUGUI totalDodgedBulletsText;
    [SerializeField] private TMPro.TextMeshProUGUI dodgedBulletsComboText;

    private int totalDodgedBullets = 0;
    private int dodgedBulletsCombo = 0;
    private int totalHitBullets = 0;

    private int totalHitNotes = 0;
    private int totalHitNotesCombo = 0;
    private int maxHitNotesCombo = 0;

    private int totalDamageDealed = 0;

    public int TotalDodgedBullets { get => totalDodgedBullets; set => totalDodgedBullets = value; }
    public int TotalHitBullets { get => totalHitBullets; set => totalHitBullets = value; }
    public int TotalHitNotes { get => totalHitNotes; set => totalHitNotes = value; }
    public int TotalHitNotesCombo { get => totalHitNotesCombo; set => totalHitNotesCombo = value; }
    public int MaxHitNotesCombo { get => maxHitNotesCombo; set => maxHitNotesCombo = value; }
    public int TotalDamageDealed { get => totalDamageDealed; set => totalDamageDealed = value; }

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateScore();
    }

    public void IncreaseDodgedBullets()
    {
        TotalDodgedBullets++;
        dodgedBulletsCombo++;
    }

    public void IncreaseHitBullets()
    {
        TotalHitBullets++;
    }

    public void IncreaseDamageDealed(int damage)
    {
        TotalDamageDealed += damage;
    }

    public void ResetCombo()
    {
        dodgedBulletsCombo = 0;
    }

    public void UpdateScore()
    {
        if (totalDodgedBulletsText == null || dodgedBulletsComboText == null)
        {
            return;
        }

        totalDodgedBulletsText.text = TotalDodgedBullets.ToString();
        dodgedBulletsComboText.text = dodgedBulletsCombo.ToString();
    }

    public void NoteHit()
    {
        TotalHitNotes++;
        TotalHitNotesCombo++;

        if (TotalHitNotesCombo > MaxHitNotesCombo)
        {
            MaxHitNotesCombo = TotalHitNotesCombo;
        }
    }

    public void NoteMiss()
    {
        TotalHitNotesCombo = 0;
    }

}
