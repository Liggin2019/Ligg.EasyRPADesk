
using  Ligg.Infrastructure.DataModels;

namespace Ligg.Infrastructure.Helpers
{

    public static class ScoreHelper
    {

        public static float ConvertScoreToDefaultFormat(this float score, ScoreFormat scoreFormat)
        {
            float retScore = -1;
            if (scoreFormat == ScoreFormat.Default)
            {
                if ((score > 0 & score < 1) | score == 1 | score == 0)
                {
                    retScore = score;
                }
            }
            else if (scoreFormat == ScoreFormat.Percent)
            {
                if ((score > 0 & score < 100) | score == 100 | score == 0)
                {
                    retScore = score / 100;
                }
            }
            else if (scoreFormat == ScoreFormat.FiveMark)
            {
                if ((score > 0 & score < 5) | score == 0 | score == 5)
                {
                    retScore = score / 5;
                }
            }

            return retScore;
        }


        public static float ComputeScore(this float score, ScoreComputation scorecomputation)
        {
            float retScore = -1;
            return retScore;
        }


    }
}
