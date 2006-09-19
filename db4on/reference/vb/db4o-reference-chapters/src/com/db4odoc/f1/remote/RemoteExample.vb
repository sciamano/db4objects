' Copyright (C) 2004 - 2006 db4objects Inc. http://www.db4o.com 
Imports System.IO
Imports com.db4o
Imports com.db4o.query
Imports com.db4o.messaging

Namespace com.db4odoc.f1.remote
    Public Class RemoteExample
        Inherits Util

        Public Shared Sub main(ByVal args() As String)
            SetObjects()
            UpdateCars()
            SetObjects()
            UpdateCarsWithMessage()

        End Sub

        Public Shared Sub SetObjects()
            File.Delete(Util.YapFileName)
            Dim db As ObjectContainer = Db4o.OpenFile(Util.YapFileName)
            Try
                Dim i As Integer
                For i = 0 To 5 - 1 Step i + 1
                    Dim car As Car = New Car("car" + i.ToString())
                    db.Set(car)
                Next
                db.Set(New RemoteExample())
            Finally
                db.Close()
            End Try
            CheckCars()
        End Sub


        Public Shared Sub UpdateCars()
            ' triggering mass updates with a singleton
            ' complete server-side execution
            Dim server As ObjectServer = Db4o.OpenServer(Util.YapFileName, 0)
            Try
                Dim client As ObjectContainer = server.OpenClient()
                Dim q As Query = client.Query()
                q.Constrain(GetType(RemoteExample))
                q.Constrain(New UpdateEvaluation())
                q.Execute()
                client.Close()
            Finally
                server.Close()
            End Try
            CheckCars()
        End Sub

        Private Shared Sub CheckCars()
            Dim db As ObjectContainer = Db4o.OpenFile(Util.YapFileName)
            Try
                Dim q As Query = db.Query()
                q.Constrain(GetType(Car))
                Dim objectSet As ObjectSet = q.Execute()
                ListResult(objectSet)
            Finally
                db.Close()
            End Try
        End Sub

        Public Shared Sub UpdateCarsWithMessage()
            Dim server As ObjectServer = Db4o.OpenServer(Util.YapFileName, 0)
            ' create message handler on the server
            server.Ext().Configure().SetMessageRecipient(New UpdateMessageRecipient())
            Try
                Dim client As ObjectContainer = server.OpenClient()
                ' send message object to the server
                Dim sender As MessageSender = client.Ext().Configure().GetMessageSender()
                sender.Send(New UpdateServer())
                client.Close()
            Finally
                server.Close()
            End Try
            CheckCars()
        End Sub
    End Class
    Public Class UpdateEvaluation
        Implements Evaluation

        Public Sub Evaluate(ByVal candidate As Candidate) Implements Evaluation.Evaluate
            ' evaluate method is executed on the server
            ' use it to run update code
            Dim objectContainer As ObjectContainer = candidate.ObjectContainer()
            Dim q2 As Query = objectContainer.Query()
            q2.Constrain(GetType(Car))
            Dim objectSet As ObjectSet = q2.Execute()
            While objectSet.HasNext()
                Dim car As Car = CType(objectSet.Next(), Car)
                car.Model = "Update1-" + car.Model
                objectContainer.Set(car)
            End While
            objectContainer.Commit()
        End Sub
    End Class
    Public Class UpdateMessageRecipient
        Implements MessageRecipient
        Public Sub ProcessMessage(ByVal objectContainer As ObjectContainer, ByVal message As Object) Implements MessageRecipient.ProcessMessage
            ' message type defines the code to be executed
            If message.GetType().Equals(GetType(UpdateServer)) Then
                Dim q As Query = objectContainer.Query()
                q.Constrain(GetType(Car))
                Dim objectSet As ObjectSet = q.Execute()
                While objectSet.HasNext()
                    Dim car As Car = CType(objectSet.Next(), Car)
                    car.Model = "Updated2-" + car.Model
                    objectContainer.Set(car)
                End While
                objectContainer.Commit()
            End If
        End Sub
    End Class
End Namespace


