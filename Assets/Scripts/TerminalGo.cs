using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerminalGo : MonoBehaviour
{
    [SerializeField] AudioClip _clipForSend;
    [SerializeField] GameObject[] _objectsToDestroy;
    
    private float _fallSpeed = 15f;
    private float _destroyHeight = -100f;

    private bool _playerInRange = false;

    private Vector3 _startPos;

    private void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Managers.Audio.PlaySound(_clipForSend);
            StartCoroutine(SendObjectsInSpace());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null) 
        {
            _playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            _playerInRange = false;
        }
    }

    private IEnumerator SendObjectsInSpace()
    {

        foreach (GameObject obj in _objectsToDestroy)
        {
            if (obj == null) continue;
            
            yield return new WaitForSeconds(1f);  // временное
            Destroy(obj);                         // решение (остальное было)


            /*
            SpaceFlyThings flyScript = obj.GetComponent<SpaceFlyThings>();

            if (flyScript != null)
            {
                flyScript.enabled = false;
            }

            GameObject temp = Instantiate(obj, obj.transform.position, obj.transform.rotation);
            temp.name = obj.name;
            temp.transform.parent = obj.transform.parent;
            Destroy(obj);

            while (temp != null && temp.transform.position.y > _destroyHeight)
            {
                temp.transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
                yield return null;
            }

            if (temp != null)
            {
                Destroy(temp);
            }

            */
        }
    }

}
