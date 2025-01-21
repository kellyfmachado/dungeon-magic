using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class SawController : MonoBehaviour
{

    [Header("Damage")]
    public int attackDamage = 1;

    [Header("Transform")]
    public float degreesPerSecond = 90f;
    public float speed = 1f;
    private float stepZPosition = 0;
    private float direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        stepZPosition += speed * Time.deltaTime;
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

    public void OnTriggerEnter (Collider other){
        OnPlayerCollisionEnter(other);
    }

    public void OnPlayerCollisionEnter (Collider other){
        
        var attacker = gameObject;
        var hitLifeScript = other.GetComponent<LifeScript>();
        hitLifeScript.InflictDamage(attacker, attackDamage);

    }
}
