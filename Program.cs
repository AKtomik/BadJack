
namespace BadJack
{
	class Player
	{
		List<string> pile;
		public int score;
		string name;

		public Player(string name = "no name guy")
		{
			this.pile = new List<string>();
			this.score = 0;
			this.name = name;
		}

		public void Draw(List<string> paquet)
		{
			string card = paquet.First();
			pile.Add(card);
			score += Game.pointsDictionary[card];
			paquet.RemoveAt(0);
		}

		public void Display(bool showSecond = true, bool showFirst = true)
		{
			string firstStr = (showFirst) ? pile[pile.Count - 1] : "?";
			string secondStr = (showSecond) ? pile[pile.Count - 2] : "?";
			Console.WriteLine("{0} : {2} {1} ({3} points)", name, firstStr, secondStr, score);
		}
	}

	class Game
	{
		public static Dictionary<string, int> pointsDictionary = new Dictionary<string, int>()
		{
			{"1", 1},
			{"2", 2},
			{"3", 3},
			{"4", 4},
			{"5", 5},
			{"6", 6},
			{"7", 7},
			{"8", 8},
			{"9", 9},
			{"10", 10},
			{"V", 10},
			{"D", 10},
			{"R", 10},
		};

		static void Main(string[] args)
		{
			// name
			Console.WriteLine("donne ton nom ou c'est moi qui choisi");
			string? humanName = Console.ReadLine();
			if (humanName == null) humanName = "un humain trop nul";

			// init
			Player playerHuman = new Player(humanName);
			Player playerComputer = new Player("ordinateur");
			List<string> paquet = new List<string>();

			// shuffle
			for (int i = 0; i < 8; i++)
				paquet.AddRange(pointsDictionary.Keys.ToList());
			List<string> paquetRandom = paquet.OrderBy(x => Guid.NewGuid()).ToList();

			// give cards
			for (int i = 0; i < 2; i++)
			{
				playerHuman.Draw(paquet);
				playerComputer.Draw(paquet);
			}
			playerHuman.Display(true, true);
			playerComputer.Display(true, true);

			// turns
			bool stopJoueur = false;
			bool stopOrdi = false;
			bool finPartie = false;

			while (!finPartie)
			{
				// player's turn
				if (!stopJoueur)
				{
					Console.WriteLine("piocher ?\no - ui\nn - nan");
					string? choixJoueur = Console.ReadLine();
					if (choixJoueur?.ToUpper() == "O")
					{
						Console.WriteLine("[joueur] je pioche");
						playerHuman.Draw(paquet);
					}
					else
					{
						Console.WriteLine("[joueur] je m'arrête");
						stopJoueur = true;
					}
				}

				// computer's turn
				if (!stopOrdi)
				{
					if (playerComputer.score <= 15)
					{
						Console.WriteLine("[ordi] je pioche");
						playerComputer.Draw(paquet);
					}
					else
					{
						Console.WriteLine("[ordi] je m'arrête");
						stopOrdi = true;
					}
				}

				// end the gmae
				finPartie = (stopJoueur && stopOrdi) || playerComputer.score >= 21 || playerHuman.score >= 21;
				playerHuman.Display(true, true);
				playerComputer.Display(false, true);
			}
		}
	}
}