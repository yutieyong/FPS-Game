using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombie : MonoBehaviour
{
    // transform component
    Transform enemy_transform; // null by default 
    // player's health, transform
    player player;
    // navigation AI agent
    UnityEngine.AI.NavMeshAgent agent;

    // movement speed for zombie
    float move_speed = 2.5f;

    // rotation speed for zombie
    float rotation_speed = 30;

    // Animator component
    Animator animator;

    // timer
    float timer = 1.0f;
    public int health = 15;

    bool can_attack = true;




    // Start is called before the first frame update
    void Start()
    {
        enemy_transform = this.transform;
        animator = this.GetComponent<Animator>();
        // get player component
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();
        // set navigation
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = move_speed;
        // set destination target
        // set the destination to be player's position
        agent.SetDestination(player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.health <= 0) {
            return;
        }
        // we need to be able to set animation status
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        // transfer among different states
        // if we are in idle state
        if (state.fullPathHash == Animator.StringToHash("Base Layer.idle") && !animator.IsInTransition(0)) {
            
            // this means we are inside which state ? idle
            // we can transfer from idle state to other state
            animator.SetBool("idle", false);
            // count down timer before entering a new state
            timer -= Time.deltaTime;
            if (timer > 0) {
                return;
            }
            if (Vector3.Distance(enemy_transform.transform.position, player.transform.position) <= 1.2f) {
                can_attack = true;
                animator.SetBool("attack", true);
            } else {
                // if distance is not close enough, we run
                timer = 1;
                agent.SetDestination(player.transform.position);
                agent.isStopped = false;
                animator.SetBool("run", true);
            }
        }
        // for each frame:
        //      update(); // if running condition, then agent.isStopped = false;
        // reduce memory burden for update function, since it's called per FRAME 
        // 80-120 FPS, frames per second, animation < 20 FPS 
        // if we have navigation AI component setDestination called each frame
        // 120 calls per second
        // agent.isStopped = false; // you visit this memory block 120 times per seconds

        // if we are in run state
        if (state.fullPathHash == Animator.StringToHash("Base Layer.run") && !animator.IsInTransition(0)) {
            animator.SetBool("run", false);
            // count down timer
            timer -= Time.deltaTime;

            if (timer < 0) {
                // run to the player's current position
                // every 1 second we run to player
                agent.SetDestination(player.transform.position);
                timer = 1;
            }
            // if distance is close enough
            if (Vector3.Distance(enemy_transform.position, player.transform.position) <= 1.2f) {
                agent.isStopped = true;
                can_attack = true;
                // enter attack state
                animator.SetBool("attack", true); // try enter attack state
            }
        }

        // if current state is attack
        if (state.fullPathHash == Animator.StringToHash("Base Layer.attack") && !animator.IsInTransition(0)) {
            // we are in attack state, when we attack, we need to face the player
            RotateToPlayer();
            animator.SetBool("attack", false);

            // play the animation until its ended
            // if the player jumps out of the attack range, the attack movement should still
            // finishes playing
            if (state.normalizedTime >= 1.0f) {
                animator.SetBool("idle", true);
                // reset timer to be 2
                timer = 2.0f;
                // Player take damage
                // TODO: Miss rate (eat buff, 闪避护符)
                if (can_attack) {
                    player.OnDamage(1);
                    can_attack = false;
                }
            } // normalized time can take value from 0 -> 1, 1 means animation 100% complete
        }

        if (state.fullPathHash == Animator.StringToHash("Base Layer.death") && !animator.IsInTransition(0)) {
            // AnimationStateInfo.normalizedTime
            if (state.normalizedTime >= 1.0) {
                GameManager.Instance.SetScore(100);
                // Destroy this zombie object
                Destroy(this.gameObject);
            }
        }
        

        // set the destination to be player's position
        // agent.SetDestination(player.transform.position);
    }

    void RotateToPlayer() {
        // target direction, from enemy face pointing to my face
        Vector3 target_direction = player.transform.position - enemy_transform.position;
        // calculate the direction for the zombie to rotate to
        // this.transform.forward is the direction (face forward) for zombie that we want to change
        Vector3 zombie_delta_dir = Vector3.RotateTowards(this.transform.forward, target_direction, rotation_speed * Time.deltaTime, 0.0f);
        // let zombie rotate with delta_dir
        enemy_transform.rotation = Quaternion.LookRotation(zombie_delta_dir);
    }


    // Enemy takes damage from player.
    // When health <= 0, die
    public void OnDamage(int damage) {
        this.health -= damage;
        if (health <= 0) {
            animator.SetBool("death", true);
        }
    }
}
