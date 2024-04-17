using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;

    public static event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.Invade);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Invade:
                HandleInvade();
                break;
            case GameState.Escape:
                HandleEscape();
                break;
            case GameState.Win:
                HandleWin();
                break;
            case GameState.Lose:
                HandleLose();
                break;
        }

        OnGameStateChange?.Invoke(newState);
    }

    private void HandleInvade()
    {

    }

    private void HandleEscape()
    {
        //Security guards chase player;
        EnemyManager.Instance.canFollow = true;
    }

    private void HandleWin()
    {

    }

    private void HandleLose()
    {

    }
}

public enum GameState
{
    Invade,
    Escape,
    Win,
    Lose
}
