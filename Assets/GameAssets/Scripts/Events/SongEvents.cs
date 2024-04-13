using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongEvents
{
    public class PlaySong
	{
		public PlaySong(Constants.Songs.Song song, Queue<string> songData)
		{
			Song = song;
			SongData = songData;
		}

		public readonly Constants.Songs.Song Song;
		public readonly Queue<string> SongData;
	}

	public class HitNote
	{
		public HitNote(GameObject enemy)
		{
			Enemy = enemy;
		}

		public readonly GameObject Enemy;
	}
}
