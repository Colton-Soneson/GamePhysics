using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Particle2D : MonoBehaviour
{
    public enum ForceType
    {
        GRAVITY,
        SLIDING,
        FRICTION,
        DRAG,
        SPRING
    };

    public ForceType typeOfForce;

    public Text RotString;
    public Text PosString;
    public Text OutputVals;
    public bool RotPosEffect;   //display to buttons

    Renderer rend;

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
    private Vector2 unitSupportNormal;
    public Vector2 f_Opposing;                  //adjust this force
    public Vector2 fluidVelocity;               //like the air
    public float frictionCoeffecient_Combined;
    public float frictionCoeffecient_Static;
    public float frictionCoeffecient_Kinetic;
    public float dragCoeffecient;
    public float springStiffnessCoeffecient;
    public float fluidDensity;
    public float springRestingLength;
    public GameObject NormalForceUnit;
    public GameObject Spring;



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

        /*
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
        */
    }

    // Start is called before the first frame update
    void Start()
    {
        debugMode = false;
        rotType = true;
        posType = true;
        startPos = this.transform.position;
        position = startPos;

        SetForceValues();

        rend = this.gameObject.GetComponent<Renderer>();
        float Rx = Mathf.Cos((NormalForceUnit.GetComponent<Transform>().rotation.eulerAngles.z) * Mathf.PI / 180);
        float Ry = Mathf.Sin((NormalForceUnit.GetComponent<Transform>().rotation.eulerAngles.z) * Mathf.PI / 180);
        //Debug.Log("x = " + Rx + "   y = " + Ry + "  rot = " + NormalForceUnit.GetComponent<Transform>().rotation.eulerAngles.z);
        unitSupportNormal = new Vector2(Rx, Ry);
        SetMass(startingMass);
    }

    void ProcessForces()
    {
        switch(typeOfForce)
        {

            case ForceType.GRAVITY:
                Vector2 f_gravity = ForceGenerator.GenerateForce_Gravity(mass, -9.8f, Vector2.up);       //mass * new Vector2(0.0f, -9.8f);
                AddForce(f_gravity);
                break;


            case ForceType.SLIDING:
                Vector2 f_gravity_slid = ForceGenerator.GenerateForce_Gravity(mass, -9.8f, Vector2.up);       //mass * new Vector2(0.0f, -9.8f);
                AddForce(f_gravity_slid);

                Vector2 f_surfaceNormal = ForceGenerator.GenerateForce_normal(f_gravity_slid, unitSupportNormal);
                AddForce(f_surfaceNormal);
                break;


            case ForceType.FRICTION:
                Vector2 f_gravity_fric = ForceGenerator.GenerateForce_Gravity(mass, -9.8f, Vector2.up);       //mass * new Vector2(0.0f, -9.8f);
                AddForce(f_gravity_fric);

                Vector2 f_surfaceNormal_fric = ForceGenerator.GenerateForce_normal(f_gravity_fric, unitSupportNormal);
                AddForce(f_surfaceNormal_fric);

                Vector2 f_friction = ForceGenerator.GenerateForce_friction(f_surfaceNormal_fric, f_Opposing, velocity, frictionCoeffecient_Combined);
                AddForce(f_friction);
                break;


            case ForceType.DRAG:
                float objectAreaCrossSection = rend.bounds.size.z * rend.bounds.size.y;
                dragCoeffecient = calculateDragCoefficient();
                Vector2 f_drag = ForceGenerator.GenerateForce_drag(velocity, fluidVelocity, fluidDensity, objectAreaCrossSection, dragCoeffecient);
                AddForce(f_drag);
                break;


            case ForceType.SPRING:
                Vector2 f_gravity_spring = ForceGenerator.GenerateForce_Gravity(mass, -9.8f, Vector2.up);       //mass * new Vector2(0.0f, -9.8f);
                AddForce(f_gravity_spring);

                Vector2 f_spring = ForceGenerator.GenerateForce_spring(position, NormalForceUnit.GetComponent<Transform>().position, springRestingLength, springStiffnessCoeffecient);
                AddForce(f_spring);
                break;
        }
    }

    void SetForceValues()
    {
        switch (typeOfForce)
        {
            case ForceType.GRAVITY:

                break;
            case ForceType.SLIDING:

                break;
            case ForceType.FRICTION:

                break;
            case ForceType.DRAG:

                break;
            case ForceType.SPRING:

                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
  
        userInput();

        process();

        UpdateAcceleration();

        transform.position = position;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);

        ProcessForces();

        //OutputVals.text = this.gameObject.name + "  AngAcc: " + (angAcceleration).ToString() + "   RotDegrees: " + (rotation % 360).ToString() + "  AccelX: " + ((acceleration.x).ToString()) + "  VelX: " + ((velocity.x).ToString());
    }

    public float calculateDragCoefficient()
    {
        float surfaceArea = 2 * (rend.bounds.size.x * rend.bounds.size.y) + 2 * (rend.bounds.size.x * rend.bounds.size.z) + 2 * (rend.bounds.size.z * rend.bounds.size.y);

        float CL = ForceGenerator.CharacteristicLength(rend.bounds.size.x * rend.bounds.size.y * rend.bounds.size.z, surfaceArea);
        float VK = ForceGenerator.Viscosity_Kinematic(ForceGenerator.Viscosity_Dynamic(force.magnitude, rend.bounds.size.x), fluidDensity);

        Debug.Log("SA = " + surfaceArea + "   CL = " + CL + "   VK = " + VK);

        float final = ForceGenerator.dragCoefficientFunc(surfaceArea,
                                                        rend.bounds.size.z * rend.bounds.size.y,
                                                        ForceGenerator.BejanNumber(fluidDensity, CL, VK),
                                                        ForceGenerator.ReynoldsNumber_Kinematic(fluidVelocity.magnitude, CL, VK));
        //Debug.Log("final = " + final);

        return final;

    }
}
