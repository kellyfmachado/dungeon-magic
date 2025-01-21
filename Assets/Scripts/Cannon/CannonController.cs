using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Projectiles;

public class CannonController : MonoBehaviour
{
    public List<GameObject> projectilePrefabs;

    public Vector2 timeInterval = new Vector2(1,1);

    public GameObject spawnPoint;

    public GameObject target;

    public int attackDamage = 1;

    public float attackImpulse = 10;
    public Vector3 aimOffset = new Vector3(0, 1.4f, 0);
    public float distanceToAttack = 5f;

    public bool isActive = true;

    private float coolDown;

    // Start is called before the first frame update
    void Start()
    {
        coolDown = Random.Range(timeInterval.x,timeInterval.y);
    }

    // Update is called once per frame
    void Update()
    {

        if(GameManager.Instance.isGameOver) return;

        if(GetDistanceToPlayer() <= distanceToAttack && isActive){

            FacePlayer();

            coolDown -= Time.deltaTime;

            if (coolDown <= 0){
                coolDown = coolDown = Random.Range(timeInterval.x,timeInterval.y);
                Fire();
            }

        }

    }

    private void Fire(){

        GameObject projectilePrefab = projectilePrefabs[Random.Range(0,projectilePrefabs.Count)];

        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.transform.position, projectilePrefab.transform.rotation);
            
        // Populate ProjectileCollision
        var projectileCollision = projectile.GetComponent<ProjectileCollision>();
        projectileCollision.attacker = gameObject;
        projectileCollision.damage = attackDamage;

        // Get stuff
        var player = GameManager.Instance.player;
        var projectileRigidbody = projectile.GetComponent<Rigidbody>();

        // Apply impulse
        var vectorToPlayer = (player.transform.position + aimOffset - spawnPoint.transform.position).normalized;
        var forceVector = spawnPoint.transform.rotation * Vector3.forward;
        forceVector = new Vector3(forceVector.x, vectorToPlayer.y, forceVector.z);
        forceVector *= attackImpulse;
        projectileRigidbody.AddForce(forceVector, ForceMode.Impulse);

        // Schedule destruction
        Object.Destroy(projectile, 30);


    }

    public float GetDistanceToPlayer(){
        var player = GameManager.Instance.player;
        var playerPosition = player.transform.position;
        var origin = gameObject.transform.position;
        var positionDifference = playerPosition - origin;
        var distance = positionDifference.magnitude;
        return distance;
    }

    public void FacePlayer(){
        var transform = gameObject.transform;
        var player = GameManager.Instance.player;
        var vecToPlayer = player.transform.position - transform.position;
        vecToPlayer.y = 0;
        vecToPlayer.Normalize();
        var desiredRotation = Quaternion.LookRotation(vecToPlayer);
        var newRotation = Quaternion.LerpUnclamped(transform.rotation, desiredRotation, 0.1f);
        transform.rotation = newRotation;
    }

    public void SetActive(){
        isActive = true;
    }

    public void SetDesactive(){
        isActive = false;
    }


}
