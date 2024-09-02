Public Class ButtonConfig
    Public Property Label As String
    Public Property Url As String
End Class

Public Class Config
    Public Property ApplicationId As String
    Public Property Name As String
    Public Property State As String
    Public Property Details As String
    Public Property LargeImageKey As String
    Public Property SmallImageKey As String
    Public Property LargeImageText As String
    Public Property SmallImageText As String
    Public Property Buttons As List(Of ButtonConfig)
End Class
