using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Particle2D : MonoBehaviour
{
    public Text RotString;
    public Text PosString;

    //lab 1 step 1
    public Vector2 position, velocity, acceleration, incrementAccel, incrementVel;
    public float rotation, angVelocity, angAcceleration, incrementAV, incrementAngAccel;

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
    //private int rotType;
    //private int posType;
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
        position = Vector2.zero;
        acceleration = Vector2.zero;

        rotation = 0.0f;
        angAcceleration = 0.0f;

        velocity.x = 1;
        angVelocity = 1;

        transform.rotation = Quaternion.Euler(0.0f,0.0f,1.0f);
        transform.position = position;


    }

    public void swapPosMode()
    {
        zeroObjectPosAndRot();
        posType = !posType;
        if (posType)
        {
            PosString.text = "Pos EE";
        }
        else
        {
            PosString.text = "Pos K";
        }
    }

    public void swapRotMode()
    {
        zeroObjectPosAndRot();
        rotType = !rotType;

        if(rotType)
        {
            RotString.text = "Rot EE";
        }
        else
        {
            RotString.text = "Rot K";
        }
    }

    public void increaseAVandV()
    {
        angVelocity += incrementAV;
        //velocity.x += incrementVel.x;
    }

    public void increaseAccelandAngAccell()
    {
        //acceleration += incrementAccel;
        angAcceleration += incrementAngAccel;
    }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        debugMode = false;
        rotType = true;
        posType = true;
        PosString.text = "Pos EE";
        RotString.text = "Rot EE";

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

        //UpdateAcceleration();

        transform.position = position;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);

        //step 4
        acceleration.x = -Mathf.Sin(Time.time);       //might need this back in

        //lab 2 step 4
        //f_gravity: f = mg
        //Vector2 f_gravity = mass * new Vector2(0.0f, -9.8f);
        //AddForce(f_gravity);
        //AddForce(ForceGenerator.GenerateForce_Gravity(mass, -9.8f, Vector2.up));
    }
}
