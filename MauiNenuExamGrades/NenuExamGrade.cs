using HtmlAgilityPack;
using MauiNenuHttpClient;
using System.Collections.Concurrent;

namespace MauiNenuExamGrades
{
    public class NenuExamGrade : NenuService
    {
        private ConcurrentBag<ExamGradeRow> _GradeTable;

        public ConcurrentBag<ExamGradeRow> GradeTable { get { return _GradeTable; } }

        public NenuExamGrade(NenuClient nenuClient) : base(nenuClient)
        {
            _GradeTable = new ConcurrentBag<ExamGradeRow>();
        }

        private List<string> GetUsualGrades(ref HtmlDocument HtmlDoc, int row)
        {
            //get url
            HtmlNode node = HtmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"" + row + "\"]/td[8]/a");
            string url = node.GetAttributeValue("onclick", "");
            int pos1, pos2;
            pos1 = url.IndexOf("'");
            pos2 = url.LastIndexOf("'");
            url = url.Substring(pos1 + 1, pos2 - pos1 - 1);
            url = "http://dsjx.nenu.edu.cn" + url;

            //get the info
            HttpResponseMessage resp = _Client.GetAsync(url).Result;
            HtmlDocument newHtmlDoc = new HtmlDocument();
            newHtmlDoc.Load(resp.Content.ReadAsStreamAsync().Result, true);
            string xpath;
            List<string> asw = new List<string>();
            for (int i = 1; i <= 7; i++)
            {
                xpath = "//*[@id=\"1\"]/td[" + i + "]";
                try
                {
                    asw.Add(newHtmlDoc.DocumentNode.SelectSingleNode(xpath).InnerHtml);
                }
                catch (NullReferenceException)
                {
                    asw.Add("null");
                }
            }
            return asw;
        }

        private void GetRowGrades(int rowidx, ref HtmlDocument HtmlDoc)
        {
            HtmlNode node;
            string xpath;
            ExamGradeRow row = new ExamGradeRow();
            //check if the row exists
            xpath = "//*[@id=\"" + rowidx + "\"]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            if (node == null) { return; }

            //columns
            //main info
            xpath = "//*[@id=\"" + rowidx + "\"]/td[4]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.开课学期 = node.GetAttributeValue("title", "");

            xpath = "//*[@id=\"" + rowidx + "\"]/td[5]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.课程编号 = node.GetAttributeValue("title", "");

            xpath = "//*[@id=\"" + rowidx + "\"]/td[6]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.课程名称 = node.GetAttributeValue("title", "");

            xpath = "//*[@id=\"" + rowidx + "\"]/td[7]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.难度系数 = decimal.Parse(node.GetAttributeValue("title", ""));

            //usual grades
            xpath = "//*[@id=\"" + rowidx + "\"]/td[8]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.总成绩 = new GradeRecord(node.GetAttributeValue("title", ""))
            {
                UsualScore = GetUsualGrades(ref HtmlDoc, rowidx)
            };

            xpath = "//*[@id=\"" + rowidx + "\"]/td[9]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.学分绩点 = decimal.Parse(node.GetAttributeValue("title", ""));

            xpath = "//*[@id=\"" + rowidx + "\"]/td[10]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.成绩标志 = node.GetAttributeValue("title", "");

            xpath = "//*[@id=\"" + rowidx + "\"]/td[11]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.课程性质 = node.GetAttributeValue("title", "");

            xpath = "//*[@id=\"" + rowidx + "\"]/td[12]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.通选课类别 = node.GetAttributeValue("title", "");

            xpath = "//*[@id=\"" + rowidx + "\"]/td[13]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.课程类别 = node.GetAttributeValue("title", "");

            xpath = "//*[@id=\"" + rowidx + "\"]/td[14]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.学时 = int.Parse(node.GetAttributeValue("title", ""));

            xpath = "//*[@id=\"" + rowidx + "\"]/td[15]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.学分 = decimal.Parse(node.GetAttributeValue("title", ""));

            xpath = "//*[@id=\"" + rowidx + "\"]/td[16]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.考试性质 = node.GetAttributeValue("title", "");

            xpath = "//*[@id=\"" + rowidx + "\"]/td[17]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.补重学期 = node.GetAttributeValue("title", "");

            xpath = "//*[@id=\"" + rowidx + "\"]/td[18]";
            node = HtmlDoc.DocumentNode.SelectSingleNode(xpath);
            row.审核状态 = node.GetAttributeValue("title", "");

            _GradeTable.Add(row);
        }

        private async Task GetPageGradesAsync(int i)
        {
            string url = "http://dsjx.nenu.edu.cn/xszqcjglAction.do?method=queryxscj";
            List<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("PageNum", i.ToString())
            };
            HttpContent reqContent = new FormUrlEncodedContent(formData);
            HtmlDocument document = new HtmlDocument();
            HttpResponseMessage resp;

            //get a whole page which includes up to 10 rows
            resp = _Client.PostAsync(url, reqContent).Result;
            document.Load(resp.Content.ReadAsStreamAsync().Result, true);

            //concurrent tasks
            List<Task> tasks = new List<Task>();
            for (int j = 1; j <= 10; j++)
            {
                int rowidx = j;
                tasks.Add(Task.Run(
                    () => { GetRowGrades(rowidx, ref document); }
                    ));
            }
            await Task.WhenAll(tasks);
        }

        public ConcurrentBag<ExamGradeRow> GetExamGrades()
        {
            string url = "http://dsjx.nenu.edu.cn/xszqcjglAction.do?method=queryxscj";
            HtmlDocument document = new HtmlDocument();
            HtmlNode node;
            HttpResponseMessage resp;
            int totalPage;

            //get totalPage
            resp = _Client.GetAsync(url).Result;
            document.Load(resp.Content.ReadAsStream(), true);
            node = document.DocumentNode.SelectSingleNode("//*[@id=\"totalPages\"]");
            totalPage = node.GetAttributeValue("value", 0);

            //detail data in each page
            //concurrent tasks
            List<Task> tasks = new List<Task>();
            for (int i = 1; i <= totalPage; i++)
            {
                int pageidx = i;
                tasks.Add(Task.Run(
                    async () => { await GetPageGradesAsync(pageidx); }
                    ));
            }
            Task.WhenAll(tasks).Wait();

            return _GradeTable;
        }
    }
}
