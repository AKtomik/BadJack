
namespace BadJack
{
	class Player
	{
		List<string> pile;
		public int score;
		string name;
		bool willChoose;

		public Player(bool willChoose = false, string name = "no name guy")
		{
			this.pile = new List<string>();
			this.score = 0;
			this.name = name;
			this.willChoose = willChoose;
		}

		public void Draw(List<string> paquet)
		{
			string card = paquet.First();
			pile.Add(card);
			if (card == "1")
			{
				Console.WriteLine("voilà un as pour {0}! Il vaut 1 ou 11?", name);
				string? input;
				
				if (willChoose)
				{
					while (true)
					{
						input = Console.ReadLine();
						if (input == "1" || input == "11") break;
						Console.WriteLine("Saisir 1 ou 11");
					}
				}
				else
				{
					if (Game.random.Next(1) == 0) input = "11";
					else input = "1";
					Console.WriteLine(input);
				}
				score += int.Parse(input);
			}
			else
			{
				score += Game.pointsDictionary[card];
			}
			paquet.RemoveAt(0);
		}

		public void Display(bool showSecond = true, bool showFirst = true)
		{
			string firstStr = (showFirst) ? pile[pile.Count - 1] : "?";
			string secondStr = (showSecond) ? pile[pile.Count - 2] : "?";
			Console.WriteLine("{0} : [{2}] [{1}] ({3} points)", name, firstStr, secondStr, score);
		}
	}

	class Game
	{
		public static Dictionary<string, int> pointsDictionary = new Dictionary<string, int>()
		{
			{"1", 1},
			{"2", 2},
			//{"3", 3},
			//{"4", 4},
			//{"5", 5},
			//{"6", 6},
			//{"7", 7},
			//{"8", 8},
			//{"9", 9},
			//{"10", 10},
			//{"V", 10},
			//{"D", 10},
			//{"R", 10},
		};
		public static Random random;

		static void Main(string[] args)
		{
			// name
			Console.WriteLine("donne ton nom ou c'est moi qui choisi");
			string? humanName = Console.ReadLine();
			if (humanName == null || humanName == "") humanName = "un humain trop nul";

			// init
			Player playerHuman = new Player(true, humanName);
			Player playerComputer = new Player(false, "ordinateur");
			List<string> paquet = new List<string>();
			random = new Random(42);

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
			string endMsg = "";

			while (!finPartie)
			{
				// player's turn
				if (!stopJoueur)
				{
					Thread.Sleep(1111);
					Console.WriteLine("piocher ?\no - ui\nn - nan");
					Thread.Sleep(666);
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
				Thread.Sleep(1111);
				playerHuman.Display(true, true);

				// computer's turn
				if (!stopOrdi)
				{
					Thread.Sleep(666);
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
				Thread.Sleep(1111);
				playerComputer.Display(true, true);

				// end the gmae
				bool looseComputer = playerComputer.score >= 21;
				bool looseHuman = playerHuman.score >= 21;

				if (looseComputer)
				{
					if (looseHuman)
					{
						endMsg = "L'ordi a dépassé 21, bien joué!";
					}
					else
					{
						endMsg = "Vous avez tous les deux dépassés 21...";
					}
					break;
				}
				else if (looseHuman)
				{
					endMsg = "Tu as dépassé 21, trop nul...";
					break;
				}
				if (stopJoueur && stopOrdi)
				{
					if (playerComputer.score > playerHuman.score)
					{
						endMsg = "L'ordinateur t'as roulé dessus...";
					}
					else if (playerComputer.score < playerHuman.score)
					{
						endMsg = "Tu as roulé l'ordinateur! GG";
					}
					else
					{
						endMsg = "Personne n'a gangné...";
					}
					break;
				}
			}
			Console.WriteLine("C'est fini!");
			Thread.Sleep(2222);
			Console.WriteLine(endMsg);
		}
	}
}