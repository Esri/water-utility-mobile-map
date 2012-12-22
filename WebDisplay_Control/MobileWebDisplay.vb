' | Version 10.1.1
' | Copyright 2012 Esri
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


Imports ESRI.ArcGIS.Mobile
Imports System.Windows.Forms
Imports Esri.ArcGISTemplates

Public Class MobileWebDisplay

    Dim WithEvents m_WebBrowser As BrowseDisplay
    Public Event WebPanelToggle(visible As Boolean)

    Dim m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub initControl(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map)
        m_Map = map



        m_WebBrowser = New BrowseDisplay
        m_WebBrowser.Dock = DockStyle.Fill
        m_WebBrowser.webDisplay.ScriptErrorsSuppressed = True
        m_WebBrowser.webDisplay.WebBrowserShortcutsEnabled = True

        m_WebBrowser.webDisplay.Dock = DockStyle.Fill
        m_WebBrowser.Visible = False

        m_Map.Parent.Controls.Add(m_WebBrowser)

        If Me.Parent IsNot Nothing Then
            AddHandler Me.Parent.VisibleChanged, AddressOf parentVisibleChange

        End If
    End Sub
    Private Sub parentVisibleChange(ByVal sender As Object, ByVal e As System.EventArgs)
        If CType(sender, Control).Visible = False Then
            toggleWebDisplay(False)
        Else

        End If
    End Sub
    Public Sub initSites()
        Dim pl As ListViewItem

        For Each Site As MobileConfigClass.MobileConfigMobileMapConfigWebPanelWebSitesWebSite In GlobalsFunctions.appConfig.WebPanel.WebSites.WebSite



            pl = New ListViewItem(Site.DisplayText)
            pl.Tag = Site.URL

            lstSites.Items.Add(pl)



        Next
    End Sub
    Public Sub AddSite(ByVal strLink As String, ByVal show As Boolean)
        lstSites.Select()

        Dim pl As ListViewItem
        pl = lstSites.FindItemWithText(strLink) 'findItem(strLink)

        If pl Is Nothing Then
            pl = New ListViewItem(strLink, strLink)
            pl.Tag = strLink
            'file: // /

            pl.ImageKey = strLink

            lstSites.Items.Add(pl)
       

        End If
        pl.Selected = True
        lstSites.Items(pl.Index).Selected = True

        If show Then
            toggleWebDisplay(True)
            'lstSites.Items(lstSites.Items.Count - 1).Selected = True

            ' MsgBox(lstSites.SelectedItems.Count)

            navigateTo(strLink, strLink)
        End If
    End Sub
    Private Function findItem(ByVal linkVal As String) As ListViewItem
        For Each itm As ListViewItem In lstSites.Items
            If itm.Text = linkVal Then
                Return itm

            End If
        Next
        Return Nothing
    End Function
    Public Sub toggleWebDisplay(ByVal Show As Boolean)
        If Show Then
            btnToggleLinkDisplay.BackgroundImage = My.Resources.globe
        Else
            btnToggleLinkDisplay.BackgroundImage = My.Resources.File
        End If
        m_WebBrowser.Visible = Show
        m_Map.Visible = Not Show

    End Sub


    Private Sub lstSites_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstSites.SelectedIndexChanged

    End Sub

    Private Sub lstSites_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstSites.MouseClick
        If lstSites.SelectedItems.Count <= 0 Then Return
        toggleWebDisplay(True)
      

        navigateTo(lstSites.SelectedItems(0).Tag.ToString(), lstSites.SelectedItems(0).Text)

    End Sub
    Private Sub navigateTo(ByVal strLink As String, ByVal strDis As String)
        Dim uriTest As Uri
        Dim url As String = ""

        Try

            url = strLink
        Catch ex As Exception
        End Try

        Try

            'Uri = lstSites.SelectedItems(0).Tag.ToString.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ?                 url : this.url = "http://" + url;
            uriTest = New Uri(url)

            m_WebBrowser.webDisplay.Url = uriTest
            m_WebBrowser.lblCurrentDoc.Text = strDis
        Catch ex As Exception
            'Dim st As New StackTrace
            'MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            'st = Nothing
            MsgBox(ex.Message)

            m_WebBrowser.lblCurrentDoc.Text = strDis
            m_WebBrowser.webDisplay.DocumentText = url

            'm_WebBrowser.webDisplay
        End Try
        lstSites.Focus()

    End Sub
    Public Function isDefaultSite(siteToCheck As ListViewItem) As Boolean


        For Each Site As MobileConfigClass.MobileConfigMobileMapConfigWebPanelWebSitesWebSite In GlobalsFunctions.appConfig.WebPanel.WebSites.WebSite

            If siteToCheck.Text = Site.DisplayText And siteToCheck.Tag = Site.URL Then
                Return True
            End If




        Next
    End Function
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            If lstSites.SelectedItems.Count = 0 Then Return
            If isDefaultSite(lstSites.SelectedItems(0)) Then Return

            lstSites.SelectedItems(0).Remove()
            m_WebBrowser.webDisplay.DocumentText = ""
            m_WebBrowser.lblCurrentDoc.Text = ""
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try

    End Sub

    Private Sub btnOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpen.Click
        Try

            If lstSites.SelectedItems IsNot Nothing Then
                If lstSites.SelectedItems.Count > 0 Then
                    Dim pSys As System.Diagnostics.Process = Process.Start(lstSites.SelectedItems(0).Tag.ToString())
                End If
            End If

            ' MsgBox(pSys.StandardError.ToString)



        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try


    End Sub

    Private Sub btnToggleLinkDisplay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnToggleLinkDisplay.Click

        toggleWebDisplay(Not m_WebBrowser.Visible)

    End Sub

    Private Sub m_WebBrowser_WebPanelToggle(visible As Boolean) Handles m_WebBrowser.WebPanelToggle
        RaiseEvent WebPanelToggle(visible)
    End Sub
End Class
