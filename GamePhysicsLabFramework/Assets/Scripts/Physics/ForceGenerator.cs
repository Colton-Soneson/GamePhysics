using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGenerator
{
    public static float CharacteristicLength(float volumeOfSystem, float areaOfSurface)
    {
        return volumeOfSystem / areaOfSurface;
    }

    public static float Viscosity_Kinematic(float dynamicViscosity, float fluidDensity)
    {
        return dynamicViscosity / fluidDensity;
    }

    public static float Viscosity_Dynamic(float force, float area)
    {
        if(force == 0)
        {
            return  1 / area;
        }
        else
        {
            return force / area;
        }
    }

    public static float ReynoldsNumber_Kinematic(float fluidVelocity, float characteristicLength, float kinematicViscosity)
    {
        return ((fluidVelocity * characteristicLength) / kinematicViscosity);
    }

    public static float BejanNumber(float fluidDensity, float channelLength, float viscosity)
    {
        return channelLength * channelLength / fluidDensity * viscosity;
    }

    public static float dragCoefficientFunc(float wetArea, float frontArea, float bejanNum, float reynoldsNum)
    {
        return (wetArea / frontArea) * (bejanNum / reynoldsNum * reynoldsNum);
    }

    /*
    public static float ReynoldsNumber_Dynamic(float fluidDensity, float fluidVelocity, float characteristicLength, float dynamicViscosity)
    {

    }
    */

    public static Vector2 GenerateForce_Gravity(float particleMass, float gravitationalConstant, Vector2 WorldUp)
    {
        // f_gravity: f = mg
        Vector2 f_gravity = particleMass * gravitationalConstant * WorldUp;
        return f_gravity;
    }

    
    public static Vector2 GenerateForce_normal(Vector2 f_gravity, Vector2 surfaceNormal_unit)
    {
        // f_normal = proj(f_gravity, surfaceNormal_unit)

        Vector2 f_normal = Vector3.Project(-f_gravity, surfaceNormal_unit);

        //Vector2 f_normal = (Vector2.Dot(f_gravity, surfaceNormal_unit) / f_gravity.magnitude * f_gravity.magnitude) * f_gravity;

        return f_normal;
    }

    
    public static Vector2 GenerateForce_sliding(Vector2 f_gravity, Vector2 f_normal)
    {
        // f_sliding = f_gravity + f_normal
        Vector2 f_sliding = f_gravity + f_normal;
        return f_sliding;
    }

    public static Vector2 GenerateForce_friction(Vector2 f_normal, Vector2 f_opposing, Vector2 particleVelocity, float frictionCoefficient)
    {
        Vector2 f_friction;

        float fO_mag = f_opposing.magnitude;
        float max = frictionCoefficient * f_normal.magnitude;
        
        if(particleVelocity == Vector2.zero)    //if it aint moving then we do static
        {
            if (fO_mag < max)
            {
                f_friction = -f_opposing;
            }
            else if(fO_mag > max)
            {
                f_friction = -f_opposing.normalized * max;   //in the direction of f_opposing quant of max
            }
            else
            {
                f_friction = f_normal;
            }
        }
        else     //otherwise we doin kinetic
        {
            f_friction = -max * particleVelocity.normalized;
        }

        return f_friction;
        
    }

    
    public static Vector2 GenerateForce_friction_static(Vector2 f_normal, Vector2 f_opposing, float frictionCoefficient_static)
    {
        // f_friction_s = -f_opposing if less than max, else -coeff*f_normal (max amount is coeff*|f_normal|)
        Vector2 f_friction_s;

        if(f_opposing.magnitude < (frictionCoefficient_static * f_normal.magnitude))
        {
            f_friction_s = -f_opposing;
        }
        else
        {
            f_friction_s = -frictionCoefficient_static * f_normal;
        }

        return f_friction_s;
    }
    
    public static Vector2 GenerateForce_friction_kinetic(Vector2 f_normal, Vector2 particleVelocity, float frictionCoefficient_kinetic)
    {
        // f_friction_k = -coeff*|f_normal| * unit(vel)
        Vector2 f_friction_k = -frictionCoefficient_kinetic * f_normal.magnitude * particleVelocity;

        return f_friction_k;
    }

    
    public static Vector2 GenerateForce_drag(Vector2 particleVelocity, Vector2 fluidVelocity, float fluidDensity, float objectArea_crossSection, float objectDragCoefficient)
    {
        // f_drag = (p * u^2 * area * coeff)/2

        //do relative velocity not particle velocity
        Vector2 relVelocity = particleVelocity - fluidVelocity;

        Vector2 f_drag = (fluidDensity * (relVelocity.magnitude * relVelocity) * objectArea_crossSection * objectDragCoefficient) * 0.5f;

        return -f_drag;

    }
    
    public static Vector2 GenerateForce_spring(Vector2 particlePosition, Vector2 anchorPosition, float springRestingLength, float springStiffnessCoefficient)
    {
        // f_spring = -coeff*(spring length - spring resting length)

       

        Vector2 f_spring = new Vector2(0.0f,0.0f);

        Vector2 springEndToAttachedParticle = particlePosition - anchorPosition;

        float length = springEndToAttachedParticle.magnitude - springRestingLength;

        f_spring = -springStiffnessCoefficient * length * springEndToAttachedParticle.normalized;

        //Debug.Log("particlePosition = " + particlePosition + "  anchorPosition  = " + anchorPosition + "   f_spring = " + f_spring + "  springEndToAttachedParticle = " + springEndToAttachedParticle);

        return f_spring;

    }
    
    
}