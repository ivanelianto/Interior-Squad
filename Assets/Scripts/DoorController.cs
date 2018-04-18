using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public DoorState state;

    public enum DoorState
    {
        Open, Close
    }

    private float incrementZ = 6f,
                  incrementRotationY = 90f;

    private Vector3 closedPosition,
                    openedPosition;

    private Quaternion closedRotation,
                       openedRotation;

    public void Start()
    {
        state = DoorState.Close;
    }

    //public void PrepareDoorHinge()
    //{
    //    closedPosition = transform.position;

    //    closedRotation = transform.rotation;

    //    openedPosition = transform.position;
    //    //openedPosition.x += incrementX;
    //    openedPosition.z += incrementZ;

    //    openedRotation = transform.rotation;
    //    openedRotation.y += incrementRotationY;
    //}

    public IEnumerator OpenDoorWrapper(Transform characterTransform)
    {
        yield return StartCoroutine(OpenDoor());
    }

    public IEnumerator OpenDoor()
    {
        FindObjectOfType<Gameplay>().gameObject.transform.Find("OpenDoorAudio").GetComponent<AudioSource>().Play();

        Vector3 newPosition = transform.position;

        newPosition.y = 0;

        if (transform.rotation.eulerAngles.y == 0)
        {
            newPosition.x = (transform.forward.x * 4); // Ok
            newPosition.z = (transform.forward.z * 4); // Ok
            transform.Rotate(Vector3.up * 90f);
        }
        else if (transform.rotation.eulerAngles.y == 90)
        {
            newPosition.x = 0; // -
            newPosition.z = (Vector3.back.z * 4); // +
            transform.Rotate(Vector3.down * 90f);
        }

        state = DoorState.Open;

        transform.Translate(newPosition);

        yield return null;
    }

    public IEnumerator CloseDoor()
    {
        FindObjectOfType<Gameplay>().gameObject.transform.Find("CloseDoorAudio").GetComponent<AudioSource>().Play();

        Vector3 newPosition = transform.position;

        newPosition.y = 0;

        if (transform.rotation.eulerAngles.y == 0)
        {
            newPosition.x = (transform.right.x * -4);
            newPosition.z = 0;
            transform.Rotate(Vector3.up * 90f);
        }
        else if (transform.rotation.eulerAngles.y == 90)
        {
            newPosition.x = ((Mathf.Round(transform.right.x) - 1) * 4);
            newPosition.z = 0;
            transform.Rotate(Vector3.down * 90f);
        }

        state = DoorState.Close;
        transform.Translate(newPosition);
        yield return null;
    }
}
