using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;
    private Transform spawnPoint;
    private Transform endPoint;
    private Lane lane;

    // Pass the spawn and end points from the Lane script
    public void Initialize(Transform spawnNotePoint, Transform endNotePoint, Lane lane)
    {
        spawnPoint = spawnNotePoint;
        endPoint = endNotePoint;
        this.lane = lane;

    }

    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
    }

    void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));

        // despawn the note if it's x cords are reached 0
        float x = transform.position.x;
        if (Mathf.Abs(x) <= 0.05)
        {
            Destroy(gameObject);
        }


        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            // Move the note from spawnPoint to endPoint based on time
            transform.position = Vector3.Lerp(spawnPoint.position, endPoint.position, t);
            GetComponentInChildren<SpriteRenderer>().enabled = true;
        }


    }
}
