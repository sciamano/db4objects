' Copyright (C) 2004 - 2006 db4objects Inc. http://www.db4o.com 
Imports System

Namespace com.db4odoc.f1.constructors
    Class C1
        Private s As String

        Private Sub New(ByVal s As String)
            Me.s = s
        End Sub

        Public Overrides Function ToString() As String
            Return s
        End Function
    End Class
End Namespace

