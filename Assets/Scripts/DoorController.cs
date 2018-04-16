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

    private float incrementX = 5.7f,
                  incrementZ = 10f,
                  incrementRotationY = 90f;

    private Vector3 closedPosition,
                    openedPosition;

    private Quaternion closedRotation,
                       openedRotation;

    public void Start()
    {
        state = DoorState.Close;
    }

    public void PrepareDoorHinge()
    {
        closedPosition = transform.position;
        closedRotation = transform.rotation;

        openedPosition = transform.position;
        openedPosition.x += incrementX;
        openedPosition.z += incrementZ;

        openedRotation = transform.rotation;
        openedRotation.y += incrementRotationY;
    }

    public IEnumerator OpenDoor()
    {
        state = DoorState.Open;

        print("Curr : " + transform.position + " : " + transform.rotation);
        print("Open : " + openedPosition + " : " + openedRotation);
        print("Close : " + closedPosition + " : " + closedRotation);

        while (transform.position != openedPosition)
        {
            print("OpenDoor");
            transform.position = Vector3.MoveTowards(transform.position, openedPosition, Time.deltaTime * 10f);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, openedRotation, Time.deltaTime * 10f);
            yield return null;
        }
    }

    public IEnumerator CloseDoor()
    {
        state = DoorState.Close;

        while (transform.position != closedPosition &&
               transform.rotation != closedRotation)
        {
            print("CloseDoor");
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, closedRotation, Time.deltaTime);
            yield return null;
        }

        yield return null;
    }
}
