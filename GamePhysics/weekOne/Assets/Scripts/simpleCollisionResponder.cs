using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleCollisionResponder : MonoBehaviour
{
    public string name;
    
    // Start is called before the first frame update
    void Start()
    {
        name = this.gameObject.name;
        Debug.Log("SCR : " + name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        string colName = collision.gameObject.name;
        Debug.Log(name + " has hit " + colName);
    }
}
