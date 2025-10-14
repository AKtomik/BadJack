
namespace BadJack
{
	class Settings
	{
		// here if you want to add a card
		public static Dictionary<string, int> CardsAndPoints = new Dictionary<string, int>()
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
		
		// here if you want change cards suits
		public static List<CardSuit> globalSuits =
		[
			// defaults suits
			new("♥", ConsoleColor.White, ConsoleColor.DarkRed),
			new("♠", ConsoleColor.Black, ConsoleColor.DarkGray),
			new("♦", ConsoleColor.White, ConsoleColor.DarkRed),
			new("♣", ConsoleColor.Black, ConsoleColor.DarkGray),
			// negative suits
			new("♡", ConsoleColor.White, ConsoleColor.DarkRed),
			new("♤", ConsoleColor.Black, ConsoleColor.DarkGray),
			new("♢", ConsoleColor.White, ConsoleColor.DarkRed),
			new("♧", ConsoleColor.Black, ConsoleColor.DarkGray),
			// funny suits
			new("☺", ConsoleColor.Black, ConsoleColor.Yellow),
			new("☠", ConsoleColor.Black, ConsoleColor.White),
		];

		//
		public static List<string> deckComposition = CardsAndPoints.Keys.ToList();

		public static string humanName = "un humain trop nul";
		
		public static int deckColorAmount = 4;
		public static int deckPileAmount = 2;
	}

	// Player contains drawed cards, points, ect.
	class Player
	{
		List<Card> pile;
		public int score;
		string name;
		bool willChoose;
		ConsoleColor color;

		public Player(bool willChoose = false, string name = "no name guy", ConsoleColor color = ConsoleColor.White)
		{
			this.pile = new List<Card>();
			this.score = 0;
			this.name = name;
			this.willChoose = willChoose;
			this.color = color;
		}

		public void Draw(List<Card> paquet)
		{
			Thread.Sleep(333);
			if (paquet.Count == 0)
			{
				Game.SetConsoleColor(ConsoleColor.DarkGray);
				Console.WriteLine("Le paquet est vide");
				return;
			}

			Card cardTop = paquet.First();
			pile.Add(cardTop);
			if (cardTop.IsValue("1"))
			{//ace score pick
				Game.SetConsoleColor(ConsoleColor.Yellow);
				Console.Write("voilà un as pour {0}!", name);
				Game.SetConsoleColor(ConsoleColor.DarkGray);
				Console.WriteLine(" Il vaut 1 ou 11?");
				string? input;

				if (willChoose)
				{//player can choose
					while (true)
					{
						Game.SetConsoleColor(color);
						input = Console.ReadLine();
						if (input == "1" || input == "11") break;
						Game.SetConsoleColor(ConsoleColor.Red);
						Console.WriteLine("Saisir 1 ou 11");
					}
				}
				else
				{//is random
					if (Game.random?.Next(1) == 0) input = "11";
					else input = "1";
					Game.SetConsoleColor(color);
					Console.WriteLine(input);
				}
				score += int.Parse(input);
			}
			else
			{
				score += cardTop.GetPoints();
			}
			paquet.RemoveAt(0);
		}

		public void Display(bool showFirst = true)
		{
			int startIndex = 0;

			Game.SetConsoleColor(color);
			Console.Write("{0} : ", name);

			if (!showFirst)
			{
				Game.SetConsoleColor(ConsoleColor.DarkGray);
				Console.Write("[?] ");
				startIndex = 1;
			}

			for (int i = pile.Count - 1; i >= startIndex; i--)
			{
				pile[i].Display();
				Game.ClearConsoleColor();
				Console.Write(" ");
			}

			Game.SetConsoleColor(color);
			Console.WriteLine("({0} points)", showFirst ? score : "?");
		}
	}



	// a card of deck
	class Card
	{
		string value;
		CardSuit suit;

		public Card(string symbol, CardSuit suit)
		{
			this.value = symbol;
			this.suit = suit;
		}

		public void Display()
		{
			this.suit.Display(value);
		}

		public bool IsValue(string compare)
		{
			return compare == this.value;
		}
		public int GetPoints()
		{
			return Settings.CardsAndPoints[this.value];
		}
	}
	

	// reprsent a color+symbol card suit (heart...)
	class CardSuit
	{
		string symbol = "";
		ConsoleColor foreground;
		ConsoleColor background;

		public CardSuit(string symbol, ConsoleColor foreground, ConsoleColor background)
		{
			this.symbol = symbol;
			this.foreground = foreground;
			this.background = background;
		}

		public void Display(string value)
		{
			Game.SetConsoleColor(foreground, background);
			Console.Write("[{0}{1}]", symbol, value);
		}
	}

	class Game
	{
		// function to get input as int without crash
		static int IntPut(int inputMinIncluded, int inputMaxIncluded, int? defaultValue = null)
		{
			while (true)
			{
				SetConsoleColor(ConsoleColor.Cyan);
				string? inputString = Console.ReadLine();
				int inputInt;
				if (defaultValue != null && (inputString == null || inputString == ""))
					return (int)defaultValue;
				else if (int.TryParse(inputString, out inputInt) && inputInt >= inputMinIncluded && inputInt <= inputMaxIncluded)
					return inputInt;
				else
				{
					SetConsoleColor(ConsoleColor.Red);
					Console.WriteLine("donne un nombre valide ({0}-{1})", inputMinIncluded, inputMaxIncluded);
				}
			}
		}

		// change console's color
		public static void SetConsoleColor(ConsoleColor? foreground = ConsoleColor.White, ConsoleColor? background = null)
		{
			Console.ResetColor();
			if (foreground != null) Console.ForegroundColor = (ConsoleColor)foreground;
			if (background != null) Console.BackgroundColor = (ConsoleColor)background;
		}
		
		// just an alias
		public static void ClearConsoleColor()
		{
			SetConsoleColor();
		}


		public static Random random = new Random();



		static void SetSettings()
		{
			// pick name
			ClearConsoleColor();
			Console.WriteLine("donne ton nom ou c'est moi qui choisi");
			SetConsoleColor(ConsoleColor.Cyan);
			string? nameInput = Console.ReadLine();
			if (!(nameInput == null || nameInput == "")) Settings.humanName = nameInput;

			// * deck composition
			// pick cards of deck
			List<string> aviableCards = [.. Settings.CardsAndPoints.Keys];
			List<string> paquet = Settings.deckComposition;
			ClearConsoleColor();
			Console.WriteLine("voici le jeu de cartes :");
			while (true)
			{
				// console
				SetConsoleColor(ConsoleColor.Blue);
				Console.WriteLine(string.Join(" ", paquet));
				SetConsoleColor(ConsoleColor.DarkGray);
				Console.WriteLine("<entrée> pour valider, ou en saisir un nouveau");
				SetConsoleColor(ConsoleColor.Cyan);
				string? inputed = Console.ReadLine();
				// valid if nothing
				if (inputed == null || inputed == "") break;
				// check proposition
				List<string> paquetProposed = [.. inputed.Split(" ")];
				bool valid = true;
				paquetProposed.RemoveAll(v => v == "");
				foreach (string v in paquetProposed)
				{
					if (!aviableCards.Contains(v))
					{
						SetConsoleColor(ConsoleColor.Red);
						Console.WriteLine("la carte [{0}] n'existe pas", v);
						valid = false;
					}
				}
				// validate proposition
				if (valid)
				{
					paquet = paquetProposed;
					ClearConsoleColor();
					Console.WriteLine("nouveau paquet :");
				}
			}
			Settings.deckComposition = paquet;

			// pick colors of deck
			ClearConsoleColor();
			Console.Write("choisir le nombre de couleurs ");
			SetConsoleColor(ConsoleColor.DarkGray);
			Console.WriteLine("({0} par défaut)", Settings.deckColorAmount);
			Settings.deckColorAmount = IntPut(0, 8, Settings.deckColorAmount);
			
			// pick amount of deck
			ClearConsoleColor();
			Console.Write("choisir le nombre de paquets ");
			SetConsoleColor(ConsoleColor.DarkGray);
			Console.WriteLine("({0} par défaut)", Settings.deckPileAmount);
			Settings.deckPileAmount = IntPut(0, 42, Settings.deckPileAmount);
		}


		static void Main(string[] args)
		{
			SetSettings();

			// init
			Player playerHuman = new(true, Settings.humanName, ConsoleColor.Cyan);
			Player playerComputer = new(false, "ordinateur", ConsoleColor.Blue);
			List<Card> cards = [];

			// shuffle
			for (int i = 0; i < Settings.deckColorAmount * Settings.deckPileAmount; i++)
			{
				List<Card> cardsColor = Settings.deckComposition.ConvertAll(v => new Card(v, Settings.globalSuits[i]));
				cards.AddRange(cardsColor);
			}
			cards = cards.OrderBy(x => Guid.NewGuid()).ToList();
			SetConsoleColor(ConsoleColor.White);
			Console.WriteLine("Il y a {0} cartes dans le paquet.", cards.Count);

			// start
			SetConsoleColor(ConsoleColor.White);
			Console.WriteLine("\n///////////////////\n///  blackjack  ///\n///////////////////\n");
			SetConsoleColor(ConsoleColor.White);
			Console.WriteLine("Chacun pioche 2 cartes...");

			// give cards
			for (int i = 0; i < 2; i++)
			{
				playerHuman.Draw(cards);
				playerComputer.Draw(cards);
			}

			// turns
			bool stopJoueur = false;
			bool stopOrdi = false;
			string endMsg = "";
			ConsoleColor? endMsgColor = null;

			// blackjack checks
			if (playerHuman.score == 21 || playerComputer.score == 21)
			{
				bool jackComputer = playerComputer.score == 21;
				bool jackHuman = playerHuman.score == 21;
				SetConsoleColor(ConsoleColor.Black, ConsoleColor.Yellow);
				Console.Write(" ! BLACKJACK ! ");
				ClearConsoleColor();
				Console.WriteLine("");
				if (jackComputer && jackHuman)
				{
					endMsgColor = ConsoleColor.Yellow;
					endMsg = "L'ordi et toi avez blackjack!!!! vous avez trichés??";
				}
				else if (jackComputer)
				{
					endMsgColor = ConsoleColor.Red;
					endMsg = "L'ordi a blackjack!! dommage pour toi...";
				}
				else if (jackHuman)
				{
					endMsgColor = ConsoleColor.Green;
					endMsg = "Tu as blackjack!! bien joué!";
				}
			}

			else
			{
				playerHuman.Display(true);
				playerComputer.Display(false);
				while (true)
				{
					// endgame triggers
					bool looseComputer = playerComputer.score >= 21;
					bool looseHuman = playerHuman.score >= 21;

					if (looseComputer)
					{
						if (looseHuman)
						{
							endMsgColor = ConsoleColor.Yellow;
							endMsg = "Vous avez tous les deux dépassés 21...";
						}
						else
						{
							endMsgColor = ConsoleColor.Green;
							endMsg = "L'ordi a dépassé 21, bien joué!";
						}
						break;//finish game
					}

					if (looseHuman)
					{
						endMsgColor = ConsoleColor.Red;
						endMsg = "Tu as dépassé 21, trop nul...";
						break;//finish game
					}

					if (stopJoueur && stopOrdi)
					{
						break;//finish game
					}

					// no cards
					if (cards.Count == 0)
					{
						ClearConsoleColor();
						Console.WriteLine("apu de carte :(");
						break;
					}


					// player's turn
					if (!stopJoueur)
					{
						Thread.Sleep(1111);

						ClearConsoleColor();
						Console.WriteLine("piocher ?");
						SetConsoleColor(ConsoleColor.DarkGray);
						Console.WriteLine("o - ui\nn - nan");
						Thread.Sleep(666);
						SetConsoleColor(ConsoleColor.Cyan);
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

					// no cards
					if (cards.Count == 0)
					{
						SetConsoleColor(ConsoleColor.White);
						Console.WriteLine("apu de carte :(");
						break;
					}

					// computer's turn
					if (!stopOrdi)
					{
						Thread.Sleep(666);
						SetConsoleColor(ConsoleColor.Blue);
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
				}
			}

			if (endMsg == "")
			{//happening when both stop or no more cards
				if (playerComputer.score > playerHuman.score)
				{
					endMsgColor = ConsoleColor.Red;
					endMsg = "L'ordinateur t'as roulé dessus...";
				}
				else if (playerComputer.score < playerHuman.score)
				{
					endMsgColor = ConsoleColor.Green;
					endMsg = "Tu as roulé l'ordinateur! GG";
				}
				else
				{
					endMsgColor = ConsoleColor.Yellow;
					endMsg = "Personne n'a gangné...";
				}
			}
			ClearConsoleColor();
			Console.WriteLine("\nC'est fini!");
			Thread.Sleep(2222);
			playerHuman.Display(true);
			playerComputer.Display(true);
			Thread.Sleep(666);
			SetConsoleColor(endMsgColor);
			Console.WriteLine(endMsg);
		}
	}
}