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
		public static int minimumBet = 100;
		public static int deptAmount = 500;
		
		// here if you want to change some detailed rules
		public static bool botDrawIfPlayerFail = true;
		public static bool ifPickableAce = true;
		public static bool ifBotPickSmartAce = false;


		// here if you want to add a card
		public static Dictionary<string, int> CardsAndPoints = new Dictionary<string, int>()
		{
			{"A", 1},
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
			new("♡", ConsoleColor.White, ConsoleColor.DarkGreen),
			new("♤", ConsoleColor.Black, ConsoleColor.Cyan),
			new("♢", ConsoleColor.White, ConsoleColor.DarkBlue),
			new("♧", ConsoleColor.Black, ConsoleColor.Magenta),
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
		public static int deptActiviationAdd = 5;
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
				Write.SetColor(ConsoleColor.DarkGray);
				Write.SpeakLine("Le paquet est vide");
				return;
			}

			Card cardTop = paquet.First();
			pile.Add(cardTop);
			if (cardTop.IsValue("A"))
			{//ace score pick
				Write.SetColor(ConsoleColor.Yellow);
				Write.Speak("voilà un as pour {0} !", name);
				Write.SetColor(ConsoleColor.DarkGray);

				if (Settings.ifPickableAce)
				{//else smart for everyone
					Write.SpeakLine(" Il vaut 1 ou 11?");
					string? input;

					if (willChoose)
					{//player can choose
						while (true)
						{
							Write.SetColor(color);
							input = Console.ReadLine();
							if (input == "1" || input == "11") break;
							Write.SetColor(ConsoleColor.Red);
							Write.SpeakLine("Saisir 1 ou 11");
						}
					}
					else
					{//bot pick
						if (Settings.ifBotPickSmartAce)
						{//is smart
							input = (score + 11 > Settings.objectiveScore) ? "1" : "11";
						}
						else
						{//is random
							if (Game.random?.Next(1) == 0) input = "11";
							else input = "1";
						}
						Write.SetColor(color);
						Write.SpeakLine(input);
					}
					score += int.Parse(input);
				}
				else
				{
					int aceValue = (score + 11 > Settings.objectiveScore) ? 1 : 11;
					Write.SetColor(color);
					Write.SpeakLine(aceValue.ToString());
				}
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

			Write.SetColor(color);
			Write.Speak("{0} : ", name);

			if (!showFirst && pile.Count > 0)
			{
				Write.SetColor(ConsoleColor.DarkGray);
				Write.Speak("[?] ");
				startIndex = 1;
			}

			for (int i = startIndex; i < pile.Count; i++)
			{
				pile[i].Display();
				Write.ClearColor();
				Write.Speak(" ");
			}

			Write.SetColor(color);
			Write.SpeakLine("({0} points)", showFirst ? score : "?");
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
			Write.SetColor(foreground, background);
			Write.Speak("[{0}{1}]", symbol, value);
		}
	}

	class Write
	{
		// list of all avialbe colors
		public static List<ConsoleColor> colors = [
			ConsoleColor.Black, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.DarkBlue,
			ConsoleColor.DarkCyan, ConsoleColor.DarkGray, ConsoleColor.DarkGreen, ConsoleColor.DarkMagenta,
			ConsoleColor.DarkRed, ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.Gray,
			ConsoleColor.Green, ConsoleColor.Magenta, ConsoleColor.Red, ConsoleColor.White, ConsoleColor.Yellow
			];

		public static void Speak(string message)
		{
			Console.Write(message);
		}
		public static void Speak(string message, params object[]? args)
		{
			Console.Write(message, args);
		}
		public static void SpeakLine(string message)
		{
			Console.WriteLine(message);
		}
		public static void SpeakLine(string message, params object[]? args)
		{
			Console.WriteLine(message, args);
		}

		// change console's color
		public static void SetColor(ConsoleColor? foreground = ConsoleColor.White, ConsoleColor? background = null)
		{
			Console.ResetColor();
			if (foreground != null) Console.ForegroundColor = (ConsoleColor)foreground;
			if (background != null) Console.BackgroundColor = (ConsoleColor)background;
		}

		// just an alias for clear console color
		public static void ClearColor()
		{
			SetColor();
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
			Write.SetColor(ConsoleColor.White);
			Write.SpeakLine("Il y a {0} cartes dans le paquet.", cards.Count);

			// start
			Write.SetColor(ConsoleColor.White);
			Write.SpeakLine("\n///////////////////\n///  blackjack  ///\n///////////////////\n");
			Write.SetColor(ConsoleColor.White);
			Write.SpeakLine("Chacun pioche 2 cartes...");

			// give cards
			for (int i = 0; i < 2; i++)
			{
				playerHuman.Draw(cards);
				playerComputer.Draw(cards);
			}

			RoundEnd endReslut = Loop();

			Write.ClearColor();
			Write.SpeakLine("\nC'est fini !");
			Thread.Sleep(2222);
			playerHuman.Display(true);
			playerComputer.Display(true);
			Thread.Sleep(666);
			Write.SetColor(endReslut.color);
			Write.SpeakLine(endReslut.message);
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
				Write.SetColor(ConsoleColor.Black, ConsoleColor.Yellow);
				Write.Speak(" ! BLACKJACK ! ");
				Write.ClearColor();
				Write.SpeakLine("");
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
							Write.SetColor(ConsoleColor.Yellow);
							Write.SpeakLine("Vous avez tous les deux dépassés {0}...", Settings.objectiveScore);
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
						Write.ClearColor();
						Write.SpeakLine("apu de carte :(");
						return CalculateNearestEnd();
					}


					// player's turn
					if (!stopJoueur)
					{
						Thread.Sleep(1111);

						Write.ClearColor();
						Write.SpeakLine("piocher ?");
						Write.SetColor(ConsoleColor.DarkGray);
						Write.SpeakLine("o - ui\nn - nan");
						Thread.Sleep(666);
						string? choixJoueur;

						while (true)
						{
							Write.SetColor(ConsoleColor.Cyan);
							choixJoueur = Console.ReadLine();
							choixJoueur = choixJoueur?.ToUpper();
							if (choixJoueur == "O" || choixJoueur == "N") break;
							Write.SetColor(ConsoleColor.Red);
							Write.SpeakLine("Saisir O ou N");
						}

						if (choixJoueur == "O")
						{
							Write.SpeakLine("[joueur] je pioche");
							playerHuman.Draw(cards);
						}
						else
						{
							Write.SpeakLine("[joueur] je m'arrête");
							stopJoueur = true;
						}
					}
					Thread.Sleep(1111);
					playerHuman.Display(true);

					// middle checks
					if (!Settings.botDrawIfPlayerFail && playerHuman.score > Settings.objectiveScore)
					{
						return new RoundEnd(0, ConsoleColor.Red, "Tu as dépassé "+Settings.objectiveScore+", trop nul...");
					}
					if (cards.Count == 0)
					{
						Write.SetColor(ConsoleColor.White);
						Write.SpeakLine("apu de carte :(");
						return CalculateNearestEnd();
					}

					// computer's turn
					if (!stopOrdi)
					{
						Thread.Sleep(666);
						Write.SetColor(ConsoleColor.Blue);
						if (playerComputer.score <= 15)
						{
							Write.SpeakLine("[ordi] je pioche");
							playerComputer.Draw(cards);
						}
						else
						{
							Write.SpeakLine("[ordi] je m'arrête");
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
				Write.SetColor(ConsoleColor.Cyan);
				string? inputString = Console.ReadLine();
				int inputInt;
				if (defaultValue != null && (inputString == null || inputString == ""))
					return (int)defaultValue;
				else if (int.TryParse(inputString, out inputInt) && inputInt >= inputMinIncluded && inputInt <= inputMaxIncluded)
					return inputInt;
				else
				{
					Write.SetColor(ConsoleColor.Red);
					Write.SpeakLine("donne un nombre valide ({0}-{1})", inputMinIncluded, inputMaxIncluded);
				}
			}
		}


		public static Random random = new Random();

		static void SetSettings()
		{
			// pick name
			Write.ClearColor();
			Write.SpeakLine("donne ton nom ou c'est moi qui choisi");
			Write.SetColor(ConsoleColor.Cyan);
			string? nameInput = Console.ReadLine();
			if (!(nameInput == null || nameInput == "")) Settings.humanName = nameInput;

			// * deck composition
			// pick cards of deck
			List<string> aviableCards = [.. Settings.CardsAndPoints.Keys];
			List<string> paquet = Settings.deckComposition;
			Write.ClearColor();
			Write.SpeakLine("voici le jeu de cartes :");
			while (true)
			{
				// console
				Write.SetColor(ConsoleColor.Blue);
				Write.SpeakLine(string.Join(" ", paquet));
				Write.SetColor(ConsoleColor.DarkGray);
				Write.SpeakLine("<entrée> pour valider, ou en saisir un nouveau");
				Write.SetColor(ConsoleColor.Cyan);
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
						Write.SetColor(ConsoleColor.Red);
						Write.SpeakLine("la carte [{0}] n'existe pas", v);
						valid = false;
					}
				}
				// validate proposition
				if (valid)
				{
					paquet = paquetProposed;
					Write.ClearColor();
					Write.SpeakLine("nouveau paquet :");
				}
			}
			Settings.deckComposition = paquet;

			// pick colors of deck
			Write.ClearColor();
			Write.Speak("choisir le nombre de couleurs ");
			Write.SetColor(ConsoleColor.DarkGray);
			Write.SpeakLine("({0} par défaut)", Settings.deckColorAmount);
			Settings.deckColorAmount = IntPut(0, 10, Settings.deckColorAmount);

			// pick amount of deck
			Write.ClearColor();
			Write.Speak("choisir le nombre de paquets ");
			Write.SetColor(ConsoleColor.DarkGray);
			Write.SpeakLine("({0} par défaut)", Settings.deckPileAmount);
			Settings.deckPileAmount = IntPut(0, 42, Settings.deckPileAmount);

			// start display
			Thread.Sleep(666);
			Write.SetColor(ConsoleColor.Black, ConsoleColor.Yellow);
			Write.Speak(" parviendras-tu à être millionaire ? ");
			Write.SetColor();
			Write.Speak("\n\n");
			Thread.Sleep(666);
		}

		static int money;
		static int dept = 0;
		static int bet;

		static void onDept(int amout)
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
		
		public static void exitDeptMessage()
		{
			Write.SetColor(ConsoleColor.Red);
			string randomMessage = Settings.deptMessages[random.Next(Settings.deptMessages.Count)];
			Write.SpeakLine(randomMessage, dept);
			Write.ClearColor();
		}

		static void Main(string[] args)
		{
			SetSettings();

			money = Settings.startingMoney;

			while (true)
			{
				GameDepts();//calculate dept
				GameBet();//ask for the bet

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
			if (money < Settings.minimumBet)
				Write.SetColor(ConsoleColor.Red);
			else
				Write.SetColor(ConsoleColor.Yellow);
			Write.SpeakLine("tu as {0}$", money);
			Thread.Sleep(666);

			// dept
			bool deptAction = false;
			if (dept > 0)
			{
				if (money - Settings.minimumBet >= dept)
				{
					deptAction = true;
					Thread.Sleep(1111);
					Write.SetColor(ConsoleColor.White, ConsoleColor.Green);
					Write.Speak("tu as réglé ta dette ! (-{0}$)", dept);
					offDept();
					Write.SetColor();
					Write.SpeakLine("");
					Thread.Sleep(2222);
					Write.SetColor(ConsoleColor.Green);
					Write.SpeakLine("tu es libre de partir");
				}
				else
				{
					int intrest = (int)(dept * Settings.deptIntrestFactor) + Settings.deptIntrestAdd;
					dept += intrest;
					Thread.Sleep(1111);
					Write.SetColor(ConsoleColor.Red);
					Write.SpeakLine("ta dette est désormais à {0}$ (+{1}$ d'intrérêts)", dept, intrest);
				}
			}

			if (money < Settings.minimumBet)
			{
				deptAction = true;
				int amount = Settings.deptAmount;
				bool wasUndepted = dept == 0;
				onDept(amount);
				int intrest = (int)(dept * Settings.deptActiviationFactor) + Settings.deptActiviationAdd;
				dept += intrest;
				Write.SetColor(ConsoleColor.Black, ConsoleColor.Red);
				Thread.Sleep(2222);
				Write.Speak("tu n'as plus assez d'argent! ");
				Write.ClearColor();
				Write.SpeakLine("");
				if (wasUndepted)
				{
					Thread.Sleep(2222);
					Write.SetColor(ConsoleColor.Red);
					Write.SpeakLine("la gambling addicition t'as vaincue.");
					Thread.Sleep(2222);
					Write.SetColor(ConsoleColor.Red);
					Write.SpeakLine("tu fais un emprunt de {0}$ pour continuer. (+{1}$ à rebourser)", amount, intrest);
				}
				else
				{
					Thread.Sleep(2222);
					Write.SetColor(ConsoleColor.Red);
					Write.SpeakLine("la gambling addicition t'as encore vaincue.");
					Thread.Sleep(2222);
					Write.SetColor(ConsoleColor.Red);
					Write.SpeakLine("tu fais un emprunt de {0}$ supplémentaires. (dette totale de {1}$)", amount, dept);
				}
				Thread.Sleep(1111);
				Write.SetColor(ConsoleColor.Red);
				Write.SpeakLine("tu devera régler cette dette avant de partir.");
			}

			if (deptAction)
			{
				// money
				Thread.Sleep(1111);
				if (money < Settings.minimumBet)
					Write.SetColor(ConsoleColor.Red);
				else
					Write.SetColor(ConsoleColor.Yellow);
				Write.SpeakLine("tu as maintenant {0}$", money);
			}
		}

		static void GameBet()
		{
			// bet
			Write.SetColor();
			Write.Speak("combien tu veux miser ?");
			Write.SetColor(ConsoleColor.DarkGray);
			Write.SpeakLine(" attention à la banqueroute !");
			bet = IntPut(Settings.minimumBet, money);
		}
	}

	class Invicible
	{
		static Action cancelAction = Game.exitDeptMessage;

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