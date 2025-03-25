using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotChangeEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public void ChangeAudioMixerSnapshot(string snapshotName)
    {
        AudioMixerManager.Instance.ChangeMusicSnapshot(snapshotName, 0);
    }
}
