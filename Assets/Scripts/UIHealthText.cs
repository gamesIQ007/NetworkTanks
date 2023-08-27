using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение количества здоровья
    /// </summary>
    public class UIHealthText : MonoBehaviour
    {
        /// <summary>
        /// Текст
        /// </summary>
        [SerializeField] private Text text;


        private void Update()
        {
            if (Player.Local == null) return;
            if (Player.Local.ActiveVehicle == null) return;

            text.text = Player.Local.ActiveVehicle.HitPoint.ToString();
        }
    }
}