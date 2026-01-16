using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiesPerWave = 5;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f;

    public int currentWave = 0;

    public float waveCooldown = 10.0f;

    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<Zombie> currentZombiesAlive;

    public GameObject zombiePrefab;

    public TextMeshProUGUI titleWaveOver;
    public TextMeshProUGUI cooldowncount;

    public TextMeshProUGUI currentWaveUI;

    private void Start()
    {
        currentZombiesPerWave = initialZombiesPerWave;
        currentZombiesAlive = new List<Zombie>();

        StartNextWave();


    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();



        currentWave++;

        GlobalReferences.Instance.waveNumber = currentWave;
        currentWaveUI.text = "Wave: " + currentWave.ToString();



        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            Vector3 spawnOffset = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            );

            Vector3 spawnPosition = transform.position + spawnOffset;

            GameObject zombieObj = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            Zombie zombieScript = zombieObj.GetComponent<Zombie>();

            currentZombiesAlive.Add(zombieScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update()
    {
        List<Zombie> zombiesToRemove = new List<Zombie>();

        foreach (Zombie zombie in currentZombiesAlive)
        {
            if (zombie.isDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }

        foreach (Zombie zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }

        if (currentZombiesAlive.Count == 0 && !inCooldown)
        {
            StartCoroutine(WaveCooldown());
        }

        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            cooldownCounter = waveCooldown;
        }

        cooldowncount.text = cooldownCounter.ToString("F0");
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;

        titleWaveOver.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;
        titleWaveOver.gameObject.SetActive(false );

        currentZombiesPerWave *= 2;
        StartNextWave();
    }
}
