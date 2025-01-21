using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Player.States;
using UnityEngine;
using EventArgs;
using System;

public class PlayerController : MonoBehaviour
{
    // State Machine
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public Idle idleState;
    [HideInInspector] public Walking walkingState;
    [HideInInspector] public Jump jumpState;
    [HideInInspector] public Attack attackState;
    [HideInInspector] public Defend defendState;
    [HideInInspector] public Hurt hurtState;
    [HideInInspector] public Dead deadState;

    // Movement
    [Header("Movement")]
    public float movementSpeed = 10;
    public float maxSpeed = 10;
    [HideInInspector] public Vector2 movementVector;

    // Jump
    [Header("Jump")]
    public float jumpPower = 5;
    public float jumpMovementFactor = 1f;
    [HideInInspector] public bool hasJumpInput;

    // Slope
    [Header("Slope")]
    public float maxSlopeAngle = 45;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isOnSlope;
    [HideInInspector] public Vector3 slopeNormal;

    // Attack
    [Header("Attack")]
    public int attackStages;
    public List<float> attackStageDurations;
    public List<float> attackStageMaxIntervals;
    public List<float> attackStageImpulses;
    public GameObject swordHitbox;
    public float swordKnockbackImpulse = 10;
    public List<int> damageByStage;

    // Defend
    [Header("Defend")]
    public GameObject shieldHitbox;
    public float shieldKnockbackImpulse = 10;
    [HideInInspector] public bool hasDefenseInput;

    // Internal Properties
    [HideInInspector] public Rigidbody thisRigidyBody;
    [HideInInspector] public Collider thisCollider;
    [HideInInspector] public Animator thisAnimator;
    [HideInInspector] public LifeScript thisLife;

    // Hurt
    [Header("Hurt")]
    public float hurtDuration = 0.2f;
    public float invulnerabilityDuration = 1f;
    internal float invulnerabilityTimeLeft;

    // Effects
    [Header("Effects")]
    public GameObject hitEffect;

    [Header("SoundEffects")]
    public AudioSource stepSound;
    public AudioSource jumpSound;

    void Awake()
    {
        thisRigidyBody = GetComponent<Rigidbody>();
        thisCollider = GetComponent<Collider>();
        thisAnimator = GetComponent<Animator>();
        thisLife = GetComponent<LifeScript>();

        if(thisLife != null){
            thisLife.OnDamage += OnDamage;
            thisLife.OnHeal += OnHeal;
            thisLife.canInflictDamageDelegate += CanInflictDamage;
        }

        GlobalEvents.Instance.OnGameWon += OnGameWon;
    }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new StateMachine();
        idleState = new Idle(this);
        walkingState = new Walking(this);
        jumpState = new Jump(this);
        attackState = new Attack(this);
        defendState = new Defend(this);
        hurtState = new Hurt(this);
        deadState = new Dead(this);
        stateMachine.ChangeState(idleState);

        swordHitbox.SetActive(false);
        shieldHitbox.SetActive(false);

        // Update UI
        var gameplayUI = GameManager.Instance.gameplayUI;
        gameplayUI.playerHealthBar.SetMaxHealth(thisLife.maxHealth);
    }

    // Update is called once per frame
    void Update()
    {

        bool isUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool isDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool isLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool isRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        float inputX = isRight? 1 : isLeft? -1 : 0;
        float inputY = isUp? 1 : isDown? -1 : 0;
        movementVector = new Vector2(inputX, inputY);

        hasJumpInput = Input.GetKey(KeyCode.Space);

        hasDefenseInput = Input.GetMouseButton(1);

        float velocity = thisRigidyBody.velocity.magnitude;
        float velocityRate = velocity / maxSpeed;
        thisAnimator.SetFloat("fVelocity", velocityRate);

        DetectGround();
        DetectSlope();

        //  Restore vulnerability
        var gameManager = GameManager.Instance;
        if (!gameManager.isGameOver && !gameManager.isGameWon) {
            if (invulnerabilityTimeLeft > 0 && !thisLife.isVulnerable) {
                if ((invulnerabilityTimeLeft -= Time.deltaTime) <= 0f) {
                    thisLife.isVulnerable = true;
                }
            }
        }
        
        // StateMachine
        var bossBattleHandler = GameManager.Instance.bossBattleHandler;
        var isInCutscene = bossBattleHandler.IsInCutscene();
        if (isInCutscene && stateMachine.currentStateName != idleState.name) {
            stateMachine.ChangeState(idleState);
        }

        stateMachine.Update();
    }   

    void LateUpdate(){
        stateMachine.LateUpdate();
    }

    void FixedUpdate(){

        Vector3 gravityForce = Physics.gravity * (isOnSlope ? 0.005f : 1f);
        thisRigidyBody.AddForce(gravityForce, ForceMode.Acceleration);

        LimitSpeed();

        stateMachine.FixedUpdate();
    }

    private void OnDamage(object sender, DamageEventArgs args){
        
        // Ignore if game is over
        if (GameManager.Instance.isGameOver) return;
        if (GameManager.Instance.isGameWon) return;

        // Switch to hurt
        Debug.Log("Player recebeu um dano de " + args.damage + " do " + args.attacker.name);
        stateMachine.ChangeState(hurtState); 
    }   

    private void OnGameWon(object sender, GameWonArgs args) {
        stateMachine.ChangeState(idleState);
        thisLife.isVulnerable = false;
    }

    private void OnHeal(object sender, HealEventArgs args) {
        var gameplayUI = GameManager.Instance.gameplayUI;
        gameplayUI.playerHealthBar.SetHealth(thisLife.health);
        Debug.Log("Player recebeu uma cura.");
    }

    public void OnSwordCollisionEnter(Collider other){

        var otherObject = other.gameObject;
        var otherRigidbody = otherObject.GetComponent<Rigidbody>();
        var otherLife = otherObject.GetComponent<LifeScript>();
        var otherCollider = otherObject.GetComponent<Collider>();

        int bit = 1 << otherObject.layer;
        int mask = LayerMask.GetMask("Target", "Creatures");
        bool isBitInMask = (bit & mask) == bit; 
        var isTarget = isBitInMask;
        
        if(isTarget && otherRigidbody != null){

            if(otherLife != null){
                var damage = damageByStage[attackState.stage - 1];
                otherLife.InflictDamage(gameObject, damage);
            }

            if(otherRigidbody != null){
                var positionDiff = otherObject.transform.position - gameObject.transform.position;
                var impulseVector = new Vector3(positionDiff.normalized.x, 0, positionDiff.normalized.z);
                impulseVector *= swordKnockbackImpulse;
                otherRigidbody.AddForce(impulseVector, ForceMode.Impulse);
            }
            
        }

        // Hit effect
        if (hitEffect != null && (1 << otherCollider.gameObject.layer != LayerMask.GetMask("Player")) ) {
            var hitPosition = otherCollider.ClosestPointOnBounds(swordHitbox.transform.position);
            var hitRotation = hitEffect.transform.rotation;
            Instantiate(hitEffect, hitPosition, hitRotation);   
        }   

    }

    public void OnShieldCollisionEnter(Collider other){

        var otherObject = other.gameObject;
        var otherRigidbody = otherObject.GetComponent<Rigidbody>();
        var isTarget = true;
        if(isTarget && otherRigidbody != null){
            var positionDiff = otherObject.transform.position - gameObject.transform.position;
            var impulseVector = new Vector3(positionDiff.normalized.x, 0, positionDiff.normalized.z);
            impulseVector *= shieldKnockbackImpulse;
            otherRigidbody.AddForce(impulseVector, ForceMode.Impulse);
        }

    }

    private bool CanInflictDamage(GameObject attacker, int damage) {
        var isDefending = stateMachine.currentStateName == defendState.name;
        if (isDefending) {
            Vector3 playerDirection = transform.TransformDirection(Vector3.forward);
            Vector3 attackDirection = (transform.position - attacker.transform.position).normalized;
            float dot = Vector3.Dot(playerDirection, attackDirection);
            if (dot < -0.25) {
                // OnShieldCollisionEnter(attacker);
                return false;
            }
        }
        return true;
    }

    public Quaternion GetForward(){
        Camera camera = Camera.main;
        float eulerY = camera.transform.eulerAngles.y;
        return Quaternion.Euler(0, eulerY, 0);
    }

    public void RotateBodyToFaceInput(float alpha = 0.225f){

        if(movementVector.IsZero()) return;

        Camera camera = Camera.main;
        Vector3 inputVector = new Vector3(movementVector.x, 0, movementVector.y);
        Quaternion q1 = Quaternion.LookRotation(inputVector, Vector3.up);
        Quaternion q2 = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0);
        Quaternion toRotation = q1 * q2;
        Quaternion newRotation = Quaternion.LerpUnclamped(transform.rotation, toRotation, alpha);

        thisRigidyBody.MoveRotation(newRotation);
    }

    public bool AttemptToAttack(){
        if(Input.GetMouseButtonDown(0)){
            var isAttacking = stateMachine.currentStateName == attackState.name;
            var canAttack = !isAttacking || attackState.CanSwitchStages();
            if(canAttack){
                var attackStage = isAttacking ? (attackState.stage + 1) : 1;
                attackState.stage = attackStage;
                stateMachine.ChangeState(attackState);
                return true;
            }
        }
        return false;
    }

    public void DetectGround(){

        isGrounded = false;

        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float maxDistance = 0.1f;
        LayerMask groundLayer = GameManager.Instance.groundLayer;
        if(Physics.Raycast(origin, direction,  maxDistance, groundLayer )){

            isGrounded = true;

        }

    }

    public void DetectSlope(){

        isOnSlope = false;
        slopeNormal = Vector3.zero;

        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float maxDistance = 0.2f;
        if(Physics.Raycast(origin, direction, out var slopeHitInfo, maxDistance)){

            float angle = Vector3.Angle(Vector3.up, slopeHitInfo.normal);
            isOnSlope = angle < maxSlopeAngle && angle != 0;
            slopeNormal = isOnSlope ? slopeHitInfo.normal : Vector3.zero;

        }

    }

    public void LimitSpeed(){

        Vector3 flatVelocity = new Vector3(thisRigidyBody.velocity.x, 0, thisRigidyBody.velocity.z);
        if(flatVelocity.magnitude > maxSpeed){
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            thisRigidyBody.velocity = new Vector3(limitedVelocity.x, thisRigidyBody.velocity.y, limitedVelocity.z);
        }

    }

     private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("BossRoomSensor")) {
            GlobalEvents.Instance.InvokeBossRoomEnter(this, new BossRoomEnterArgs());
            Destroy(other.gameObject);
        }
    }

    // void OnDrawGizmos (){
    //     if(!thisCollider) return;

    //     Vector3 origin = transform.position;
    //     Vector3 direction = Vector3.down;
    //     Bounds bounds = thisCollider.bounds;
    //     float radius = bounds.size.x * 0.33f;
    //     float maxDistance = bounds.size.y * 0.25f;

    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawRay(origin, direction * maxDistance);

    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawSphere(origin, 0.1f);

    //     Vector3 spherePosition = direction * maxDistance + origin;
    //     Gizmos.color = isGrounded? Color.magenta : Color.cyan;
    //     Gizmos.DrawSphere(spherePosition, radius);

    // }

    // void OnGUI(){
    //     Rect rect = new Rect(10, 10, 100, 50);
    //     string text = stateMachine.currentStateName;
    //     GUIStyle style = new GUIStyle();
    //     style.fontSize = (int) (50f * (Screen.width / 1920f));
    //     GUI.Label(rect, text, style);
    // }
}
