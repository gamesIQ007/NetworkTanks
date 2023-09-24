using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Бот
    /// </summary>
    public class Bot : MatchMember
    {
        /// <summary>
        /// Транспорт
        /// </summary>
        [SerializeField] private Vehicle vehicle;


        public override void OnStartServer()
        {
            base.OnStartServer();

            teamId = MatchController.GetNextTeam();
            nickname = "b_" + GetRandomName();

            data = new MatchMemberData((int)netId, nickname, teamId, netIdentity);

            transform.position = NetworkSessionManager.Instance.GetSpawnPointByTeam(teamId);

            ActiveVehicle = vehicle;
            ActiveVehicle.TeamID = teamId;
            ActiveVehicle.Owner = netIdentity;
            ActiveVehicle.name = nickname;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            MatchMemberList.Instance.SvRemoveMatchMember(data);
        }


        public override void OnStartClient()
        {
            base.OnStartClient();

            ActiveVehicle = vehicle;
            ActiveVehicle.TeamID = teamId;
            ActiveVehicle.Owner = netIdentity;
            ActiveVehicle.name = nickname;
        }


        private void Start()
        {
            if (isServer)
            {
                MatchMemberList.Instance.SvAddMatchMember(data);
            }
        }


        /// <summary>
        /// Задать случайное имя
        /// </summary>
        /// <returns>Случайное имя</returns>
        private string GetRandomName()
        {
            string[] names = { "Аббадон", "Абдусциус", "Абигор", "Адрамалех", "Агалиарепт", "Агварес", "Азазель", "Аластор", "Амдусциас", "Андрас", "Асмодей", "Астарот", "Ахерон", "Барбатос",
                "Бегемот", "Бельфегор", "Бес", "Ваал", "Ваалберит", "Валафар", "Велиар", "Вельзевул", "Верделет", "Вин", "Гласиалаболас", "Гомори", "Дагон", "Данталиан", "Дюббук", "Залпас",
                "Зепар", "Инкубус", "Кайм", "Ксафан", "Ламия", "Левиафан", "Леонард", "Лерайе", "Люфицер", "Люцифуг Рофокал", "Маммон", "Марбас", "Мельхом", "Мефистофель", "Молох", "Мулцибер",
                "Навки", "Небирос", "Нибрас", "Нисрок", "Ойедлет", "Оливьер", "Парки", "Паймон", "Пут Сатанакия", "Сабнак", "Саламандры", "Саргатанас", "Сатана", "Сеера", "Ситри", "Суккубус",
                "Уфир", "Утбурд", "Филотанус", "Флеврети", "Фурфур", "Хабарил", "Шакс"};
            return names[Random.Range(0, names.Length)];
        }
    }
}