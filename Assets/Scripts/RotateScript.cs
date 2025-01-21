using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour
{

    public float degreesPerSecond = 90f;

    private float stepZPosition = 0;
    private float direction = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        stepZPosition += Time.deltaTime;
        var movement = 5.5f*Mathf.Cos(stepZPosition);
        transform.localPosition = new Vector3(movement,0.65f,0);

        float stepZ = degreesPerSecond * Time.deltaTime;
        if(movement < -5.45f){
            direction = -1;
        } else if (movement > 5.45f) {
            direction = 1;
        }
        stepZ *= direction;
        transform.Rotate(0,0,stepZ);
        
    }
}
