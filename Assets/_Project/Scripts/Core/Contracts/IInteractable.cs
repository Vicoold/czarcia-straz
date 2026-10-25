using UnityEngine;

namespace SlavicTD.Core
{
    /// <summary>
    /// Shared contract for objects that the Witch interacts with by approaching
    /// and holding (e.g., Blueprint foundations filling with coins, the Sanctuary starting night).
    /// </summary>
    public interface IInteractable
    {
        float InteractionRadius { get; }
        bool CanInteract(GameObject interactor);
        void OnInteractStay(GameObject interactor, float deltaTime);
    }
}
