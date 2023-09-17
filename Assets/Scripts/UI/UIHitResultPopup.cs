using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Всплывающее сообщение с результатом попадания
    /// </summary>
    public class UIHitResultPopup : MonoBehaviour
    {
        /// <summary>
        /// Текст с типом попадания
        /// </summary>
        [SerializeField] private Text type;
        /// <summary>
        /// Текст с уроном попадания
        /// </summary>
        [SerializeField] private Text damage;


        /// <summary>
        /// Задать тип попадания
        /// </summary>
        /// <param name="textResult">Тип попадания</param>
        public void SetTypeResult(string textResult)
        {
            type.text = textResult;
        }

        /// <summary>
        /// Задать урон попадания
        /// </summary>
        /// <param name="damage">Урон</param>
        public void SetDamageResult(float damage)
        {
            if (damage <= 0) return;

            this.damage.text = "-" + damage.ToString("F0");
        }
    }
}