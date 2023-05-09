using  Ligg.Infrastructure.DataModels;

namespace  Ligg.Infrastructure.DataModels
{

    public class ScoreResult
    {
        public ScoreResult(float score, string msg) { Score = score; Message = msg; }
        public ScoreResult() { }
        public float Score { get; set; }
        public string Message { get; set; }





    }
}



