using UnityEngine;

namespace SlavicTD.Core
{
    public enum DamageType
    {
        Physical,
        Magic,
        Fire,
        Poison,
        Siege
    }

    /// <summary>
    /// Shared contract for all entities that can take damage:
    /// Witch, Allied Cultists, Towers, Defensive Walls, Enemies, and the Main Sanctuary Base.
    /// </summary>
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        Transform TargetTransform { get; }
        void TakeDamage(float amount, DamageType type, GameObject source);
    }
}
