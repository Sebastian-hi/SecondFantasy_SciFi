using System.Collections;
using UnityEngine;

public class SoldierEnemy : MonoBehaviour, IEnemyInterface
{
    [SerializeField] AudioSource fireSource;
    [SerializeField] AudioClip fireClip;
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

    private bool _isAlive = true;
    private bool _soldierAlreadyDead = false;
    private bool _alreadyAttacking = false;

    private float _speedMove = 2f;
    private float _obstacleRange = 2f;

    private float _attackRange = 14; // ������ �����������.
    private float _attackCooldown = 2f; // ����� ����� ����������.
    private float _lastAttackTime;

    private bool _isChasingPlayer = false;
    private bool _isAvoidingObstacle = false;

    private Vector3 _moveDirection;
    private Vector3 _lastPosition;

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
        if (!_isAlive || playerTransform == null) return;

        if (!Managers.Player.PlayerIsDead)
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

            if (Health <= 0 && !_soldierAlreadyDead)
            {
                _soldierAlreadyDead = true;
                StartCoroutine(DeathSoldier());
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
                   ChasePlayer(); // ����� � �����
                }
                else
                {
                    Wandering(); // ������ ������ �����
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
        if (_isAvoidingObstacle) return; // ��� ���� �������.

        _rb.MovePosition(transform.position + transform.forward * _speedMove * Time.fixedDeltaTime);

        float moveSpeed = (transform.position - _lastPosition).magnitude / Time.fixedDeltaTime;
        _animator.SetFloat("EnemySpeed", Mathf.Clamp(moveSpeed, 0, 5f)); // ��������� ��������
        Debug.Log($"Wandering: Speed = {moveSpeed}");

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

        // ��������� ���� ��� ������
        float randomAngle = Random.Range(-90f, 90f);
        Quaternion newRotation = Quaternion.Euler(0, randomAngle, 0);

        float avoidTime = 2f; // ������������ ������
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
            Debug.Log("������ ������� ����, ��������� ����. ����� �� ���������? ChasePlayer()");

            if (CanAttackPlayer())
            {
                Debug.Log("����� ���������!");

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
            Debug.Log($"ChasePlayer: Speed = {moveSpeed}, Position = {transform.position}, LastPosition = {_lastPosition}");

            Debug.Log("���������� � ChasePlayer()");
        }

        // ��������������� � �����
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
                Debug.Log("����� ������, �� �� � CanSeePlayer().");
                return true; // ������!
            }
        }
        return false; // ���� ���.
    }

    private bool CanAttackPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;

        Debug.DrawRay(rayOrigin, directionToPlayer * _attackRange, Color.red, 0.1f);  // ������������� ���

        Ray ray = new Ray(rayOrigin, directionToPlayer);
        RaycastHit hit;

        int shootableLayer = LayerMask.GetMask("Shootable");

        if (Physics.Raycast(ray, out hit, _attackRange, ~shootableLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("����� �����, ����� ��������");
                return true; // ����� �����
            }
            else
            {
                
                Debug.Log("���� ������������ ��������: " + hit.collider.name);
                
            }
        }
        Debug.Log("��� ������ �� ����� ����");
        return false;
    }

   // ����
    /*
      private bool CanAttackPlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        RaycastHit hit;

        int shootableLayer = LayerMask.GetMask("Shootable");
        float sphereRadius = 10f;

        if (Physics.SphereCast(transform.position, sphereRadius, directionToPlayer.normalized, out hit, _attackRange , ~shootableLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("����� �����, ����� �������� � ");
                return true; // ����� �����
            }
            else
            {
                if (hit.collider.CompareTag("Shootable"))
                {
                    Debug.Log("���� ������������ ��������: " + hit.collider.name);
                }
            }
        }
        return false;
    }
    */


    public void AttackHero()
    {
        LookAtPlayer();
        if (Time.time - _lastAttackTime >= _attackCooldown)
        {
            _lastAttackTime = Time.time;

            if (CanAttackPlayer())
            {
                Debug.Log("���� �������!");

                _animator.SetFloat("EnemySpeed", 0);

                if (!_alreadyAttacking)
                {
                    StartCoroutine(StartShoot());
                }
            }
            else
            {
                Debug.Log("���� �� ����� ��������� (���� ������������)");
            }

        }
    }

    private IEnumerator StartShoot()
    {
        _alreadyAttacking = true;
        Debug.Log("������� ��������!");

        _animator.SetBool("Shoot", true);
        _ShootEffect.SetActive(true);
        fireSource.PlayOneShot(fireClip);
        Managers.Player.ChangeShield(_damageSoldier); // � �� ��� ���� Broadcast.

        yield return new WaitForSeconds(1f); // ������������ �������� ��������
        Debug.Log("��������� �������� ��������");

        _ShootEffect.SetActive(false);
        _animator.SetBool("Shoot", false);
        Debug.Log("������� �����������");
        _animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(2f);
        Debug.Log("�������� �����������.");
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
            StartCoroutine(DeathSoldier());
        }
    }


    private IEnumerator DeathSoldier()
    {
        _isAlive = false;

        _animator.SetFloat("EnemySpeed", 0);
        _animator.SetBool("isDead", true);
        PlayDeathSoldier();

        yield return new WaitForSeconds(4f);

        Destroy(gameObject);  
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _speedRotationLook); // �������� ��������
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}