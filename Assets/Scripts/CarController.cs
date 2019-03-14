using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    public void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        frontLeftW.steerAngle = m_steeringAngle;
        frontRightW.steerAngle = m_steeringAngle;

    }

    private void Accelerate()
    {
        frontLeftW.motorTorque = m_verticalInput * motorForce;
        frontRightW.motorTorque = m_verticalInput * motorForce;
    
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftW, frontLeftT);
        UpdateWheelPose(frontRightW, frontRightT);
        UpdateWheelPose(backLeftW, backLeftT);
        UpdateWheelPose(BackRightW, backRightT);
    }


    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;

    }

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }

    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    public WheelCollider frontLeftW, frontRightW;
    public WheelCollider backLeftW, BackRightW;
    public Transform frontLeftT, frontRightT;
    public Transform backLeftT, backRightT;
    [SerializeField] public float maxSteerAngle = 30;
    [SerializeField] public float motorForce = 50;

}
