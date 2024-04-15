using System;

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
        public Final(int score, float accuracy)
        {
            Score = score;
            Accuracy = accuracy;

        }
        public readonly int Score;
        public readonly float Accuracy;

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





