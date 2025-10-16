using System;
using System.Runtime.InteropServices;
using BadJack;

namespace BadJack
{
	class Settings
	{
		// here if you want to change basic rule
		public static int objectiveScore = 21;
		public static int startingMoney = 1000;
		public static int deptAmount = 500;


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

		// settings you can edit in terminal
		public static List<string> deckComposition = CardsAndPoints.Keys.ToList();
		public static string humanName = "un humain trop nul";
		public static int deckColorAmount = 4;
		public static int deckPileAmount = 2;

		// other custom settings
		public static double deptActiviationFactor = 0;
		public static int deptActiviationAdd = 10;
		public static double deptIntrestFactor = 0.05;
		public static int deptIntrestAdd = 0;
		public static List<string> deptMessages = [
			"tu veux partir? tu n'as qu'à pas être PAUVRE !",
			"tu dois {0}$ à la banque, par le droit d'abandonner !",
			"il est illégal de partir, tu dois {0}$ à la banque !",
			"rembourse les {0}$ que tu as empruntés avant !",
			"tu as devoir gamble jusqu'à rembourser {0}$ !",
			"tu n'as pas le droit de partir, rembourse les {0}$ !",
			"règle tes dettes avant de partir, il te reste {0}$ de dettes !",
			"tu resteras ici tant que tu n'as pas reboursé les {0}$ !",
		];

		public static List<string> randomBotName = new List<string> { 
			"CrazyBot","CrazyVerno","Sheeesh","CrazyScale","CrazyTismé","CrazyMillionaire","WoodyWoodpecker","CrazyValet","CrazyAnge", "CrazySteiro"
			};
	}

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
				Color.SetConsole(ConsoleColor.DarkGray);
				Console.WriteLine("Le paquet est vide");
				return;
			}

			Card cardTop = paquet.First();
			pile.Add(cardTop);
			if (cardTop.IsValue("1"))
			{//ace score pick
				Color.SetConsole(ConsoleColor.Yellow);
				Console.Write("voilà un as pour {0} !", name);
				Color.SetConsole(ConsoleColor.DarkGray);
				Console.WriteLine(" Il vaut 1 ou 11?");
				string? input;

				if (willChoose)
				{//player can choose
					while (true)
					{
						Color.SetConsole(color);
						input = Console.ReadLine();
						if (input == "1" || input == "11") break;
						Color.SetConsole(ConsoleColor.Red);
						Console.WriteLine("Saisir 1 ou 11");
					}
				}
				else
				{//is random
					if (Game.random?.Next(1) == 0) input = "11";
					else input = "1";
					Color.SetConsole(color);
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

			Color.SetConsole(color);
			Console.Write("{0} : ", name);

			if (!showFirst)
			{
				Color.SetConsole(ConsoleColor.DarkGray);
				Console.Write("[?] ");
				startIndex = 1;
			}

			for (int i = pile.Count - 1; i >= startIndex; i--)
			{
				pile[i].Display();
				Color.ClearConsole();
				Console.Write(" ");
			}

			Color.SetConsole(color);
			Console.WriteLine("({0} points)", showFirst ? score : "?");
		}
	}

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
			Color.SetConsole(foreground, background);
			Console.Write("[{0}{1}]", symbol, value);
		}
	}

	class Color
	{
		public static List<ConsoleColor> colors = [
			ConsoleColor.Black, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.DarkBlue,
			ConsoleColor.DarkCyan, ConsoleColor.DarkGray, ConsoleColor.DarkGreen, ConsoleColor.DarkMagenta,
			ConsoleColor.DarkRed, ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.Gray,
			ConsoleColor.Green, ConsoleColor.Magenta, ConsoleColor.Red, ConsoleColor.White, ConsoleColor.Yellow
			];

		// change console's color
		public static void SetConsole(ConsoleColor? foreground = ConsoleColor.White, ConsoleColor? background = null)
		{
			Console.ResetColor();
			if (foreground != null) Console.ForegroundColor = (ConsoleColor)foreground;
			if (background != null) Console.BackgroundColor = (ConsoleColor)background;
		}

		// just an alias
		public static void ClearConsole()
		{
			SetConsole();
		}
	}

	class RoundEnd
	{
		public ConsoleColor color;
		public string message;
		public double factor;

		public RoundEnd(double factor, ConsoleColor color, string message)
		{
			this.color = color;
			this.message = message;
			this.factor = factor;
		}
	};


	class Round
	{

		// init
		Player playerHuman;
		Player playerComputer;
		List<Card> cards;

		public Round()
		{
			// init
			playerHuman = new(true, Settings.humanName, ConsoleColor.Cyan);
			Random rnd = new Random();
			string botname = Settings.randomBotName[rnd.Next(Settings.randomBotName.Count)];
			playerComputer = new(false, botname, ConsoleColor.Blue);
			cards = [];
		}

		public double Play()
		{
			// shuffle
			for (int i = 0; i < Settings.deckColorAmount; i++)
			{
				for (int j = 0; j < Settings.deckColorAmount; j++)
				{
					List<Card> cardsColor = Settings.deckComposition.ConvertAll(v => new Card(v, Settings.globalSuits[i]));
					cards.AddRange(cardsColor);
				}
			}
			cards = cards.OrderBy(x => Guid.NewGuid()).ToList();
			Color.SetConsole(ConsoleColor.White);
			Console.WriteLine("Il y a {0} cartes dans le paquet.", cards.Count);

			// start
			Color.SetConsole(ConsoleColor.White);
			Console.WriteLine("\n///////////////////\n///  blackjack  ///\n///////////////////\n");
			Color.SetConsole(ConsoleColor.White);
			Console.WriteLine("Chacun pioche 2 cartes...");

			// give cards
			for (int i = 0; i < 2; i++)
			{
				playerHuman.Draw(cards);
				playerComputer.Draw(cards);
			}

			RoundEnd endReslut = Loop();

			Color.ClearConsole();
			Console.WriteLine("\nC'est fini !");
			Thread.Sleep(2222);
			playerHuman.Display(true);
			playerComputer.Display(true);
			Thread.Sleep(666);
			Color.SetConsole(endReslut.color);
			Console.WriteLine(endReslut.message);
			return endReslut.factor;
		}

		RoundEnd Loop()
		{
			// turns
			bool stopJoueur = false;
			bool stopOrdi = false;

			// blackjack checks
			bool jackComputer = playerComputer.score == Settings.objectiveScore;
			bool jackHuman = playerHuman.score == Settings.objectiveScore;
			if (jackComputer || jackHuman)
			{
				Color.SetConsole(ConsoleColor.Black, ConsoleColor.Yellow);
				Console.Write(" ! BLACKJACK ! ");
				Color.ClearConsole();
				Console.WriteLine("");
				if (jackComputer && jackHuman)
				{
					return new RoundEnd(1, ConsoleColor.Yellow, "L'ordi et toi avez blackjack!!!! vous avez trichés??");
				}
				else if (jackComputer)
				{
					return new RoundEnd(0, ConsoleColor.Red, "L'ordi a blackjack!! dommage pour toi...");
				}
				else if (jackHuman)
				{
					return new RoundEnd(2.5, ConsoleColor.Green, "Tu as blackjack!! bien joué !");
				}
			}
			else
			{
				playerHuman.Display(true);
				playerComputer.Display(false);
				while (true)
				{
					// endgame triggers
					bool looseComputer = playerComputer.score > Settings.objectiveScore;
					bool looseHuman = playerHuman.score > Settings.objectiveScore;

					if (looseComputer)
					{
						if (looseHuman)
						{
							Color.SetConsole(ConsoleColor.Yellow);
							Console.WriteLine("Vous avez tous les deux dépassés {0}...", Settings.objectiveScore);
							return CalculateNearestEnd();
						}
						else
						{
							return new RoundEnd(2, ConsoleColor.Green, "L'ordi a dépassé "+Settings.objectiveScore+", bien joué !");
						}
					}

					if (looseHuman)
					{
						return new RoundEnd(0, ConsoleColor.Red, "Tu as dépassé "+Settings.objectiveScore+", trop nul...");
					}

					if (stopJoueur && stopOrdi)
					{
						return CalculateNearestEnd();
					}

					// no cards
					if (cards.Count == 0)
					{
						Color.ClearConsole();
						Console.WriteLine("apu de carte :(");
						return CalculateNearestEnd();
					}


					// player's turn
					if (!stopJoueur)
					{
						Thread.Sleep(1111);

						Color.ClearConsole();
						Console.WriteLine("piocher ?");
						Color.SetConsole(ConsoleColor.DarkGray);
						Console.WriteLine("o - ui\nn - nan");
						Thread.Sleep(666);
						string? choixJoueur;

						while (true)
						{
							Color.SetConsole(ConsoleColor.Cyan);
							choixJoueur = Console.ReadLine();
							choixJoueur = choixJoueur?.ToUpper();
							if (choixJoueur == "O" || choixJoueur == "N") break;
							Color.SetConsole(ConsoleColor.Red);
							Console.WriteLine("Saisir O ou N");
						}

						if (choixJoueur == "O")
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
						Color.SetConsole(ConsoleColor.White);
						Console.WriteLine("apu de carte :(");
						return CalculateNearestEnd();
					}

					// computer's turn
					if (!stopOrdi)
					{
						Thread.Sleep(666);
						Color.SetConsole(ConsoleColor.Blue);
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
			// will never happend
			return CalculateNearestEnd();
		}

		RoundEnd CalculateNearestEnd()
		{//happening when both stop or no more cards
			int playerComputerDiff = Math.Abs(playerComputer.score - Settings.objectiveScore);
			int playerHumanDiff = Math.Abs(playerHuman.score - Settings.objectiveScore);
			if (playerComputerDiff < playerHumanDiff)
			{
				return new RoundEnd(0, ConsoleColor.Red, "L'ordinateur t'as roulé dessus...");
			}
			else if (playerComputerDiff > playerHumanDiff)
			{
				return new RoundEnd(2, ConsoleColor.Green, "Tu as roulé l'ordinateur! GG");
			}
			else
			{
				return new RoundEnd(1, ConsoleColor.Yellow, "Personne n'a gangné...");
			}
		}
	}

	class Game
		{
			// function to get input as int without crash
			static int IntPut(int inputMinIncluded, int inputMaxIncluded, int? defaultValue = null)
			{
				while (true)
				{
					Color.SetConsole(ConsoleColor.Cyan);
					string? inputString = Console.ReadLine();
					int inputInt;
					if (defaultValue != null && (inputString == null || inputString == ""))
						return (int)defaultValue;
					else if (int.TryParse(inputString, out inputInt) && inputInt >= inputMinIncluded && inputInt <= inputMaxIncluded)
						return inputInt;
					else
					{
						Color.SetConsole(ConsoleColor.Red);
						Console.WriteLine("donne un nombre valide ({0}-{1})", inputMinIncluded, inputMaxIncluded);
					}
				}
			}


			public static Random random = new Random();

			static void SetSettings()
			{
				// pick name
				Color.ClearConsole();
				Console.WriteLine("donne ton nom ou c'est moi qui choisi");
				Color.SetConsole(ConsoleColor.Cyan);
				string? nameInput = Console.ReadLine();
				if (!(nameInput == null || nameInput == "")) Settings.humanName = nameInput;

				// * deck composition
				// pick cards of deck
				List<string> aviableCards = [.. Settings.CardsAndPoints.Keys];
				List<string> paquet = Settings.deckComposition;
				Color.ClearConsole();
				Console.WriteLine("voici le jeu de cartes :");
				while (true)
				{
					// console
					Color.SetConsole(ConsoleColor.Blue);
					Console.WriteLine(string.Join(" ", paquet));
					Color.SetConsole(ConsoleColor.DarkGray);
					Console.WriteLine("<entrée> pour valider, ou en saisir un nouveau");
					Color.SetConsole(ConsoleColor.Cyan);
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
							Color.SetConsole(ConsoleColor.Red);
							Console.WriteLine("la carte [{0}] n'existe pas", v);
							valid = false;
						}
					}
					// validate proposition
					if (valid)
					{
						paquet = paquetProposed;
						Color.ClearConsole();
						Console.WriteLine("nouveau paquet :");
					}
				}
				Settings.deckComposition = paquet;

				// pick colors of deck
				Color.ClearConsole();
				Console.Write("choisir le nombre de couleurs ");
				Color.SetConsole(ConsoleColor.DarkGray);
				Console.WriteLine("({0} par défaut)", Settings.deckColorAmount);
				Settings.deckColorAmount = IntPut(0, 8, Settings.deckColorAmount);

				// pick amount of deck
				Color.ClearConsole();
				Console.Write("choisir le nombre de paquets ");
				Color.SetConsole(ConsoleColor.DarkGray);
				Console.WriteLine("({0} par défaut)", Settings.deckPileAmount);
				Settings.deckPileAmount = IntPut(0, 42, Settings.deckPileAmount);
			}

			static int money;
			static int dept = 0;

			static void onDept(int amout = 100)
			{
				dept += amout;
				money += amout;
				Invicible.On();
			}
			static void offDept()
			{
				money -= dept;
				dept = 0;
				Invicible.Off();
			}
			
			public static void deptMessage()
			{
				Color.SetConsole(ConsoleColor.Red);
				string randomMessage = Settings.deptMessages[random.Next(Settings.deptMessages.Count)];
				Console.WriteLine(randomMessage, dept);
				Color.ClearConsole();
			}

			static void Main(string[] args)
			{
				SetSettings();

				money = Settings.startingMoney;

				while (true)
				{
					GameDepts();

					// bet
					Color.SetConsole();
					Console.Write("combien tu veux miser ?");
					Color.SetConsole(ConsoleColor.DarkGray);
					Console.WriteLine(" attention à la banqueroute !");
					int bet = IntPut(10, money);

					// play
					money -= bet;
					Round round = new();
					double victory = round.Play();

					// earn
					money += (int)Math.Round(victory * bet);
				}
			}

			static void GameDepts()
			{
				// money
				Thread.Sleep(666);
				if (money < 10)
					Color.SetConsole(ConsoleColor.Red);
				else
					Color.SetConsole(ConsoleColor.Yellow);
				Console.WriteLine("tu as {0}$", money);
				Thread.Sleep(666);

				// dept
				bool deptAction = false;
				if (dept > 0)
				{
					deptAction = true;
					if (money - 10 >= dept)
					{
						Thread.Sleep(1111);
						Color.SetConsole(ConsoleColor.White, ConsoleColor.Green);
						Console.Write("tu as réglé ta dette ! (-{0}$)", dept);
						offDept();
						Color.SetConsole();
						Console.WriteLine("");
						Thread.Sleep(2222);
						Color.SetConsole(ConsoleColor.Green);
						Console.WriteLine("tu es libre de partir");
					}
					else
					{
						int intrest = (int)(dept * Settings.deptIntrestFactor) + Settings.deptIntrestAdd;
						dept += intrest;
						Thread.Sleep(1111);
						Color.SetConsole(ConsoleColor.Red);
						Console.WriteLine("ta dette est désormais à {0}$ (dont {1}$ d'intrérêts)", dept, intrest);
					}
				}
				
				if (money < 10)
				{
					deptAction = true;
					int amount = Settings.deptAmount;
					bool wasUndepted = dept == 0;
					onDept(amount);
					int intrest = (int)(dept * Settings.deptActiviationFactor) + Settings.deptActiviationAdd;
					dept += intrest;
					Color.SetConsole(ConsoleColor.Black, ConsoleColor.Red);
					Thread.Sleep(2222);
					Console.Write("tu n'as plus assez d'argent! ");
					Color.ClearConsole();
					Console.WriteLine("");
					if (wasUndepted)
					{
						Thread.Sleep(2222);
						Color.SetConsole(ConsoleColor.Red);
						Console.WriteLine("la gambling addicition t'as vaincue.");
						Thread.Sleep(2222);
						Color.SetConsole(ConsoleColor.Red);
						Console.WriteLine("tu fais un emprunt de {0}$ pour continuer. (+{1}$ à rebourser)", amount, intrest);
					}
					else
					{
						Thread.Sleep(2222);
						Color.SetConsole(ConsoleColor.Red);
						Console.WriteLine("la gambling addicition t'as encore vaincue.");
						Thread.Sleep(2222);
						Color.SetConsole(ConsoleColor.Red);
						Console.WriteLine("tu fais un emprunt de {0}$ supplémentaires. (dette totale de {1}$)", amount, dept);
					}
					Thread.Sleep(1111);
					Color.SetConsole(ConsoleColor.Red);
					Console.WriteLine("tu devera régler cette dette avant de partir.");
				}
				
				if (deptAction)
				{
					// money
					Thread.Sleep(1111);
					if (money < 10)
						Color.SetConsole(ConsoleColor.Red);
					else
						Color.SetConsole(ConsoleColor.Yellow);
					Console.WriteLine("tu as maintenant {0}$", money);
				}
			}
		}

	class Invicible
	{
		static Action cancelAction = Game.deptMessage;

		private delegate bool CancelExitDelegate(int eventType);
		private static CancelExitDelegate _handler = new(CancelExit);
		private static bool _killDisabled = false;

		public static void On()
		{
			if (_killDisabled) return;
			Console.CancelKeyPress += CancelKey;
			_killDisabled = true;
		}

		public static void Off()
		{
			if (!_killDisabled) return;
			Console.CancelKeyPress -= CancelKey;
			_killDisabled = false;
		}

		private static void CancelKey(object? sender, ConsoleCancelEventArgs e)
		{
			cancelAction();
			e.Cancel = true;
		}

		private static bool CancelExit(int eventType)
		{
			cancelAction();
			return true;
		}
	}
}