
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
			{//ace score pick
				Console.WriteLine("voilà un as pour {0}! Il vaut 1 ou 11?", name);
				string? input;

				if (willChoose)
				{//player can choose
					while (true)
					{
						input = Console.ReadLine();
						if (input == "1" || input == "11") break;
						Console.WriteLine("Saisir 1 ou 11");
					}
				}
				else
				{//is random
					if (Game.random?.Next(1) == 0) input = "11";
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

		public void Display(bool showFirst = true)
		{
			int startIndex = 0;
			string arrayStr = "";
			if (!showFirst)
			{
				arrayStr += "[?] ";
				startIndex = 1;
			}
			for (int i = pile.Count - 1; i >= startIndex; i--)
			{
				arrayStr += "["+pile[i]+"] ";
			}
			Console.WriteLine("{0} : {1}({2} points)", name, arrayStr, score);
		}
	}

	class Game
	{
		// function to get input as int without crash
		static int IntPut(int inputMinIncluded, int inputMaxIncluded, int? defaultValue = null)
		{
			while (true)
			{
				string? inputString = Console.ReadLine();
				int inputInt;
				if (defaultValue != null && (inputString == null || inputString == ""))
					return (int)defaultValue;
				else if (int.TryParse(inputString, out inputInt) && inputInt >= inputMinIncluded && inputInt <= inputMaxIncluded)
					return inputInt;
				else
					Console.WriteLine("donne un nombre valide ({0}-{1})", inputMinIncluded, inputMaxIncluded);
			}
		}

		// the score dico
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


		public static Random random = new Random();

		static void Main(string[] args)
		{
			// name
			Console.WriteLine("donne ton nom ou c'est moi qui choisi");
			string? humanName = Console.ReadLine();
			if (humanName == null || humanName == "") humanName = "un humain trop nul";

			// cards
			Console.WriteLine("voici le jeu de cartes :");
			List<string> aviableCards = pointsDictionary.Keys.ToList();
			List<string> paquet = pointsDictionary.Keys.ToList();
			while (true)
			{
				Console.WriteLine(string.Join(" ", paquet));
				Console.WriteLine("entrée pour valider, ou en saisir un nouveau");
				string? inputed = Console.ReadLine();
				if (inputed == null || inputed == "") break;
				List<string> paquetProposed = [.. inputed.Split(" ")];
				bool valid = true;
				paquetProposed.RemoveAll(v => v == "" );
				foreach (string v in paquetProposed)
				{
					if (!aviableCards.Contains(v))
					{
						Console.WriteLine("la carte [{0}] n'existe pas", v);
						valid = false;
					}
				}
				if (valid)
				{
					paquet = paquetProposed;
					Console.WriteLine("nouveau paquet :");
				}
			}

			Console.WriteLine("choisir le nombre de couleurs (4 par défaut)");
			int colorAmount = IntPut(0, 42, 2);
			
			Console.WriteLine("choisir le nombre de paquets (un paquet = {0} couleurs) (2 par défaut)", colorAmount);
			int paquetAmount = IntPut(0, 42, 2);

			// start
			Console.WriteLine("///////////////////\n///  blackjack  ///\n///////////////////\n");

			// init
			Player playerHuman = new(true, humanName);
			Player playerComputer = new(false, "ordinateur");
			List<string> cards = [];

			// shuffle
			for (int i = 0; i < paquetAmount*colorAmount; i++)
				cards.AddRange(paquet);
			cards = cards.OrderBy(x => Guid.NewGuid()).ToList();

			// give cards
			for (int i = 0; i < 2; i++)
			{
				playerHuman.Draw(cards);
				playerComputer.Draw(cards);
			}
			playerHuman.Display(true);
			playerComputer.Display(false);

			// turns
			bool stopJoueur = false;
			bool stopOrdi = false;
			bool blackjack = false;
			string endMsg = "";

			// blackjack checks
			if (playerHuman.score == 21 || playerComputer.score == 21)
			{
				blackjack = true;
				bool jackComputer = playerComputer.score == 21;
				bool jackHuman = playerHuman.score == 21;
				if (jackComputer && jackHuman)
				{
					endMsg = "L'ordi et toi avez blackjack!!!! vous avez trichés??";
				}
				else if (jackComputer)
				{
					endMsg = "L'ordi a blackjack!! dommage pour toi...";
				}
				else if (jackHuman)
				{
					endMsg = "Tu as blackjack!! bien joué!";
				}
			}

			if (!blackjack) while (true)
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
						playerHuman.Draw(cards);
					}
					else
					{
						Console.WriteLine("[joueur] je m'arrête");
						stopJoueur = true;
					}
				}
				Thread.Sleep(1111);
				playerHuman.Display(true);

				if (paquet.Count == 0) 
				{
					Console.WriteLine("apu de carte :(");
					break;
				}

				// computer's turn
				if (!stopOrdi)
				{
					Thread.Sleep(666);
					if (playerComputer.score <= 15)
					{
						Console.WriteLine("[ordi] je pioche");
						playerComputer.Draw(cards);
					}
					else
					{
						Console.WriteLine("[ordi] je m'arrête");
						stopOrdi = true;
					}
				}
				Thread.Sleep(1111);
				playerComputer.Display(false);

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
					break;//finish game
				}

				if (looseHuman)
				{
					endMsg = "Tu as dépassé 21, trop nul...";
					break;//finish game
				}

				if (stopJoueur && stopOrdi)
				{
					break;//finish game
				}
				
				if (paquet.Count == 0) 
				{
					Console.WriteLine("apu de carte :(");
					break;
				}
			}

			if (endMsg == "")
			{//happening when both stop or no more cards
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
			}
			Console.WriteLine("C'est fini!");
			Thread.Sleep(2222);
			//playerHuman.Display(true);
			//playerComputer.Display(true);
			//Thread.Sleep(666);
			Console.WriteLine(endMsg);
		}
	}
}