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


Imports System
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Collections.Generic
Imports System.Windows.Forms

Namespace SendFileTo
    Class SendToEmail
        Public Sub New()

        End Sub

        Public Function OpenEmail(ByVal EmailAddress As String, _
Optional ByVal Subject As String = "", _
Optional ByVal Body As String = "", Optional ByVal Attachment As String = "") _
As Boolean

            Dim bAns As Boolean = True
            Dim sParams As String
            sParams = EmailAddress
            If LCase(Strings.Left(sParams, 7)) <> "mailto:" Then _
                sParams = "mailto:" & sParams

            If Subject <> "" Then sParams = sParams & _
                  "?subject=" & Subject

            If Body <> "" Then
                sParams = sParams & IIf(Subject = "", "?", "&")
                sParams = sParams & "body=" & Body
            End If
            If Attachment <> "" Then
                sParams = sParams & IIf(Subject = "", "?", "&")
                sParams = sParams & "attach=" & Attachment
            End If
            If Attachment <> "" Then
                sParams = sParams & IIf(Subject = "", "?", "&")
                sParams = sParams & "Attach=" & Attachment
            End If

            If Attachment <> "" Then
                sParams = sParams & IIf(Subject = "", "?", "&")
                sParams = sParams & "Attachment=" & Attachment
            End If

            If Attachment <> "" Then
                sParams = sParams & IIf(Subject = "", "?", "&")
                sParams = sParams & "attachment=" & Attachment
            End If
            If Attachment <> "" Then
                sParams = sParams & IIf(Subject = "", "?", "&")
                sParams = sParams & "Attachments=" & Attachment
            End If

            If Attachment <> "" Then
                sParams = sParams & IIf(Subject = "", "?", "&")
                sParams = sParams & "attachments=" & Attachment
            End If

            Try

                System.Diagnostics.Process.Start(sParams)

            Catch
                bAns = False
            End Try

            Return bAns

        End Function

        Public Sub send()
            Dim Maildb As Object
            Dim MailDoc As Object
            Dim Body As Object
            Dim Session As Object
            'Start a session to notes
            Session = CreateObject("Lotus.NotesSession")
            'This line prompts for password of current ID noted in Notes.INI
            Call Session.Initialize()
            'or use below to supply password of the current ID
            'Call Session.Initialize("<password>")
            'Open the mail database in notes
            Maildb = Session.GETDATABASE("", "c:\notes\data\mail\mymail.nsf")
            If Not Maildb.IsOpen = True Then
                Call Maildb.Open()
            End If
            'Create the mail document
            MailDoc = Maildb.CREATEDOCUMENT
            Call MailDoc.ReplaceItemValue("Form", "Memo")
            'Set the recipient
            Call MailDoc.ReplaceItemValue("SendTo", "John Doe")
            'Set subject
            Call MailDoc.ReplaceItemValue("Subject", "Subject Text")
            'Create and set the Body content
            Body = MailDoc.CREATERICHTEXTITEM("Body")
            Call Body.APPENDTEXT("Body text here")
            'Example to create an attachment (optional)
            Call Body.ADDNEWLINE(2)
            Call Body.EMBEDOBJECT(1454, "", "C:\filename", "Attachment")
            'Example to save the message (optional)
            MailDoc.SAVEMESSAGEONSEND = True
            'Send the document
            'Gets the mail to appear in the Sent items folder
            Call MailDoc.ReplaceItemValue("PostedDate", Now())
            Call MailDoc.SEND(False)
            'Clean Up
            Maildb = Nothing
            MailDoc = Nothing
            Body = Nothing
            Session = Nothing
        End Sub
    End Class
    Class MAPI
        Public Function AddRecipientTo(ByVal email As String) As Boolean
            Return AddRecipient(email, howTo.MAPI_TO)
        End Function

        Public Function AddRecipientCC(ByVal email As String) As Boolean
            Return AddRecipient(email, howTo.MAPI_TO)
        End Function

        Public Function AddRecipientBCC(ByVal email As String) As Boolean
            Return AddRecipient(email, howTo.MAPI_TO)
        End Function

        Public Sub AddAttachment(ByVal strAttachmentFileName As String)
            m_attachments.Add(strAttachmentFileName)
        End Sub

        Public Function SendMailPopup(ByVal strSubject As String, ByVal strBody As String) As Integer
            Return SendMail(strSubject, strBody, MAPI_LOGON_UI Or MAPI_DIALOG)
        End Function

        Public Function SendMailDirect(ByVal strSubject As String, ByVal strBody As String) As Integer
            Return SendMail(strSubject, strBody, MAPI_LOGON_UI)
        End Function


        <DllImport("MAPI32.DLL")> _
        Private Shared Function MAPISendMail(ByVal sess As IntPtr, ByVal hwnd As IntPtr, ByVal message As MapiMessage, ByVal flg As Integer, ByVal rsv As Integer) As Integer
        End Function

        Private Function SendMail(ByVal strSubject As String, ByVal strBody As String, ByVal how As Integer) As Integer
            Dim msg As MapiMessage = New MapiMessage()
            msg.subject = strSubject
            msg.noteText = strBody

            msg.recips = GetRecipients(msg.recipCount)
            msg.files = GetAttachments(msg.fileCount)

            m_lastError = MAPISendMail(New IntPtr(0), New IntPtr(0), msg, how, 0)
            If m_lastError > 1 Then
                MessageBox.Show("MAPISendMail failed! " + GetLastError(), "MAPISendMail")
            End If

            Cleanup(msg)
            Return m_lastError
        End Function

        Private Function AddRecipient(ByVal email As String, ByVal howTo As howTo) As Boolean
            Dim recipient As MapiRecipDesc = New MapiRecipDesc()

            recipient.recipClass = CType(howTo, Integer)
            recipient.name = email
            m_recipients.Add(recipient)

            Return True
        End Function

        Private Function GetRecipients(ByRef recipCount As Integer) As IntPtr
            recipCount = 0
            If m_recipients.Count = 0 Then
                Return 0
            End If

            Dim size As Integer = Marshal.SizeOf(GetType(MapiRecipDesc))
            Dim intPtr As IntPtr = Marshal.AllocHGlobal(m_recipients.Count * size)

            Dim ptr As Integer = CType(intPtr, Integer)
            Dim mapiDesc As MapiRecipDesc
            For Each mapiDesc In m_recipients
                Marshal.StructureToPtr(mapiDesc, CType(ptr, IntPtr), False)
                ptr += size
            Next

            recipCount = m_recipients.Count
            Return intPtr
        End Function

        Private Function GetAttachments(ByRef fileCount As Integer) As IntPtr
            fileCount = 0
            If m_attachments Is Nothing Then
                Return 0
            End If

            If (m_attachments.Count <= 0) Or (m_attachments.Count > maxAttachments) Then
                Return 0
            End If

            Dim size As Integer = Marshal.SizeOf(GetType(MapiFileDesc))
            Dim intPtr As IntPtr = Marshal.AllocHGlobal(m_attachments.Count * size)

            Dim mapiFileDesc As MapiFileDesc = New MapiFileDesc()
            mapiFileDesc.position = -1
            Dim ptr As Integer = CType(intPtr, Integer)

            Dim strAttachment As String
            For Each strAttachment In m_attachments
                mapiFileDesc.name = Path.GetFileName(strAttachment)
                mapiFileDesc.path = strAttachment
                Marshal.StructureToPtr(mapiFileDesc, CType(ptr, IntPtr), False)
                ptr += size
            Next

            fileCount = m_attachments.Count
            Return intPtr
        End Function

        Private Sub Cleanup(ByRef msg As MapiMessage)
            Dim size As Integer = Marshal.SizeOf(GetType(MapiRecipDesc))
            Dim ptr As Integer = 0

            If msg.recips <> IntPtr.Zero Then
                ptr = CType(msg.recips, Integer)
                Dim i As Integer
                For i = 0 To msg.recipCount - 1 Step i + 1
                    Marshal.DestroyStructure(CType(ptr, IntPtr), GetType(MapiRecipDesc))
                    ptr += size
                Next
                Marshal.FreeHGlobal(msg.recips)
            End If

            If msg.files <> IntPtr.Zero Then
                size = Marshal.SizeOf(GetType(MapiFileDesc))

                ptr = CType(msg.files, Integer)
                Dim i As Integer
                For i = 0 To msg.fileCount - 1 Step i + 1
                    Marshal.DestroyStructure(CType(ptr, IntPtr), GetType(MapiFileDesc))
                    ptr += size
                Next
                Marshal.FreeHGlobal(msg.files)
            End If

            m_recipients.Clear()
            m_attachments.Clear()
            m_lastError = 0
        End Sub

        Public Function GetLastError() As String
            If m_lastError <= 26 Then
                Return errors(m_lastError)
            End If
            Return "MAPI error [" + m_lastError.ToString() + "]"
        End Function

        ReadOnly errors() As String = New String() {"OK [0]", "User abort [1]", "General MAPI failure [2]", "MAPI login failure [3]", "Disk full [4]", "Insufficient memory [5]", "Access denied [6]", "-unknown- [7]", "Too many sessions [8]", "Too many files were specified [9]", "Too many recipients were specified [10]", "A specified attachment was not found [11]", "Attachment open failure [12]", "Attachment write failure [13]", "Unknown recipient [14]", "Bad recipient type [15]", "No messages [16]", "Invalid message [17]", "Text too large [18]", "Invalid session [19]", "Type not supported [20]", "A recipient was specified ambiguously [21]", "Message in use [22]", "Network failure [23]", "Invalid edit fields [24]", "Invalid recipients [25]", "Not supported [26]"}

        Dim m_recipients As New List(Of MapiRecipDesc)
        Dim m_attachments As New List(Of String)
        Dim m_lastError As Integer = 0

        Private Const MAPI_LOGON_UI As Integer = &H1
        Private Const MAPI_DIALOG As Integer = &H8
        Private Const maxAttachments As Integer = 20

        Enum howTo
            MAPI_ORIG = 0
            MAPI_TO
            MAPI_CC
            MAPI_BCC
        End Enum

    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Public Class MapiMessage
        Public reserved As Integer
        Public subject As String
        Public noteText As String
        Public messageType As String
        Public dateReceived As String
        Public conversationID As String
        Public flags As Integer
        Public originator As IntPtr
        Public recipCount As Integer
        Public recips As IntPtr
        Public fileCount As Integer
        Public files As IntPtr
    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Public Class MapiFileDesc
        Public reserved As Integer
        Public flags As Integer
        Public position As Integer
        Public path As String
        Public name As String
        Public type As IntPtr
    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Public Class MapiRecipDesc
        Public reserved As Integer
        Public recipClass As Integer
        Public name As String
        Public address As String
        Public eIDSize As Integer
        Public enTryID As IntPtr
    End Class
End Namespace
