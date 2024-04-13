
public partial class Constants
{
	public class Songs
	{
		public enum Song
		{
			TestSong1,
			TestSong2,
			TestSong3
		}

		public const float TimeToSweetSpot = 3f;
		public const float SongStartDelay = 3f;

		public const float EarlyThresholdMin = -0.33f;
		public const float EarlyThresholdMax = -0.11f;
		public const float PerfectThresholdMin = -0.10f;
		public const float PerfectThresholdMax = 0.10f;
		public const float LateThresholdMin = 0.11f;
		public const float LateThresholdMax = 0.33f;
	}
}
