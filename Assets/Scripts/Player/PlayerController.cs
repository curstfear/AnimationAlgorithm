using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //ссылка на объект StateMachine
    StateMachine _stateMachine;

    [SerializeField] private float _speed;
    [SerializeField] private float attackDuration = 0.7f;

    private bool isAttacking = false;
    private bool isHurt = false;

    public Animator _animator;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
        _stateMachine.currentState = StateMachine.State.Idle;

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
            _stateMachine.ChangeState(StateMachine.State.Run);
        }
        else
        {
            _stateMachine.ChangeState(StateMachine.State.Idle);
        }
    }

    // метод для атаки (чисто анимка)
    private void Attack()
    {
        isAttacking = true;
        _stateMachine.ChangeState(StateMachine.State.Attack);
        _speed = 0f;
        StartCoroutine(EndAttack());
    }

    // корутина окончания атаки
    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;

        if (_stateMachine.currentState == StateMachine.State.Attack)
        {
            _stateMachine.ChangeState(StateMachine.State.Idle);
            _speed = 8f;
        }
    }

    //метод получения урона (отталкивание персонажа и анимка)
    void Hurt(Vector2 damageSource)
    {
        if (isHurt) return;

        isHurt = true;
        _stateMachine.ChangeState(StateMachine.State.Hurt);

        Vector2 knockbackDirection = (Vector2)transform.position - damageSource;
        knockbackDirection.Normalize();
        _rb.AddForce(knockbackDirection * 5, ForceMode2D.Impulse);

        StartCoroutine(RecoverFromHurt());
    }

    // метод для окончания анимации получения урона
    private IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(0.6f);
        isHurt = false;

        _stateMachine.ChangeState(StateMachine.State.Idle);
    }

    //столкновение (только с шипом)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Spike")
        {
            Hurt(collision.gameObject.transform.position);
        }
    }
}
