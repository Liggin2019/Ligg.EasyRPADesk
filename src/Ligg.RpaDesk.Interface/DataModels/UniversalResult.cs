
namespace Ligg.RpaDesk.Interface
{

    public class UniversalResult
    {
        public UniversalResult(bool isOk, string txt) { Success = isOk; Message = txt; }
        public UniversalResult() { }
        public bool Success { get; set; }
        public string Message { get; set; }

    }
}



