using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    [Serializable] 
    private class PoolEntry
    {
        public ProjectileData projectileData;

        [Range(1, 50)]
        public int initialSize = 10;

        [NonSerialized]
        public List<GameObject> projectiles = new();
    }

    [Header("Projectile Types")]
    [SerializeField] private List<PoolEntry> _projectileTypes = new();

    private void Awake()
    {
        CreateInitialPools();
    }

    private void CreateInitialPools()
    {
        foreach (PoolEntry entry in _projectileTypes)
        {
            if (entry.projectileData == null)
                continue;

            for (int i = 0; i < entry.initialSize; i++)
                CreateProjectile(entry);
        }
    }

    private GameObject CreateProjectile(PoolEntry entry)
    {
        GameObject projectile =
            Instantiate(
                entry.projectileData.ProjectilePrefab,
                transform
            );

        projectile.SetActive(false);

        entry.projectiles.Add(projectile);
        
        return projectile;
    }

    public GameObject GetProjectile(
        ProjectileData projectileData
    )
    {
        if (projectileData == null)
            return null;

        PoolEntry entry = null;

        foreach (PoolEntry poolEntry in _projectileTypes)
        {
            if (poolEntry.projectileData == projectileData)
            {
                entry = poolEntry;
                break;
            }
        }

        if (entry == null)
        {
            Debug.LogError(
                $"No projectile pool entry found for " +
                $"{projectileData.name}.", this
            );
            return null;
        }

        return CreateProjectile(entry);
    }
}
