using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    Node tree;
    Node.Status treeStatus = Node.Status.RUNNING;
    // Keep track if our agent is in the middle of an action or not
    public enum ActionState { IDLE, WORKING }
    ActionState state = ActionState.IDLE;

    // If agent is within this range, the agent will pursue and attack the player with melee
    [SerializeField] float meleeAttackRange = 5f;
    // If the agent is outside of meleeAttackRange but within this range, the agent will shoot at the player
    [SerializeField] float rangeAttackRange = 7f;

    NavMeshAgent agent;
    GameObject player;
    CharacterMovement playerMovement;

    //This data is for attacks
    //Animator animator;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackPointRadius = 0.5f;
    [SerializeField] GameObject arrow;
    LayerMask playerLayer;
    [SerializeField] float coolDownTimer = 5f;
    float coolDown;

    Vector3 wanderTarget = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<CharacterMovement>();
        playerLayer = LayerMask.GetMask("Player");
        coolDown = coolDownTimer;

        tree = new Node();
        Selector selectPriority = new Selector();
        Sequence meleeSequence = new Sequence();
        Sequence rangeSequence = new Sequence();
        Leaf wander = new Leaf(Wander);
        Leaf isInMeleeRange = new Leaf(CheckMeleeRange);
        Leaf pursue = new Leaf(Pursue);
        Leaf meleeAttack = new Leaf(MeleeAttack);
        Leaf isInRangedRange = new Leaf(CheckRangedRange);
        Leaf rangeAttack = new Leaf(RangeAttack);

        rangeSequence.AddChild(isInRangedRange);
        rangeSequence.AddChild(rangeAttack);

        meleeSequence.AddChild(isInMeleeRange);
        meleeSequence.AddChild(pursue);
        meleeSequence.AddChild(meleeAttack);

        // If can melee, agent will melee. Else if can fire ranged projectile, agent will fire at target. Else agent will just wander. 
        selectPriority.AddChild(meleeSequence);
        selectPriority.AddChild(rangeSequence);
        selectPriority.AddChild(wander);

        tree.AddChild(selectPriority);
    }

    // Update is called once per frame
    void Update()
    {
        coolDown -= Time.deltaTime;
        if (treeStatus == Node.Status.SUCCESS) treeStatus = Node.Status.RUNNING;
        if (treeStatus != Node.Status.SUCCESS)
            treeStatus = tree.Process();
    }

    // Helper function for if the agent needs to go to a location
    Node.Status Seek(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        // If agent path is insufficient and cannot get to the end destination, do not bother
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }
    Node.Status Wander()
    {
        // Debug.Log("Wander is running right now");
        float wanderRadius = 4f;
        //float wanderDistance = Random.Range(-0.5f,0.5f);
        float wanderDistance = Mathf.PerlinNoise(Time.time, 0) / 4;
        float wanderJitter = 1f;

        wanderTarget += new Vector3(Random.Range(-1f, 1f) * wanderJitter, 0, Random.Range(-1f, 1f) * wanderJitter);
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

        return Seek(targetWorld);
    }
    Node.Status CheckMeleeRange()
    {
        // Debug.Log("Check Melee Range is running right now");
        float distance = (player.transform.position - this.transform.position).sqrMagnitude;
        if (distance <= meleeAttackRange * meleeAttackRange) return Node.Status.SUCCESS;
        return Node.Status.FAILURE;
    }
    Node.Status CheckRangedRange()
    {
        float distance = (player.transform.position - this.transform.position).sqrMagnitude;
        if (meleeAttackRange * meleeAttackRange <= distance && distance < rangeAttackRange * rangeAttackRange) return Node.Status.SUCCESS;
        return Node.Status.FAILURE;
    }
    Node.Status Pursue()
    {
        // POTENTIAL BUG: Stuck in pursue state because toTarget will be < 90 yet the player can still be behind the agent. This is probably due to a bug in CharacterMovement script.
        // Debug.Log("Pursue is running right now");
        Vector3 targetDirection = player.transform.position - this.transform.position;
        float toTarget = Vector3.Angle(this.transform.forward, this.transform.TransformVector(targetDirection));
        float relativeHeading = Vector3.Angle(this.transform.forward, this.transform.TransformVector(player.transform.forward));
        // if agent is in front of target, turn around and seek. Or if target stopped moving, seek. 
        if (toTarget > 90 && relativeHeading < 20 || playerMovement.getPlayerCurrentMovement().sqrMagnitude * Time.deltaTime < 0.1f)
        {
            // Debug.Log("toTarget: " + toTarget);
            // Debug.Log("relHeading: " + relativeHeading);
            // Debug.Log("currentSpeed: " + playerMovement.getPlayerCurrentMovement().sqrMagnitude * Time.deltaTime);
            return Seek(player.transform.position);
        }
        float lookAhead = targetDirection.sqrMagnitude / (agent.speed * agent.speed + playerMovement.getPlayerCurrentMovement().sqrMagnitude);
        return Seek(player.transform.position + player.transform.forward * lookAhead);
    }

    Node.Status MeleeAttack()
    {
        // Cooldown bug ... TODO: Fixit 
        // if (coolDown > 0) return Node.Status.FAILURE;
        // Debug.Log("Melee Attack is running right now");
        Collider[] hitCollision = Physics.OverlapSphere(attackPoint.position, attackPointRadius, playerLayer);
        foreach (Collider col in hitCollision)
        {
            Debug.Log("Attacked Player!");
            coolDown = coolDownTimer;
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }
    Node.Status RangeAttack()
    {
        if (coolDown > 0) return Node.Status.FAILURE;
        Debug.Log("Ranged Attack is running right now");
        Vector3 direction = (player.transform.position - this.transform.position);
        Quaternion lookAtRotation = Quaternion.LookRotation(direction);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookAtRotation, agent.angularSpeed * Time.deltaTime);
        GameObject newArrow = Instantiate(arrow, this.transform);
        newArrow.AddComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 10), ForceMode.Impulse);
        Destroy(newArrow, coolDownTimer);
        coolDown = coolDownTimer;
        return Node.Status.SUCCESS;
    }

    // Draws a wireframe of enemy attackPoint in order to visualize it easily in the scene view
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackPointRadius);
    }

}
