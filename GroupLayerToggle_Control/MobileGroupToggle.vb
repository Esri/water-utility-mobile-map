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


Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGIS.Mobile.SpatialReferences
Imports Esri.ArcGISTemplates

Imports System.Windows.Forms
Imports System
Public Class mobileGroupToggle
    Private m_MaxTop As Integer
    Private m_buttonHeightLayers As Integer = 30
    Private Const m_buttonWidthLayers As Integer = 110

    Private Const m_buttonHeightExpand As Integer = 50
    Private Const m_buttonWidthExpand As Integer = 50
    Private WithEvents m_GroupPanel As CustomPanel
    Private m_groupButtonPanel As Panel

    Public Event LayersChange()
    Public Event syncBaseMapLayer(ByVal BaseMapLayer As Esri.ArcGIS.Mobile.MapLayer)

    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Private m_BtnLayersList As ArrayList
    Private m_BtnLocalList As ArrayList
    Private m_BtnWebList As ArrayList
    Private m_BtnLayOptList As ArrayList
    Private m_BtnShowLayers As Button

    Private m_FntSize As Integer

    Private m_Font As System.Drawing.Font = New System.Drawing.Font("Veranda", 10, System.Drawing.FontStyle.Bold)
    Private m_BackColor As System.Drawing.Color = Drawing.Color.LightGray


    Private m_FlowLayerArrows As FlowArrowsLayer
    Private m_ToggleBase As Boolean = True


    Public Sub New(ByRef mobileMap As Esri.ArcGIS.Mobile.WinForms.Map)

        Try


            If mobileMap Is Nothing Then Return
            'Assign map to global variable
            m_Map = mobileMap
            m_FntSize = GlobalsFunctions.GetConfigValue("FontSize", 10)

            'm_Fnt = New System.Drawing.Font("Microsoft Sans Serif", m_FntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
            m_Font = New System.Drawing.Font("Microsoft Sans Serif", m_FntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)


            m_FlowLayerArrows = New FlowArrowsLayer("FlowArrows")
            m_FlowLayerArrows.LayerName = "FlowArrows"
            m_FlowLayerArrows.Visible = True

            m_BtnLayersList = New ArrayList
            m_BtnLocalList = New ArrayList
            m_BtnWebList = New ArrayList
            m_BtnLayOptList = New ArrayList



            If (GlobalsFunctions.appConfig.LayerOptions.ShowToggle IsNot Nothing) Then
                If (GlobalsFunctions.appConfig.LayerOptions.ShowToggle <> "") Then
                    If (GlobalsFunctions.appConfig.LayerOptions.ShowToggle.ToUpper = "TRUE") Then
                        m_ToggleBase = True
                    Else

                        m_ToggleBase = True
                    End If
                Else

                    m_ToggleBase = True
                End If
            Else

                m_ToggleBase = True
            End If


            m_GroupPanel = New CustomPanel
            m_GroupPanel.Name = "Group"
            Dim g As System.Drawing.Graphics = m_GroupPanel.CreateGraphics


            m_buttonHeightLayers = CInt(g.MeasureString("Test", m_Font).Height) + 12
            g = Nothing


            m_GroupPanel.Visible = False
            m_GroupPanel.BorderStyle = BorderStyle.FixedSingle
            'm_GroupForm.BorderColor = Drawing.Pens.Black
            'm_GroupForm.BorderColor.Width = 3

            m_GroupPanel.BackColor = m_BackColor


            m_groupButtonPanel = New Panel
            m_groupButtonPanel.AutoScroll = True
            m_groupButtonPanel.BackColor = m_BackColor
            m_groupButtonPanel.Dock = DockStyle.Fill
            m_GroupPanel.Controls.Add(m_groupButtonPanel)
            m_Map.Controls.Add(m_GroupPanel)

            m_Map.MapGraphicLayers.Add(m_FlowLayerArrows)


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Public Sub AddButtons(ByVal Layers As Boolean, ByVal WebMaps As Boolean, ByVal localCacheMaps As Boolean, ByVal ParentControl As Control)
        Try
            Dim bButtonAdded As Boolean = False

            Dim boolOn As Boolean = True
            If Layers = True Then
                For Each layGp In GlobalsFunctions.appConfig.LayerOptions.LayerGroupings.LayersGroup
                    Dim strLayLst As List(Of String) = New List(Of String)

                    For Each Lay In layGp.LayerGroup
                        strLayLst.Add(Lay.Name)
                    Next
                    Boolean.TryParse(layGp.Visible, boolOn)
                    AddGroupButton(strLayLst, layGp.DisplayName, layerTypes.Layers, boolOn)

                Next

                'If m_ConfigValue <> "" Then
                '    Dim pGroupLay() As String = m_ConfigValue.Split(CChar("|"))
                '    For Each strLayers As String In pGroupLay

                '        If (strLayers = "") Then Continue For
                '        Dim strTitlLay() As String
                '        Dim strDis() As String
                '        Dim boolOn As Boolean = False
                '        Dim pLayLst() As String
                '        If strLayers.Contains("[") And strLayers.Contains("]") Then
                '            strTitlLay = strLayers.Trim.Split(New Char() {CChar("]")}, StringSplitOptions.RemoveEmptyEntries)
                '            strDis = (strTitlLay(0).Replace(CChar("["), "")).ToString().Trim.Split(",")

                '            If strDis.Length > 1 Then
                '                If strDis(1).ToUpper() = "TRUE" Then
                '                    boolOn = True

                '                End If
                '            End If
                '            pLayLst = strTitlLay(1).Split(CChar(","))
                '        Else
                '            pLayLst = New String() {strLayers}
                '            strDis = New String() {strLayers}
                '            boolOn = False

                '        End If



                '        AddGroupButton(pLayLst, strDis(0), layerTypes.Layers, boolOn)

                '        bButtonAdded = True


                '    Next
                'End If
            End If

            If WebMaps = True Or localCacheMaps = True Then
                'Dim pMblLayInspect As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer = CType(m_Map.MapLayers(LayerName), MobileCacheMapLayer)
                'If pMblLayInspect Is Nothing Then
                Dim pMobileCacheLayer As MobileCacheMapLayer
                For Each pL As Esri.ArcGIS.Mobile.MapLayer In m_Map.MapLayers

                    If TypeOf pL Is ESRI.ArcGIS.Mobile.DataProducts.RasterData.TileCacheMapLayer And localCacheMaps Then
                        AddGroupButton(New List(Of String)(New String() {pL.Name}), pL.Name, layerTypes.LocalCacheMaps, False)
                        bButtonAdded = True
                    ElseIf TypeOf pL Is ESRI.ArcGIS.Mobile.DataProducts.StreetMapData.StreetMapLayer And localCacheMaps Then
                        AddGroupButton(New List(Of String)(New String() {pL.Name}), pL.Name, layerTypes.LocalCacheMaps, False)
                        bButtonAdded = True
                    ElseIf TypeOf pL Is ESRI.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer And WebMaps Then
                        'If m_Map.SpatialReference.CoordinateSystemString <> pL.SpatialReference.CoordinateSystemString Then
                        '    MsgBox(0, String.Format(GlobalsFunctions.appConfig.LayerOptions.UIComponents.LayerSpatialReferenceDontMatch, pL.Name))

                        'Else
                        AddGroupButton(New List(Of String)(New String() {pL.Name}), pL.Name, layerTypes.WebMaps, False)
                        bButtonAdded = True
                        ' End If

                    Else

                        pMobileCacheLayer = TryCast(pL, MobileCacheMapLayer)
                        ' MsgBox("Fix basemap caches")

                        If pMobileCacheLayer.MobileCache.StoragePath <> GlobalsFunctions.generateCachePath() Then

                            Dim strPath As String = pMobileCacheLayer.MobileCache.StoragePath
                            strPath = GlobalsFunctions.FolderFromFileName(strPath)

                            For Each obj As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsCachedMapsCachedMap In GlobalsFunctions.appConfig.LayerOptions.CachedMaps.CachedMap
                                If obj.Value = strPath Or obj.Value = pMobileCacheLayer.MobileCache.StoragePath Then
                                    strPath = obj.DisplayName
                                    Continue For

                                End If

                            Next
                            'strPath = System.IO.Path.
                            '
                            Dim pMobileCache As MobileCache = pMobileCacheLayer.MobileCache


                            AddGroupButton(New List(Of String)(New String() {pL.Name}), strPath, layerTypes.LocalMobileCache, False)
                            bButtonAdded = True
                        End If

                    End If
                Next
                Dim boolMapOn As Boolean = False
                If m_BtnLocalList IsNot Nothing Then
                    If m_BtnLocalList.Count > 0 Then
                        Call buttonClick(m_BtnLocalList.Item(0), Nothing)
                        boolMapOn = True
                    End If
                End If
                If boolMapOn = False And m_BtnWebList IsNot Nothing Then
                    If m_BtnWebList.Count > 0 Then
                        Call buttonClick(m_BtnWebList.Item(0), Nothing)
                        boolMapOn = True
                    End If
                End If
            End If

            AddFlowButton(False)


            addShowLayersGroupButton()
            reLocateButtonsCenter()

            '  reLocateButtons()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try

    End Sub
    Private Enum layerTypes
        Layers
        LocalCacheMaps
        LocalMobileCache
        WebMaps
    End Enum
    Private Sub addShowLayersGroupButton()


        m_BtnShowLayers = New Button

        m_BtnShowLayers.BackgroundImage = My.Resources.Expand
        m_BtnShowLayers.BackgroundImage.Tag = "Off"

        m_BtnShowLayers.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        m_BtnShowLayers.FlatAppearance.BorderSize = 0
        m_BtnShowLayers.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        m_BtnShowLayers.Font = m_Font 'New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        m_BtnShowLayers.ForeColor = System.Drawing.Color.White
        '      m_BtnShowLayers.Location = New System.Drawing.Point(4, 4)
        m_BtnShowLayers.Margin = New System.Windows.Forms.Padding(1)


        m_BtnShowLayers.Size = New System.Drawing.Size(m_buttonWidthExpand, m_buttonHeightExpand)

        m_BtnShowLayers.Text = ""
        m_BtnShowLayers.UseVisualStyleBackColor = True

        m_BtnShowLayers.Tag = "Off"
        AddHandler m_BtnShowLayers.Click, AddressOf buttonShowLayersClick
        m_Map.Controls.Add(m_BtnShowLayers)



    End Sub
    Private Sub AddFlowButton(ByVal boolOn As Boolean)
        If GlobalsFunctions.appConfig.LayerOptions.FlowArrowsLayers.FlowArrowsLayer.Count = 0 Then Return


        Dim pBtn As Button

        pBtn = New Button

        If Not boolOn Then
            pBtn.BackgroundImage = My.Resources.ButtonsVerticalBase
            pBtn.BackgroundImage.Tag = "Off"
            m_FlowLayerArrows.Visible = False
        Else
            pBtn.BackgroundImage = My.Resources.ButtonsVerticalBaseDown
            pBtn.BackgroundImage.Tag = "On"
            m_FlowLayerArrows.Visible = True
        End If

        pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        pBtn.FlatAppearance.BorderSize = 0
        pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        pBtn.Font = m_Font 'New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        pBtn.ForeColor = System.Drawing.Color.White
        '      pBtn.Location = New System.Drawing.Point(4, 4)
        pBtn.Margin = New System.Windows.Forms.Padding(1)

        ' pBtn.Dock = DockStyle.Bottom

        pBtn.Size = New System.Drawing.Size(m_buttonWidthLayers, m_buttonHeightLayers)

        pBtn.Text = GlobalsFunctions.appConfig.LayerOptions.FlowArrowsLayers.ButtonText
        pBtn.UseVisualStyleBackColor = True

        pBtn.Tag = "*Flow*"
        pBtn.Name = "*Flow*"
        AddHandler pBtn.Click, AddressOf buttonClick
        m_groupButtonPanel.Controls.Add(pBtn)


        m_BtnLayOptList.Add(pBtn)


    End Sub

    Private Sub AddGroupButton(ByVal layerList As List(Of String), ByVal strLabel As String, ByVal btnType As layerTypes, ByVal boolOn As Boolean)
        Dim pBtn As Button
        Dim boolBtnFound As Boolean = False

        If btnType = layerTypes.LocalMobileCache Then
            For Each pBtn In m_BtnLocalList
                If pBtn.Name.Contains(strLabel) Then
                    boolBtnFound = True

                End If
            Next
            If boolBtnFound = False Then
                pBtn = New Button
            End If
        Else
            pBtn = New Button
        End If



        If boolBtnFound = False Then

            If Not boolOn Then
                pBtn.BackgroundImage = My.Resources.ButtonsVerticalBase
                pBtn.BackgroundImage.Tag = "Off"
            Else
                pBtn.BackgroundImage = My.Resources.ButtonsVerticalBaseDown
                pBtn.BackgroundImage.Tag = "On"
            End If

            pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            pBtn.FlatAppearance.BorderSize = 0
            pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            pBtn.Font = m_Font 'New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            pBtn.ForeColor = System.Drawing.Color.White
            '      pBtn.Location = New System.Drawing.Point(4, 4)
            pBtn.Margin = New System.Windows.Forms.Padding(1)


            pBtn.Size = New System.Drawing.Size(m_buttonWidthLayers, m_buttonHeightLayers)

            pBtn.Text = strLabel
            pBtn.UseVisualStyleBackColor = True

            pBtn.Tag = layerList
            AddHandler pBtn.Click, AddressOf buttonClick
            m_groupButtonPanel.Controls.Add(pBtn)
        End If

        Select Case btnType
            Case layerTypes.Layers
                pBtn.Name = "btnLayers_" & strLabel
                m_BtnLayersList.Add(pBtn)
                toggleVis(layerList, boolOn, btnType)
            Case layerTypes.LocalCacheMaps
                pBtn.Name = "btnLocal_" & strLabel
                m_BtnLocalList.Add(pBtn)
                toggleVis(layerList, boolOn, btnType)
            Case layerTypes.LocalMobileCache
                If boolBtnFound = False Then


                    pBtn.Name = "btnLocal_" & strLabel
                    m_BtnLocalList.Add(pBtn)
                Else
                    Dim pLi As List(Of String) = New List(Of String)
                    pLi.AddRange(CType(pBtn.Tag, List(Of String)))
                    pLi.AddRange(layerList)
                    pBtn.Tag = pLi

                End If
                toggleVis(layerList, boolOn, btnType)
            Case layerTypes.WebMaps
                pBtn.Name = "btnWeb_" & strLabel
                m_BtnWebList.Add(pBtn)
                toggleVis(layerList, boolOn, btnType)
        End Select




    End Sub
 
    Private Sub toggleVis(ByVal strLayers As List(Of String), ByVal LayerVisible As Boolean, ByVal lyrType As layerTypes)
        Try

            ' m_Map.SuspendLayout()
            ' m_Map.RefreshOnDataChange = False
            'm_Map.UseThreadedDrawing = True
            m_Map.DisableDrawing()


            Dim pML As ESRI.ArcGIS.Mobile.MapLayer
            Dim pFSwD As FeatureSourceWithDef
            Dim pFLayDef As MobileCacheMapLayerDefinition
            For Each StrLay As String In strLayers
                If lyrType = layerTypes.Layers Then
                    pFSwD = GlobalsFunctions.GetFeatureSource(StrLay, m_Map)
                    If pFSwD.FeatureSource IsNot Nothing Then
                        pFLayDef = GlobalsFunctions.GetLayerDefinition(m_Map, pFSwD)

                        pFLayDef.Visibility = LayerVisible


                    End If


                Else
                    pML = GlobalsFunctions.GetMapLayer(StrLay, m_Map)

                    If pML IsNot Nothing Then

                        pML.Visible = LayerVisible

                        If LayerVisible And (lyrType = layerTypes.LocalCacheMaps Or lyrType = layerTypes.WebMaps) Then

                            RaiseEvent syncBaseMapLayer(pML)
                        End If
                    End If
                End If
                
            Next
            pML = Nothing
            'm_Map.ResumeLayout()
            ' m_Map.RefreshOnDataChange = True
            m_Map.EnableDrawing()
            RaiseEvent LayersChange()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub reLocateButtonsGrouped()
        Try
            'Relocate Show Group Button
            CType(m_BtnShowLayers, Button).Left = m_Map.Width - 35 - CType(m_BtnShowLayers, Button).Width
            CType(m_BtnShowLayers, Button).Top = m_Map.Height - 25 - CType(m_BtnShowLayers, Button).Height
            ' m_GroupForm.Left = m_BtnShowLayers.Left - m_GroupForm.Width - 15
            'm_GroupForm.Top = m_BtnShowLayers.Top - m_GroupForm.Height
            m_GroupPanel.SuspendLayout()
            m_GroupPanel.Left = m_BtnShowLayers.Left - m_GroupPanel.Width - 15

            m_GroupPanel.Height = m_MaxTop

            If m_GroupPanel.Height > m_BtnShowLayers.Top - 80 Then
                m_GroupPanel.Height = m_BtnShowLayers.Top - 80
            Else


            End If
            m_GroupPanel.Top = m_BtnShowLayers.Top - m_GroupPanel.Height - 5
            'm_GroupPanel.Height = m_BtnShowLayers.Top - m_GroupPanel.Top
            ' m_GroupPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            'If m_BtnShowLayers.Tag = "On" Then

            '    reLocateButtonsCenter(True, m_GroupForm)
            'End If

            m_GroupPanel.ResumeLayout()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub relocateGroupBox()

    End Sub
    Private Sub reLocateButtonsCenter()


        'Layers

        Dim pSpaceVert As Integer = 10
        Dim pSpaceHort As Integer = 15
        Dim pLeft As Integer = 10
        Dim pTop As Integer = 50
        Dim pMaxTop As Integer = 0
        'Dim g As System.Drawing.Graphics
        ' Dim s As System.Drawing.SizeF

        If m_BtnLayersList IsNot Nothing Then
            If m_BtnLayersList.Count > 0 Then
                Dim pLbl As Label = New Label
                pLbl.Font = m_Font
                pLbl.Text = "Operational Layers"

                pLbl.Top = 5
                'g = pLbl.CreateGraphics
                's = g.MeasureString(pLbl.Text, pLbl.Font)
                pLbl.Width = 95 's.Width
                pLbl.Height = 50
                'pLbl.AutoSize = True

                pLbl.Left = CInt((pLeft + (m_buttonWidthLayers / 2) - (95 / 2)))

                'g = Nothing
                's = Nothing
                m_groupButtonPanel.Controls.Add(pLbl)

                For i As Integer = 0 To m_BtnLayersList.Count - 1
                    CType(m_BtnLayersList(i), Button).Left = pLeft
                    CType(m_BtnLayersList(i), Button).Top = pTop
                    pTop = pTop + m_buttonHeightLayers + pSpaceVert

                Next

                pLeft = pLeft + m_buttonWidthLayers + pSpaceHort
                If pMaxTop < pTop Then
                    pMaxTop = pTop
                End If
                pTop = 50
            End If
        End If


        If m_BtnLocalList IsNot Nothing Then
            If m_BtnLocalList.Count > 0 Then
                Dim pLbl As Label = New Label
                pLbl.Text = "Offline Basemaps"
                pLbl.Top = 5
                pLbl.Font = m_Font
                'g = pLbl.CreateGraphics


                's = g.MeasureString(pLbl.Text, pLbl.Font)
                'pLbl.Width = s.Width
                'pLbl.Height = 20
                ' pLbl.AutoSize = True
                pLbl.Width = 95 's.Width
                pLbl.Height = 50
                'pLbl.AutoSize = True

                pLbl.Left = CInt(pLeft + (m_buttonWidthLayers / 2) - (95 / 2))
                'g = Nothing
                's = Nothing
                m_groupButtonPanel.Controls.Add(pLbl)


                For i As Integer = 0 To m_BtnLocalList.Count - 1
                    CType(m_BtnLocalList(i), Button).Left = pLeft
                    CType(m_BtnLocalList(i), Button).Top = pTop
                    pTop = pTop + m_buttonHeightLayers + pSpaceVert

                Next

                pLeft = pLeft + m_buttonWidthLayers + pSpaceHort
                If pMaxTop < pTop Then
                    pMaxTop = pTop
                End If
                pTop = 50
            End If
        End If


        If m_BtnWebList IsNot Nothing Then
            If m_BtnWebList.Count > 0 Then
                Dim pLbl As Label = New Label
                pLbl.Font = m_Font
                pLbl.Text = "Online Basemaps"
                pLbl.Top = 5
                'g = pLbl.CreateGraphics
                's = g.MeasureString(pLbl.Text, pLbl.Font)
                'pLbl.Width = s.Width
                'pLbl.Height = 20
                '                pLbl.AutoSize = True
                pLbl.Width = 95 's.Width
                pLbl.Height = 50
                'pLbl.AutoSize = True

                pLbl.Left = CInt((pLeft + (m_buttonWidthLayers / 2) - (95 / 2)))

                'g = Nothing
                '                s = Nothing

                m_groupButtonPanel.Controls.Add(pLbl)

                For i As Integer = 0 To m_BtnWebList.Count - 1
                    CType(m_BtnWebList(i), Button).Left = pLeft
                    CType(m_BtnWebList(i), Button).Top = pTop
                    pTop = pTop + m_buttonHeightLayers + pSpaceVert

                Next
                If pMaxTop < pTop Then
                    pMaxTop = pTop
                End If
                pLeft = pLeft + m_buttonWidthLayers + pSpaceHort
                pTop = 50
            End If
        End If
        m_GroupPanel.Width = pLeft + 10

        If m_BtnLayOptList.Count > 0 Then

            pTop = pMaxTop

            For i As Integer = 0 To m_BtnLayOptList.Count - 1
                CType(m_BtnLayOptList(i), Button).Width = CInt(m_groupButtonPanel.Width - (m_groupButtonPanel.Width * 0.1))

                CType(m_BtnLayOptList(i), Button).Left = CInt((m_groupButtonPanel.Width / 2) - (CType(m_BtnLayOptList(i), Button).Width / 2))

                CType(m_BtnLayOptList(i), Button).Top = pTop

                pTop = pTop + m_buttonHeightLayers + pSpaceVert

            Next

            m_GroupPanel.Height = pTop

        Else
            m_GroupPanel.Height = pMaxTop + 20

        End If

        'If m_GroupPanel.Height > m_GroupPanel.Top - m_GroupPanel.Height Then
        '    // m_BtnShowLayers.Top - 80 m_GroupPanel.Height = m_BtnShowLayers.Top - 80

        'End If
        m_MaxTop = m_GroupPanel.Height

    End Sub
    Private Sub reLocateButtons()
        'Layers
        Dim pSpace As Integer = 10
        Dim pRight As Integer = m_Map.Width - 35

        Dim pBtm As Integer = m_Map.Height - 35


        If m_BtnLayersList Is Nothing Then Return
        If m_BtnLayersList.Count = 0 Then Return
        For i As Integer = 0 To m_BtnLayersList.Count - 1
            CType(m_BtnLayersList(i), Button).Left = pRight - CType(m_BtnLayersList(i), Button).Width
            CType(m_BtnLayersList(i), Button).Top = pBtm - CType(m_BtnLayersList(i), Button).Height
            pRight = CType(m_BtnLayersList(i), Button).Left - pSpace

        Next


        pSpace = 10
        pRight = 35

        pBtm = m_Map.Height - 35


        If m_BtnLocalList Is Nothing Then Return
        If m_BtnLocalList.Count = 0 Then Return
        For i As Integer = 0 To m_BtnLocalList.Count - 1
            CType(m_BtnLocalList(i), Button).Left = pRight
            CType(m_BtnLocalList(i), Button).Top = pBtm - CType(m_BtnLocalList(i), Button).Height
            pRight = CType(m_BtnLocalList(i), Button).Left + CType(m_BtnLocalList(i), Button).Width + pSpace

        Next




        pSpace = 10
        pRight = 35

        pBtm = m_Map.Height - 85


        If m_BtnWebList Is Nothing Then Return
        If m_BtnWebList.Count = 0 Then Return
        For i As Integer = 0 To m_BtnWebList.Count - 1
            CType(m_BtnWebList(i), Button).Left = pRight
            CType(m_BtnWebList(i), Button).Top = pBtm - CType(m_BtnWebList(i), Button).Height
            pRight = CType(m_BtnWebList(i), Button).Left + CType(m_BtnWebList(i), Button).Width + pSpace

        Next



        '        If m_BtnList.Count Mod 2 = 0 Then
        '            Dim midPoint As Integer = m_Map.Width / 2


        '            For i As Integer = 0 To m_BtnList.Count - 1
        '                CType(m_BtnList(0), Button).Left = midPoint - (CType(m_BtnList(0), Button).Width / 2)

        'midPoint = 
        '            Next
        '        Else
        '            For i As Integer = 0 To m_BtnList.Count - 1


        '            Next

        '        End If
    End Sub
    Private Sub buttonClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Try


            buttonClicked(CType(sender, Button), m_ToggleBase)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub buttonShowLayersClick(ByVal sender As Object, ByVal e As System.EventArgs)
        If m_BtnShowLayers.Tag.ToString = "Off" Then
            turnOffCustomPanels(m_GroupPanel)

            m_BtnShowLayers.BackgroundImage = My.Resources.Collapse
            m_BtnShowLayers.Tag = "On"
            m_GroupPanel.Visible = True
            ' reLocateButtonsCenter(True, m_GroupForm)

        Else
            m_BtnShowLayers.BackgroundImage = My.Resources.Expand
            m_BtnShowLayers.Tag = "Off"
            m_GroupPanel.Visible = False
            'reLocateButtonsCenter(False, m_GroupForm)
        End If
    End Sub
    Private Sub turnOffBasemaps()
        Try


            Dim i As Integer
            For i = 0 To m_BtnLocalList.Count - 1
                Dim btn As Button = CType(m_BtnLocalList.Item(i), Button)

                If btn.BackgroundImage.Tag.ToString = "On" Then
                    'RemoveHandler btn.Click, AddressOf buttonClick

                    buttonClicked(btn, False)
                    'AddHandler btn.Click, AddressOf buttonClick

                End If

            Next i
            For i = 0 To m_BtnWebList.Count - 1
                Dim btn As Button = CType(m_BtnWebList.Item(i), Button)
                If btn.BackgroundImage.Tag.ToString = "On" Then
                    ' RemoveHandler btn.Click, AddressOf buttonClick

                    buttonClicked(btn, False)
                    'AddHandler btn.Click, AddressOf buttonClick

                End If

            Next i
            RaiseEvent LayersChange()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub
    Private Sub buttonClicked(ByVal btn As Button, ByVal toggleBaseMaps As Boolean)
        Try

            m_Map.DisableDrawing()

            Dim pLyType As layerTypes
            If btn.Name = "*Flow*" Then
                m_FlowLayerArrows.Visible = Not m_FlowLayerArrows.Visible
                If btn.BackgroundImage.Tag.ToString = "Off" Then
                    '  m_Map.SuspendLayout()

                    btn.BackgroundImage = My.Resources.ButtonsVerticalBaseDown
                    btn.BackgroundImage.Tag = "On"
                    '  m_Map.ResumeLayout()
                    ' m_Map.Refresh()
                Else
                    '  m_Map.SuspendLayout()

                    btn.BackgroundImage = My.Resources.ButtonsVerticalBase
                    btn.BackgroundImage.Tag = "Off"
                    '  m_Map.ResumeLayout()

                    '  m_Map.Refresh()
                End If
                m_Map.Refresh()

            Else
                If btn.Name.Contains("btnLayers_") Then
                    pLyType = layerTypes.Layers
                ElseIf btn.Name.Contains("btnWeb_") Then
                    If toggleBaseMaps Then turnOffBasemaps()
                    pLyType = layerTypes.WebMaps
                ElseIf btn.Name.Contains("btnLocal_") Then
                    If toggleBaseMaps Then turnOffBasemaps()
                    pLyType = layerTypes.LocalCacheMaps
                End If
                If btn.BackgroundImage.Tag.ToString = "Off" Then
                    ' m_Map.SuspendLayout()
                    'toggleVis(New List(Of String)(New String() {btn.Tag.ToString}), True, pLyType)
                    toggleVis(btn.Tag, True, pLyType)
                    btn.BackgroundImage = My.Resources.ButtonsVerticalBaseDown
                    btn.BackgroundImage.Tag = "On"
                    ' m_Map.ResumeLayout()
                    ' m_Map.Refresh()
                Else
                    '  m_Map.SuspendLayout()
                    toggleVis(btn.Tag, False, pLyType)
                    btn.BackgroundImage = My.Resources.ButtonsVerticalBase
                    btn.BackgroundImage.Tag = "Off"
                    '  m_Map.ResumeLayout()

                    '  m_Map.Refresh()
                End If
            End If
            m_Map.EnableDrawing()

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Sub m_Map_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.Resize
        'reLocateButtons()
        reLocateButtonsGrouped()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        m_BtnLayersList = Nothing
        m_BtnLocalList = Nothing
        m_BtnWebList = Nothing
        m_BtnLayOptList = Nothing
    End Sub

    Private Sub m_GroupPanel_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_GroupPanel.VisibleChanged
        If m_BtnShowLayers Is Nothing Or m_GroupPanel Is Nothing Then
            Return

        End If
        If m_GroupPanel.Visible = False Then

            m_BtnShowLayers.BackgroundImage = My.Resources.Expand
            m_BtnShowLayers.Tag = "Off"

        End If

    End Sub
    Private Sub turnOffCustomPanels(ByVal exclude As Control)

        For Each ct As Control In m_Map.Controls

            If TypeOf (ct) Is CustomPanel And ct IsNot exclude Then
                ct.Visible = False

            End If
        Next

    End Sub
End Class
