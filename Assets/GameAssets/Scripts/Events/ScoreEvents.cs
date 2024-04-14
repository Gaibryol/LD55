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
        public Miss()
        {

        }
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





