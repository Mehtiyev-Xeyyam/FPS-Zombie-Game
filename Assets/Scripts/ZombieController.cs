using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public enum EnemyType { Light, Normal, Heavy }
    [SerializeField] private EnemyType enemyType = EnemyType.Normal;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float maxHealth = 100f;
    private bool isdead = false; 
    public PlayerMovement Player;
    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime = 0f;
    private float currentHealth;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
        Player = FindAnyObjectByType<PlayerMovement>();
    }

    void Update()
    {
        if (Player == null || agent == null || !agent.enabled) return;

        agent.SetDestination(Player.transform.position);
        float distanceToTarget = Vector3.Distance(transform.position, Player.transform.position);

        // Movement animation based on velocity
        bool moving = agent.velocity.magnitude > 0.1f && distanceToTarget > attackRange;
        animator.SetBool("IsMoving", moving);

        // Attack logic
        if (distanceToTarget <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;

        // Choose attack trigger based on enemy type
        switch (enemyType)
        {
            case EnemyType.Light:
                animator.SetTrigger("AttackLight");
                break;
            case EnemyType.Normal:
                animator.SetTrigger("AttackNormal");
                break;
            case EnemyType.Heavy:
                animator.SetTrigger("AttackHeavy");
                break;
        }

        // Deal damage if still in range
        float distanceToTarget = Vector3.Distance(transform.position, Player.transform.position);
        if (distanceToTarget <= attackRange && Player != null)
        {
            Player.currentHealth -= attackDamage;
            Debug.Log("Player's health: " + Player.currentHealth);
            Player.Die();
        }
    }

    // Called by Animation Event at the end of attack animation
    public void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Zombie took {damage} damage. Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isdead)return;
        {
            isdead = true;
        }
        GetComponent<CapsuleCollider>().enabled = false;
        Debug.Log("Zombie died!!!!!!!!!!!!!!!");
        animator.SetTrigger("death");
        animator.CrossFadeInFixedTime("death",0.01f);
        agent.enabled = false;

        this.enabled = false;

        Destroy(gameObject, 10f);
        GetComponent<ZombieController>().enabled = false;


    }

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}