using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение времени матча
    /// </summary>
    public class UIMatchTimer : MonoBehaviour
    {
        /// <summary>
        /// Таймер матча
        /// </summary>
        [SerializeField] private MatchTimer timer;
        /// <summary>
        /// Текст
        /// </summary>
        [SerializeField] private Text text;


        private void Update()
        {
            // вызывать каждые 0,5 секунд, если дорабатывать
            text.text = timer.TimeLeft.ToString("F0");
        }
    }
}