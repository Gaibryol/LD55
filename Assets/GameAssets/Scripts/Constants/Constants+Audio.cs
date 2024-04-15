using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constants
{
	public class Audio
	{
		public const string MusicVolumePP = "Music";
		public const string SFXVolumePP = "SFX";

		public const float DefaultMusicVolume = 0.05f;
		public const float DefaultSFXVolume = 0.25f;

		public const float MusicFadeSpeed = 0.1f;

		public class Music
		{
			public const string MainMenuTheme = "MainMenuTheme";
			public const string Song1 = "Song1";
			public const string Song1Cut = "Song1Cut";
			public const string Song2 = "Song2";
		}

		public class SFX
		{
			public const string TestSound = "TestSound";
		}
	}
}