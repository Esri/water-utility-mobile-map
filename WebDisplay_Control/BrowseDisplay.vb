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


Public Class BrowseDisplay
    Public Event WebPanelToggle(visible As Boolean)

    Private m_Factor As Integer = 100

    Private Sub btnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBack.Click
        webDisplay.GoBack()
    End Sub

    Private Sub btnForward_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnForward.Click
        webDisplay.GoForward()

    End Sub

    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
        webDisplay.Refresh(Windows.Forms.WebBrowserRefreshOption.Normal)

    End Sub

    Private Sub gpBoxControls_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles gpBoxControls.Resize
        'btnForward.Left = CInt(gpBoxControls.Width / 2 - btnForward.Width / 2)
        'btnBack.Left = btnForward.Left - btnBack.Width - 15
        'btnRefresh.Left = btnForward.Left + btnForward.Width + 15

    End Sub

    Private Sub btnBack_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        btnBack.Image = My.Resources.LeftrArrowDown

    End Sub

    Private Sub btnBack_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        btnBack.Image = My.Resources.LeftArrow

    End Sub

    Private Sub btnForward_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        btnForward.Image = My.Resources.rightarrowDown

    End Sub

    Private Sub btnForward_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        btnForward.Image = My.Resources.rightArrow

    End Sub

    Private Sub btnRefresh_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        btnRefresh.Image = My.Resources.RefreshDown

    End Sub

    Private Sub btnRefresh_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        btnRefresh.Image = My.Resources.refresh

    End Sub

    Private Sub BrowseDisplay_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        'btnRefresh.Parent.Width
        'btnRefresh.Left = btnRefresh.Width
        'btnBack
        'btnForward
        ' lblCurrentDoc.Width = pnlControls.Left - 25

    End Sub



    Private Sub btnZoomIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZoomIn.Click
        m_Factor = m_Factor + 10

        webDisplay.Zoom(m_Factor)
    End Sub

    Private Sub webDisplay_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles webDisplay.DocumentCompleted
        If m_Factor <> 100 Then
            m_Factor = 100

            webDisplay.Zoom(m_Factor)
        End If

    End Sub

    Private Sub btnZoomOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZoomOut.Click
        m_Factor = m_Factor - 10

        webDisplay.Zoom(m_Factor)
    End Sub

    Private Sub webDisplay_Navigated(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatedEventArgs) Handles webDisplay.Navigated

    End Sub

    Private Sub BrowseDisplay_VisibleChanged(sender As Object, e As System.EventArgs) Handles Me.VisibleChanged

        RaiseEvent WebPanelToggle(Me.Visible)
        
    End Sub
End Class
