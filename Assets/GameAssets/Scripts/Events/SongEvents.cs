using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SongEvents
{
    public class PlaySong
	{
		public PlaySong(Constants.Songs.Song song)
		{
			Song = song;
		}

		public readonly Constants.Songs.Song Song;
	}

	public class HitNote
	{
		public HitNote(GameObject enemy)
		{
			Enemy = enemy;
		}

		public readonly GameObject Enemy;
	}

	public class GetSongData
	{
		public GetSongData(Constants.Songs.Song song, Action<List<Queue<float>>> processData)
		{
			Song = song;
			ProcessData = processData;
		}

		public readonly Constants.Songs.Song Song;
		public readonly Action<List<Queue<float>>> ProcessData;
	}

	public class SongEnded
	{
		public SongEnded() { }
	}
}
