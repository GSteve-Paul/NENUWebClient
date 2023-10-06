Imports System.CodeDom.Compiler
Imports System.Net
Imports System.Net.Http
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Xml

Public Class NenuClient
    Inherits HttpClient
    Private __Username As String
    Private __Password As String
    Private __IsLogin As Boolean

    Property Username As String
        Get
            Return __Username
        End Get
        Set(value As String)
            __Username = value
        End Set
    End Property

    Property Password As String
        Get
            Return __Password
        End Get
        Set(value As String)
            __Password = value
        End Set
    End Property

    ReadOnly Property IsLogin As Boolean
        Get
            Return __IsLogin
        End Get
    End Property

    Public Sub New()
        MyBase.New(New HttpClientHandler With {
        .CookieContainer = New CookieContainer(),
        .UseCookies = True,
        .AllowAutoRedirect = False,
        .ServerCertificateCustomValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                         Return True
                                                     End Function
        })
        __IsLogin = False
    End Sub

    Public Sub New(Username As String, Password As String)
        Me.New()
        __Username = Username
        __Password = Password
        __IsLogin = False
    End Sub

    Public Sub Login()
        NenuLogin.Client = Me
        NenuLogin.Login(__Username, __Password)
        __IsLogin = True
    End Sub


    Public Sub Login(Username As String, Password As String)
        __Username = Username
        __Password = Password
        Login()
    End Sub

End Class
