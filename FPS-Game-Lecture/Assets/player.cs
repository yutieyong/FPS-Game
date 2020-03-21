using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour // Game Object
{
    // Player's transform component
    public Transform player_transform;

    // Camera's transform component
    Transform camera_transform;

    float camera_height = 1.5f;

    // camera rotation angle, local cached value of player's rotation status in Vector 3 type
    // whenever player rotate their characters by mouse, we need to calculate the new rotation
    // based on the current rotation status stored in this vector.
    
    Vector3 camera_rotation;

    // Player's Character Controller component
    CharacterController player_controller;

    float moveSpeed = 8.0f;

    float gravity = 2.0f;

    float cameraRotationFactor = 1.8f;

    public int health = 5;

    Transform muzzle_transform;

    // 子弹与物体的碰撞层
    public LayerMask layer;
    // particle effect (bullet hit effect 粒子效果)
    public Transform particle_effect;

    // fire sounds
    public AudioClip audio_clip; // null

    AudioSource audio_source; // null

    // CD for shooting, counting down to 0
    // Once it's 0, it means we can fire
    // Once its greater than 0, it means we are in CD
    float shoot_cooldown = 0;



    // Start is called before the first frame update
    void Start()
    {
        // Get this player's transform component
        player_transform = this.transform; // get game object transform reference
        // Get character controller component
        player_controller = this.GetComponent<CharacterController>();
        // Get camera transform component
        camera_transform = Camera.main.transform;
        // Set camera initial position
        Vector3 position = player_transform.position;
        position.y += camera_height;
        camera_transform.position = position;
        // Set Camera rotation angle/direction based on players' rotation
        camera_transform.rotation = player_transform.rotation; // current angle
        camera_rotation = player_transform.eulerAngles; // delta in rotation angle

        muzzle_transform = camera_transform.Find("M4A1_PBR/M4A1_FrontSight");
        audio_source = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() // update player information
    {
        // move
        if (health <= 0) {
            return;
        }
        updatePosition();
        // be able to shoot every frame
        shoot_cooldown -= Time.deltaTime; 
        // Time.deltaTime is a variable, depending on CPU clock time
        // FPS高 => Time.deltaTime smaller
        if (Input.GetMouseButton(0) && shoot_cooldown <= 0) {
            // reset cooldown
            shoot_cooldown = 0.1f;
            audio_source.PlayOneShot(audio_clip);
            GameManager.Instance.SetBullets(1);
            // shoot the bullet
            RaycastHit hit;
            bool hitOrNot = Physics.Raycast(muzzle_transform.position, camera_transform.TransformDirection(Vector3.forward), out hit, 100, layer);
            if (hitOrNot) {
                Instantiate(particle_effect, hit.point, hit.transform.rotation);
            }
        }
    }

    void updatePosition() {
        // X, Y, Z delta
        float deltaX = 0, deltaY = 0, deltaZ = 0;
        // apply gravity (Y axis)
        deltaY -= gravity * Time.deltaTime;
        // forward backward left right movement
        // get user keyboard input WASD
        if (Input.GetKey(KeyCode.W)) {
            deltaZ += moveSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.S)) {
            deltaZ -= moveSpeed * Time.deltaTime;
        }
        // move left and right
        if (Input.GetKey(KeyCode.A)) {
            deltaX -= moveSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.D)) {
            deltaX += moveSpeed * Time.deltaTime;
        }

        // mouse move, update camera rotation angle
        // x(left/right) & y(forward/backward)
        float mHorizontal = Input.GetAxis("Mouse X"); // deltaX
        float mVertical = Input.GetAxis("Mouse Y"); // deltaY
        camera_rotation.x -= mVertical * cameraRotationFactor;  // up & down
        camera_rotation.y += mHorizontal * cameraRotationFactor; // left & right
        camera_transform.eulerAngles = camera_rotation; // pass the 3D vector to eulerAngles 
        // Set rotation angle for player
        // euler angle variable to be assigned to the player
        Vector3 playerRotation = camera_transform.eulerAngles;
        playerRotation.x = 0;
        playerRotation.z = 0;
        player_transform.eulerAngles = playerRotation;

        // move the character to a new position
        player_controller.Move(player_transform.TransformDirection(new Vector3(deltaX, deltaY, deltaZ)));

        // set camera to be at the updated position, same as the player
        Vector3 position = player_transform.position;
        position.y += camera_height;
        camera_transform.position = position;
    }

    // Subtract damage from player's health
    public void OnDamage(int damage) {
        this.health -= damage;
        GameManager.Instance.SetHealth(this.health);
        if (this.health <= 0) {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
