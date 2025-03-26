using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private CharacterController _charController;        // для хранения данных о столкновении
    private Animator _animator;

    public float MoveSpeed = 4f;
    private float _baseSpeed;

    private readonly float _sprintMult = 1.5f;
    public bool IsShootingOrReloading { get; set; } = false; // Флаг стрельбы

    public float RotSpeed = 6.0f;
    private float _vertSpeed;
    private readonly float _gravity = -9.8f;
    private readonly float _maxFallSpeed = -10f;

    [Space]
    [Space]
    [SerializeField] private GameObject[] _jetpackFire;
    private readonly float _jetpackForce = 10f;
    private readonly float _maxJetpackSpeed = 5f;
    private bool _isUsingJetpack = false;
    [NonSerialized] public bool jetpackActivated = false;

    private bool _playerAlreadyDead = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _baseSpeed = MoveSpeed;
    }

    private void Start()
    {
        _charController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Managers.Player.Shield == 0 && !_playerAlreadyDead)
        {
            _animator.SetBool("DeadPlayer", true);
            Managers.Player.PlayerIsDead = true;
            _playerAlreadyDead = true;

            StartCoroutine(DisableAnimator());
        }

        if (!Managers.Player.PlayerIsDead)
        {
            if (!IsShootingOrReloading)
            {
                Movement();
                AnimationMovement();
            }
            if (Input.GetKey(KeyCode.Space) && !jetpackActivated)
            {
                StartCoroutine(StartJetpack());
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                StopCoroutine(StartJetpack());
                _isUsingJetpack = false;
                jetpackActivated = false;
            }
        }
    }

    private void Movement()
    {
        Vector3 movement = Vector3.zero; // Вектор (0,0,0)

        float HorInput = Input.GetAxis("Horizontal");
        float VertInput = Input.GetAxis("Vertical");

        if (HorInput != 0 || VertInput != 0)
        {
            Vector3 right = _target.right;
            Vector3 Forward = Vector3.Cross(right, Vector3.up);    // направление игрока

            movement = (right * HorInput) + (Forward * VertInput); // складываем вместе чтобы получить комбинир. вектор движ.
            movement *= MoveSpeed;
            movement = Vector3.ClampMagnitude(movement, MoveSpeed);

            if (movement.sqrMagnitude > 0.01f)  // двигается ли перс
            {
                Vector3 lookDirection = _target.forward; // Смотрим в сторону камеры, но без бокового наклона
                lookDirection.y = 0; // Убираем наклон вверх-вниз
                transform.forward = lookDirection;
            }
        }

        _animator.SetFloat("MoveSpeed", movement.sqrMagnitude);

        if (_isUsingJetpack)
        {
            _vertSpeed += _jetpackForce * Time.deltaTime;
            _vertSpeed = Mathf.Clamp(_vertSpeed, -_maxJetpackSpeed, _maxJetpackSpeed);
            _animator.SetBool("Jetpack", true);
        }
        else
        {
            _animator.SetBool("Jetpack", false);

            _vertSpeed += _gravity * 2 *  Time.deltaTime;
            _vertSpeed = Mathf.Clamp(_vertSpeed, _maxFallSpeed, _maxJetpackSpeed);
        }


        if (_jetpackFire != null)
        {
            foreach (GameObject fire in _jetpackFire)
            {
                if (fire != null)
                {
                    fire.SetActive(_isUsingJetpack);
                }
            }
        } 
        movement.y = _vertSpeed;
        movement *= Time.deltaTime;

        _charController.Move(movement);
    }


    private void AnimationMovement()
    {
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        _animator.SetBool("WalkRight", horInput > 0);
        _animator.SetBool("WalkLeft", horInput < 0);
        _animator.SetBool("WalkFront", vertInput > 0);
        _animator.SetBool("WalkBack", vertInput < 0);

        if (Input.GetKey(KeyCode.LeftShift) && vertInput > 0) // чтобы спринт был когда бежим вперёд
        {
            MoveSpeed = _baseSpeed * _sprintMult;
            _animator.SetBool("Sprint", true);
        }
        else
        {
            MoveSpeed = _baseSpeed;
            _animator.SetBool("Sprint", false);
        }
    }


    private IEnumerator StartJetpack()
    {
        jetpackActivated = true;
        yield return new WaitForSeconds(0.5f);

        if (Input.GetKey(KeyCode.Space))
        {
            _isUsingJetpack = true;
        } 
        else
        {
            jetpackActivated = false;
        }
    }

    private IEnumerator DisableAnimator()
    {
        yield return new WaitForSeconds(2.8f); // чтобы доиграла анимация
        _animator.enabled = false;
        transform.position = transform.position;
        transform.rotation = transform.rotation;
    }
}