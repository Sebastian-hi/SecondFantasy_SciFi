using Cinemachine;
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class PlayerShoot : MonoBehaviour
{ 
    private Animator _animator;

    [SerializeField] private AudioSource _playerShootSource;
    [SerializeField] private AudioClip[] _playerShootClip;
    [SerializeField] private AudioClip  _ultraDamageShoot;
    [SerializeField] private AudioSource _playerReloading;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CinemachineFreeLook _cinemachineFreeLook;

    [SerializeField] private GameObject _explosionShootPlayer;


    private PlayerMovement _playerMovement;

    private Camera _cam;

    private bool _isShooting = false;

    private bool _isAiming = false;

    private bool _isReloading = false;

    private Coroutine fovCoroutine = null;

    private GameObject _hitObject;

    private int _damage = 50;
    private int _heatshotDamage = 100;
    private int _ultraDamage = 200;

    private float _normalSpeedXCam = 300f;
    private float _whenPlayerAim = 150f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _cam = Camera.main;
    }

    private void Update()
    {
        if (!Managers.Player.PlayerIsDead)
        {
            if (_cameraTransform != null)
            {
                Vector3 lookDirectionPlayer = _cameraTransform.forward;// направление камеры
                lookDirectionPlayer.y = 0; // убираем наклон вверх-вниз
                transform.forward = Vector3.Lerp(transform.forward, lookDirectionPlayer, Time.deltaTime * 10f); // плавно поворачивает игрока к прицелу
            }

            if (Input.GetMouseButtonDown(1) && !_isAiming)
            {           
                PlayerAiming(true);
            }
            else if (Input.GetMouseButtonUp(1) && _isAiming) 
            { 
                PlayerAiming(false);
            }
            

            if (Input.GetMouseButtonDown(0) && !_isShooting && !_isReloading && Managers.Player.CurAmmo > 0)
            {
                StartCoroutine(PlayerStartShoot());
                RayCastFire();
            }


            if (Input.GetKeyDown(KeyCode.R) && !_isReloading)
            {
                StartCoroutine(PlayerReloading());
            }

            if (Managers.Player.CurAmmo <= 0 && !_isReloading && Managers.Player.MaxAmmo > 0)
            {
                StartCoroutine(PlayerReloading());
            }


            if (Managers.Player.ECoin >= Managers.Player.PriceUltraDamage && Input.GetKey(KeyCode.Q) && !Managers.Battle.UseUltraPower)
            {
                Managers.Player.UseUltraDamageMinCoin(); // просто счёт денег
                StartCoroutine(StartUltraDamage());
            }
        }
    }

    private void PlayerAiming(bool aiming)
    {
        if (_isAiming == aiming) return;

        _isAiming = aiming;

        if (aiming)
        {
            _animator.SetTrigger("StartAim");
        }
        else
        {
            _animator.SetTrigger("StopAim");
        }
        
        float camFOV = _isAiming ? 30f : 50f;
        float speedXAxis = _isAiming ? _whenPlayerAim : _normalSpeedXCam;

        if (fovCoroutine != null)
        {
            StopCoroutine(fovCoroutine);
        }

        fovCoroutine = StartCoroutine(SmoothFOVandSensChange(camFOV, speedXAxis));
    }

    private IEnumerator SmoothFOVandSensChange(float camFOV, float speedXAxis)
    {
        float startFOV = _cinemachineFreeLook.m_Lens.FieldOfView;
        float startSens = _cinemachineFreeLook.m_XAxis.m_MaxSpeed;

        float elapsedTime = 0f;
        float duration = 0.3f; // время для плавн. измен.

        while (elapsedTime < duration)
        {
            float lerpFOV = Mathf.Lerp(startFOV, camFOV, Mathf.SmoothStep(0f, 1f, elapsedTime / duration));
            float lerpSpeed = Mathf.Lerp(startSens, speedXAxis, Mathf.SmoothStep(0f, 1f, elapsedTime / duration));

            _cinemachineFreeLook.m_Lens.FieldOfView = lerpFOV;
            _cinemachineFreeLook.m_XAxis.m_MaxSpeed = lerpSpeed;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _cinemachineFreeLook.m_Lens.FieldOfView = camFOV;
        _cinemachineFreeLook.m_XAxis.m_MaxSpeed = speedXAxis;

    }

    private IEnumerator PlayerStartShoot()
    {
        _isShooting = true; // блок. повторной стрельбы
        _playerMovement.IsShootingOrReloading = true; // блок движения

        _animator.SetTrigger("ShootTrigger");

        _isAiming = false;
        if (fovCoroutine != null)
        {
            StopCoroutine(fovCoroutine);
        }
       
        StartCoroutine(SmoothFOVandSensChange(50f, _normalSpeedXCam));

        if (Managers.Battle.UseUltraPower)
        {
            _playerShootSource.PlayOneShot(_ultraDamageShoot);
        }
        else
        {
            int playRandomShoot = Random.Range(0, _playerShootClip.Length);
            _playerShootSource.PlayOneShot(_playerShootClip[playRandomShoot]);
        }

        Managers.Player.ChangeAmmo(-1);

        yield return new WaitForSeconds(0.7f); // окончание анимации
        _isShooting = false;
        _playerMovement.IsShootingOrReloading = false;
    }


    private IEnumerator PlayerReloading()
    {
        _isReloading = true;
        _playerMovement.IsShootingOrReloading = true;

        _animator.SetBool("WalkFront", false);
        _animator.SetBool("WalkLeft", false);
        _animator.SetBool("WalkRight", false);
        _animator.SetBool("WalkBack", false);
        _animator.SetBool("Sprint", false);

        _playerReloading.Play();

        _animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(0.1f);

        int needAmmo = Managers.Player.AmmoInRifleMagazine - Managers.Player.CurAmmo;
        int ammoToLoad = Mathf.Min(needAmmo, Managers.Player.MaxAmmo);
        Managers.Player.ChangeAmmo(ammoToLoad);
        
        _animator.SetBool("Reloading", false);
        _animator.SetBool("Aiming", _isAiming); // возвр. прицеливание
        yield return new WaitForSeconds(1f);
        _isReloading = false;
        _playerMovement.IsShootingOrReloading = false;
    }

    private void RayCastFire()
    {
        Vector3 point = new(_cam.pixelWidth / 2, _cam.pixelHeight / 2, 0);
        Ray ray = _cam.ScreenPointToRay(point);
        RaycastHit hit;

        int layerMask = ~LayerMask.GetMask("Player", "Ignore Raycast", "Bortic"); // игнор этого тэга

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
        {
            _hitObject = hit.collider.gameObject;

            Debug.Log("Hit object: " + _hitObject.name); // Проверка, что именно попадание

            float distancefromCamera = Vector3.Distance(_cam.transform.position, hit.point);
            float distanceFromHero = Vector3.Distance(transform.position, hit.point);

            if (distancefromCamera < distanceFromHero) // загораживает
            {
                CheckHitAgain(ray, layerMask);
            }
            else
            {
                ProcessDirectHit(hit.point, _hitObject);
            }
        }
        else
        {
            Debug.Log("Луч не попал в цель.");
            StartCoroutine(StartExplosion(ray.origin + ray.direction * 50)); // Взрыв в точке далеко от камеры
        }
    }

    private void CheckHitAgain(Ray ray, int layerMask)
    {
        Debug.Log("Камере что-то мешает");
        int originalLayer = _hitObject.layer;

        try
        {
            _hitObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            RaycastHit secondHit;

            if (Physics.Raycast(ray, out secondHit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
            {
                GameObject secondHitObject = secondHit.collider.gameObject;

                Debug.Log("Второй объект: " + secondHitObject);

                ProcessDirectHit(secondHit.point, secondHitObject);
            }
        }
        finally
        {
            _hitObject.layer = originalLayer;
        }

    }

    private void ProcessDirectHit(Vector3 hitPoint, GameObject _hitObject)
    {
        StartCoroutine(StartExplosion(hitPoint));

        IEnemyInterface target = _hitObject.GetComponentInParent<IEnemyInterface>();

        if (target != null)
        {
            Debug.Log("Hit enemy!"); // Отладка, попали ли мы в врага

            bool isHeadshot = _hitObject.CompareTag("Head");
            int damage = isHeadshot ? _heatshotDamage : _damage;

            target.HurtEnemy(damage);

        }
    }
    private IEnumerator StartExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(_explosionShootPlayer, position, Quaternion.identity); // Quaternion.identity - без вращения (поворот 0 по всем осям)
        yield return new WaitForSeconds(1f);
        Destroy(explosion);
    }

    private IEnumerator StartUltraDamage()
    {
        Debug.Log("Супер сила началась");

        Managers.Battle.UseUltraPower = true;

        Managers.Audio.StopMusic();

        StartCoroutine(Managers.Audio.FadeIn(Managers.Audio.UltraPowerSource, 1.5f));

        _damage = _ultraDamage;
        _heatshotDamage = _ultraDamage;
        _animator.speed = 2;
        _playerMovement.MoveSpeed += 3f;
        Managers.Player.ChangeShield(100);

        yield return new WaitForSeconds(15f);

        StartCoroutine(Managers.Audio.FadeOut(Managers.Audio.UltraPowerSource, 2f));

        Managers.Battle.UseUltraPower = false;

        _damage = 50;
        _heatshotDamage = 100;
        _animator.speed = 1;
        _playerMovement.MoveSpeed -= 3f;

        if (Managers.Player.ECoin < Managers.Player.NextThreshold - Managers.Player.PriceUltraDamage)
        {
            Managers.Player.NextThreshold = Mathf.Max(Managers.Player.PriceUltraDamage, 
            (Managers.Player.ECoin / Managers.Player.PriceUltraDamage + 1) * Managers.Player.PriceUltraDamage);
        }

        Managers.Audio.PlayAmbientSound();

        Debug.Log("Супер сила закончилась");

    }
}