using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using picEncoder;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace picEncoder.Items
{

	public class TestSword : ModItem
	{
		// The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.picEncoder.hjson file.
		static ushort[] customTileIDs = new ushort[]
		{
			TileID.Asphalt,
			TileID.ObsidianBrick,
			TileID.Titanstone,
			TileID.DemoniteBrick,
			TileID.Mudstone,
			TileID.LeadBrick,
			TileID.GrayBrick,
			TileID.StoneSlab,
			TileID.Stone,
			TileID.TinBrick,
			TileID.PearlstoneBrick,
			TileID.Marble,
			TileID.GrayStucco,
			TileID.SnowBrick,
            TileID.PlatinumBrick
		};

		static (ushort, byte[])[] blockRGBs = new (ushort, byte[])[]
		{
			(TileID.Ash , new byte[] {64,57,65}),
			(TileID.Asphalt, new byte[] {18,19,19}),
			(TileID.CobaltBrick, new byte[] {14,71,108}),
			(TileID.CopperBrick, new byte[] {137,73,35}),
			(TileID.CrimstoneBrick, new byte[] {113,55,56}),
			(TileID.CrispyHoneyBlock, new byte[] {52,28,8}),
			(TileID.GoldBrick, new byte[] {85,71,23}),
			(TileID.GraniteBlock, new byte[] {24,27,66}),
			(TileID.GrayBrick, new byte[] {72,72,72}),
			(TileID.GreenDungeonBrick, new byte[] {48,61,55}),
			(TileID.GreenStucco, new byte[] {68,107,85}),
			(TileID.HellstoneBrick, new byte[] {83,27,23}),
			(TileID.HoneyBlock, new byte[] {163,117,10}),
			(TileID.IceBrick, new byte[] {82,121,177}),
			(TileID.IridescentBrick, new byte[] {43,34,53}),
			(TileID.LeadBrick, new byte[] {48,56,65}),
			(TileID.LeafBlock, new byte[] {13,65,31}),
			(TileID.ObsidianBrick, new byte[] {31,28,30}),
			(TileID.PinkDungeonBrick, new byte[] {85,44,67}),
			(TileID.TeamBlockPink, new byte[] {109,53,128}),
			(TileID.Marble, new byte[] {107,113,127}),
			(TileID.SnowBlock, new byte[] {123,146,173}),
			(TileID.TinBrick, new byte[] {96,92,73}),
			(TileID.YellowStucco, new byte[] {119,110,57})
		};

		//frame delay between finishing placement and starting next placement
		private int frameDelay = 4;
		private static bool started = false;
		private int timePassed = 0;
		static private int currentFilePathIndex = 0;
		//Path for chum screenshots, reads newest file in folder then deletes rest of them (so folder doesnt just fill up with a million chum files)
		static string directoryPath = @"C:\Users\brend\Documents\My Games\Terraria\tModLoader\ModSources\picEncoder\Items\chum";
		//Path for gif chums, have to be called reformatted0.chum, reformatted1.chum and so on
		static string gifPath = @"C:\Users\brend\Documents\My Games\Terraria\tModLoader\ModSources\picEncoder\Items\doom";
		//Path for picture to place
		static string picPath = @"C:\Users\brend\Documents\My Games\Terraria\tModLoader\ModSources\picEncoder\Items\doom";
		//0 for gifs, 1 for gameplay, 2 for picture
		readonly static int MODE = 1;

		static string[] files = Directory.GetFiles(gifPath, "reformatted*.chum");
		

		static string currentFile = @"C:\Users\brend\Documents\My Games\Terraria\tModLoader\ModSources\picEncoder\Items\reformatted.chum";

		//default mod code, didn't know what i could delete so I didn't bother
		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		//Right click usage
		public override bool AltFunctionUse(Player player)
		{
			var sortedPaths = files.OrderBy(path => int.Parse(System.IO.Path.GetFileNameWithoutExtension(path).Substring("reformatted".Length)));
			files = sortedPaths.ToArray();
			if(MODE != 2)
            {
				started = !started;
			}
			startPlacement(player);

			// Call the base UseItem method to ensure the default item behavior
			base.UseItem(player);
			return true;
        }

		public void startPlacement(Player player)
        {
			byte[,] colorsArray = ReadColorsFromFile();
			if(colorsArray != null)
            {
				PlacementLoop(colorsArray, player);
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public static void updateFiles()
        {
			files = Directory.GetFiles(directoryPath, "reformatted*.chum");
			currentFile = files[files.Length - 1];
		}

		static string GetNewestFilePath(string directoryPath)
		{
			try
			{
				DirectoryInfo directory = new DirectoryInfo(directoryPath);

				// Get all files in the directory
				FileInfo[] files = directory.GetFiles();

				// Order files by creation time in descending order and take the first one
				FileInfo newestFile = files.OrderByDescending(f => f.CreationTime).FirstOrDefault();

				return newestFile?.FullName;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				return null;
			}
		}

		static void DeleteAllExceptNewest(string directoryPath, string newestFilePath)
		{
			try
			{
				DirectoryInfo directory = new DirectoryInfo(directoryPath);

				// Get all files in the directory
				FileInfo[] files = directory.GetFiles();

				foreach (FileInfo file in files)
				{
					// Delete all files except the newest one
					if (file.FullName != newestFilePath)
					{
						file.Delete();
						Console.WriteLine($"Deleted: {file.FullName}");
					}
				}

				Console.WriteLine("Deletion complete.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}


		public static void PlacementLoop(byte[,] colorsArray, Player player)
        {
			string filePath = "";

			switch (MODE)
            {
				case 0:
					filePath = files[currentFilePathIndex];
					currentFilePathIndex++;
					if (currentFilePathIndex >= files.Length) { currentFilePathIndex = 0; };
					break;
				case 1:
					currentFile = GetNewestFilePath(directoryPath);
					DeleteAllExceptNewest(directoryPath, currentFile);
					if (currentFile == null) { return; }
					filePath = currentFile;
					break;
				case 2:
					currentFile = picPath;
					break;
            }

			byte width1 = 0;
			byte width2 = 0;
			byte height1 = 0;
			byte height2 = 0;
			short width = 0;
			short height = 0;

			try
			{
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					// Read the first byte (width)
					width1 = (byte)fileStream.ReadByte();
					width2 = (byte)fileStream.ReadByte();
					// Read the second byte (height)
					height1 = (byte)fileStream.ReadByte();
					height2 = (byte)fileStream.ReadByte();
				}
				width = (short)((width1 << 8) | width2);
				height = (short)((height1 << 8) | height2);
				//Main.NewText($"Width: {width}, Height: {height}");
				// Output the width and height
				//Console.WriteLine($"Width: {width}, Height: {height}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading file: {ex.Message}");
			}

			int pixelIndex = 0;
			ushort blockToPlace = 0;
			for (int y = 0; y < height; y++)
            {
				for(int x = 0; x < width; x++)
                {
					//luminanceRank = calculateLuminance(colorsArray[pixelIndex,0], colorsArray[pixelIndex, 1], colorsArray[pixelIndex, 2]);
					//PlaceCustomBlock((int)player.Center.X / 16 + x, (int)player.Center.Y / 16 + y, customTileIDs[luminanceRank]);

					blockToPlace = findClosestColor(colorsArray[pixelIndex, 0], colorsArray[pixelIndex, 1], colorsArray[pixelIndex, 2]);
					PlaceCustomBlock((int)player.Center.X / 16 + x - (width/2), (int)player.Center.Y / 16 + y - (height/2), blockToPlace);
					RevealAroundPoint((int)player.Center.X / 16 + x - (width / 2), (int)player.Center.Y / 16 + y - (height / 2));
					pixelIndex++;
				}
            }

		}

		//Function kindly provided by jopojelly. thank you :)
		public static int MapRevealSize = 3;
		public static void RevealAroundPoint(int x, int y)
		{
			for (int i = x - MapRevealSize / 2; i < x + MapRevealSize / 2; i++)
			{
				for (int j = y - MapRevealSize / 2; j < y + MapRevealSize / 2; j++)
				{
					if (WorldGen.InWorld(i, j) && Main.sectionManager.TileLoaded(i, j))
						Main.Map.Update(i, j, 255);
				}
			}
			Main.refreshMap = true;
		}


		public static ushort findClosestColor(int r, int g, int b)
		{
			ushort closestColor = 0;
			double minDistanceSquared = double.PositiveInfinity; 

			foreach (var pair in blockRGBs)
			{
				var (block, byteArray) = pair;
				int dr = r - byteArray[0];
				int dg = g - byteArray[1];
				int db = b - byteArray[2];

				double distanceSquared = dr * dr + dg * dg + db * db;

				if (distanceSquared < minDistanceSquared)
				{
					minDistanceSquared = distanceSquared;
					closestColor = block;
				}
			}

			return closestColor;
		}

		public static void PlaceCustomBlock(int x, int y, int type)
		{
			WorldGen.PlaceTile(x, y, type, true, true);
		}

		public static int calculateLuminance(int r, int g, int b)
        {
			double lum = ((double)r) * 0.2126 + ((double)g) * 0.7152 + ((double)b) * 0.0722;
			int rank = (int)lum / 17;
			return rank;
        }

		public static byte[,] ReadColorsFromFile()
		{
			string filePath = "";

			switch (MODE)
            {
				case 0:
					filePath = files[currentFilePathIndex];
					currentFilePathIndex++;
					if (currentFilePathIndex >= files.Length) { currentFilePathIndex = 0; };
					break;
				case 1:
					currentFile = GetNewestFilePath(directoryPath);
					DeleteAllExceptNewest(directoryPath, currentFile);
					if (currentFile == null) { return null; }
					filePath = currentFile;
					break;
				case 2:
					currentFile = picPath;
					break;
			}

			try
			{
				const int maxAttempts = 100;
				int attempts = 0;

				while (attempts < maxAttempts)
				{
					try
					{
						using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
						{
							// Skip the first 4 bytes
							fileStream.Seek(4, SeekOrigin.Begin);

							// Initialize a 2D array to store RGB values
							byte[,] rgbValues = new byte[fileStream.Length / 3, 3];

							int rowIndex = 0;

							// Read RGB values
							while (fileStream.Position + 2 < fileStream.Length)
							{
								// Read the next triplet
								rgbValues[rowIndex, 0] = (byte)fileStream.ReadByte(); // Red
								rgbValues[rowIndex, 1] = (byte)fileStream.ReadByte(); // Green
								rgbValues[rowIndex, 2] = (byte)fileStream.ReadByte(); // Blue

								// Move to the next triplet
								rowIndex++;
							}

							// Return the RGB values array
							return rgbValues;
						}
					}
					catch (IOException ex)
					{
						// The file is being used by another process, retry after a delay
						attempts++;
						//Console.WriteLine($"Attempt {attempts} failed. Retrying after a short delay...");
						Thread.Sleep(5); // Adjust the delay as needed
					}
				}

				//Console.WriteLine($"Error: Unable to read the file after {maxAttempts} attempts.");
				return null; // Return null in case of an error
			}
			catch (Exception ex)
			{
				//Console.WriteLine($"Error reading file: {ex.Message}");
				return null; // Return null in case of an error
			}
		}

		public override void PostUpdate()
		{
			
			timePassed++;

			if (started && timePassed >= frameDelay)
			{
				startPlacement(Main.LocalPlayer);
				timePassed = 0;
			}
			base.PostUpdate();
		}

        public override void UpdateInventory(Player player)
        {
			timePassed++;

			if (started && timePassed >= frameDelay)
			{
				startPlacement(Main.LocalPlayer);
				timePassed = 0;
			}
			base.PostUpdate();
			base.UpdateInventory(player);
        }
    }
}