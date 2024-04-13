using UnityEngine;

public partial class Constants
{
	public class Game
	{
		public enum Directions
		{
			Up,
			Down,
			Left,
			Right
		}

		public const float PlayerHitRadius = 0.22f;

		public static Vector3 UpExtraDistance = new Vector3(0, -0.5f, 0);
		public static Vector3 DownExtraDistance = new Vector3(0, 0.5f, 0);
		public static Vector3 LeftExtraDistance = new Vector3(0.5f, 0, 0);
		public static Vector3 RightExtraDistance = new Vector3(-0.5f, 0, 0);

		//Points for hits
		public static int PerfectHit = 100;
		public static int OkayHit = 50;
		public static int BadHit = 25;
	}
}