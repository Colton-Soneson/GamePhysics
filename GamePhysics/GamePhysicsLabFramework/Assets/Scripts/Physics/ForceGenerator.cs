using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGenerator
{
    public static Vector2 GenerateForce_Gravity(float particleMass, float gravitationalConstant, Vector2 WorldUp)
    {
        // f_gravity: f = mg
        Vector2 f_gravity = particleMass * gravitationalConstant * WorldUp;
        return f_gravity;
    }
}