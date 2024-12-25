using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 5.0f;
    private GameObject focalPoint;
    public bool hasPowerUp;
    private float powerUpStrength = 15.0f;
    public GameObject powerUpIndicator;
    public PowerUpType currentPowerUp = PowerUpType.None;
    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerUpCountDown;
    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    bool smashing = false;
    float floorY;

    void Start()
    {
        playerRb= GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("FocalPoint");
    }
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);
        powerUpIndicator.transform.position = transform.position;
        if(currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F)){
            LaunchRockets();
        }
        if(currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing){
            smashing = true;
            StartCoroutine(Smash());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            Destroy(other.gameObject);
            StartCoroutine(PowerupCountdownRoutine());   
            powerUpIndicator.gameObject.SetActive(true);    
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;        
            if(powerUpCountDown != null){
                StopCoroutine(powerUpCountDown);
            }              
            powerUpCountDown = StartCoroutine(PowerupCountdownRoutine());
        }
    }
    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerUp = false;
        powerUpIndicator.gameObject.SetActive(false); 
        currentPowerUp = PowerUpType.None; 
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy") && hasPowerUp && currentPowerUp == PowerUpType.Pushback)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);

            Debug.Log("player collided with" + collision.gameObject + "with powerup set to" + currentPowerUp.ToString());
            enemyRigidbody.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
        }
    }

    void LaunchRockets(){
        foreach(var enemy in FindObjectsOfType<Enemy>()){
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }

    IEnumerator Smash(){
        var enemies = FindObjectsOfType<Enemy>();
        floorY = transform.position.y;
        float jumpTime = Time.time + hangTime;
        while(Time.time < jumpTime){
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }
        while(transform.position.y > floorY){
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed* 2);
            yield return null;
        }
        for(int i=0; i<enemies.Length; i++){
            if(enemies[i] != null){
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, 
                explosionRadius,0.0f,ForceMode.Impulse);
            }
            smashing = false;
        }
    }
}
