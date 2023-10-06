Imports System.CodeDom.Compiler
Imports System.Net.Http
Imports HtmlAgilityPack
Imports Microsoft.ClearScript.V8

Friend Module NenuLogin
    Friend Client As HttpClient
    Dim lt, dllt, execution, _eventId, rmShown, pwdDefaultEncryptSalt As String
    Dim LoginUrl As String = "https://authserver.nenu.edu.cn/authserver/login?service=http%3A%2F%2Fdsjx.nenu.edu.cn%3A80%2F"

    Private Sub getFormParameters()
        Dim webContentResponse As HttpResponseMessage = Client.GetAsync(LoginUrl).Result
        Dim str_webContent As String = webContentResponse.Content.ReadAsStringAsync().Result
        Dim htmlDoc As New HtmlDocument()
        htmlDoc.LoadHtml(str_webContent)
        Dim node As HtmlNode
        'lt
        node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=""casLoginForm""]/input[1]")
        lt = node.GetAttributeValue("value", "")
        'dllt
        node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=""casLoginForm""]/input[2]")
        dllt = node.GetAttributeValue("value", "")
        'execution
        node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=""casLoginForm""]/input[3]")
        execution = node.GetAttributeValue("value", "")
        '_eventId
        node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=""casLoginForm""]/input[4]")
        _eventId = node.GetAttributeValue("value", "")
        'rmShown
        node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=""casLoginForm""]/input[5]")
        rmShown = node.GetAttributeValue("value", "")
        'pwdDefaultEncryptSalt
        node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=""casLoginForm""]/input[6]")
        pwdDefaultEncryptSalt = node.GetAttributeValue("value", "")
    End Sub

    Private Function getEncryptedPassword(psw As String, mask As String) As String

        'get the JS code which can encrypt the password
        Dim str_JsCode As String
        Dim url As String = "https://authserver.nenu.edu.cn/authserver/custom/js/encrypt.js"
        Dim jsResponse As HttpResponseMessage = Client.GetAsync(url).Result
        str_JsCode = jsResponse.Content.ReadAsStringAsync().Result

        'run the JS code 
        Dim jsEngine As New V8ScriptEngine
        jsEngine.DocumentSettings.AccessFlags = Microsoft.ClearScript.DocumentAccessFlags.EnableFileLoading
        jsEngine.DefaultAccess = Microsoft.ClearScript.ScriptAccess.Full
        Dim script As V8Script = jsEngine.Compile(str_JsCode)
        jsEngine.Execute(script)
        Dim res As String = jsEngine.Script.encryptAES(psw, mask)
        Return res
    End Function

    Sub Login(ByVal username As String, ByVal password As String)
        Try
            getFormParameters()
        Catch ex As AggregateException
            Throw New InvalidNetworkException
            Return
        End Try
        password = getEncryptedPassword(password, pwdDefaultEncryptSalt)
        Try
            Dim formData As New List(Of KeyValuePair(Of String, String))({
                New KeyValuePair(Of String, String)("username", username),
                New KeyValuePair(Of String, String)("password", password),
                New KeyValuePair(Of String, String)("lt", lt),
                New KeyValuePair(Of String, String)("dllt", dllt),
                New KeyValuePair(Of String, String)("execution", execution),
                New KeyValuePair(Of String, String)("_eventId", _eventId),
                New KeyValuePair(Of String, String)("rmShown", rmShown)
                })
            Dim loginRequestContent As HttpContent = New FormUrlEncodedContent(formData)
            Dim loginResponse As HttpResponseMessage = Client.PostAsync(LoginUrl, loginRequestContent).Result '302

            Dim ticketUrl As String = loginResponse.Headers.GetValues("Location")(0)
            Dim ticketResponse As HttpResponseMessage = Client.GetAsync(ticketUrl).Result          '302

            Dim jsessionidUrl As String = ticketResponse.Headers.GetValues("Location")(0)
            Dim jsessionidResponse As HttpResponseMessage = Client.GetAsync(jsessionidUrl).Result  '302

            Dim dsjxUrl As String = "http://dsjx.nenu.edu.cn/framework/main.jsp"
            Dim dsjxResponse As HttpResponseMessage = Client.GetAsync(dsjxUrl).Result              '302

            Dim logonBySSOUrl As String = "http://dsjx.nenu.edu.cn/Logon.do?method=logonBySSO"
            Dim logonBySSOResponse As HttpResponseMessage = Client.PostAsync(logonBySSOUrl, New StringContent("")).Result '200

        Catch ex As InvalidOperationException
            Throw New InvalidUsernamePasswordException()
            Return
        End Try
    End Sub
End Module
