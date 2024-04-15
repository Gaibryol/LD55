
public partial class Constants
{
	public class Songs
	{
		public enum Song
		{
			Song1,
			Song2,
		}

		public enum Difficulties
		{
			Normal,
			Hard
		}

		public const float TimeToSweetSpot = 3f;
		public const float SongStartDelay = 3f;

		public const float BadThreshold = 0.35f;
		public const float OkThreshold = 0.22f;
		public const float PerfectThreshold = 0.10f;
	}
}
