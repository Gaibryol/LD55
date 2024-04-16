using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SongEvents
{
    public class PlaySong
	{
		public PlaySong(Constants.Songs.Song song, Constants.Songs.Difficulties difficulty)
		{
			Song = song;
			Difficulty = difficulty;
		}

		public readonly Constants.Songs.Song Song;
		public readonly Constants.Songs.Difficulties Difficulty;
	}

	public class HitNote
	{
		public HitNote(Constants.Game.Directions direction, GameObject enemy)
		{
			Direction = direction;
			Enemy = enemy;
		}

		public readonly Constants.Game.Directions Direction;
		public readonly GameObject Enemy;
	}

	public class GetSongData
	{
		public GetSongData(Constants.Songs.Song song, Constants.Songs.Difficulties difficulty, Action<List<Queue<float>>> processData)
		{
			Song = song;
			Difficulty = difficulty;
			ProcessData = processData;
		}

		public readonly Constants.Songs.Song Song;
		public readonly Constants.Songs.Difficulties Difficulty;
		public readonly Action<List<Queue<float>>> ProcessData;
	}

	public class GetSongLength
	{
		public GetSongLength(Constants.Songs.Song song, Action<float> processData)
		{
			ProcessData = processData;
		}

		public readonly Action<float> ProcessData;
	}

	public class SongEnded
	{
		public SongEnded(bool success) 
		{
			Success = success;
		}

		public readonly bool Success;
	}
}
