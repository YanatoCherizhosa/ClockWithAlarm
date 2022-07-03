using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class RotationToMouse : MonoBehaviour
{

    [SerializeField]
    protected int angelRotationModifier;
    [SerializeField]
    private GameObject parent;

    private TimeController timeController;
    private Vector3 mouseWorldPos;
    private Camera main_camera;
    private CreateAnAlarm alarmButton;
    bool mouseDown = false;

    private void Awake()
    {
        main_camera = Camera.main;
    }

    void Start()
    {
        timeController = GameObject.Find("TimeController").GetComponent<TimeController>();
        alarmButton = GameObject.Find("Alarm").GetComponent<CreateAnAlarm>();
    }

    void Update()
    {
        ButtonLogic();
    }

    void ButtonLogic()
    {
        if (Input.GetMouseButton(0) && timeController.GetIsAlarmChanging() == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                float laserLength = 2000f;
                mouseWorldPos = main_camera.ScreenToWorldPoint(Input.mousePosition);
                if (Physics2D.Raycast(mouseWorldPos, Vector3.forward, laserLength))
                {
                    RaycastHit2D thisHit2D = Physics2D.Raycast(mouseWorldPos, Vector3.forward, laserLength);
                    if (thisHit2D.collider.gameObject.name == gameObject.name)
                    { 
                        mouseDown = true;
                        alarmButton.SetIsArrowChangingNow(true);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0) || timeController.GetIsAlarmChanging() == 1)
        {
            mouseDown = false;
            alarmButton.SetIsArrowChangingNow(false);
        }

        if (mouseDown)
        {
            MovePacifier();
        }
    }

    void MovePacifier()
    {
        if (mouseDown)
        {
            mouseWorldPos = main_camera.ScreenToWorldPoint(Input.mousePosition);
            var rotationTarget = mouseWorldPos;
            rotationTarget.z = rotationTarget.z - 3;

            var _lookRotation = Quaternion.LookRotation(parent.GetComponent<Rigidbody2D>().transform.position - rotationTarget, Vector3.forward);

            _lookRotation.x = 0.0f;
            _lookRotation.y = 0.0f;

            parent.GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Slerp(parent.GetComponent<Rigidbody2D>().transform.rotation, _lookRotation, Time.deltaTime * 500f);

            float a = parent.transform.rotation.eulerAngles.z - (parent.transform.rotation.eulerAngles.z % (6 * angelRotationModifier));
            
            parent.GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Euler(0, 0, a);
        }
    }
}