using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Условие - командный дефматч
    /// </summary>
    public class ConditionTeamDeathmatch : MonoBehaviour, IMatchCondition
    {
        /// <summary>
        /// Количество танков красной команды
        /// </summary>
        private int red;
        /// <summary>
        /// Количество танков синей команды
        /// </summary>
        private int blue;

        /// <summary>
        /// ID победившей команды
        /// </summary>
        private int winTeamID = -1;
        public int WinTeamID => winTeamID;

        /// <summary>
        /// Условие выполнено
        /// </summary>
        private bool triggered;


        bool IMatchCondition.IsTriggered => triggered;

        void IMatchCondition.OnServerMatchStart(MatchController controller)
        {
            Reset();

            foreach (var v in FindObjectsOfType<Player>())
            {
                if (v.ActiveVehicle != null)
                {
                    v.ActiveVehicle.OnEventDeath.AddListener(OnEventDeathHandler);

                    if (v.TeamID == TeamSide.TeamRed) red++;
                    else
                    if (v.TeamID == TeamSide.TeamBlue) blue++;
                }
            }
        }

        void IMatchCondition.OnServerMatchEnd(MatchController controller)
        {
            
        }

        /// <summary>
        /// Обработчик события смерти
        /// </summary>
        /// <param name="destructible">Погибший дестрактибл</param>
        private void OnEventDeathHandler(Destructible destructible)
        {
            var ownerPlayer = destructible.Owner?.GetComponent<Player>();

            if (ownerPlayer == null) return;

            switch (ownerPlayer.TeamID)
            {
                case TeamSide.TeamRed:
                    red--;
                    break;
                case TeamSide.TeamBlue:
                    blue--;
                    break;
            }

            if (red == 0)
            {
                winTeamID = 1;
                triggered = true;
            }
            else if (blue == 0)
            {
                winTeamID = 0;
                triggered = true;
            }
        }


        /// <summary>
        /// Сброс
        /// </summary>
        private void Reset()
        {
            red = 0;
            blue = 0;
            triggered = false;
        }
    }
}