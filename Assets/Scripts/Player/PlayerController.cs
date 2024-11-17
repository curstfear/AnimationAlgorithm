using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Run,
        Attack,
        Hurt
    }
    private State currentState;

    [SerializeField] private float _speed;
    private float attackDuration = 0.7f;
    private bool isAttacking;
    private bool isHurt;

    private Animator _animator;
    private Rigidbody2D _rb;

    private void Awake()
    {
        currentState = State.Idle;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isAttacking && !isHurt)
        {
            Movement();
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking && !isHurt)
        {
            Attack();
        }
    }

    private void Movement()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        Vector3 moveAmount = moveInput.normalized * _speed * Time.fixedDeltaTime;
        transform.position += moveAmount;

        if (moveInput.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1, 1);
            ChangeState(State.Run);
        }
        else
        {
            ChangeState(State.Idle);
        }
    }

    // метод для атаки (чисто анимка)
    private void Attack()
    {
        isAttacking = true;
        ChangeState(State.Attack);
        _speed = 0f;
        StartCoroutine(EndAttack());
    }

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;

        if (currentState == State.Attack)
        {
            ChangeState(State.Idle);
            _speed = 8f;
        }
    }

    //метод получения урона (тут чисто отталкивание персонажа и анимка)
    void Hurt(Vector2 damageSource)
    {
        if (isHurt) return;

        isHurt = true;
        ChangeState(State.Hurt);

        Vector2 knockbackDirection = (Vector2)transform.position - damageSource;
        knockbackDirection.Normalize();
        _rb.AddForce(knockbackDirection * 5, ForceMode2D.Impulse);

        StartCoroutine(RecoverFromHurt());
    }

    private IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(0.6f);
        isHurt = false;

        ChangeState(State.Idle);
    }

    //столкновение (ток с шипом)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Spike")
        {
            Hurt(collision.gameObject.transform.position);
        }
    }

    //смена состояний
    private void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case State.Idle:
                _animator.Play("Idle");
                break;
            case State.Run:
                _animator.Play("Run");
                break;
            case State.Attack:
                _animator.Play("Attack");
                break;
            case State.Hurt:
                _animator.Play("Hurt");
                break;
        }
    }

    
}
