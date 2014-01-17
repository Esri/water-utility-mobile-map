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


Public Class frmSelectOptionCombo
    Private m_SelectedOption As String
    Public Sub New(ByVal Label As String, DataTable As DataTable, displayCol As String, currentVal As String, _
                   checkBoxLabel As String, chk As Boolean)

        ' This call is required by the designer.
        InitializeComponent()
        Dim img As System.Drawing.Bitmap

        img = btnAccept.BackgroundImage
        img.MakeTransparent(img.GetPixel(img.Width - 1, 1))
        btnAccept.BackgroundImage = img

        img = btnClose.BackgroundImage
        img.MakeTransparent(img.GetPixel(img.Width - 1, 1))
        btnClose.BackgroundImage = img

        lblMessage.Text = Label

        Dim drarray() As DataRow

        ComboBox1.Items.Clear()

        drarray = DataTable.Select("", displayCol)
        If GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.AllWOText <> "" Then
            ComboBox1.Items.Add(GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.AllWOText.ToString)

        End If
        For i = 0 To (drarray.Length - 1)
            ComboBox1.Items.Add(drarray(i)(displayCol).ToString)
        Next
        'ComboBox1.DataSource = DataTable.Select("", displayCol)
        'ComboBox1.Sorted = True

        ' ComboBox1.DisplayMember = displayCol
        ' ComboBox1.ValueMember = valueCol
        If checkBoxLabel = "" Then
            chkPersist.Visible = False
        Else
            chkPersist.Visible = True
            chkPersist.Text = checkBoxLabel
            chkPersist.Checked = chk

        End If
        ComboBox1.Text = currentVal
        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public ReadOnly Property selectedOption As String
        Get
            Return m_SelectedOption

        End Get
        'Set(ByVal value As Boolean)


        'End Set
    End Property

    Public ReadOnly Property checkboxState As Boolean
        Get
            Return chkPersist.Checked

        End Get
        'Set(ByVal value As Boolean)


        'End Set
    End Property


    Private Sub btnAccept_Click(sender As System.Object, e As System.EventArgs) Handles btnAccept.Click
        m_SelectedOption = ComboBox1.Text
        Me.Close()
    End Sub

    Private Sub btnClose_Click(sender As System.Object, e As System.EventArgs) Handles btnClose.Click
        m_SelectedOption = ""
        Me.Close()
    End Sub
End Class