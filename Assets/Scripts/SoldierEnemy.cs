using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SoldierEnemy : MonoBehaviour, IEnemyInterface
{
    [SerializeField] AudioSource fireSource;
    [SerializeField] AudioClip fireClip;
    [Space]
    [SerializeField] AudioSource deathSource;
    [SerializeField] AudioClip[] deathClips;
    [Space]
    [SerializeField] AudioSource teleportSource;

    public int Health { get; set; } = 100;
    private float _speedEnemy = 2f;

    private Transform playerTransform;
    private Rigidbody _rb;
    private Hurt hurt;
    private Animator _animator;

    private NavMeshAgent _navMeshAgent;
    private float _attackRange = 10f; // радиус обнаружения.
    private float _attackCooldown = 2f; // время между выстрелами.
    private float _lastAttackTime;

    private bool _isAlive;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _isAlive = true;
        teleportSource.Play();
        //StartPatrol();            ПОКА ВЫРУБЛЕНО
    }


    private void FixedUpdate()
    {
        if (_isAlive)
        {
            Vector3 moveDirection = transform.forward * _speedEnemy * Time.deltaTime;
            _rb.MovePosition(_rb.position + moveDirection);
        }
       
    }

    private void Update()
    {
        /*
        if(!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 1f)
        {
            StartPatrol();
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if ( distanceToPlayer <= _attackRange)
            {
                AttackHero();
            }
            else
            {
                ChasePlayer();
            }
            if (Health <= 0)
            {
                StartCoroutine(DeathSoldier());
            }
        }
        */
    }


    private void StartPatrol()
    {
        Vector3 randomPatrolPoint = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        _navMeshAgent.SetDestination(randomPatrolPoint);
        // задаем случ. точку для передвижения
    }

    public void AttackHero()
    {
        if (Time.time - _lastAttackTime >= _attackCooldown)
        {
            StartCoroutine(StartShoot());
            hurt.HurtPlayer(-20); // в нём уже есть Broadcast.
            _lastAttackTime = Time.time; // обновляем время
        }
       
    }

    private IEnumerator StartShoot()
    {
        _animator.SetBool("Aiming", true);
        _animator.SetBool("Shoot", true);
        fireSource.PlayOneShot(fireClip);

        yield return new WaitForSeconds(2f);

        _animator.SetBool("Aiming", false);
        _animator.SetBool("Shoot", false);
        _animator.SetBool("Reloading", true);
    }

    private void ChasePlayer()
    {
        _navMeshAgent.SetDestination(playerTransform.position);
    }
    /*
    private void AvoidObstacle() // НЕ ИСПОЛЬЗУЮ ПОКА ЧТО!!!!!!!
    {
        float angle = Random.Range(-110f, 110f);
        Quaternion newRotation = Quaternion.Euler(0, angle, 0) * transform.rotation;
        _rb.MoveRotation(Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 2));
    }


    public void SetTarget(Transform target)         // И ЭТО!
    {
        playerTransform = target;
        LookAtPlayer();
    }

    private void LookAtPlayer()      // Не использую!!!!!
    {
        if (playerTransform != null)
        {
            transform.LookAt(playerTransform);
        }
    }
    */

    public void HurtEnemy(int damage)
    {
        Health -= damage;
        Debug.Log("Enemy health: " + Health);
        if (Health <= 0)
        {
            _isAlive = false;
            StartCoroutine(DeathSoldier());
        }
    } 


    private IEnumerator DeathSoldier()
    {
        PlayDeathSoldier();
        _animator.SetBool("Dead", true);
        yield return new WaitForSeconds(5f);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}