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


Public Class frmEnterText

    Public Sub New(ByVal Label As String)

        ' This call is required by the designer.
        InitializeComponent()
        Dim img As System.Drawing.Bitmap

        img = btnAcceptText.BackgroundImage
        img.MakeTransparent(img.GetPixel(img.Width - 1, 1))
        btnAcceptText.BackgroundImage = img

        img = btnCloseText.BackgroundImage
        img.MakeTransparent(img.GetPixel(img.Width - 1, 1))
        btnCloseText.BackgroundImage = img

        lblMessageText.Text = Label

        TextBox1.Focus()


    End Sub


    Public m_Value As String

    Private Sub btnAccept_Click(sender As System.Object, e As System.EventArgs) Handles btnAcceptText.Click
        m_Value = TextBox1.Text
        Me.Close()
    End Sub

    Private Sub btnClose_Click(sender As System.Object, e As System.EventArgs) Handles btnCloseText.Click
        m_Value = ""
        Me.Close()
    End Sub

    Private Sub frmEnterText_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        TextBox1.Focus()

    End Sub

    Private Sub TextBox1_KeyUp(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyUp
        If e.KeyCode = Windows.Forms.Keys.Enter Then
            btnAccept_Click(Nothing, Nothing)
        End If

    End Sub

    Private Sub frmEnterText_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        TextBox1.Focus()

    End Sub
End Class