using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public GameObject obj;
    public float x = 0;
    public float y = 0; 
    public float z = 0;
     
    
    private void Update()
    {
        obj.transform.Rotate(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
    }
}
