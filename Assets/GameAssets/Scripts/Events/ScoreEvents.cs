using System;
using UnityEngine;

public class ScoreEvents
{
    public class PerfectHit
    {
        public PerfectHit()
        {
        }
    }

    public class OkayHit
    {
        public OkayHit()
        {
        }
    }

    public class BadHit
    {
        public BadHit()
        {
        }
    }

    public class Miss
    {
        public readonly Constants.Game.Directions Direction;
        public Miss(Constants.Game.Directions direction)
        {
            Direction = direction;
        }
    }

    public class TotalNotes
    {
        public TotalNotes(Constants.Songs.Song song, Constants.Songs.Difficulties difficulty, Action<int> processData)
        {
			Song = song;
			Difficulty = difficulty;
            ProcessData = processData;
        }

		public readonly Constants.Songs.Song Song;
		public readonly Constants.Songs.Difficulties Difficulty;
        public readonly Action<int> ProcessData;
    }

    public class Final
    {
        public Final(string title, Constants.Songs.Difficulties difficulty, Constants.Songs.Song song, int score, float accuracy, int highscore, int perfectHit, int okayHit, int badHit, int misses, int maxCombo, bool newRecord, bool allPerfect, Sprite summon)
        {
			Title = title;
			Difficulty = difficulty;
			Song = song;
            Score = score;
            Accuracy = accuracy;
            Highscore = highscore;
            PerfectHit = perfectHit;
            OkayHit = okayHit;
            BadHit = badHit;
            Misses = misses;
			MaxCombo = maxCombo;
			NewRecord = newRecord;
			AllPerfect = allPerfect;
			Summon = summon;
        }

		public readonly string Title;
		public readonly Constants.Songs.Difficulties Difficulty;
		public readonly Constants.Songs.Song Song;
        public readonly int Score;
        public readonly float Accuracy;
        public readonly int Highscore;
        public readonly int PerfectHit;
        public readonly int OkayHit;
        public readonly int BadHit;
        public readonly int Misses;
		public readonly int MaxCombo;
		public readonly bool NewRecord;
		public readonly bool AllPerfect;
		public readonly Sprite Summon;

    }

    public class Ascended
    {
        public Ascended(bool ascend = false)
        {
            Ascend = ascend;
        }
        public readonly bool Ascend;
    }
}





