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
		public static bool botDrawIfPlayerFail = false;
		public static bool ifPickableAce = true;
		public static bool ifBotPickSmartAce = true;


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
			new("💧", ConsoleColor.White, ConsoleColor.Blue),
			new("$", ConsoleColor.White, ConsoleColor.Green),
		];

		// settings you can edit in terminal
		public static List<string> deckComposition = CardsAndPoints.Keys.ToList();
		public static string humanName = "Un humain trop nul";
		public static int deckColorAmount = 4;
		public static int deckPileAmount = 2;

		// other custom settings
		public static int moneyObjective = 1000000;
		public static bool invicibleWhenInDept = true;
		public static double deptActiviationFactor = 0;
		public static int deptActiviationAdd = 5;
		public static double deptIntrestFactor = 0.05;
		public static int deptIntrestAdd = 0;
		public static double betableByMoneyFactor = 2;
		public static List<string> deptMessages = [
			"Tu veux partir? Tu n'as qu'à pas être PAUVRE !",
			"Tu dois {0}$ à la banque, par le droit d'abandonner !",
			"Il est illégal de partir, tu dois {0}$ à la banque !",
			"Rembourse les {0}$ que tu as empruntés avant !",
			"Tu vas devoir gamble jusqu'à rembourser {0}$ !",
			"Tu n'as pas le droit de partir, rembourse les {0}$ !",
			"Règle tes dettes avant de partir, il te reste {0}$ de dettes !",
			"Tu resteras ici tant que tu n'as pas remboursé les {0}$ !",
		];

		public static List<string> randomBotName = new List<string> {
			"CrazyBot","CrazyVerno","Sheeesh","CrazyScale","CrazyTismé","CrazyMillionaire","WoodyWoodpecker","CrazyValet","CrazyAnge", "CrazySteiro", "Beep-Boop", "El Robotito", 
			};

		public static int WaitSpeakLetter = 10;
		public static int WaitSpeakSpace = 50;
	}

	class Player
	{
		List<Card> pile;
		public int score;
		public string name;
		bool willChoose;
		ConsoleColor color;

		public Player(bool willChoose = false, string name = "No-name guy", ConsoleColor color = ConsoleColor.White)
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
				Write.Speak("Voilà un as pour {0} !", name);
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
							Write.PrintLine("Saisir 1 ou 11");
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

		// private write
		public static void Speaking(string message)
		{
			//Console.Write(message);
			foreach (char letter in message)
			{
				Console.Write(letter);
				Thread.Sleep((letter == ' ') ? Settings.WaitSpeakSpace : Settings.WaitSpeakLetter);
			}
		}

		public static void Printing(string message)
		{
			Console.Write(message);
		}

		public static void JumpLine()
		{
			ConsoleColor ForegroundColor = Console.ForegroundColor;
			ConsoleColor BackgroundColor = Console.BackgroundColor;
			Console.ResetColor();
			Console.WriteLine();
			Console.ForegroundColor = ForegroundColor;
			Console.BackgroundColor = BackgroundColor;
		}

		// write interfaces
		public static void Speak(string message)
		{
			Speaking(message);
		}
		public static void Speak(string message, params object[] args)
		{
			message = string.Format(message, args);
			Speaking(message);
		}
		public static void SpeakLine(string message)
		{
			Speaking(message);
			JumpLine();
		}
		public static void SpeakLine(string message, params object[] args)
		{
			message = string.Format(message, args);
			Speaking(message);
			JumpLine();
		}

		public static void Print(string message)
		{
			Printing(message);
		}
		public static void Print(string message, params object[] args)
		{
			message = string.Format(message, args);
			Printing(message);
		}
		public static void PrintLine(string message)
		{
			Printing(message);
			JumpLine();
		}
		public static void PrintLine(string message, params object[] args)
		{
			message = string.Format(message, args);
			Printing(message);
			JumpLine();
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
			Write.PrintLine("\n///////////////////\n///	blackjack	///\n///////////////////\n");
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
					return new RoundEnd(1, ConsoleColor.Yellow, "L'ordi et toi avez blackjack !!!! vous avez trichés??");
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
							return new RoundEnd(2, ConsoleColor.Green, "L'ordi a dépassé " + Settings.objectiveScore + ", bien joué !");
						}
					}

					if (looseHuman)
					{
						return new RoundEnd(0, ConsoleColor.Red, "Tu as dépassé " + Settings.objectiveScore + ", trop nul...");
					}

					if (stopJoueur && stopOrdi)
					{
						return CalculateNearestEnd();
					}

					// no cards
					if (cards.Count == 0)
					{
						Write.ClearColor();
						Write.SpeakLine("Yapu de carte :(");
						return CalculateNearestEnd();
					}


					// player's turn
					if (!stopJoueur)
					{
						Thread.Sleep(1111);

						Write.ClearColor();
						Write.SpeakLine("Piocher ?");
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
							Write.PrintLine("Saisir O ou N");
						}

						if (choixJoueur == "O")
						{
							Write.SpeakLine("[{0}] Je pioche", playerHuman.name);
							playerHuman.Draw(cards);
						}
						else
						{
							Write.SpeakLine("[{0}] Je m'arrête", playerHuman.name);
							stopJoueur = true;
						}
					}
					Thread.Sleep(1111);
					playerHuman.Display(true);

					// middle checks
					if (!Settings.botDrawIfPlayerFail && playerHuman.score > Settings.objectiveScore)
					{
						return new RoundEnd(0, ConsoleColor.Red, "Tu as dépassé " + Settings.objectiveScore + ", trop nul...");
					}
					if (cards.Count == 0)
					{
						Write.SetColor(ConsoleColor.White);
						Write.SpeakLine("Yapu de carte :(");
						return CalculateNearestEnd();
					}

					// computer's turn
					if (!stopOrdi)
					{
						Thread.Sleep(666);
						Write.SetColor(ConsoleColor.Blue);
						if (playerComputer.score <= 15)
						{
							Write.SpeakLine("[{0}] Je pioche", playerComputer.name);
							playerComputer.Draw(cards);
						}
						else
						{
							Write.SpeakLine("[{0}] Je m'arrête", playerComputer.name);
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
				return new RoundEnd(1, ConsoleColor.Yellow, "Personne n'a gagné...");
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
					Write.PrintLine("Donne un nombre valide ({0}-{1})", inputMinIncluded, inputMaxIncluded);
				}
			}
		}


		public static Random random = new Random();

		static void SetSettings()
		{
			// pick name
			Write.ClearColor();
			Write.SpeakLine("Donne ton nom ou c'est moi qui choisis");
			Write.SetColor(ConsoleColor.Cyan);
			string? nameInput = Console.ReadLine();
			if (!(nameInput == null || nameInput == "")) Settings.humanName = nameInput;

			// * deck composition
			// pick cards of deck
			List<string> aviableCards = [.. Settings.CardsAndPoints.Keys];
			List<string> paquet = Settings.deckComposition;
			Write.ClearColor();
			Write.SpeakLine("Voici le jeu de cartes :");
			while (true)
			{
				// console
				Write.SetColor(ConsoleColor.Blue);
				Write.PrintLine(string.Join(" ", paquet));
				Write.SetColor(ConsoleColor.DarkGray);
				Write.SpeakLine("Appuyez sur <entrée> pour valider, ou en saisir un nouveau");
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
						Write.SpeakLine("La carte [{0}] n'existe pas", v);
						valid = false;
					}
				}
				// validate proposition
				if (valid)
				{
					paquet = paquetProposed;
					Write.ClearColor();
					Write.SpeakLine("Nouveau paquet :");
				}
			}
			Settings.deckComposition = paquet;

			// pick colors of deck
			Write.ClearColor();
			Write.Speak("Choisir le nombre de couleurs ");
			Write.SetColor(ConsoleColor.DarkGray);
			Write.SpeakLine("({0} par défaut)", Settings.deckColorAmount);
			Settings.deckColorAmount = IntPut(0, Settings.globalSuits.Count, Settings.deckColorAmount);

			// pick amount of deck
			Write.ClearColor();
			Write.Speak("Choisir le nombre de paquets ");
			Write.SetColor(ConsoleColor.DarkGray);
			Write.SpeakLine("({0} par défaut)", Settings.deckPileAmount);
			Settings.deckPileAmount = IntPut(0, 42, Settings.deckPileAmount);

			GameObjective();
		}

		static int money;
		static int dept = 0;
		static int bet;
		static int objectiveState = 0;

		static void onDept(int amout)
		{
			dept += amout;
			money += amout;
			if (Settings.invicibleWhenInDept) Invicible.On();
		}
		static void offDept()
		{
			money -= dept;
			dept = 0;
			if (Settings.invicibleWhenInDept) Invicible.Off();
		}

		public static void exitDeptMessage()
		{
			Write.SetColor(ConsoleColor.Red);
			string randomMessage = Settings.deptMessages[random.Next(Settings.deptMessages.Count)];
			Write.PrintLine(randomMessage, dept);
			Write.ClearColor();
		}

		static void Main(string[] args)
		{
			SetSettings();

			money = Settings.startingMoney;

			while (true)
			{
				GameDepts();//calculate dept
				GameObjective();//check if million
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
			Write.SpeakLine("Tu as {0}$", money);
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
					Write.Speak("Tu as réglé ta dette ! (-{0}$)", dept);
					offDept();
					Write.SetColor();
					Write.SpeakLine("");
					Thread.Sleep(1111);
					Write.SetColor(ConsoleColor.Green);
					Write.SpeakLine("Tu es libre de partir");
				}
				else
				{
					int intrest = (int)(dept * Settings.deptIntrestFactor) + Settings.deptIntrestAdd;
					dept += intrest;
					Thread.Sleep(1111);
					Write.SetColor(ConsoleColor.Red);
					Write.SpeakLine("Ta dette est désormais à {0}$ (+{1}$ d'intérêts)", dept, intrest);
				}
			}

			if (money < Settings.minimumBet)
			{
				deptAction = true;
				int amount = Settings.deptAmount + ((money < 0) ? Math.Abs(money) : 0);
				bool wasUndepted = dept == 0;
				onDept(amount);
				int intrest = (int)(dept * Settings.deptActiviationFactor) + Settings.deptActiviationAdd;
				dept += intrest;
				Write.SetColor(ConsoleColor.Black, ConsoleColor.Red);
				Thread.Sleep(1111);
				Write.Speak("Tu n'as plus assez d'argent!");
				Write.ClearColor();
				Write.SpeakLine("");
				Write.SetColor(ConsoleColor.Red);
				if (wasUndepted)
				{
					Thread.Sleep(1111);
					Write.SpeakLine("La gambling addicition t'as vaincue.");
					Thread.Sleep(1111);
					Write.SpeakLine("Tu fais un emprunt de {0}$ pour continuer. (+{1}$ à rebourser)", amount, intrest);
				}
				else
				{
					Thread.Sleep(666);
					Write.SpeakLine("La gambling addicition t'as encore vaincue.");
					Thread.Sleep(666);
					Write.SpeakLine("Tu fais un emprunt de {0}$ supplémentaires. (dette totale de {1}$)", amount, dept);
				}
				Thread.Sleep(1111);
				Write.SpeakLine("Tu devras régler cette dette avant de partir.");
			}

			if (deptAction)
			{
				// money
				Thread.Sleep(1111);
				if (money < Settings.minimumBet)
					Write.SetColor(ConsoleColor.Red);
				else
					Write.SetColor(ConsoleColor.Yellow);
				Write.SpeakLine("Tu as maintenant {0}$", money);
			}
		}

		static void GameObjective()
		{
			if (objectiveState == 0)
			{
				objectiveState++;
				Write.SetColor(ConsoleColor.Black, ConsoleColor.Yellow);
				Write.Speak(" Parviendras-tu à être millionaire ? ");
				Write.SetColor();
				Write.Speak("\n\n");
				Thread.Sleep(666);
			}
			else if (objectiveState == 1 && money >= Settings.moneyObjective)
			{
				objectiveState++;

				Write.SetColor(ConsoleColor.White, ConsoleColor.Yellow);
				Thread.Sleep(666);
				Write.Speak(" Tu ");
				Thread.Sleep(666);
				Write.Speak("es ");
				Thread.Sleep(2222);
				Write.Speak("millionaire");
				Thread.Sleep(666);
				Write.Speak(" ! ");
				Thread.Sleep(2222);
				Write.SetColor();
				Write.Speak("\n");
				Write.SetColor(ConsoleColor.Magenta);
				Write.SpeakLine("mais ce n'est pas pour ça que tu va t'arrêter, non ?\n");
			}
		}
		
		static void GameBet()
		{
			// bet
			Write.SetColor();
			Write.Speak("Combien veux-tu miser ?");
			Write.SetColor(ConsoleColor.DarkGray);
			Write.SpeakLine(" Attention à la banqueroute !");
			bet = IntPut(Settings.minimumBet, (int)Math.Round(money * Settings.betableByMoneyFactor));
			//if (bet > money)
		}
	}

	class Invicible
	{
		// settings
		static Action cancelAction = Game.exitDeptMessage;
		private static bool _killDisabled = false;


		private static PosixSignalRegistration? _sigTerm;
		private static PosixSignalRegistration? _sigHup;
		private static PosixSignalRegistration? _sigInt;

#if WINDOWS
		// Windows console exit catch
		private delegate bool ConsoleEventDelegate(int eventType);
		private static ConsoleEventDelegate? _winHandler;

		[DllImport("Kernel32")]
		private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
#endif

		public static void On()
		{
			if (_killDisabled) return;
			_killDisabled = true;

			Console.CancelKeyPress += CancelKey;

#if WINDOWS
			// Windows-specific: stop console close & logoff
			_winHandler = new ConsoleEventDelegate(_ => true);
			SetConsoleCtrlHandler(_winHandler, true);
#else
			// Unix-like: stop soft signals kill
			if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
			{
					_sigTerm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, CancelTermination);
					_sigHup = PosixSignalRegistration.Create(PosixSignal.SIGHUP, CancelTermination);
					_sigInt = PosixSignalRegistration.Create(PosixSignal.SIGINT, CancelTermination);
			}
#endif

		}

		public static void Off()
		{
			if (!_killDisabled) return;
			_killDisabled = false;

#if WINDOWS
			if (_winHandler != null) SetConsoleCtrlHandler(_winHandler, false);
			_winHandler = null;
#else
			_sigTerm?.Dispose(); _sigHup?.Dispose(); _sigInt?.Dispose();
			_sigTerm = _sigHup = _sigInt = null;
#endif

			Console.CancelKeyPress -= CancelKey;
		}

		private static void CancelKey(object? sender, ConsoleCancelEventArgs e)
		{
			cancelAction();
			e.Cancel = true;
		}

		private static void CancelTermination(PosixSignalContext ctx)
		{
			cancelAction();
			ctx.Cancel = true;
		}
	}
}