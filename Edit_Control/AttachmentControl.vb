Imports System.Drawing
Imports System.Windows
Imports Esri.ArcGIS
Imports Esri.ArcGIS.Mobile
Imports System.IO
Imports Esri.ArcGIS.Mobile.Geometries
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGIS.Mobile.WinForms

Imports Esri.ArcGISTemplates
Imports MobileControls


Imports System.Windows.Forms

Public Class AttachmentControl
    Public Event AttachmentSelected(filename As attFiles)
    Public Event DeleteAttachment(id As Integer)
    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Private WithEvents m_SketchGraphics As Esri.ArcGIS.Mobile.WinForms.MapGraphicLayer
    Private WithEvents m_SketchMA As SketchMapAction
    Private m_lastMA As MapAction = Nothing

    Public Function hasSketches() As Boolean
        If m_SketchGraphics Is Nothing Then Return False
        If m_SketchGraphics.Graphics Is Nothing Then Return False

        If m_SketchGraphics.Graphics.Count > 0 Then
            Return True
        Else
            Return False

        End If
    End Function


    Public Sub New(ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map)

        ' This call is required by the designer.
        InitializeComponent()
        m_Map = Map

        ' Add any initialization after the InitializeComponent() call.
        lstAtt.DrawMode = DrawMode.OwnerDrawVariable

        lstAtt.Font = New System.Drawing.Font("Tahoma", 12.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0)


        lstAtt.HorizontalScrollbar = True
        lstAtt.ItemHeight = 50

        AddHandler lstAtt.MouseClick, AddressOf attlistclick
        AddHandler lstAtt.DrawItem, AddressOf attdraw
        SplitContainer1.Panel2Collapsed = True

        If InitNavigateBar() = True Then
            InitNavigateButtons()
            LoadLayerDetails()

            If Map.MapGraphicLayers.Count > 0 Then
                m_SketchGraphics = Map.MapGraphicLayers.Item("Sketching")
                If m_SketchGraphics Is Nothing Then
                    m_SketchGraphics = New Esri.ArcGIS.Mobile.WinForms.MapGraphicLayer("Sketching")
                    Map.MapGraphicLayers.Add(m_SketchGraphics)

                End If
            Else
                m_SketchGraphics = New Esri.ArcGIS.Mobile.WinForms.MapGraphicLayer("Sketching")
                Map.MapGraphicLayers.Add(m_SketchGraphics)

            End If
            setSketchStatus()

            m_SketchMA = New SketchMapAction
            m_SketchMA.ManualSetMap = Map
        Else
            btnAddSketch.Visible = False

        End If



    End Sub
#Region "SketchBar"
    Private m_OutlookNavigatePane As NavigateBar
    Private m_NVBButton As List(Of NavigateBarButton)
    Private splitterNavigateMenu As MTSplitter = Nothing

    Private Function InitNavigateBar() As Boolean
        Try
            If GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions Is Nothing Then Return False
            If GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups Is Nothing Then Return False
            If GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.AttachmentSketchGroup Is Nothing Then Return False
            If GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.AttachmentSketchGroup.Count = 0 Then Return False

            m_OutlookNavigatePane = New NavigateBar()
            m_OutlookNavigatePane.SaveAndRestoreSettings = False

            m_OutlookNavigatePane.Dock = DockStyle.Left
            m_OutlookNavigatePane.IsShowCollapsibleScreen = True
            m_OutlookNavigatePane.IsCollapseScreenShowOnButtonSelect = False
            m_OutlookNavigatePane.IsCollapsible = False
            m_OutlookNavigatePane.IsShowCollapseButton = False
            m_OutlookNavigatePane.IsShowCollapsibleScreen = False
            m_OutlookNavigatePane.IsNavButtonSplitterVisible = False

            Try


                If (GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.ButtonSize)) Then

                    m_OutlookNavigatePane.NavigateBarButtonHeight = CInt(GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.ButtonSize)
                Else
                    m_OutlookNavigatePane.NavigateBarButtonHeight = 40
                End If
            Catch ex As Exception
                m_OutlookNavigatePane.NavigateBarButtonHeight = 40
            End Try




            m_OutlookNavigatePane.IsOverFlowPanelVisible = False

            AddHandler m_OutlookNavigatePane.OnNavigateBarDisplayedButtonCountChanged, AddressOf NavigationPane_OnNavigateBarDisplayedButtonCountChanged



            AddHandler m_OutlookNavigatePane.OnNavigateBarButtonSelected, AddressOf outlookNavigatePane_OnNavigateBarButtonSelected
            'AddHandler m_OutlookNavigatePane.OnNavigateBarButtonSelecting, AddressOf outlookNavigatePane_OnNavigateBarButtonSelecting


            AddHandler m_OutlookNavigatePane.OnNavigateBarColorChanged, AddressOf outlookNavigatePane_OnNavigateBarColorChanged
            AddHandler m_OutlookNavigatePane.HandleCreated, AddressOf outlookNavigatePane_HandleCreated
            'm_OutlookNavigatePane.Width = 150
            Try


                If (GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.ListFontSize)) Then

                    m_OutlookNavigatePane.Font = New Font("Tahoma", CInt(GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.ListFontSize), FontStyle.Bold)
                Else
                    m_OutlookNavigatePane.Font = New Font("Tahoma", 14, FontStyle.Bold)
                End If
            Catch ex As Exception
                m_OutlookNavigatePane.Font = New Font("Tahoma", 14, FontStyle.Bold)
            End Try

            ' Splitter

            splitterNavigateMenu = New MTSplitter()
            splitterNavigateMenu.Size = New Size(7, 100)
            splitterNavigateMenu.SplitterPointCount = 10
            splitterNavigateMenu.SplitterPaintAngle = 360.0F
            splitterNavigateMenu.Dock = DockStyle.Left

            ' Navigatebar Remote Control
            m_OutlookNavigatePane.IsCollapsible = False


            splitterNavigateMenu.Visible = False
            'm_OutlookNavigatePane.Margin = New Padding(0, 0, 0, 8)
            'splitterNavigateMenu.Margin = New Padding(0, 0, 0, 8)
            m_OutlookNavigatePane.Dock = DockStyle.Fill

            pnlSketchPalette.Controls.AddRange(New Control() {splitterNavigateMenu, m_OutlookNavigatePane})
            Return True

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return False
        End Try

    End Function
    Private Sub InitNavigateButtons()
        Dim nvbBtn As NavigateBarButton
        Dim pLst As ListView

        Dim skGrp As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigCreateFeaturePanelAttachmentOptionsAttachmentSketchGroupsAttachmentSketchGroup

        Try
            If m_OutlookNavigatePane Is Nothing Then Return


            If GlobalsFunctions.appConfig.WorkorderPanel IsNot Nothing Then
                If GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups IsNot Nothing Then
                    If GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.AttachmentSketchGroup IsNot Nothing Then
                        If GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.AttachmentSketchGroup.Count > 0 Then
                            m_NVBButton = New List(Of NavigateBarButton)


                            For Each skGrp In GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.AttachmentSketchGroup

                                nvbBtn = New NavigateBarButton()
                                pLst = New ListView

                                pLst.HideSelection = False

                                AddHandler pLst.SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged
                                AddHandler pLst.Resize, AddressOf resizeLst
                                AddHandler pLst.MouseDoubleClick, AddressOf ListView_MouseDoubleClick
                                pLst.View = View.Details
                                pLst.Columns.Add("")
                                ' pLst.Columns(0).AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)

                                pLst.Columns(0).Width = pLst.Width - 10

                                'pLst.Columns.Add("FID")
                                'pLst.Columns("FID").Width = 0

                                pLst.GridLines = True
                                pLst.HeaderStyle = ColumnHeaderStyle.None
                                pLst.SmallImageList = New ImageList
                                pLst.SmallImageList.ImageSize = New Size(40, 40)





                                Try
                                    If (GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.ListFontSize)) Then
                                        pLst.Font = New Font("Tahoma", CInt(GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.ListFontSize), FontStyle.Bold)
                                    Else
                                        pLst.Font = New Font("Tahoma", 12, FontStyle.Bold)
                                    End If
                                Catch ex As Exception
                                    pLst.Font = New Font("Tahoma", 12, FontStyle.Bold)
                                End Try

                                nvbBtn.RelatedControl = pLst
                                'nvbBtn.RelatedControl = New DataGridView()
                                'AddHandler nvbBtn.RelatedControl.VisibleChanged, AddressOf testEvent

                                nvbBtn.Caption = skGrp.Label
                                'nvbBtn.CaptionDescription = "High Priority Workorders"
                                'nvbHigh.Image = Properties.Resources.Mail24
                                nvbBtn.Enabled = True
                                nvbBtn.Key = skGrp.Label
                                nvbBtn.IsShowCaptionImage = False
                                nvbBtn.IsShowCaption = True
                                nvbBtn.IsShowCaptionDescription = False

                                Try


                                    If (GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.ButtonFontSize)) Then

                                        nvbBtn.Font = New Font("Tahoma", CInt(GlobalsFunctions.appConfig.CreateFeaturePanel.AttachmentOptions.AttachmentSketchGroups.ButtonFontSize), FontStyle.Bold)
                                    Else
                                        nvbBtn.Font = New Font("Tahoma", 16, FontStyle.Italic Or FontStyle.Bold)
                                    End If
                                Catch ex As Exception
                                    nvbBtn.Font = New Font("Tahoma", 16, FontStyle.Italic Or FontStyle.Bold)
                                End Try



                                nvbBtn.Tag = skGrp
                                AddHandler nvbBtn.OnNavigateBarButtonSelected, AddressOf nvbMail_OnNavigateBarButtonSelected
                                m_NVBButton.Add(nvbBtn)

                            Next



                        End If

                    End If

                End If
            End If

            If m_NVBButton IsNot Nothing Then
                m_OutlookNavigatePane.NavigateBarButtons.AddRange(m_NVBButton.ToArray)


            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        Finally
            nvbBtn = Nothing
            pLst = Nothing

            skGrp = Nothing

        End Try


    End Sub
    Public Sub LoadLayerDetails()

        Dim skGrp As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigCreateFeaturePanelAttachmentOptionsAttachmentSketchGroupsAttachmentSketchGroup = Nothing

        Try

            For Each nvBtn As NavigateBarButton In m_NVBButton
                skGrp = nvBtn.Tag

                Dim pFSWDef As FeatureSourceWithDef = GlobalsFunctions.GetFeatureSource(skGrp.Layer, m_Map)
                If pFSWDef IsNot Nothing Then
                    If pFSWDef.FeatureSource IsNot Nothing Then
                        nvBtn.RelatedControl.Tag = pFSWDef.FeatureSource
                        If pFSWDef.FeatureSource.IsAnnotationSource Then
                            CType(nvBtn.RelatedControl, ListView).SmallImageList = Nothing

                        End If
                        LoadSketchItems(nvBtn.RelatedControl, pFSWDef)
                    End If
                End If
                'LoadSketchItemsRenderer(nvBtn.RelatedControl, GlobalsFunctions.GetFeatureSource(skGrp.Layer, m_Map))
                ' pFSWDef.FeatureSource.IsAnnotationSource


            Next


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name)
            st = Nothing

        End Try


    End Sub

    Private Sub LoadSketchItems(ByVal LstView As ListView, pFS As FeatureSourceWithDef)
        Try



            ' Contains the map layer legend swatches and labels 
            Dim legendSwatchesList As IList(Of LegendItem)
            Dim lstItm As ListViewItem
            legendSwatchesList = pFS.FeatureSource.GetLegendItems(LstView.BackColor, New Drawing.Size(24, 24), True)
            If legendSwatchesList IsNot Nothing Then

                Dim imageIndex As Integer = 0
                For k As Integer = legendSwatchesList.Count - 1 To 0 Step -1
                    ' Adds the swatch image to the imagelist 
                    Dim pLbl As String = legendSwatchesList(k).Symbol.Label


                    If pFS.FeatureSource.IsAnnotationSource Then
                        Dim sts As SimpleTextSymbol = legendSwatchesList(k).Symbol
                        ' Dim m As String = sts.Label


                        If Trim(pLbl) = "" Then
                            pLbl = String.Format("{0} ({1})", sts.Font.Name, sts.Font.SizeInPoints)
                        Else
                            pLbl = Trim(pLbl)
                        End If
                        ' Creates a new child node using the legend's swatch label 
                        lstItm = New ListViewItem(pLbl)

                        lstItm.Font = New Font(sts.Font.Name, 14, sts.Font.Style, sts.Font.Unit)



                        lstItm.ForeColor = sts.TextColor


                    Else

                        LstView.SmallImageList.Images.Add(legendSwatchesList(k).Swatch)
                        If Trim(pLbl) = "" Then
                            pLbl = "Default"
                        Else
                            pLbl = Trim(pLbl)
                        End If
                        ' Creates a new child node using the legend's swatch label 
                        lstItm = New ListViewItem(pLbl, imageIndex)

                    End If



                    lstItm.Tag = legendSwatchesList(k).Symbol

                    ' Adds the layer node to the treeview 

                    LstView.Items.Add(lstItm)


                    imageIndex += 1
                Next k
            End If


        Catch ex As Exception

        End Try

    End Sub

    'Private Sub LoadSketchItemsRenderer(ByVal LstView As ListView, pFS As FeatureSourceWithDef)
    '    Try
    '        m_imgLst = New ImageList


    '        For Each sym As Esri.ArcGIS.Mobile.FeatureCaching.Symbol In pFS.FeatureSource.Renderer.Symbols


    '            Dim lstItm As New ListViewItem(sym.Label)


    '            ' Adds the layer node to the treeview 

    '            LstView.Items.Add(lstItm)


    '        Next

    '    Catch ex As Exception

    '    End Try

    'End Sub
    Public Enum GeometryType As Integer
        Null = 0
        Point = 1

        Multipoint = 2
        Polyline = 3
        Polygon = 4
        Envelope = 5
        Anno = 6
    End Enum


    Private Sub ListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If CType(sender, ListView).SelectedItems IsNot Nothing Then

            'If CType(sender, ListView).SelectedItems.Count > 0 Then




            'End Ifm_SketchMA.SketchMode = "Freehand"
            m_SketchMA.SketchMode = ""
            If CType(sender, ListView).SelectedItems.Count > 0 Then
                If CType(sender, ListView).SelectedItems(0).Text.ToUpper().Contains("FREEHAND") Then
                    m_SketchMA.SketchMode = "Freehand"


                End If
            End If
            Dim pFS As FeatureSource = CType(sender, ListView).Tag
            If pFS.IsAnnotationSource Then
                m_SketchMA.GeoType = GeometryType.Anno

            Else
                m_SketchMA.GeoType = pFS.GeometryType

            End If


            If m_Map.MapAction IsNot m_SketchMA Then
                m_lastMA = m_Map.MapAction
            End If
            m_Map.MapAction = m_SketchMA
        End If



    End Sub
    Private Sub ListView_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If CType(sender, ListView).SelectedItems IsNot Nothing Then

            If CType(sender, ListView).SelectedItems.Count > 0 Then

                ' Dim woFilt As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilter = m_OutlookNavigatePane.SelectedButton.Tag

                'btnViewWorkDetails_Click(Nothing, Nothing)

                'm_AttDisplay.IdentifyLocation(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry)




            End If
        End If


    End Sub

    Private Sub outlookNavigatePane_OnNavigateBarColorChanged(ByVal sender As Object, ByVal e As EventArgs)
        splitterNavigateMenu.SplitterLightColor = m_OutlookNavigatePane.NavigateBarColorTable.ButtonNormalBegin
        splitterNavigateMenu.SplitterDarkColor = m_OutlookNavigatePane.NavigateBarColorTable.ButtonNormalEnd
        splitterNavigateMenu.SplitterBorderColor = Color.Transparent
    End Sub

    ''' <summary>
    ''' Set new color (passing saved color table in settings file )
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub outlookNavigatePane_HandleCreated(ByVal sender As Object, ByVal e As EventArgs)
        'if (MessageBox.Show("Do you want use system colors", "Theme Color", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        '    outlookNavigatePane.NavigateBarColorTable = NavigateBarColorTable.SystemColor;
    End Sub


    ''' <summary>
    ''' Any button selected
    ''' </summary>
    ''' <param name="tNavigateBarButton"></param>
    Private Sub outlookNavigatePane_OnNavigateBarButtonSelected(ByVal tNavigateBarButton As NavigateBarButton)
        If TypeOf (tNavigateBarButton.RelatedControl) Is DataGridView Then

        ElseIf TypeOf (tNavigateBarButton.RelatedControl) Is ListView Then



        End If

        ' MsgBox(pDGrid.ColumnCount)

    End Sub

    ''' <summary>
    ''' Check selecting for any button
    ''' </summary>
    ''' <param name="e"></param>
    'Private Sub outlookNavigatePane_OnNavigateBarButtonSelecting(ByVal e As NavigateBarButtonCancelEventArgs)
    '    Dim pDGrid As DataGridView = e.Selected.RelatedControl
    '    MsgBox(pDGrid.ColumnCount)

    'End Sub
    'Private Sub testEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Dim pDGrid As DataGridView = sender
    '    MsgBox(pDGrid.ColumnCount)

    'End Sub
    Private Sub nvbMail_OnNavigateBarButtonSelected(ByVal e As NavigateBarButtonEventArgs)

    End Sub

    Private Sub NavigationPane_OnNavigateBarDisplayedButtonCountChanged()

    End Sub


    Private Sub resizeLst(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim lstView As ListView = sender
        If lstView.Columns.Count > 0 Then
            lstView.Columns(0).Width = sender.width - 10


        End If
    End Sub




#End Region







    Public ReadOnly Property ListBox As ListBox
        Get
            Return lstAtt

        End Get

    End Property
    Private Sub attlistclick(sender As Object, e As MouseEventArgs)
        'MsgBox(CType(CType(sender, ListBox).SelectedItem, Attachment).FilePath)
        ' Process.Start(CType(CType(sender, ListBox).SelectedItem, Attachment).FilePath)
    End Sub

    Private Sub attdraw(sender As Object, e As System.Windows.Forms.DrawItemEventArgs)

        ' Draw the background of the ListBox control for each item.
        e.DrawBackground()
        ' Define the default color of the brush as black.
        Dim myBrush As Brush = Brushes.Black



        ' Draw the current item text based on the current Font  
        ' and the custom brush settings.

        If TypeOf (sender.Items(e.Index)) Is attFiles Then
            myBrush = Brushes.Red
            e.Graphics.DrawString(CType(sender.Items(e.Index), attFiles).fileName.ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)

        Else
            myBrush = Brushes.Blue
            e.Graphics.DrawString(CType(sender.Items(e.Index), Attachment).Name.ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)

        End If

        'e.Graphics.DrawString(sender.Items(e.Index).ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)
        ' If the ListBox has focus, draw a focus rectangle around the selected item.
        e.DrawFocusRectangle()

    End Sub

    Private Sub SplitContainer1_Panel2_Paint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles splitMainAtt.Panel2.Paint

    End Sub


    Private Sub btnAdd_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            'Opens a dialog to browse out for an image
            Dim openFileDialog1 As System.Windows.Forms.OpenFileDialog

            openFileDialog1 = New System.Windows.Forms.OpenFileDialog()



            'Filter the image types
            ' openFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF"
            'If the user selects an image
            If openFileDialog1.ShowDialog() = DialogResult.OK Then
                'Set the path of the image to the text box
                Dim atfl As attFiles = New attFiles
                atfl.filePath = openFileDialog1.FileName
                atfl.fileName = Path.GetFileName(openFileDialog1.FileName)


                RaiseEvent AttachmentSelected(atfl)

                lstAtt.Items.Add(atfl)

            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub btnRemove_Click(sender As System.Object, e As System.EventArgs) Handles btnRemove.Click
        Try

            If TypeOf lstAtt.SelectedItem Is Attachment Then
                RaiseEvent DeleteAttachment(CType(lstAtt.SelectedItem, Attachment).Id)

                lstAtt.Items.Remove(lstAtt.SelectedItem)



            Else
                lstAtt.Items.Remove(lstAtt.SelectedItem)
            End If


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub AttachmentControl_Disposed(sender As Object, e As System.EventArgs) Handles Me.Disposed
        If m_SketchGraphics IsNot Nothing Then
            If m_SketchGraphics.Graphics IsNot Nothing Then
                If m_SketchGraphics.Graphics.Count > 0 Then
                    m_SketchGraphics.Graphics.Clear()
                End If
            End If

        End If
    End Sub
    Public Sub ClearGraphLayer()
        If m_SketchGraphics IsNot Nothing Then
            If m_SketchGraphics.Graphics IsNot Nothing Then
                If m_SketchGraphics.Graphics.Count > 0 Then
                    m_SketchGraphics.Graphics.Clear()
                End If
            End If

        End If
    End Sub
    Private Sub AttachmentControl_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        Try
            GlobalsFunctions.CenterButtonInControl(sender)

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub btnRemove_SizeChanged(sender As System.Object, e As System.EventArgs) Handles btnRemove.SizeChanged

    End Sub

    Private Sub lstAtt_DoubleClick(sender As System.Object, e As System.EventArgs) Handles lstAtt.DoubleClick
        If TypeOf (CType(sender, ListBox).SelectedItem) Is Attachment Then
            Process.Start(CType(CType(sender, ListBox).SelectedItem, Attachment).FilePath)
        Else
            Process.Start(CType(CType(sender, ListBox).SelectedItem, attFiles).filePath)
        End If

    End Sub

    Private Sub btnAttCam_Click(sender As System.Object, e As System.EventArgs) Handles btnAttCam.Click
        Dim stPath As String = GlobalsFunctions.getImageFromCam()

        If stPath <> "" Then
            'Set the path of the image to the text box
            Dim atfl As attFiles = New attFiles
            atfl.filePath = stPath
            atfl.fileName = Path.GetFileName(stPath)


            RaiseEvent AttachmentSelected(atfl)

            lstAtt.Items.Add(atfl)

        End If

    End Sub

    Private Sub btnAddSketch_Click(sender As System.Object, e As System.EventArgs) Handles btnAddSketch.Click
        If SplitContainer1.Panel2Collapsed Then
            SplitContainer1.Panel2Collapsed = False
            SplitContainer1.Panel1Collapsed = True
            btnAddSketch.BackgroundImage = My.Resources.AddSketchAttred
        Else
            SplitContainer1.Panel1Collapsed = False
            SplitContainer1.Panel2Collapsed = True
            btnAddSketch.BackgroundImage = My.Resources.AddSketchAtt
            If m_Map.MapAction Is m_SketchMA Then
                m_Map.MapAction = m_lastMA

            End If
        End If

    End Sub

    Private Sub lstAtt_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lstAtt.SelectedIndexChanged

    End Sub

    Private Sub pnlSketchButtons_Resize(sender As System.Object, e As System.EventArgs) Handles pnlSketchButtons.Resize
        ' GlobalsFunctions.CenterButtonInControl(sender)

    End Sub

    Private Sub btnClear_Click(sender As System.Object, e As System.EventArgs) Handles btnClear.Click
        m_SketchGraphics.Graphics.Clear()
        m_SketchGraphics.Refresh()

        m_Map.Invalidate()
    End Sub

    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click
        saveSketch()

    End Sub
    Public Sub saveSketch()
        Dim image As System.Drawing.Image = m_Map.ToBitmap



        Dim strPath As String = GlobalsFunctions.getRandomFileNameInLocation("MapImage_{0}.png")

        image.Save(strPath, System.Drawing.Imaging.ImageFormat.Png)
        Dim atfl As attFiles = New attFiles
        atfl.filePath = strPath
        atfl.fileName = Path.GetFileName(strPath)


        RaiseEvent AttachmentSelected(atfl)

        lstAtt.Items.Add(atfl)
        m_SketchGraphics.Graphics.Clear()
        m_Map.Invalidate()
        Call btnAddSketch_Click(Nothing, Nothing)
    End Sub
    Private Sub m_SketchMA_SketchComplete() Handles m_SketchMA.SketchComplete
        If m_SketchMA Is Nothing Then Return
        If m_SketchMA.geometery Is Nothing Then Return
        If m_SketchMA.geometery.IsValid = False Then Return


        Dim pMpGp As SketchMapGraphic = New SketchMapGraphic

        pMpGp.Geometry = m_SketchMA.geometery
        If m_SketchMA.GeoType = 6 Then

            pMpGp.isAnno = True
            Dim fm As frmEnterText = New frmEnterText("Enter annotation to display")
            fm.ShowDialog()

            pMpGp.Text = fm.m_Value

        End If

        ' MsgBox(.ToString)
        If m_OutlookNavigatePane IsNot Nothing Then
            If m_OutlookNavigatePane.SelectedButton IsNot Nothing Then
                If CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems IsNot Nothing Then

                    If CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems.Count > 0 Then
                        If CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems(0).Text.Contains("DashDot") Then
                            pMpGp.LineStyle = Drawing2D.DashStyle.DashDot
                        ElseIf CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems(0).Text.Contains("Dash") Then
                            pMpGp.LineStyle = Drawing2D.DashStyle.Dash
                        ElseIf CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems(0).Text.Contains("Dot") Then
                            pMpGp.LineStyle = Drawing2D.DashStyle.Dot
                        End If
                        If CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems(0).Text.Contains("Arrow") Then
                            pMpGp.LineArrow = True

                        End If
                        If CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems(0).Text.Contains("Trans") Then
                            pMpGp.fillTrans = 180

                        End If

                        pMpGp.Geometry = m_SketchMA.geometery
                        pMpGp.Symbol = CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems(0).Tag

                        m_SketchGraphics.Graphics.Add(pMpGp)
                        m_SketchMA.geometery = Nothing

                    End If


                End If

            End If

        End If
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles btnRemoveLast.Click
        If m_SketchGraphics.Graphics.Count = 0 Then Return
        Try


            m_SketchGraphics.Graphics.RemoveAt(m_SketchGraphics.Graphics.Count - 1)


        Catch ex As Exception
            m_Map.Invalidate()
            m_SketchGraphics.Refresh()

        End Try
    End Sub

    Private Sub pnlSketchPalette_Paint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles pnlSketchPalette.Paint

    End Sub

    Private Sub m_SketchGraphics_DataChanged(sender As Object, e As Esri.ArcGIS.Mobile.DataChangedEventArgs) Handles m_SketchGraphics.DataChanged
        setSketchStatus()
    End Sub
    Private Sub setSketchStatus()
        If m_SketchGraphics.Graphics.Count = 0 Then
            btnSave.Enabled = False
            btnSave.BackgroundImage = My.Resources.SaveSketchGray

            btnClear.Enabled = False
            btnClear.BackgroundImage = My.Resources.ClearSketchGray

            btnRemoveLast.Enabled = False
            btnRemoveLast.BackgroundImage = My.Resources.RotateLeftGray

        Else
            btnSave.Enabled = True
            btnSave.BackgroundImage = My.Resources.SaveSketch

            btnClear.Enabled = True
            btnClear.BackgroundImage = My.Resources.ClearSketch

            btnRemoveLast.Enabled = True
            btnRemoveLast.BackgroundImage = My.Resources.RotateLeft

        End If

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

