using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerShoot : MonoBehaviour
{ 
    private Animator _animator;

    [SerializeField] private AudioSource _playerShootSource;
    [SerializeField] private AudioClip[] _playerShootClip;
    [SerializeField] private AudioSource _playerReloading;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CinemachineFreeLook _cinemachineFreeLook;

    [SerializeField] private GameObject _explosionShootPlayer;


    private PlayerMovement _playerMovement;

    private Camera _cam;

    private bool _isShooting = false;

    private bool _isAiming = false;

    private bool _isReloading = false;

    private GameObject hitColiiderObject;

    private int _damage = 50;
    private int _heatshotDamage = 100;
    private int _ultraDamage = 200;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _cam = Camera.main;
    }

    private void OnGUI()
    {
        int size = 14;
        float posX = _cam.pixelWidth / 2 - size / 4;
        float posY = _cam.pixelHeight / 2 - size / 1.5f;

        GUI.Label(new Rect(posX, posY, size, size), "o");
    }



    private void Update()
    {
        if (_cameraTransform != null)
        {
            Vector3 lookDirectionPlayer = _cameraTransform.forward;// направление камеры
            lookDirectionPlayer.y = 0; // убираем наклон вверх-вниз
            transform.forward = Vector3.Lerp(transform.forward, lookDirectionPlayer, Time.deltaTime * 10f); // плавно поворачивает игрока к прицелу
        }

        bool newAimingState = Input.GetMouseButton(1);
        

        if (!_isReloading && _isAiming != newAimingState)
        {
            _isAiming = newAimingState;
            _animator.SetBool("Aiming", _isAiming);
            StartCoroutine(ChangeFOV(_isAiming ? 30f : 50f));

        }

        if (Input.GetMouseButtonDown(0) && !_isShooting && !_isReloading)
        {
            StartCoroutine(PlayerStartShoot());
            RayCastFire();
        }


        if (Input.GetKeyDown(KeyCode.R) && !_isReloading) 
        {
            StartCoroutine(PlayerReloading());
        }

        if (Managers.Player.CurAmmo <= 0 && !_isReloading)
        {
            StartCoroutine(PlayerReloading());
        }

        
        if (Managers.Battle.haveUltraPower && Input.GetKey(KeyCode.Q) )
        {
            StartCoroutine(StartUltraDamage());
        }
        
    }

    private IEnumerator PlayerStartShoot()
    {
        _isShooting = true; // блок. повторной стрельбы
        _playerMovement.IsShooting = true; // блок движения

        _animator.SetBool("Aiming", false);
        yield return new WaitForEndOfFrame();


        int playRandomShoot = Random.Range(0, _playerShootClip.Length);
        _animator.SetTrigger("ShootTrigger");
        _playerShootSource.PlayOneShot(_playerShootClip[playRandomShoot]);

        Managers.Player.ChangeAmmo(-1);
        

        yield return new WaitForSeconds(0.7f); // окончание анимации
        _animator.SetBool("Aiming", _isAiming); //   Возвращаем прицеливание

        yield return new WaitForSeconds(0.1f);

        _isShooting = false;
        _playerMovement.IsShooting = false;
    }


    private IEnumerator PlayerReloading()
    {
        _isReloading = true;
        _playerMovement.IsShooting = true;

        _playerReloading.Play();
        _animator.SetBool("Aiming", false);
        _animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(1f);

        int needAmmo = 5 - Managers.Player.CurAmmo;
        int ammoToLoad = Mathf.Min(needAmmo, Managers.Player.MaxAmmo);
        Managers.Player.ChangeAmmo(ammoToLoad);
        
        _animator.SetBool("Reloading", false);
        _animator.SetBool("Aiming", _isAiming); // возвр. прицеливание

        _isReloading = false;
        _playerMovement.IsShooting = false;
    }

    private void RayCastFire()
    {
        Vector3 point = new(_cam.pixelWidth/2, _cam.pixelHeight/2, 0);
        Ray ray = _cam.ScreenPointToRay(point);
        RaycastHit hit;

        int layerMask = ~LayerMask.GetMask("Player", "Ignore Raycast"); // игнор этого тэга

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
        {
            hitColiiderObject = hit.collider.gameObject;

            Debug.Log("Hit object: " + hitColiiderObject.name); // Проверка, что именно попадание

            StartCoroutine(StartExplosion(hit.point));

            IEnemyInterface target = hitColiiderObject.GetComponentInParent<IEnemyInterface>();

            if (target != null)
            {
                Debug.Log("Hit enemy!"); // Отладка, попали ли мы в врага

                if (hitColiiderObject.CompareTag("Head") || hitColiiderObject.name == "Head") // hitObject.name == headcollider";
                {
                    target.HurtEnemy(_heatshotDamage);
                }
                else
                {
                    target.HurtEnemy(_damage);
                }
            }
        }
    }

    private IEnumerator StartExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(_explosionShootPlayer, position, Quaternion.identity); // Quaternion.identity - без вращения (поворот 0 по всем осям)
        yield return new WaitForSeconds(1f);
        Destroy(explosion);
    }

    private IEnumerator ChangeFOV(float targetFOV)
    {
        float startFOV = _cinemachineFreeLook.m_Lens.FieldOfView;
        float duration = 0.2f; // скорость изменения (время, за которое мы хотим завершить анимацию)
        float elapsed = 0; // скок времени прошло (с 0 секунд до двух получается)

        while (elapsed < duration)
        {
            _cinemachineFreeLook.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
            elapsed += Time.deltaTime; // плавно увеличиваем
            yield return null;
        }

        _cinemachineFreeLook.m_Lens.FieldOfView = targetFOV;
    }

    private IEnumerator StartUltraDamage()
    {
        Debug.Log("Супер сила началась");
        Managers.Battle.haveUltraPower = false;

        Managers.Audio.StopMusic();
        Managers.Audio.UltraPowerSource.Play();

        _damage = _ultraDamage;
        _heatshotDamage = _ultraDamage;
        _animator.speed = 2;
        _playerMovement.MoveSpeed += 3f;

        yield return new WaitForSeconds(15f);
        Managers.Audio.UltraPowerSource.Stop();
        Managers.Audio.ambientSource.Play();

        _damage = 50;
        _heatshotDamage = 100;
        _animator.speed = 1;
        _playerMovement.MoveSpeed -= 3f;

        Debug.Log("Супер сила закончилась");

    }
}