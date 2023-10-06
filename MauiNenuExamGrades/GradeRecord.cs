namespace MauiNenuExamGrades
{
    public class GradeRecord : IComparable
    {
        private string _TotalScore;
        private List<string> _UsualScore;

        public string TotalScore { get { return _TotalScore; } set { _TotalScore = value; } }
        public List<string> UsualScore { get { return _UsualScore; } set { _UsualScore = value; } }

        public GradeRecord() { }

        public GradeRecord(string totalRecord) { _TotalScore = totalRecord; }

        public int CompareTo(object obj)
        {
            GradeRecord other = obj as GradeRecord;
            int res1, res2;
            if (other != null)
            {
                if (!int.TryParse(this._TotalScore, out res1))
                    res1 = 0;
                if (!int.TryParse(other._TotalScore, out res2))
                    res2 = 0;
                if (res1 < res2)
                    return -1;
                else if (res2 < res1)
                    return 1;
                else
                    return 0;
            }
            else
            {
                return 1;
            }
        }

        public override string ToString()
        {
            return _TotalScore;
        }
    }
}