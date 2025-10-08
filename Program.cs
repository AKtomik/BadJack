
namespace BadJack
{
	class Game
	{
		static Dictionary<string, int> pointsDictionary = new Dictionary<string, int>()
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
			// init
			List<string> joueurH = new List<string>();
			List<string> joueurO = new List<string>();
			List<string> paquet = new List<string>();
			int scoreH = 0;
			int scoreO = 0;

			// shuffle
			for (int i = 0; i < 8; i++)
				paquet.AddRange(pointsDictionary.Keys.ToList());
			List<string> paquetRandom = paquet.OrderBy(x => Guid.NewGuid()).ToList();

			// give cards
			for (int i = 0; i < 2; i++)
			{
				scoreH += Draw(paquet, joueurH);
				scoreO += Draw(paquet, joueurO);
			}
			Display(joueurH, joueurO);

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
					string choixJoueur = Console.ReadLine();
					if (choixJoueur?.ToUpper() == "O")
					{
						Console.WriteLine("[joueur] je pioche");
						scoreH += Draw(paquet, joueurH);
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
					if (scoreO <= 15)
					{
						Console.WriteLine("[ordi] je pioche");
						scoreO += Draw(paquet, joueurO);
					}
					else
					{
						Console.WriteLine("[ordi] je m'arrête");
						stopOrdi = true;
					}
				}

				// end the gmae
				finPartie = (stopJoueur && stopOrdi) || scoreO > 20 || scoreH > 20; 
				Display(joueurH, joueurO);
			}
		}


		static void Display(List<string> joueurH, List<string> joueurO)
		{
			Console.WriteLine("joueur : {0} {1}", joueurH[joueurH.Count - 2], joueurH[joueurH.Count - 1]);
			Console.WriteLine("ordi : ? {0}", joueurO[joueurO.Count - 1]);
		}

		static int Draw(List<string> paquet, List<string> pile)
		{
			string card = paquet.First();
			pile.Add(card);
			paquet.RemoveAt(0);
			return pointsDictionary[card];
		}
	}
}