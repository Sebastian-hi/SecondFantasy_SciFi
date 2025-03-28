using System.Collections;
using UnityEngine;

public class SoldierEnemy : MonoBehaviour, IEnemyInterface
{
    [SerializeField] AudioSource fireSource;
    [SerializeField] AudioClip[] fireClips;
    [SerializeField] AudioClip[] reloadedClips;
    [Space]
    [SerializeField] AudioSource deathSource;
    [SerializeField] AudioClip[] deathClips;
    [Space]
    [SerializeField] AudioSource teleportSource;
    [SerializeField] GameObject _ShootEffect;

    private float _speedRotationLook = 5f;

    public int Health { get; set; } = 100;

    private int _damageSoldier = -15;

    private Transform playerTransform;
    private Animator _animator;
    private Rigidbody _rb;

    private bool _isAlive;
    private bool _soldierAlreadyDead = false;
    private bool _alreadyAttacking = false;

    private float _speedMove = 2f;
    private float _obstacleRange = 2f;

    private float _attackRange = 30; // радиус обнаружения.
    private float _attackCooldown = 2f; // время между выстрелами.
    private float _lastAttackTime;

    private bool _isChasingPlayer = false;
    private bool _isAvoidingObstacle = false;

    private Vector3 _moveDirection;
    private Vector3 _lastPosition;

    private Coroutine ShootCoroutine = null;
    private Coroutine ReloadCoroutine = null;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Start()
    {
        _isAlive = true;
        playerTransform = Managers.Player.playerTransform;
        _ShootEffect.SetActive(false);
        teleportSource.Play();

        ChooseNewDirection();
    }

    private void Update()
    { 
        if (_isAlive || !Managers.Player.PlayerIsDead)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= _attackRange && CanSeePlayer())
            {
                _isChasingPlayer = true;
                _lastPosition = playerTransform.position;
            }
            else if (_isChasingPlayer)
            {
                _isChasingPlayer = true;
            }
            else
            {
                _isChasingPlayer = false;
            }
        }

        if (!_isAlive)
        {
            if (ReloadCoroutine != null)
            {
                StopCoroutine(FirstShoot());
            }
            if (ReloadCoroutine != null)
            {
                StopCoroutine(SecondReload());
            }

        }

        Ray rayGround = new(transform.position, Vector3.down);

        if (!Physics.Raycast(rayGround, 1.1f, LayerMask.GetMask("Ground")))
        {
            _rb.AddForce(Vector3.down * 50f, ForceMode.Acceleration);

            if (transform.position.y < -100)
            {
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!Managers.Player.PlayerIsDead)
        {
            if (_isAlive)
            {
                if (_isChasingPlayer)
                {
                   ChasePlayer(); // здесь и атака
                }
                else
                {
                    Wandering(); // просто чиллим ходим
                }

                float speed = (transform.position - _lastPosition).magnitude / Time.fixedDeltaTime;
                _lastPosition = transform.position;

                speed = Mathf.Clamp(speed, 0, 5f);
                _animator.SetFloat("EnemySpeed", speed);
            }
        }
    }

    private void Wandering()
    {
        if (_isAvoidingObstacle) return; // ждём пока обходим.

        _rb.MovePosition(transform.position + transform.forward * _speedMove * Time.fixedDeltaTime);

        float moveSpeed = (transform.position - _lastPosition).magnitude / Time.fixedDeltaTime;
        _animator.SetFloat("EnemySpeed", Mathf.Clamp(moveSpeed, 0, 5f)); // Обновляем скорость

        Vector3 rayOrigin = transform.position + Vector3.down * 0.5f;

        Ray ray = new Ray(rayOrigin, transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 0.75f, out hit, _obstacleRange))
        {
            if (!hit.transform.CompareTag("Player"))
            {
                StartCoroutine(AvoidObstacle());
            }
        }
    }

    private IEnumerator AvoidObstacle()
    {
        _isAvoidingObstacle = true;

        yield return new WaitForSeconds(0.5f);

        // Случайный угол для обхода
        float randomAngle = Random.Range(-90f, 90f);
        Quaternion newRotation = Quaternion.Euler(0, randomAngle, 0);

        float avoidTime = 2f; // Длительность обхода
        float elapsedTime = 0;

        while (elapsedTime < avoidTime)
        {
            _rb.MovePosition(transform.position + newRotation * Vector3.forward * _speedMove * Time.fixedDeltaTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _isAvoidingObstacle = false;
    }
    
    private void ChasePlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= _attackRange)
        {
            Debug.Log("Первое условие есть, дистанция норм. Может ли атаковать? ChasePlayer()");

            if (CanAttackPlayer())
            {
                Debug.Log("Можем атаковать!");

                _animator.SetFloat("EnemySpeed", 0);
                AttackHero();
            }
            
        }
        else
        {
            _alreadyAttacking = false;
            _animator.SetBool("Shoot", false);
            _animator.SetBool("Reloading", false);
            _rb.MovePosition(transform.position + directionToPlayer * _speedMove * Time.fixedDeltaTime);

            float moveSpeed = (transform.position - _lastPosition).magnitude / Time.fixedDeltaTime;
            _animator.SetFloat("EnemySpeed", Mathf.Clamp(moveSpeed, 0, 5f));

            Debug.Log("Преследуем в ChasePlayer()");
        }

        // разворачиваемся к герою
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5);
    }

    private IEnumerator AvoidBlockPath()
    {
        _isAvoidingObstacle = true;
        yield return new WaitForSeconds(0.5f);

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 newDirection = Vector3.Cross(Vector3.up, directionToPlayer).normalized;

        Vector3 avoidDirection = (Random.value > 0.5f) ? newDirection : -newDirection;

        float avoidTime = 1.5f;
        float elapsedTime = 0;

        while (elapsedTime < avoidTime)
        {
            _rb.MovePosition(transform.position + transform.forward * _speedMove * Time.fixedDeltaTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _isAvoidingObstacle = false;
    }

    private bool CanSeePlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _attackRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Debug.Log("Видим игрока, всё ок в CanSeePlayer().");
                return true; // найден!
            }
        }
        return false; // если нет.
    }

    private bool CanAttackPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;

        Debug.DrawRay(rayOrigin, directionToPlayer * _attackRange, Color.red, 0.1f);  // Визуализируем луч

        Ray ray = new Ray(rayOrigin, directionToPlayer);
        RaycastHit hit;

        int shootableLayer = LayerMask.GetMask("Shootable", "Bortic", "Ignore Raycast");

        if (Physics.Raycast(ray, out hit, _attackRange, ~shootableLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Игрок виден, атака возможна");
                return true; // Игрок виден
            }
            else
            {
                
                Debug.Log("Путь заблокирован объектом: " + hit.collider.name);
                
            }
        }
        Debug.Log("Луч никуда не попал чёто");
        return false;
    }

    public void AttackHero()
    {
        LookAtPlayer();

        if (Time.time - _lastAttackTime >= _attackCooldown)
        {
            _lastAttackTime = Time.time;

            if (CanAttackPlayer())
            {
                Debug.Log("Враг атакует!");

                _animator.SetFloat("EnemySpeed", 0);

                if (!_alreadyAttacking && _isAlive)
                {
                    StartShoot();
                }
            }
            else
            {
                Debug.Log("Враг не может атаковать (путь заблокирован)");
            }
        }
    }

    private void StartShoot()
    {
        _alreadyAttacking = true;

        ShootCoroutine = StartCoroutine(FirstShoot());
    }

    private IEnumerator FirstShoot()
    {
        Debug.Log("Начинаю стрелять!");

        _animator.SetBool("Shoot", true);
        _ShootEffect.SetActive(true);

        int randomShootSound = Random.Range(0, fireClips.Length);
        fireSource.PlayOneShot(fireClips[randomShootSound]);

        yield return new WaitForSeconds(1f); // длительность анимации стрельбы
        Managers.Player.ChangeShield(_damageSoldier); // в нём уже есть Broadcast.

        Debug.Log("Подождали анимацию стрельбы");

        _ShootEffect.SetActive(false);

        _animator.SetBool("Shoot", false);

        yield return ReloadCoroutine = StartCoroutine(SecondReload());
    }


    private IEnumerator SecondReload()
    {
        Debug.Log("Начинаю перезарядку");

        _animator.SetBool("Reloading", true);
        
        int randomReloadSound = Random.Range(0, reloadedClips.Length);
        fireSource.PlayOneShot(reloadedClips[randomReloadSound]);

        yield return new WaitForSeconds(3f);
      
        Debug.Log("Закончил перезарядку.");
        _animator.SetBool("Reloading", false);
        _alreadyAttacking = false;
    }

    public void HurtEnemy(int damage)
    {
        Health -= damage;
        Debug.Log("Enemy health: " + Health);

        if (Health <= 0 && !_soldierAlreadyDead)
        {
            _soldierAlreadyDead = true;
            _isAlive = false;

            PlayDeathSoldier();
            Destroy(gameObject);
        }
    }

    private void PlayDeathSoldier()
    {
        if (deathClips.Length > 0)
        {
            int randomDeathClip = Random.Range(0, deathClips.Length);
            deathSource.PlayOneShot(deathClips[randomDeathClip]);
        }
    }

    private void ChooseNewDirection()
    {
        float randomAngle = Random.Range(0, 360);
        _moveDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
    }

    private void LookAtPlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _speedRotationLook); // скорость поворота
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}