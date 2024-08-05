using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerStop : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && ObjectiveManager.Instance.bigItemCollected >= 1)
        {
            //stop time
            FindObjectOfType<Timer>().runTime = false;
            //game state = win
            GameManager.Instance.UpdateGameState(GameState.Win);
        }
    }
}
