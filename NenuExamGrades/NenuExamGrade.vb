Imports System.Net.Http
Imports NenuHttpClient
Imports HtmlAgilityPack
Imports System.Collections.Concurrent
Imports System.Data

Public Class NenuExamGrade
    Inherits NenuService(Of DataTable)
    Dim RowQueue As ConcurrentQueue(Of DataRow)

    Public Sub New(NenuClient As NenuClient)
        MyBase.New(NenuClient)
        __ServiceValue = New DataTable()
        Dim DataCols As New List(Of DataColumn)({
            New DataColumn("开课学期", GetType(String)),
            New DataColumn("课程编号", GetType(String)),
            New DataColumn("课程名称", GetType(String)),
            New DataColumn("难度系数", GetType(Decimal)),
            New DataColumn("总成绩", GetType(GradeRecord)),
            New DataColumn("学分绩点", GetType(Decimal)),
            New DataColumn("成绩标志", GetType(String)),
            New DataColumn("课程性质", GetType(String)),
            New DataColumn("通选课类别", GetType(String)),
            New DataColumn("课程类别", GetType(String)),
            New DataColumn("学时", GetType(Integer)),
            New DataColumn("学分", GetType(Decimal)),
            New DataColumn("考试性质", GetType(String)),
            New DataColumn("补重学期", GetType(String)),
            New DataColumn("审核状态", GetType(String))
        })
        __ServiceValue.Columns.AddRange(DataCols.ToArray())
        RowQueue = New ConcurrentQueue(Of DataRow)
    End Sub

    Private Function GetUsualGrades(ByRef HtmlDoc As HtmlDocument, row As Integer) As List(Of String)
        'get the url
        Dim usualGradeNode As HtmlNode = HtmlDoc.DocumentNode.SelectSingleNode("//*[@id=""" & row & """]/td[8]/a")
        Dim usualGradeUrl As String = usualGradeNode.GetAttributeValue("onclick", "")
        Dim pos1, pos2 As Integer
        pos1 = usualGradeUrl.IndexOf("'")
        pos2 = usualGradeUrl.LastIndexOf("'")
        usualGradeUrl = usualGradeUrl.Substring(pos1 + 1, pos2 - pos1 - 1)
        usualGradeUrl = "http://dsjx.nenu.edu.cn" + usualGradeUrl
        'get the info
        Dim usualGradeResponse As HttpResponseMessage = __Client.GetAsync(usualGradeUrl).Result
        Dim newHtmlDoc As New HtmlDocument
        newHtmlDoc.Load(usualGradeResponse.Content.ReadAsStreamAsync().Result, True)
        Dim xpath As String
        Dim asw As New List(Of String)
        For i = 1 To 7
            xpath = "//*[@id=""1""]/td[" & i & "]"
            Try
                asw.Add(newHtmlDoc.DocumentNode.SelectSingleNode(xpath).InnerHtml)
            Catch ex As NullReferenceException
                asw.Add("null")
            End Try
        Next
        Return asw
    End Function

    Private Sub GetRowGrades(rowidx As Integer, ByRef HtmlDoc As HtmlDocument)
        Dim tmpHtmlNode As HtmlNode
        Dim xpath As String
        Dim row As DataRow
        RowQueue.TryDequeue(row)
        'check if the row exists
        xpath = "//*[@id=""" & rowidx & """]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        If tmpHtmlNode Is Nothing Then
            Return
        End If

        'columns

        'the main info
        xpath = "//*[@id=""" & rowidx & """]/td[4]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(0) = tmpHtmlNode.GetAttributeValue("title", "")

        xpath = "//*[@id=""" & rowidx & """]/td[5]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(1) = tmpHtmlNode.GetAttributeValue("title", "")

        xpath = "//*[@id=""" & rowidx & """]/td[6]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(2) = tmpHtmlNode.GetAttributeValue("title", "")

        xpath = "//*[@id=""" & rowidx & """]/td[7]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(3) = Decimal.Parse(tmpHtmlNode.GetAttributeValue("title", ""))

        xpath = "//*[@id=""" & rowidx & """]/td[9]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(5) = Decimal.Parse(tmpHtmlNode.GetAttributeValue("title", ""))

        xpath = "//*[@id=""" & rowidx & """]/td[10]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(6) = tmpHtmlNode.GetAttributeValue("title", "")

        xpath = "//*[@id=""" & rowidx & """]/td[11]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(7) = tmpHtmlNode.GetAttributeValue("title", "")

        xpath = "//*[@id=""" & rowidx & """]/td[12]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(8) = tmpHtmlNode.GetAttributeValue("title", "")

        xpath = "//*[@id=""" & rowidx & """]/td[13]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(9) = tmpHtmlNode.GetAttributeValue("title", "")

        xpath = "//*[@id=""" & rowidx & """]/td[14]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(10) = Integer.Parse(tmpHtmlNode.GetAttributeValue("title", ""))

        xpath = "//*[@id=""" & rowidx & """]/td[15]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(11) = Decimal.Parse(tmpHtmlNode.GetAttributeValue("title", ""))

        xpath = "//*[@id=""" & rowidx & """]/td[16]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(12) = tmpHtmlNode.GetAttributeValue("title", "")

        xpath = "//*[@id=""" & rowidx & """]/td[17]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(13) = tmpHtmlNode.GetAttributeValue("title", "")

        xpath = "//*[@id=""" & rowidx & """]/td[18]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(14) = tmpHtmlNode.GetAttributeValue("title", "")

        'the usual grades
        xpath = "//*[@id=""" & rowidx & """]/td[8]"
        tmpHtmlNode = HtmlDoc.DocumentNode.SelectSingleNode(xpath)
        row.Item(4) = New GradeRecord(tmpHtmlNode.GetAttributeValue("title", ""))
        row.Item(4).UsualScore = GetUsualGrades(HtmlDoc, rowidx)

        RowQueue.Enqueue(row)
    End Sub

    Private Async Function GetPageGradesAsync(i As Integer) As Task
        Dim gradesUrl As String = "http://dsjx.nenu.edu.cn/xszqcjglAction.do?method=queryxscj"
        Dim formData As New List(Of KeyValuePair(Of String, String))({
                    New KeyValuePair(Of String, String)("PageNum", i)})
        Dim requestContent As HttpContent = New FormUrlEncodedContent(formData)
        Dim tmpHtmlDoc As New HtmlDocument
        Dim tmpResponse As HttpResponseMessage
        'get a whole page which includes up to 10 rows
        tmpResponse = __Client.PostAsync(gradesUrl, requestContent).Result
        tmpHtmlDoc.Load(tmpResponse.Content.ReadAsStreamAsync().Result, True)

        Dim tasks As New List(Of Task)
        For j = 1 To 10 'rows 
            Dim rowidx As Integer = j
            tasks.Add(Task.Run(Sub()
                                   GetRowGrades(rowidx, tmpHtmlDoc)
                               End Sub))
        Next
        Await Task.WhenAll(tasks)
    End Function

    Private Function GetExamGrades() As DataTable
        Dim gradesUrl As String = "http://dsjx.nenu.edu.cn/xszqcjglAction.do?method=queryxscj"
        Dim tmpHtmlDoc As New HtmlDocument
        Dim tmpHtmlNode As HtmlNode
        Dim tmpResponse As HttpResponseMessage
        Dim totalPages As Integer

        'get totalPages and totalRows
        tmpResponse = __Client.GetAsync(gradesUrl).Result
        tmpHtmlDoc.Load(tmpResponse.Content.ReadAsStreamAsync().Result, True)
        tmpHtmlNode = tmpHtmlDoc.DocumentNode.SelectSingleNode("//*[@id=""totalPages""]")
        totalPages = tmpHtmlNode.GetAttributeValue("value", 0)

        For i = 1 To totalPages * 10
            RowQueue.Enqueue(__ServiceValue.NewRow())
        Next

        'get detail data 
        Dim tasks As New List(Of Task)
        For i = 1 To totalPages 'page
            Dim pageidx As Integer = i
            tasks.Add(Task.Run(Async Function()
                                   Await GetPageGradesAsync(pageidx)
                               End Function))
        Next
        Task.WhenAll(tasks).Wait()


        For Each row In RowQueue
            If IsNothing(row.Item(1)) Then
                Continue For
            End If
            __ServiceValue.Rows.Add(row)
        Next

        Return __ServiceValue
    End Function

    Public Overrides Function DoService(obj As Object) As DataTable
        Return GetExamGrades()
    End Function
End Class
