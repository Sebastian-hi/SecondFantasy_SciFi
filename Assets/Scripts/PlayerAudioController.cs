using System.Collections;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private AudioSource _audioSourceWalk;
    [SerializeField] private AudioClip _audioClipWalk;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private AudioSource _jetpack;

    private readonly float _walkSpeed = 1f;
    private readonly float _runSpeed = 7f;

    private readonly float _walkDelay = 0.68f;
    private readonly float _runDelay = 0.30f;

    private Coroutine _footstepCoroutine;
    private bool _isPlayingJetpack = false;

    private void Update()
    {
        JetpackAndGravity();
    }

    private void JetpackAndGravity()
    {
        if (_playerMovement.jetpackActivated)
        {
            if (!_isPlayingJetpack)
            {
                _jetpack.Play();
                _isPlayingJetpack = true;
            }
            StopFootSteps();
            return;
        }
        else
        {
            _jetpack.Stop();
            _isPlayingJetpack = false;
        }

        if (characterController.isGrounded && !Managers.Player.InMenu)
        {
            float playerSpeed = characterController.velocity.magnitude;

            if (playerSpeed > _walkSpeed && playerSpeed < _runSpeed)
            {
                StartFootSteps(_walkDelay);
            }
            else if (playerSpeed > _runSpeed)
            {
                StopFootSteps();
                StartFootSteps(_runDelay);
            }
            else
            {
                StopFootSteps();
            }
        }
        else
        {
            StopFootSteps();
        }
    }

    private void StartFootSteps(float delay)
    {
        if (_footstepCoroutine == null) // если не корутина запущена
        {
            _footstepCoroutine = StartCoroutine(PlayFootSteps(delay));
        }
    }

    private IEnumerator PlayFootSteps(float delay)
    {
        while (true)
        {
            if (_audioClipWalk != null && _audioSourceWalk != null)
            {
                if(!_audioSourceWalk.isPlaying)
                {
                    _audioSourceWalk.PlayOneShot(_audioClipWalk);
                }
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private void StopFootSteps()
    {
        if (_footstepCoroutine != null)
        {
            StopCoroutine(_footstepCoroutine);
            _footstepCoroutine = null;
        }
    }
}