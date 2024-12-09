using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    PlayerController _playerController;
    public enum State
    {
        Idle,
        Run,
        Attack,
        Hurt
    }

    public State currentState;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case State.Idle:
                _playerController._animator.Play("Idle");
                break;
            case State.Run:
                _playerController._animator.Play("Run");
                break;
            case State.Attack:
                _playerController._animator.Play("Attack");
                break;
            case State.Hurt:
                _playerController._animator.Play("Hurt");
                break;
        }
    }
}
