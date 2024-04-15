using Unity.VisualScripting;
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

		public const float UpExtraDistance = -0.5f;
		public const float DownExtraDistance = 0.5f;
		public const float LeftExtraDistance = 0.5f;
		public const float RightExtraDistance = -0.5f;

		public static string PlayerVisualLatency = "PlayerVisualLatency";
		public static string PlayerInputLatency = "PlayerInputLatency";

		//Points for hits
		public static int PerfectHit = 200;
		public static int OkayHit = 100;
		public static int BadHit = 50;

		public static int MissDamage = -20;
		public static int PerfectHitHeal = 10;

		public static float MetersAwayToSweetSpot = 10.47874f;


    }
}