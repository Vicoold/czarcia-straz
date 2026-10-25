using UnityEngine;
using SlavicTD.Core;

namespace SlavicTD.Spells
{
    /// <summary>
    /// Base ScriptableObject class for Witch spells (Kingdom Rush style 3-spell loadout).
    /// </summary>
    public abstract class SpellDefinition : ScriptableObject
    {
        [Header("Display & Economy")]
        public string spellName;
        [TextArea] public string description;
        public Sprite icon;
        public float cooldown = 15f;
        public float effectRadius = 4f;

        [Header("Visual Effects")]
        public GameObject castVfxPrefab;
        public GameObject hitVfxPrefab;

        /// <summary>
        /// Cast the spell at the targeted world position.
        /// </summary>
        public abstract void Cast(Vector3 targetPosition, GameObject caster);
    }
}
