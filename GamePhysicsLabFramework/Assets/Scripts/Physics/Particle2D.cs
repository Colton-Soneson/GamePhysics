using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Particle2D : MonoBehaviour
{
    public Text RotString;
    public Text PosString;
    public Text OutputVals;
    public bool RotPosEffect;   //display to buttons

    //lab 1 step 1
    public Vector2 position, velocity, acceleration, incrementAccel, incrementVel;
    public float rotation, angVelocity, angAcceleration, incrementAV, incrementAngAccel;
    private Vector2 startPos;

    //lab 2 step 1
    public float startingMass;
    private float mass, massInv;
    public void SetMass(float newMass)
    {
        mass = newMass > 0.0f ? newMass : 0.0f;             //one line if statement, the "newMass : 0.0f" is for the true or false outcome
        massInv = mass > 0.0f ? 1.0f / mass : 0.0f;
    }                           
    //public void SetMass(float newMass) { mass = Mathf.Max(0.0f, newMass); }      //another option
    public float GetMass() { return mass; }

    //lab 2 step 2
    Vector2 force;
    public void AddForce(Vector2 newForce)
    {
        //D'Alembert
        force += newForce;
    }

    void UpdateAcceleration()
    {
        //its a conversion, so we don't need to integrate (no dt requirement)

        //Newton 2
        acceleration = force * massInv;

        //clear the forces so we dont just keep accumulating force nonstop
        force.Set(0.0f, 0.0f);
    }

    private bool debugMode;
    bool rotType;
    bool posType;

    //step 2
    void updatePositionEulerExplicit(float dt)
    {
        // x(t+dt) = x(t) + v(t)dt
        // Euler's method:
        //      F(t+td) = F(t) + f(t)dt
        //                     + (dF/dt)dt
        position += velocity * dt;

        //Kinematic Formula

        //v(t+dt) = v(t) + a(t)dt
        velocity += acceleration * dt;

        if (debugMode)
        {
            Debug.Log("PosType = Euler Explicit");
        }
    }

    void updatePositionKinematic(float dt)
    {
        // x(t+dt) = x(t) + v(t)dt + (0.5)a(t)dt^2

        position += velocity * dt + acceleration * dt * dt * 0.5f;

        velocity += acceleration * dt;

        if (debugMode)
        {
            Debug.Log("PosType = Kinematic");
        }
    }

    void updateRotationEulerExplicit(float dt)
    {
        rotation += angVelocity * dt;

        angVelocity += angAcceleration * dt;

        if (debugMode)
        {
            Debug.Log("RotType = Euler Explicit");
        }
    }

    void updateRotationKinematic(float dt)
    {
        rotation += angVelocity * dt + angAcceleration * dt * dt * 0.5f;

        angVelocity += angAcceleration * dt;

        if(debugMode)
        {
            Debug.Log("RotType = Kinematic");
        }
    }

    void zeroObjectPosAndRot()
    {
        //acceleration.x = -Mathf.Sin(Time.fixedTime);

        //velocity.x = 1;
        //angVelocity = 1.0f;

        rotation = 0.0f;
        position = startPos;
        //transform.rotation = Quaternion.Euler(0.0f,0.0f,1.0f);
        //transform.position = position;


    }

    public void swapPosMode()
    {
        zeroObjectPosAndRot();
        posType = !posType;
        
    }

    public void swapRotMode()
    {
        zeroObjectPosAndRot();
        rotType = !rotType;
        
    }

    public void increaseAV() { angVelocity += incrementAV; }
    public void decreaseAV() { angVelocity -= incrementAV; }
    public void increaseAngAccell() { angAcceleration += incrementAngAccel; }
    public void decreaseAngAccel() { angAcceleration -= incrementAngAccel; }
    public void increaseAccel() { incrementAccel.x++; }
    public void decreaseAccel() { incrementAccel.x--; }

    void userInput()
    {
       
        //debug mode
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            debugMode = !debugMode;
            Debug.Log("DebugMode = " + debugMode);
        }
        
    }

    void process()
    {
        if(posType)
        {
            updatePositionEulerExplicit(Time.fixedDeltaTime);
        }
        else
        {
            updatePositionKinematic(Time.fixedDeltaTime);
        }

        if (rotType)
        {
            updateRotationEulerExplicit(Time.fixedDeltaTime);
        }
        else
        {
            updateRotationKinematic(Time.fixedDeltaTime);
        }

        if (RotPosEffect)
        {
            if (posType)
            {
                PosString.text = "Pos EE: ";
                PosString.text += position.x;
            }
            else
            {
                PosString.text = "Pos K: ";
                PosString.text += position.x;
            }

            if (rotType)
            {
                RotString.text = "Rot EE: ";
                RotString.text += rotation;
            }
            else
            {
                RotString.text = "Rot K: ";
                RotString.text += rotation;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        debugMode = false;
        rotType = true;
        posType = true;
        startPos = this.transform.position;
        position = startPos;

        SetMass(startingMass);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //step 3
        //updatePositionEulerExplicit(Time.fixedDeltaTime);
        //transform.position = position;

        userInput();

        process();

        UpdateAcceleration();

        transform.position = position;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);

        //lab 2 step 4
        //f_gravity: f = mg
        Vector2 f_gravity = mass * new Vector2(0.0f, -9.8f);
        AddForce(f_gravity);
        AddForce(ForceGenerator.GenerateForce_Gravity(mass, -9.8f, Vector2.up));

        OutputVals.text = this.gameObject.name + "  AngAcc: " + (angAcceleration).ToString() + "   RotDegrees: " + (rotation % 360).ToString() + "  AccelX: " + ((acceleration.x).ToString()) + "  VelX: " + ((velocity.x).ToString());
    }
}
