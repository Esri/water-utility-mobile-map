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


Public Class frmSelectOption
    Private m_SelectedOption As String
    Public Sub New(ByVal Label As String, ByVal Option1 As String, ByVal Option2 As String)

        ' This call is required by the designer.
        InitializeComponent()
        lblMessage.Text = Label
        btnOpt1.Text = Option1
        btnOpt2.Text = Option2

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public ReadOnly Property selectedOption As String
        Get
            Return m_SelectedOption
        End Get
        'Set(ByVal value As Boolean)


        'End Set
    End Property

    Private Sub btnOpt1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpt1.Click
        m_SelectedOption = btnOpt1.Text
        Me.Close()

    End Sub

    Private Sub btnOpt2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpt2.Click
        m_SelectedOption = btnOpt2.Text
        Me.Close()
    End Sub

    Private Sub frmSelectOption_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate

    End Sub

    Private Sub frmSelectOption_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


    End Sub
End Class