using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Управление транспортом
    /// </summary>
    
    [RequireComponent(typeof(Player))]

    public class VehicleInputControl : MonoBehaviour
    {
        /// <summary>
        /// Игрок
        /// </summary>
        private Player player;


        private void Awake()
        {
            player = GetComponent<Player>();
        }

        protected virtual void Update()
        {
            if (player == null) return;
            if (player.ActiveVehicle == null) return;

            if (player.isOwned && player.isLocalPlayer)
            {
                player.ActiveVehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
            }
        }
    }
}