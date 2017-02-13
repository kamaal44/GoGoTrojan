﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mover : MonoBehaviour
{
    public Vector3 com = new Vector3(0,0,0);
    public float maxMotorTorque = 10000;
    public float maxBrakeTorque = 20000;
    public float steerRatio = 10;
    public float speedThreshold=1;
    public int stepsBelowThreshold=15, stepsAboveThreshold=12;
    public GameObject frontLeft, frontRight, rearLeft, rearRight;
    public float forwardStiffness = 5, sidewayStiffness = 10;
    public Text speedText;
    public float topSpeed = 100 * 1000 / 3600;//(100 km/h)

    private bool isN2OReady  = true;
    private Rigidbody rb;
    private WheelController flController, frController, rlController, rrController;
    public float N2OPower = 2;

    public float N2OTime = 3;
    public ParticleSystem[] N2OParticles;
    

    // Use this for initialization 
    void Start()
    {
       
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass =com;
        if (speedText != null)
        {
            speedText.text = "0 km/h";
        }
        flController = frontLeft.GetComponent<WheelController>();
        frController = frontRight.GetComponent<WheelController>();
        rlController = rearLeft.GetComponent<WheelController>();
        rrController = rearRight.GetComponent<WheelController>();
        
        flController.ConfigureWheelSubsteps(speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
        flController.ConfigureFriction(forwardStiffness, sidewayStiffness);
        frController.ConfigureWheelSubsteps(speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
        frController.ConfigureFriction(forwardStiffness, sidewayStiffness);
        rlController.ConfigureWheelSubsteps(speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
        rlController.ConfigureFriction(forwardStiffness, sidewayStiffness);
        rrController.ConfigureWheelSubsteps(speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
        rrController.ConfigureFriction(forwardStiffness, sidewayStiffness);
    }



    // Update is called once per frame 
    void Update()
    {
        if (speedText != null)
        {
            speedText.text = Mathf.Round((rb.velocity.magnitude * 3600 / 1000) * 10) / 10f + " km/h";
        }
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.N) && isN2OReady)
        {
            StartCoroutine(Nitrous());
        }
        float motorTorque = maxMotorTorque;// * Input.GetAxis("Vertical");
        float brakeTorque = maxBrakeTorque * Input.GetAxis("Jump") ;
        flController.ApplyThrottle(motorTorque);
        frController.ApplyThrottle(motorTorque);
        rlController.ApplyThrottle(motorTorque);
        rrController.ApplyThrottle(motorTorque);
        
        
        flController.ApplyBrake(brakeTorque);
        frController.ApplyBrake(brakeTorque);
        rlController.ApplyBrake(brakeTorque);
        rrController.ApplyBrake(brakeTorque);


        float steerAngle = steerRatio *Input.GetAxis("Horizontal");// Input.acceleration.x;//

        flController.ApplySteer(steerAngle);
        frController.ApplySteer(steerAngle);

        if (rb.velocity.magnitude > topSpeed)
        {
            float slowDownRatio = rb.velocity.magnitude / topSpeed;
            rb.velocity /= slowDownRatio;
        }
    }

    public void speedDebuff(float debuffRatio)
    {
        topSpeed /= debuffRatio;
    }

    public void removeDebuff(float debuffRatio)
    {
        topSpeed *= debuffRatio;
    }

    private IEnumerator Nitrous()
    {
        isN2OReady = false;
        int i = 0;
        foreach (ParticleSystem N2O in N2OParticles)
        {
            Debug.Log("nitro"+(++i)+" is activated");
            N2O.Play();
        }
        
        maxMotorTorque *= N2OPower; 
        rb.AddForce(transform.forward * N2OPower, ForceMode.Acceleration);
        yield return new WaitForSeconds(N2OTime);
        maxMotorTorque /= N2OPower;

        foreach (ParticleSystem N2O in N2OParticles)
        {
            N2O.Stop();
        }
        isN2OReady = true;


    }
}