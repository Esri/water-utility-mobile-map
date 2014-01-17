' | Version 10.2
' | Copyright 2014 Esri
' |
' | Licensed under the Apache License, Version 2.0 (the "License");
' | you may not use this file except in compliance with the License.
' | You may obtain a copy of the License at
' |
' |    http://www.apache.org/licenses/LICENSE-2.0
' |
' | Unless required by applicable law or agreed to in writing, software
' | distributed under the License is distributed on an "AS IS" BASIS,
' | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' | See the License for the specific language governing permissions and
' | limitations under the License.


Imports System.Net.Mail

Public Class frmLink
    Private m_imgPath As String
    Private WithEvents imgWorker As New System.ComponentModel.BackgroundWorker

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        
    End Sub
    Public Function setPath(pathToImage As String) As Boolean
        If imgWorker.IsBusy Then Return False

        m_imgPath = pathToImage
        ' Add any initialization after the InitializeComponent() call.
        picBox.ImageLocation = pathToImage
        Return True

    End Function
    Private Sub btnClose_Click(sender As System.Object, e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnPreview_Click(sender As System.Object, e As System.EventArgs) Handles btnPreview.Click
        System.Diagnostics.Process.Start(m_imgPath)
        '  Me.Close()
    End Sub

    Private Sub btnEmail_Click(sender As System.Object, e As System.EventArgs) Handles btnEmail.Click

       
        If UCase(GlobalsFunctions.appConfig.ApplicationSettings.MapExport.mailClient) <> "OUTLOOK" Then
            Dim sndTo As SendFileTo.SendToEmail = New SendFileTo.SendToEmail
            Windows.Forms.Clipboard.SetDataObject(m_imgPath)

            sndTo.OpenEmail("", "", GlobalsFunctions.appConfig.ApplicationSettings.MapExport.textToInclude, "")
            sndTo = Nothing
        Else
            Dim mapi As New SendFileTo.MAPI
            mapi.AddAttachment(m_imgPath)
            mapi.SendMailPopup("", "")
            mapi = Nothing
        End If
        'Dim mapi As New SendFileTo.MAPI
        'mapi.AddAttachment(m_imgPath)
        'mapi.SendMailPopup("", "")
        'mapi = Nothing


       

    End Sub

    Private Sub btnOpenLoc_Click(sender As System.Object, e As System.EventArgs) Handles btnOpenLoc.Click
        System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(m_imgPath))
        '  Me.Close()
    End Sub

    Private Sub imgWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles imgWorker.DoWork
        Dim mapi As New SendFileTo.MAPI
        mapi.AddAttachment(m_imgPath)
        mapi.SendMailPopup("", "")
        mapi = Nothing

    End Sub

    Private Sub imgWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles imgWorker.RunWorkerCompleted

    End Sub
End Class