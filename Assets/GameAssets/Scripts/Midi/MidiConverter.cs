using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiConverter : MonoBehaviour
{

    public void JsonConvert(TextAsset jsonFile, out Queue<float> leftQueue, out Queue<float> upQueue, out Queue<float> rightQueue, out Queue<float> downQueue)
    {
        Notes notesInJson = JsonUtility.FromJson<Notes>(jsonFile.text);
        leftQueue = new Queue<float>();
        upQueue = new Queue<float>();
        rightQueue = new Queue<float>();
        downQueue = new Queue<float>();

        foreach (Note note in notesInJson.notes)
        {
            char direction = note.name[0];
            if(direction == 'C')
            {
                leftQueue.Enqueue(note.time);
            }
            else if (direction == 'D')
            {
                upQueue.Enqueue(note.time);
            }
            else if(direction == 'E')
            {
                rightQueue.Enqueue(note.time);
            }
            else if (direction == 'F')
            {
                downQueue.Enqueue(note.time);
            }
        }
    }
}
