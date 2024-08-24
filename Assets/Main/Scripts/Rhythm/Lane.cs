using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    //public KeyCode input;
    public bool isLeftLane;
    public GameObject notePrefab;
    public BulletSpawner bulletSpawner; // Reference to the BulletSpawner
    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();
    public Transform spawnNotePoint;
    public Transform endNotePoint;

    public int spawnIndex = 0;
    public int inputIndex = 0;

    void Start()
    {
        // Ensure bulletSpawner is assigned
        if (!bulletSpawner && !notePrefab)
        {
            Debug.LogError("BulletSpawner not assigned in Lane.");
        }
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    void Update()
    {
        if (!notePrefab)
        {
            // Spawn bullets based on timeStamps
            if (spawnIndex < timeStamps.Count)
            {
                if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex])
                {
                    // Trigger BulletSpawner
                    bulletSpawner.Fire(); // You can adjust this if you need to pass specific parameters

                    spawnIndex++;
                }
            }
        }
        else
        {
            // Spawn note visuals based on timeStamps
            if (spawnIndex < timeStamps.Count)
            {
                if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
                {
                    var note = Instantiate(notePrefab, transform).GetComponent<Note>();
                    notes.Add(note);
                    note.assignedTime = (float)timeStamps[spawnIndex];

                    // Initialize the note with spawn and end points
                    note.Initialize(spawnNotePoint, endNotePoint, this);

                    spawnIndex++;
                }
            }

            // Input handling and scoring
            if (inputIndex < timeStamps.Count)
            {
                double timeStamp = timeStamps[inputIndex];
                double marginOfError = SongManager.Instance.marginOfError;
                double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

                if ((isLeftLane && PlayerInput.Instance.TapLeft()) || (!isLeftLane && PlayerInput.Instance.TapRight()))
                {
                    if (Math.Abs(audioTime - timeStamp) < marginOfError)
                    {
                        Hit();
                        //print($"Hit on {inputIndex} note");
                        if (notes[inputIndex])
                        {
                            Destroy(notes[inputIndex].gameObject);
                            inputIndex++;
                        }
                    }
                    else
                    {
                        //print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
                    }
                }
                if (timeStamp + marginOfError <= audioTime)
                {
                    Miss();
                    //print($"Missed {inputIndex} note");
                    inputIndex++;
                }
            }
        }
    }

    private void Hit()
    {
        ScoreSystem.Instance.NoteHit();
        if (Player.Instance)
        {
            Player.Instance.GetComponent<HealthSystem>().IncreaseRegenCharge();
            Player.Instance.GetComponent<PlayerAttack>().Attack();
        }
    }

    private void Miss()
    {
        ScoreSystem.Instance.NoteMiss();
    }
}
