
namespace MauiNenuExamGrades;

public class ExamGradeRow
{
    public string 开课学期 { get; set; }
    public string 课程编号 { get; set; }
    public string 课程名称 { get; set; }
    public string 成绩标志 { get; set; }
    public string 课程性质 { get; set; }
    public string 通选课类别 { get; set; }
    public string 课程类别 { get; set; }
    public string 考试性质 { get; set; }
    public string 补重学期 { get; set; }
    public string 审核状态 { get; set; }
    public decimal 难度系数 { get; set; }
    public decimal 学分绩点 { get; set; }
    public decimal 学分 { get; set; }
    public int 学时 { get; set; } 
    public GradeRecord 总成绩 { get; set; }    
    
    public ExamGradeRow() { }
}
