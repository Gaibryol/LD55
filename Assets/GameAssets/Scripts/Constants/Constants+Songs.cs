
public partial class Constants
{
	public class Songs
	{
		public enum Song
		{
			Song1,
			Song2,
			Song3,
		}

		public enum Difficulties
		{
			Normal,
			Hard
		}

		public const float TimeToSweetSpot = 3f;
		public const float SongStartDelay = 3f;

		public const float BadThreshold = 0.25f;
		public const float OkThreshold = 0.15f;
		public const float PerfectThreshold = 0.05f;

		public class Song1
		{
			public const string Title = "Push The Limit";
		}

		public class Song2
		{
			public const string Title = "Black And White";
		}

		public class Song3
		{
			public const string Title = "Moonweaver";
		}
	}
}
