using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private GameObject clientProjectile;
    [SerializeField] private GameObject serverProjectile;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private CoinCollector coinCollector;
    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire;



    private bool fire;
    private bool canFire = true;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    private void Update()
    {
        if (!IsOwner || !fire || !canFire || coinCollector.Coins.Value < costToFire)
        {
            return;
        }

        FireTimeLock();
        SpawnServerProjectileServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        // Don't Spawn dummy projectile for owners as they're spawning their own
        if (IsOwner)
        {
            return;
        }
        SpawnDummyProjectile(spawnPos, direction);
    }

    [ServerRpc]
    private void SpawnServerProjectileServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        if(coinCollector.Coins.Value < costToFire){
            return;
        }
        coinCollector.SpendCoins(costToFire);
        SpawnProjectile(serverProjectile, spawnPos, direction);
        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject spawnedProjectile = SpawnProjectile(clientProjectile, spawnPos, direction);
        Rigidbody2D rigidBody = spawnedProjectile.GetComponent<Rigidbody2D>();
        rigidBody.AddRelativeForce(direction * projectileSpeed);
        muzzleFlash.SetActive(true);
        Waiter.Wait(muzzleFlashDuration, () => muzzleFlash.SetActive(false));
    }

    private GameObject SpawnProjectile(GameObject projectile, Vector3 spawnPos, Vector3 direction)
    {
        GameObject spawnedProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
        spawnedProjectile.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, spawnedProjectile.GetComponent<Collider2D>());
        if (spawnedProjectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
        return spawnedProjectile;
    }

    private void HandlePrimaryFire(bool fire)
    {
        this.fire = fire;
    }

    private void FireTimeLock()
    {
        canFire = false;
        Waiter.Wait(fireRate, () => canFire = true);
    }
}
