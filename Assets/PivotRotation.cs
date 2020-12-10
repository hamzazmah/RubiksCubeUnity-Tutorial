using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    private List<GameObject> activeSide;
    private Vector3 localForward;
    private Vector3 mouseRef;
    bool dragging = false;
    private float senstivity = 0.4f;
    private Vector3 rotation;
    private bool autoRotating = false;
    private float speed = 300f;
    private Quaternion targetQuaternion;
    private ReadCube readCube;
    private CubeState cubeState;

    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    // Late Update is called once per frame at the End
    void LateUpdate()
    {
        if(dragging && !autoRotating)
        {
            SpinSide(activeSide);
            if(Input.GetMouseButtonUp(0))
            {
                dragging = false;
                RotateToRightAngle();

            }
        }
        if(autoRotating)
        {
            AutoRotate();

        }
    }

    private void SpinSide(List<GameObject> side)
    {
        rotation = Vector3.zero;
        Vector3 mouseOffset = (Input.mousePosition - mouseRef);

        if(side == cubeState.front)
        {
            rotation.x = (mouseOffset.x + mouseOffset.y) * senstivity * -1;
        }
        if (side == cubeState.back)
        {
            rotation.x = (mouseOffset.x + mouseOffset.y) * senstivity * 1;
            
        }
        if (side == cubeState.up)
        {
            rotation.y = (mouseOffset.x + mouseOffset.y) * senstivity * 1;
        }
        if (side == cubeState.down)
        {
            rotation.y = (mouseOffset.x + mouseOffset.y) * senstivity * -1;
        }
        if (side == cubeState.left)
        {
            rotation.z = (mouseOffset.x + mouseOffset.y) * senstivity * 1;
        }
        if (side == cubeState.right)
        {
            rotation.z = (mouseOffset.x + mouseOffset.y) * senstivity * -1;
        }

        transform.Rotate(rotation, Space.Self);
        mouseRef = Input.mousePosition;
    }

    public void Rotate(List<GameObject> side)
    {
        activeSide = side;
        mouseRef = Input.mousePosition;
        dragging = true;
        //Create a Vector to rotate around
        localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
        
    }

    public void StartAutoRotate(List<GameObject> side, float angle)
    {
        cubeState.PickUp(side);
        Vector3 localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
        targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        activeSide = side;
        autoRotating = true;
    }

    public void RotateToRightAngle()
    {
        Vector3 vec = transform.localEulerAngles;

        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;

        targetQuaternion.eulerAngles = vec;
        autoRotating = true;
    }

    private void AutoRotate()
    {
        dragging = false;
        var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);
        if(Quaternion.Angle(transform.localRotation, targetQuaternion) <= 1)
        {
            transform.localRotation = targetQuaternion;

            cubeState.PutDown(activeSide, transform.parent);
            readCube.ReadState();
            CubeState.autoRotating = false;
            autoRotating = false;
            dragging = false;

        }
    }
}
