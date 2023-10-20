using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject[] spawnPoints;
    public GameObject alien;
    public int maxAliensOnScreen;
    public int totalAliens;
    public float minSpawnTime;
    public float maxSpawnTime;
    public int aliensPerSpawn;
    private int aliensOnScreen = 0;
    private float generatedSpawnTime = 0;
    private float currentSpawnTime = 0;
    public GameObject upgradePrefab;
    public Gun gun;
    public float upgradeMaxTimeSpawn = 7.5f;
    private bool spawnedUpgrade = false;
    private float actualUpgradeTime = 0;
    private float currentUpgradeTime = 0;

    private void Start()
    {
        actualUpgradeTime = UnityEngine.Random.Range(upgradeMaxTimeSpawn - 3.0f,
            upgradeMaxTimeSpawn);
        actualUpgradeTime = Mathf.Abs(actualUpgradeTime);
    }
    public void AlienDestroyed()
    {
        aliensOnScreen -= 1;
        totalAliens -= 1;
    }
    private void Update()
    {
        currentUpgradeTime += Time.deltaTime;
        if (currentUpgradeTime > actualUpgradeTime)
        {
            // 1
            if (!spawnedUpgrade)
            {
                // 2
                int randomNumber = UnityEngine.Random.Range(0, spawnPoints.Length - 1);
                GameObject spawnLocation = spawnPoints[randomNumber];
                // 3
                GameObject upgrade = Instantiate(upgradePrefab) as GameObject;
                Upgrade upgradeScript = upgrade.GetComponent<Upgrade>();
                upgradeScript.gun = gun;
                upgrade.transform.position = spawnLocation.transform.position;
                // 4
                spawnedUpgrade = true;
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.powerUpAppear);
            }
        }
        //currentSpawnTime time passed since last spawn call
        currentSpawnTime += Time.deltaTime;
        // condition to generate a new wave of aliens
        if (currentSpawnTime > generatedSpawnTime)
        {
            // reset the timer after a spawn occurs
            currentSpawnTime = 0;
            
            // spawn time randomizer
            generatedSpawnTime = UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
            //ensure number of aliens within limits
            if (aliensPerSpawn > 0 && aliensOnScreen < totalAliens)
            {
                // this list keep track of where you have already spawned aliens
                List<int> previousSpawnLocations = new List<int>();
                // limit number of aliens to number of spawn points
                if (aliensPerSpawn > spawnPoints.Length)
                {
                    aliensPerSpawn = spawnPoints.Length - 1;
                }
                // preventative code to make sure you do not spawn
                // more aliens than you configured 
                aliensPerSpawn = (aliensPerSpawn > totalAliens) ?
                    aliensPerSpawn - totalAliens : aliensPerSpawn;
                
                // this code loops once for each spawned aliens
                for (int i = 0; i < aliensPerSpawn; i++)
                {
                    if (aliensOnScreen < maxAliensOnScreen)
                    {
                        // keep track of number of aliens spawned
                        aliensOnScreen += 1;
                        // value of -1 means no index has been assigned or found
                        // for the spawn point
                        int spawnPoint = -1;
                        // while loop keeps looking for a spawning point (index)
                        // that has not been used
                        while (spawnPoint == -1)
                        {
                            // create random index of list between 0 and number of spawn points
                            int randomNumber = UnityEngine.Random.Range(0, spawnPoints.Length - 1);
                            // check to see if random spawn point has not already been used
                            if (!previousSpawnLocations.Contains(randomNumber))
                            {
                                // add this random number to the list
                                previousSpawnLocations.Add(randomNumber);
                                // use this random number for the spawn location index
                                spawnPoint = randomNumber;
                            }
                        }
                        // actual point on arena to spawn next alien
                        GameObject spawnLocation = spawnPoints[spawnPoint];
                        // create a new alien from prefab
                        GameObject newAlien = Instantiate(alien) as GameObject;
                        // position the new alien to that random unused spawn point
                        newAlien.transform.position = spawnLocation.transform.position;
                        // get the "alien" code from the new spawned alien
                        Alien alienScript = newAlien.GetComponent<Alien>();
                        // set the new alien target to where the play currently is
                        // GameManager code affecting alien code
                        alienScript.target = player.transform;
                        // the new alien turns towards to the player
                        Vector3 targetRotation = new Vector3(player.transform.position.x,
                            newAlien.transform.position.y, player.transform.position.z);
                        newAlien.transform.LookAt(targetRotation);
                        alienScript.OnDestroy.AddListener(AlienDestroyed);
                    }
                }
            }
        }
    }
}
