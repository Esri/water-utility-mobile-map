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


Imports System.Windows.Forms
Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.Geometries
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports System.Drawing
Imports Esri.ArcGISTemplates


Public Class EditControl
    Private m_AttControlBox As AttachmentControl
    Private m_lstAttToDel As List(Of Integer)

    Private m_pTabPagAtt As TabPage = Nothing

    Private m_GPSStatus As Boolean
    Private m_AttMan As AttachmentManager

    Private m_EditOptions As MobileConfigClass.MobileConfigMobileMapConfigEditControlOptionsLayersLayer
    Public Event GetWorkorder()
    Public currentWO As String
    Public currentCrew As String

    Public Event HyperClick(ByVal PathToFile As String)
    Public Event RecordSnapped(ByVal geo As Geometry)
    Private m_shuffling As Boolean = False
    Private m_HasFilter As Boolean = False
    Private m_SettingAtts As Boolean = False

    Private m_FColor As Color = Color.Black
    Private m_BColor As Color = Color.White
    Private m_LayerOp As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerName

    Private m_RadioOnTwo As Boolean = True
    'Private m_SaveBtns As IList(Of Button)
    'Private m_ClearBtns As IList(Of Button)
    'Private m_EditBtns As IList(Of Button)
    Private m_BaseColor As Color

    Private m_NewColor As Color
    Private m_ExistingColor As Color
    Private m_GPSVal As GPSLocationDetails = Nothing
    'Events
    Public Event RecordSaved(ByVal LayerName As String, ByVal pGeo As Geometries.Geometry, ByVal OID As Integer)
    Public Event RecordClear()
    Public Event SyncLayer(layerName As String)
    Public Event RaiseMessage(ByVal Message As String)
    Public Event RecordDelete(ByVal LayerName As String)
    Public Event RaisePermMessage(ByVal Message As String, ByVal Hide As Boolean)
    Public Event MoveGeo(ByVal status As Boolean)

    'Module Level Variables
    Private m_LogEdit As Boolean = False

    'ESRI Map control
    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    'The active editable datarow
    Private m_FDR As FeatureDataRow
    Private m_FL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource
    'Create a universal font to use on all controls
    Private m_Fnt As Font '= New System.Drawing.Font("Microsoft Sans Serif", 12.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_FntLbl As Font ' = New System.Drawing.Font("Microsoft Sans Serif", 11.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Public Event CheckGPS()

    'used to handle a edit form in a popup
    Private m_CloseParentOnSave As Boolean = False
    'determines if the control will draw the geometry in the editable row
    Private m_DrawGeo As Boolean = False
    'The color and size to draw the geometry
    Private m_Pen As Pen
    Private m_Brush As New SolidBrush(Color.Transparent)
    Private m_penLineFlash As Pen = Pens.Cyan
    Private m_penFlash As Pen = New Pen(Color.Purple, 10)
    Private m_brushFlash As SolidBrush = New SolidBrush(Color.Cyan)
    Private m_PointSize As Integer = 10
    Private m_bNewRecordAfterSave As Boolean = True
    'event used so the parent can intercept the save to override or default values
    Private m_InterceptSave As Boolean = False
    Public Event SaveIntercepted()
    Private m_ResetOnClick As Boolean = False
    'Private m_UpdateDateFields As String
    'Private m_UpdateUserFields As String
    'Private m_SpatialIntersects As List(Of SpatialIntersectDef)

    Private m_Mode As String = "Edit"
    Private m_GPSButtonVisible As Boolean = True

#Region "Properties"
    Public ReadOnly Property EditingGeo As Boolean
        Get
            If btnMove.Enabled Then
                If btnMove.Checked Then
                    Return True
                End If
            End If
            Return False

        End Get
    End Property
    Public Property GPSStatus As Boolean
        Get
            Return m_GPSStatus
        End Get
        Set(ByVal value As Boolean)
            m_GPSStatus = value
            btnGPSLoc.Enabled = value
          
            disableSaveBtn()

        End Set
    End Property
    Public Property MoveGeoButtonVisible As Boolean
        Get
            Return btnMove.Visible

        End Get
        Set(ByVal value As Boolean)
            btnMove.Visible = value



            relocatebtns()
        End Set
    End Property
    Public Property DeleteButtonVisible As Boolean
        Get
            Return btnDelete.Visible

        End Get
        Set(ByVal value As Boolean)
            btnDelete.Visible = value
            btnDelete.Enabled = True



            relocatebtns()
        End Set
    End Property
    Public Property GPSButtonVisible As Boolean
        Get
            Return m_GPSButtonVisible
        End Get
        Set(ByVal value As Boolean)



            m_GPSButtonVisible = value
            If GlobalsFunctions.appConfig.NavigationOptions.GPS.Visible.ToUpper = "TRUE" Then
                btnGPSLoc.Visible = m_GPSButtonVisible
                relocatebtns()
            Else
                btnGPSLoc.Visible = False
                relocatebtns()
            End If


        End Set
    End Property
    Public Property ClearButtonVisible As Boolean
        Get
            Return btnClear.Visible
        End Get
        Set(ByVal value As Boolean)



            btnClear.Visible = value

            relocatebtns()


        End Set
    End Property

    'Private m_specialCase As String = ""

    'Public Property specialCase As String
    '    Get
    '        Return m_specialCase
    '    End Get
    '    Set(ByVal value As String)



    '        m_specialCase = value



    '    End Set
    'End Property


    Public Property Mode() As String
        Get
            Return m_Mode
        End Get
        Set(ByVal value As String)
            m_Mode = value

        End Set
    End Property
    Public Property GPSLocation() As GPSLocationDetails
        Get
            Return m_GPSVal
        End Get
        Set(ByVal value As GPSLocationDetails)
            m_GPSVal = value

        End Set
    End Property
    'Property used to notify the panel if it should create a new row after a save
    Public Property NewRecordAfterSave() As Boolean
        Get
            Return m_bNewRecordAfterSave

        End Get
        Set(ByVal value As Boolean)
            m_bNewRecordAfterSave = value
        End Set
    End Property
    'Public Property RadioOnTwo() As Boolean
    '    Get
    '        Return m_RadioOnTwo

    '    End Get
    '    Set(ByVal value As Boolean)
    '        m_RadioOnTwo = value
    '    End Set
    'End Property
    'Property used to notify the panel if it should raise the save intercept event
    Public Property InterceptSave() As Boolean
        Get
            Return m_InterceptSave
        End Get
        Set(ByVal value As Boolean)
            m_InterceptSave = value
        End Set
    End Property
    'Property used to notify the panel if it should draw the row being edited
    Public Property DrawGeo() As Boolean
        Get
            Return m_DrawGeo
        End Get
        Set(ByVal value As Boolean)
            m_DrawGeo = value
        End Set
    End Property
    'Property used for draw colors
    Public Property ActiveGeoPen() As Pen
        Get
            Return m_Pen
        End Get
        Set(ByVal value As Pen)
            m_Pen = value
        End Set
    End Property
    Public Property ActiveGeoBrush() As SolidBrush
        Get
            Return m_Brush
        End Get
        Set(ByVal value As SolidBrush)
            m_Brush = value
        End Set
    End Property
    Public Property ActiveGeoPointSize() As Integer
        Get
            Return m_PointSize
        End Get
        Set(ByVal value As Integer)
            m_PointSize = value
        End Set
    End Property
    'Get or saves the geometry into the active row
    Public Sub AddCoordToGeo(ByVal coord As Coordinate)
        If m_FDR.Geometry IsNot Nothing Then
            m_FDR.Geometry.AddCoordinate(coord)

            UpdatePanel()

        End If


    End Sub
    Public Property Geometry() As Geometries.Geometry
        Get
            Return m_FDR.Geometry
        End Get
        Set(ByVal value As Geometries.Geometry)
            m_FDR.Geometry = value
            UpdatePanel()
            If btnMove.Enabled Then
                If btnMove.Checked Then
                    btnMove.Checked = False

                End If
            End If
        End Set
    End Property
    Public Sub UpdatePanel()
        Try


            Dim pDR As FeatureDataRow = SnapToGeo()
            LoadAutoAttributes(pDR)
            disableSaveBtn()
            disableDeleteBtn()

            'Cause the map to redraw
            If m_DrawGeo Then
                m_Map.Invalidate()
            End If
            pDR = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    'Property used to let the panel know if it should close after a save, for Pop ups, do not use if the parent form is the main form
    Public Property CloseOnSave() As Boolean
        Get
            Return m_CloseParentOnSave
        End Get
        Set(ByVal value As Boolean)
            m_CloseParentOnSave = value

        End Set
    End Property
    Public WriteOnly Property DisplayFont() As Font
        Set(ByVal value As Font)
            m_Fnt = value
        End Set
    End Property
    'The ArcGIS Mobile Map Control
    Public Property mapControl() As Esri.ArcGIS.Mobile.WinForms.Map
        Get
            Return m_Map
        End Get
        Set(ByVal value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value
        End Set
    End Property
    Private Sub curRec(ByVal FeatLayer As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource, ByVal featureDT As FeatureDataTable)

        If FeatLayer Is Nothing Then
            DisplayBlank()
            m_FDR = Nothing

            Return
        End If
        If m_FL Is Nothing Then
            m_FL = FeatLayer
            m_DT = featureDT

            m_FDR = Nothing

            AddControls()
        ElseIf FeatLayer.Name <> m_FL.Name Then
            'Add the controls for each field
            m_FL = FeatLayer
            m_DT = featureDT
            m_FDR = Nothing

            AddControls()


        End If
        disableSaveBtn()
        disableDeleteBtn()
    End Sub
    Public Sub setCurrentLayer(ByVal FeatLayer As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource, ByVal editOptions As MobileConfigClass.MobileConfigMobileMapConfigEditControlOptionsLayersLayer)
        Try


            m_EditOptions = editOptions
            curRec(FeatLayer, FeatLayer.GetDataTable)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Public Sub setCurrentRecord(ByVal fdr As FeatureDataRow, ByVal editOptions As MobileConfigClass.MobileConfigMobileMapConfigEditControlOptionsLayersLayer)
        Try


            m_EditOptions = editOptions
            setCurRec(fdr)
        Catch ex As Exception

        End Try
    End Sub
    Private ReadOnly Property CurrentLayer() As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource
        Get
            Return m_FL
        End Get
        'Set(ByVal value as Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
        '    If value Is Nothing Then
        '        DisplayBlank()
        '        m_FDR = Nothing

        '        Return
        '    End If
        '    If m_FL Is Nothing Then
        '        m_FL = value
        '        m_FDR = Nothing

        '        AddControls()
        '    ElseIf value.Name <> m_FL.Name Then
        '        'Add the controls for each field
        '        m_FL = value
        '        m_FDR = Nothing

        '        AddControls()
        '    End If



        'End Set
    End Property
    Private Sub setCurRec(ByVal value As FeatureDataRow)
        If value Is Nothing Then

            Return
        End If

        curRec(value.FeatureSource, value.Table)
        m_FDR = value
        'Load the table
        If m_FDR.StoredEditSate = EditState.NotDefined Then
            loadRecordEditor(True)
        Else
            loadRecordEditor(True)
        End If

        LoadAutoAttributes(m_FDR)
        UpdatePanel()


        relocatebtns()

        If m_FDR.StoredEditSate = EditState.NotDefined And m_Mode <> "ID" Then

            For Each tbPage As TabPage In tbCntrlEdit.TabPages
                tbPage.BackColor = m_NewColor
            Next

        ElseIf m_Mode <> "ID" Then
            btnMove.Enabled = False
            If m_FDR.Geometry Is Nothing Then
                btnMove.Enabled = False
            ElseIf m_FDR.Geometry.GeometryType = GeometryType.Point Then
                btnMove.Enabled = True
            End If
            Call btnMove_CheckedChanged(Nothing, Nothing)
            For Each tbPage As TabPage In tbCntrlEdit.TabPages
                tbPage.BackColor = m_ExistingColor
            Next

        End If
    End Sub
    Public ReadOnly Property CurrentRecord() As FeatureDataRow
        Get
            Return m_FDR
        End Get
        'Set(ByVal value As FeatureDataRow)
        '    'Return if the record is null
        '    If value Is Nothing Then

        '        Return
        '    End If

        '    curRec(value.FeatureSource)
        '    m_FDR = value
        '    'Load the table
        '    If m_FDR.StoredEditSate = EditState.NotDefined Then
        '        loadRecordEditor(True)
        '    Else
        '        loadRecordEditor(True)
        '    End If


        '    UpdatePanel()


        '    relocatebtns()

        '    If m_FDR.StoredEditSate = EditState.NotDefined And m_Mode <> "ID" Then

        '        For Each tbPage As TabPage In tbCntrlEdit.TabPages
        '            tbPage.BackColor = m_NewColor
        '        Next

        '    ElseIf m_Mode <> "ID" Then
        '        btnMove.Enabled = True
        '        If m_FDR.Geometry Is Nothing Then
        '            btnMove.Enabled = False
        '        Else
        '            btnMove.Enabled = True
        '        End If

        '        For Each tbPage As TabPage In tbCntrlEdit.TabPages
        '            tbPage.BackColor = m_ExistingColor
        '        Next

        '    End If
        'End Set
    End Property
    'Public Property LogEdits() As Boolean
    '    Get
    '        Return m_LogEdit
    '    End Get
    '    Set(ByVal value As Boolean)
    '        m_LogEdit = value

    '    End Set
    'End Property
#End Region
#Region "Public Methods"

    Public Sub New(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map, ByVal NewRecord As FeatureDataRow)



        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        Try
            ' m_RadioOnTwo = radioOnTwo
            ' 'Dim myutils As ConfigUtils = New ConfigUtils()

            'm_UpdateDateFields = GlobalsFunctions.FindConfigKey("ChangeDateOnRecordLoad")
            'm_UpdateUserFields = GlobalsFunctions.FindConfigKey("ChangeOwnerOnRecordLoad")
            ' spltContEdit.SplitterDistance = 525
            'If spCntMain.Panel1Collapsed = False Then
            '    spltContEdit.SplitterDistance = MyBase.Height - spCntMain.Panel1.Height - 60
            'Else
            '    spltContEdit.SplitterDistance = MyBase.Height - 60
            'End If

            ' pnlEditBtns.Dock = DockStyle.Fill
            Dim fntSize As Single
            Single.TryParse(GlobalsFunctions.appConfig.ApplicationSettings.FontSize, fntSize)
            If Not IsNumeric(fntSize) Then
                fntSize = 10

            End If

            m_Fnt = New System.Drawing.Font("Microsoft Sans Serif", fntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
            m_FntLbl = New System.Drawing.Font("Microsoft Sans Serif", fntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)

            'LoadSpatialIntersectArray(GlobalsFunctions.FindConfigKey("SpatialIntersect"))
            Boolean.TryParse(GlobalsFunctions.appConfig.EditControlOptions.RadioOnTwo, m_RadioOnTwo)
            Boolean.TryParse(GlobalsFunctions.appConfig.EditControlOptions.LogEdits, m_LogEdit)


            Dim strConfigValue As String

            strConfigValue = GlobalsFunctions.appConfig.EditControlOptions.EditFormExistingColor 'GlobalsFunctions.FindConfigKey("EditFormExistingColor")
            '  m_ExistingColor = Color.LightSteelBlue
            If strConfigValue IsNot Nothing Then
                If strConfigValue <> "" Then


                    Dim RGB() As String = strConfigValue.Split(",")
                    If RGB.Length = 3 Then
                        If IsNumeric(RGB(0)) And IsNumeric(RGB(1)) And IsNumeric(RGB(2)) Then
                            Try
                                m_ExistingColor = Color.FromArgb(CInt(RGB(0)), CInt(RGB(1)), CInt(RGB(2)))
                            Catch ex As Exception

                            End Try

                        End If




                    End If
                End If

            End If
            strConfigValue = GlobalsFunctions.appConfig.EditControlOptions.EditFormNewColor
            m_NewColor = Color.Empty 'Color.LightGreen
            If strConfigValue IsNot Nothing Then
                If strConfigValue <> "" Then


                    Dim RGB() As String = strConfigValue.Split(Char.Parse(","))
                    If RGB.Length = 3 Then
                        If IsNumeric(RGB(0)) And IsNumeric(RGB(1)) And IsNumeric(RGB(2)) Then
                            Try
                                m_NewColor = Color.FromArgb(CInt(RGB(0)), CInt(RGB(1)), CInt(RGB(2)))
                            Catch ex As Exception

                            End Try

                        End If




                    End If
                End If

            End If




            tbCntrlEdit.Font = m_Fnt

            ' Add any initialization after the InitializeComponent() call.
            'Set the Map
            m_Map = map
            'Initilize the display colors
            m_Pen = New Pen(Color.Cyan)
            m_Pen.Width = 3

            'm_PicSaving = New PictureBox
            'm_PicSaving.Image = My.Resources.animated

            'm_PicSaving.BackColor = Color.Red
            'm_PicSaving.Width = My.Resources.animated.Width
            'm_PicSaving.Height = My.Resources.animated.Height

            'm_PicSaving.Visible = False
            'recenterImage
            'm_Map.Controls.Add(m_PicSaving)


            If GlobalsFunctions.appConfig.NavigationOptions.GPS.Visible.ToUpper = "TRUE" Then
                btnGPSLoc.Visible = m_GPSButtonVisible
                relocatebtns()
            Else
                btnGPSLoc.Visible = False
                relocatebtns()
            End If
            If GPSStatus Then
                btnGPSLoc.Enabled = True
            Else
                btnGPSLoc.Enabled = False
            End If

            'If a null record was passed in return
            If NewRecord IsNot Nothing Then
                If NewRecord.FeatureSource IsNot Nothing Then



                    'Set up the edtiable record
                    m_FDR = NewRecord

                    'Make sure there is a feature layer associated with the row

                    'Add the controls for row
                    AddControls()
                    'Load the values of the row to the form
                    loadRecordEditor()
                    LoadAutoAttributes(m_FDR)
                End If

            End If
            'Enable/Disable the save button(Only checks for a valid Geometry)

            disableSaveBtn()
            disableDeleteBtn()

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Public Function deleteRecord() As Boolean
        If m_FDR IsNot Nothing Then
            If m_FDR.StoredEditSate <> EditState.NotDefined Then
                Dim pDT As FeatureDataTable = m_FDR.Table

                m_FDR.Delete()
                pDT.SaveInFeatureSource()
                pDT.AcceptChanges()

                pDT.FeatureSource.SaveEdits(pDT)

                'Dim pRetGeo As Geometries.Geometry = Nothing
                'Dim pRetFID As Integer = Nothing
                Dim pRetName As String = m_FL.Name

                m_FDR = pDT.NewRow
                loadRecordEditor(True)
                LoadAutoAttributes(m_FDR)

                disableSaveBtn()
                disableDeleteBtn()
                'Raise the event to notify the record was saved
                RaiseEvent RecordDelete(pRetName)
                RaiseEvent RaiseMessage(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.DeleteMessage)
                pDT = Nothing
                btnMove.Checked = False

            End If
        End If


    End Function
    Public Function saveRecord() As Boolean

        'Saves the input from the form to the record
        If SaveFormToRecord() = False Then
            RaiseEvent RaiseMessage(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NotSavedMessage)

            'MsgBox("Record was not saved")
            Return False


        End If

        'If the parent form is requesting a intercept on the save, raise the event
        If m_InterceptSave Then
            RaiseEvent SaveIntercepted()

        Else
            'Save the record to the datatable
            SaveRecordToLayer()

        End If
        If m_EditOptions IsNot Nothing Then
            If GlobalsFunctions.IsNumeric(m_EditOptions.SyncOnSave) Then
                If CInt(m_EditOptions.SyncOnSave) > 0 Then
                    If CInt(m_EditOptions.SyncOnSave) >= m_FL.EditsCount Then
                        RaiseEvent SyncLayer(m_EditOptions.Name)

                    End If
                End If
            End If

        End If
    End Function
    'Sub called from the parent to commit the record after a save intercept is raise
    Public Sub commitRecord()
        SaveRecordToLayer()

    End Sub

    Public Sub NewRecord()
        m_FDR = m_FDR.Table.NewRow
        btnMove.Visible = False

        loadRecordEditor()
        LoadAutoAttributes(m_FDR)

        disableSaveBtn()
        disableDeleteBtn()
    End Sub
    Public Sub SetReadOnly(ByVal Field As String, ByVal setReadOnly As Boolean)
        Try

            Dim strFld As String
            'Gets the feature layer 
            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = m_FDR.FeatureSource
            'If the layer has subtypes, load the subtype value first
            'Loop through all the controls and set their value
            For Each pCntrl As Control In tbCntrlEdit.Controls
                If TypeOf pCntrl Is TabPage Then
                    For Each cCntrl As Control In pCntrl.Controls
                        'If the control is a 2 value domain(Checkboxs)
                        If TypeOf cCntrl Is Panel Then
                            For Each cCntrlPnl As Control In cCntrl.Controls
                                If TypeOf cCntrlPnl Is CustomPanel Then
                                    'Get the Field
                                    strFld = CType(cCntrlPnl, CustomPanel).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    If Field = strFld Then


                                        'Get the target value

                                        Dim pCsPn As CustomPanel = CType(cCntrlPnl, CustomPanel)
                                        'Loop through the checkboxes to set the proper value

                                        For Each rdCn As Control In pCsPn.Controls
                                            If TypeOf rdCn Is RadioButton Then

                                                If setReadOnly Then
                                                    cCntrlPnl.Enabled = False
                                                Else
                                                    cCntrlPnl.Enabled = True


                                                End If

                                                Exit For

                                                If setReadOnly Then
                                                    cCntrlPnl.Enabled = False
                                                Else
                                                    cCntrlPnl.Enabled = True

                                                End If

                                                Exit For



                                            End If
                                        Next
                                    End If
                                    'If the control is a text box
                                ElseIf TypeOf cCntrlPnl Is TextBox Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, TextBox).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    If Field = strFld Then
                                        'Set the Value
                                        If setReadOnly Then
                                            cCntrlPnl.Enabled = False
                                        Else
                                            cCntrlPnl.Enabled = True

                                        End If

                                        ' CType(cCntrlPnl, TextBox).Text = ""

                                    End If

                                    'if the control is a combo box(domain)
                                ElseIf TypeOf cCntrlPnl Is ComboBox Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, ComboBox).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    If Field = strFld Then
                                        Try
                                            If setReadOnly Then
                                                cCntrlPnl.Enabled = False
                                            Else
                                                cCntrlPnl.Enabled = True

                                            End If

                                        Catch ex As Exception
                                            cCntrlPnl.Enabled = True

                                        End Try

                                    End If


                                    'if the contorl is a data time field
                                ElseIf TypeOf cCntrlPnl Is DateTimePicker Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, DateTimePicker).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If '

                                    If Field = strFld Then

                                        If setReadOnly Then
                                            cCntrlPnl.Enabled = False
                                        Else
                                            cCntrlPnl.Enabled = True

                                        End If


                                    End If


                                    'If the field is a range domain
                                ElseIf TypeOf cCntrlPnl Is NumericUpDown Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, NumericUpDown).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    If Field = strFld Then
                                        If setReadOnly Then
                                            cCntrlPnl.Enabled = False
                                        Else
                                            cCntrlPnl.Enabled = True

                                        End If


                                    End If




                                End If
                            Next

                        End If

                    Next
                End If
            Next

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try


    End Sub
    Public Function GetFieldControlVisibleState(ByVal Field As String) As Boolean
        Try

            Dim strFld As String
            For Each tbPage As TabPage In tbCntrlEdit.TabPages

                For Each cCntrl As Control In tbPage.Controls

                    'If the control is a 2 value domain(Checkboxs)
                    If TypeOf cCntrl Is Panel Then
                        For Each cCntrlPnl As Control In cCntrl.Controls
                            If TypeOf cCntrlPnl Is CustomPanel Then
                                'Get the Field
                                strFld = CType(cCntrlPnl, CustomPanel).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                If Field = strFld Then
                                    'Get the target value

                                    If Field = strFld Then
                                        Return CType(cCntrlPnl, CustomPanel).Visible

                                    End If

                                End If
                                'If the control is a text box
                            ElseIf TypeOf cCntrlPnl Is TextBox Then
                                'Get the field
                                strFld = CType(cCntrlPnl, TextBox).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                If Field = strFld Then
                                    Return CType(cCntrlPnl, TextBox).Visible

                                End If


                                'if the control is a combo box(domain)
                            ElseIf TypeOf cCntrlPnl Is ComboBox Then
                                'Get the field
                                strFld = CType(cCntrlPnl, ComboBox).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                If Field = strFld Then
                                    Return CType(cCntrlPnl, ComboBox).Visible

                                End If



                                'if the control is a data time field
                            ElseIf TypeOf cCntrlPnl Is DateTimePicker Then
                                'Get the field
                                strFld = CType(cCntrlPnl, DateTimePicker).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If '

                                If Field = strFld Then
                                    Return CType(cCntrlPnl, DateTimePicker).Visible

                                End If

                                'If the field is a range domain
                            ElseIf TypeOf cCntrlPnl Is NumericUpDown Then
                                'Get the field
                                strFld = CType(cCntrlPnl, NumericUpDown).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                If Field = strFld Then
                                    Return CType(cCntrlPnl, NumericUpDown).Visible

                                End If

                            End If
                        Next

                    End If

                Next
                ' End If
            Next

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try



    End Function

    Public Sub UpdateForm(ByVal Field As String, ByVal Value As Object, ByVal setReadOnly As String, Optional bSub As Boolean = False)
        Try
            m_SettingAtts = True

            Dim strFld As String
            'Gets the feature layer 
            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = m_FDR.FeatureSource
            'If the layer has subtypes, load the subtype value first
            'Loop through all the controls and set their value
            Dim bValSet As Boolean = False

            For Each tbPage As TabPage In tbCntrlEdit.TabPages
                'If TypeOf pCntrl Is TabPage Then
                If bValSet Then Exit For

                For Each cCntrl As Control In tbPage.Controls
                    If bValSet Then Exit For

                    'If the control is a 2 value domain(Checkboxs)
                    If TypeOf cCntrl Is Panel Then
                        For Each cCntrlPnl As Control In cCntrl.Controls
                            If TypeOf cCntrlPnl Is CustomPanel Then
                                'Get the Field
                                strFld = CType(cCntrlPnl, CustomPanel).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                If Field = strFld Then


                                    'Get the target value

                                    Dim pCsPn As CustomPanel = CType(cCntrlPnl, CustomPanel)
                                    'Loop through the checkboxes to set the proper value

                                    For Each rdCn As Control In pCsPn.Controls
                                        If TypeOf rdCn Is RadioButton Then
                                            If Value Is DBNull.Value Then

                                                If setReadOnly.ToUpper() = "TRUE" Then
                                                    cCntrlPnl.Enabled = False
                                                Else
                                                    cCntrlPnl.Enabled = True


                                                End If
                                                CType(rdCn, RadioButton).Checked = False
                                            ElseIf Value Is Nothing Then

                                                If setReadOnly.ToUpper() = "TRUE" Then
                                                    cCntrlPnl.Enabled = False
                                                Else
                                                    cCntrlPnl.Enabled = True


                                                End If
                                                CType(rdCn, RadioButton).Checked = False

                                            ElseIf Value.ToString() = "<NO_MOD>" Then
                                                If setReadOnly.ToUpper() = "TRUE" Then
                                                    cCntrlPnl.Enabled = False
                                                Else
                                                    cCntrlPnl.Enabled = True


                                                End If
                                            ElseIf rdCn.Tag.ToString = Value Then
                                                If setReadOnly.ToUpper() = "TRUE" Then
                                                    cCntrlPnl.Enabled = False
                                                Else
                                                    cCntrlPnl.Enabled = True


                                                End If

                                                CType(rdCn, RadioButton).Checked = True
                                                bValSet = True


                                                Exit For


                                            End If
                                        End If
                                    Next
                                End If
                                'If the control is a text box
                            ElseIf TypeOf cCntrlPnl Is TextBox Then
                                'Get the field
                                strFld = CType(cCntrlPnl, TextBox).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                If Field = strFld Then
                                    'Set the Value
                                    If setReadOnly.ToUpper() = "TRUE" Then
                                        cCntrlPnl.Enabled = False
                                    Else
                                        cCntrlPnl.Enabled = True


                                    End If
                                    If Value Is DBNull.Value Then
                                        CType(cCntrlPnl, TextBox).Text = ""
                                    ElseIf Value Is Nothing Then
                                        CType(cCntrlPnl, TextBox).Text = ""
                                    ElseIf Value.ToString() = "<NO_MOD>" Then

                                    Else

                                        CType(cCntrlPnl, TextBox).Text = Value
                                    End If
                                    bValSet = True

                                    ' CType(cCntrlPnl, TextBox).Text = ""
                                    Exit For

                                End If

                                'if the control is a combo box(domain)
                            ElseIf TypeOf cCntrlPnl Is ComboBox Then
                                'Get the field
                                strFld = CType(cCntrlPnl, ComboBox).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                If Field = strFld Then
                                    Try
                                        If setReadOnly.ToUpper() = "TRUE" Then
                                            cCntrlPnl.Enabled = False
                                        Else
                                            cCntrlPnl.Enabled = True


                                        End If
                                        If Value Is DBNull.Value Then
                                            CType(cCntrlPnl, ComboBox).SelectedValue = Value
                                        ElseIf Value Is Nothing Then
                                            CType(cCntrlPnl, ComboBox).SelectedValue = DBNull.Value
                                        ElseIf Value.ToString() = "<NO_MOD>" Then

                                        Else
                                            'CType(cCntrlPnl, ComboBox).Text = Value.ToString()

                                            For Each item As Object In CType(cCntrlPnl, ComboBox).Items
                                                If item IsNot Nothing Then
                                                    If Not item Is DBNull.Value Then
                                                        If item.value.ToString = Value.ToString Then
                                                            CType(cCntrlPnl, ComboBox).SelectedItem = item
                                                            If bSub Then
                                                                SubtypeChange(Value, Field)

                                                            End If
                                                        End If
                                                    End If
                                                End If

                                            Next

                                        End If
                                        bValSet = True

                                        Exit For


                                    Catch ex As Exception
                                        cCntrlPnl.Enabled = True

                                    End Try

                                End If


                                'if the control is a data time field
                            ElseIf TypeOf cCntrlPnl Is DateTimePicker Then
                                'Get the field
                                strFld = CType(cCntrlPnl, DateTimePicker).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If '

                                If Field.ToUpper = strFld.ToUpper Then
                                    If Value Is Nothing Then
                                        CType(cCntrlPnl, DateTimePicker).Checked = False
                                    ElseIf Value Is DBNull.Value Then
                                        CType(cCntrlPnl, DateTimePicker).Checked = False
                                    ElseIf Value.ToString() = "<NO_MOD>" Then

                                    Else
                                        If IsDate(Value) Then

                                            CType(cCntrlPnl, DateTimePicker).Value = CDate(Value)
                                            CType(cCntrlPnl, DateTimePicker).Checked = True
                                        Else
                                            CType(cCntrlPnl, DateTimePicker).Checked = False
                                        End If
                                    End If

                                    If setReadOnly.ToUpper() = "TRUE" Then
                                        cCntrlPnl.Enabled = False
                                    Else
                                        cCntrlPnl.Enabled = True


                                    End If
                                    bValSet = True

                                    Exit For


                                End If

                                'If the field is a range domain
                            ElseIf TypeOf cCntrlPnl Is NumericUpDown Then
                                'Get the field
                                strFld = CType(cCntrlPnl, NumericUpDown).Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                End If
                                If Field = strFld Then
                                    If setReadOnly.ToUpper() = "TRUE" Then
                                        cCntrlPnl.Enabled = False
                                    Else
                                        cCntrlPnl.Enabled = True


                                    End If
                                    If Value Is Nothing Then
                                    ElseIf Value Is DBNull.Value Then
                                    ElseIf Value.ToString() = "<NO_MOD>" Then

                                    Else
                                        Try
                                            If GlobalsFunctions.IsDouble(Value) Then

                                                CType(cCntrlPnl, NumericUpDown).Value = CDec(Value)
                                            End If

                                        Catch ex As Exception

                                        End Try

                                    End If
                                    bValSet = True

                                    Exit For

                                End If




                            End If
                        Next

                    End If

                Next
                ' End If
            Next

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
        m_SettingAtts = False


    End Sub
    Public Function UpdateField(ByVal FieldInfo As String, ByVal Value As Object, ByVal pushChangeToForm As Boolean, Optional ByVal setReadOnly As String = "False") As Boolean
        'Sub to load a record to the editor
        Dim strFldInfo() As String = FieldInfo.Split("|")
        Dim Field As String = strFldInfo(0)

        Try

            If m_FDR.Table.Columns(Field) Is Nothing Then
                Return False

            End If
            Try


                If Value Is Nothing Then

                    m_FDR.Item(Field) = DBNull.Value
                ElseIf Value Is DBNull.Value Then

                    m_FDR.Item(Field) = DBNull.Value

                ElseIf Value.ToString = "<NO_MOD>" Then
                Else

                    If m_FDR.Table.Columns(Field).DataType Is System.Type.GetType("System.String") Then
                        m_FDR.Item(Field) = Value.ToString()
                    ElseIf m_FDR.Table.Columns(Field).DataType Is System.Type.GetType("System.Byte[]") Then


                        Try


                            m_FDR.Item(Field) = ConvertImageToByteArray(New Bitmap(Value.ToString()), GetImageFormat(Value))
                        Catch ex As Exception
                            Return False

                        End Try
                    ElseIf m_FDR.Table.Columns(Field).DataType.FullName = "System.Drawing.Bitmap" Then


                        Try


                            m_FDR.Item(Field) = New Bitmap(Value.ToString())

                        Catch ex As Exception
                            Return False

                            'MsgBox("Error trying to save attachment.  Only images are supported at the moment.")

                        End Try
                    Else
                        If m_FDR.Item(Field).ToString() <> Value.ToString() Then
                            Try
                                If m_FDR.Table.Columns(Field).DataType Is System.Type.GetType("System.String") Then
                                    m_FDR.Item(Field) = Value.ToString
                                ElseIf m_FDR.Table.Columns(Field).DataType Is System.Type.GetType("System.Double") Then
                                    m_FDR.Item(Field) = CDbl(Value)
                                ElseIf m_FDR.Table.Columns(Field).DataType Is System.Type.GetType("System.Single") Then

                                    m_FDR.Item(Field) = CSng(Value)

                                Else

                                    m_FDR.Item(Field) = Value

                                End If

                            Catch ex As Exception
                                Return False

                            End Try

                        End If


                    End If

                End If

            Catch ex As Exception
                'Capture error trying to set value
                Return False

            End Try
        Catch
        Finally
        End Try

        If pushChangeToForm Then
            Dim bSub As Boolean = False

            If m_FDR.FeatureSource.HasSubtypes Then
                If m_FDR.FeatureSource.SubtypeColumnName = Field Then
                    If Value Is Nothing Then
                    ElseIf Value Is DBNull.Value Then
                    ElseIf Value.ToString <> "<NO_MOD>" Then

                        bSub = True


                    End If
                End If
            End If
            UpdateForm(Field, Value, setReadOnly, bSub)

        End If
        Return True

    End Function
#End Region
#Region "Private Methods"

    Private Function SnapToGeo() As FeatureDataRow

        Dim pFSwD As FeatureSourceWithDef = Nothing
        Dim pQFilt As QueryFilter = Nothing
        Dim pCoord As Coordinate = Nothing
        Dim pDt As FeatureDataTable = Nothing
        Try
            If m_FDR.StoredEditSate = EditState.NotDefined Then


                If m_FDR.Geometry IsNot Nothing Then
                    If m_FDR.Geometry.GeometryType = GeometryType.Point Then

                        If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Point).Parts.Count = 1 Then
                            If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Point).Parts(0).Count = 1 Then
                                If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Point).Parts(0)(0).IsEmpty = False Then
                                    pCoord = CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Point).Parts(0)(0)
                                End If

                            End If
                        End If
                    ElseIf m_FDR.Geometry.GeometryType = GeometryType.Polygon Then
                        If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polygon).PartCount = 1 Then
                            If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polygon).Parts(0).Count = 1 Then
                                If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polygon).Parts(0)(0).IsEmpty = False Then
                                    pCoord = CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polygon).Parts(0)(0)
                                End If

                            End If
                        End If
                    ElseIf m_FDR.Geometry.GeometryType = GeometryType.Polyline Then
                        If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polyline).PartCount = 1 Then
                            If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polyline).Parts(0).Count = 1 Then
                                If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polyline).Parts(0)(0).IsEmpty = False Then
                                    pCoord = CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polyline).Parts(0)(0)
                                End If

                            End If
                        End If
                    End If

                    If m_EditOptions IsNot Nothing And pCoord IsNot Nothing Then
                        If m_EditOptions.SnapTo IsNot Nothing Then
                            If m_EditOptions.SnapTo <> "" Then
                                pFSwD = GlobalsFunctions.GetFeatureSource(m_EditOptions.SnapTo, m_Map)
                                If pFSwD.FeatureSource IsNot Nothing Then

                                    'Create a new query filter to find the asset being inspected
                                    pQFilt = New QueryFilter
                                    'Create a new envelope to search by
                                    Dim intBufferValueforPoint As Double
                                    intBufferValueforPoint = GlobalsFunctions.bufferToMap(m_Map, GlobalsFunctions.appConfig.EditControlOptions.SnapTolerence) 'maptobuffer()
                                    Dim pEnv As New Geometries.Envelope(pCoord, intBufferValueforPoint, intBufferValueforPoint)
                                    'Dim pEnv As New Geometries.Envelope(coord, m_SearchDistance, m_SearchDistance)
                                    'Set the enveloper to the query
                                    pQFilt.Geometry = pEnv
                                    pQFilt.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect
                                    pDt = pFSwD.FeatureSource.GetDataTable(pQFilt)
                                    If pDt.Rows.Count > 0 Then

                                        If pFSwD.FeatureSource.GeometryType = CurrentLayer.GeometryType Then
                                            m_FDR.Geometry = CType(pDt.Rows(0), FeatureDataRow).Geometry
                                        ElseIf CurrentLayer.GeometryType = GeometryType.Point Then
                                            If pFSwD.FeatureSource.GeometryType = Geometries.GeometryType.Polyline Then
                                                'Of a line, find the nearest point on the line from the user click
                                                Dim pLine As Geometries.Polyline = CType(CType(pDt.Rows(0), FeatureDataRow).Geometry, Polyline)
                                                Dim pRetPoint As Geometries.Coordinate = New Geometries.Coordinate
                                                Dim pPartIdx As Integer
                                                Dim pVertIdx As Integer
                                                Dim pSqDist As Double
                                                pLine.GetNearestCoordinate(New Esri.ArcGIS.Mobile.Geometries.Point(pCoord), pRetPoint, pPartIdx, pVertIdx, pSqDist)

                                                If pRetPoint.IsEmpty Then
                                                    m_FDR.Geometry = New Esri.ArcGIS.Mobile.Geometries.Point(pCoord)
                                                Else
                                                    'Use the location on the line
                                                    '   pFDRInspection.Geometry = New ESRI.ArcGIS.Mobile.Geometries.Point(pRetPoint)
                                                    m_FDR.Geometry = New Esri.ArcGIS.Mobile.Geometries.Point(pRetPoint)
                                                End If
                                                pLine = Nothing
                                                pRetPoint = Nothing

                                            Else
                                                'Use the mouse click for other geometry types
                                                'pFDRInspection.Geometry = New ESRI.ArcGIS.Mobile.Geometries.Point(coord)
                                                m_FDR.Geometry = New Esri.ArcGIS.Mobile.Geometries.Point(pCoord)
                                            End If
                                        Else
                                            m_FDR.Geometry = New Esri.ArcGIS.Mobile.Geometries.Point(pCoord)
                                        End If
                                        RaiseEvent RecordSnapped(m_FDR.Geometry)
                                        Return CType(pDt.Rows(0), FeatureDataRow)

                                    End If


                                End If
                            End If
                        End If
                    End If
                End If
            End If
            Return Nothing
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return Nothing

        End Try
    End Function


    Private Sub LoadAutoAttributes(ByVal featDataRow As FeatureDataRow)
        Dim pDRead As FeatureDataReader = Nothing
        Dim pQFilt As QueryFilter = Nothing
        Dim pFl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = Nothing
        Try



            If m_FDR.StoredEditSate = EditState.NotDefined Or 1 = 1 Then
                If m_EditOptions IsNot Nothing Then
                    If m_EditOptions.Field IsNot Nothing Then
                        For Each autoAttFld As MobileConfigClass.MobileConfigMobileMapConfigEditControlOptionsLayersLayerField In m_EditOptions.Field
                            Dim act As String = ""



                            Dim setRead As String = ""


                            If m_FDR.StoredEditSate = EditState.NotDefined Then
                                act = autoAttFld.OnNew

                                setRead = autoAttFld.OnNewReadOnly


                            Else
                                act = autoAttFld.OnModify
                                setRead = autoAttFld.OnModifyReadOnly
                            End If


                            getGPSCoords()
                            Select Case act.ToUpper
                                Case "Longitude".ToUpper
                                    If m_GPSVal IsNot Nothing Then
                                        UpdateField(autoAttFld.Name, m_GPSVal.Longitude, True, setRead)

                                    End If
                                Case "Latitude".ToUpper
                                    If m_GPSVal IsNot Nothing Then
                                        UpdateField(autoAttFld.Name, m_GPSVal.Latitude, True, setRead)

                                    End If
                                Case "PDOP".ToUpper
                                    If m_GPSVal IsNot Nothing Then
                                        UpdateField(autoAttFld.Name, m_GPSVal.PositionDilutionOfPrecision, True, setRead)

                                    End If

                                Case "HDOP".ToUpper
                                    If m_GPSVal IsNot Nothing Then
                                        UpdateField(autoAttFld.Name, m_GPSVal.HorizontalDilutionOfPrecision, True, setRead)

                                    End If

                                Case "VDOP".ToUpper
                                    If m_GPSVal IsNot Nothing Then
                                        UpdateField(autoAttFld.Name, m_GPSVal.VerticalDilutionOfPrecision, True, setRead)

                                    End If

                                Case "SatCount".ToUpper
                                    If m_GPSVal IsNot Nothing Then
                                        UpdateField(autoAttFld.Name, m_GPSVal.FixSatelliteCount, True, setRead)

                                    End If

                                Case "FixStatus".ToUpper
                                    If m_GPSVal IsNot Nothing Then
                                        UpdateField(autoAttFld.Name, m_GPSVal.FixStatus, True, setRead)

                                    End If


                                Case "TIME".ToUpper
                                    UpdateField(autoAttFld.Name, CStr(Now.ToShortTimeString), True, setRead)
                                Case "Date".ToUpper
                                    UpdateField(autoAttFld.Name, DateTime.Today.ToString("MM-dd-yyyy"), True, setRead)
                                Case "CPUNAME".ToUpper
                                    UpdateField(autoAttFld.Name, Environment.MachineName.ToString(), True, setRead)
                                Case "DateTime".ToUpper

                                    UpdateField(autoAttFld.Name, Now.ToString(), True, setRead)

                                Case "User".ToUpper
                                    UpdateField(autoAttFld.Name, Environment.UserName, True, setRead)
                                Case "FullUser".ToUpper
                                    UpdateField(autoAttFld.Name, Environment.UserDomainName & "\\" & Environment.UserName, True, setRead)
                                Case "Value".ToUpper
                                    UpdateField(autoAttFld.Name, autoAttFld.ValueText, True, setRead)
                                Case "WOCrew".ToUpper
                                    RaiseEvent GetWorkorder()
                                    If currentCrew <> "" Then
                                        UpdateField(autoAttFld.Name, currentCrew, True, setRead)
                                    Else
                                        UpdateField(autoAttFld.Name, currentCrew, True, "FALSE")
                                    End If
                                    currentCrew = ""

                                Case "GUID".ToUpper

                                    UpdateField(autoAttFld.Name, System.Guid.NewGuid.ToString(), True, setRead)
                                Case "ID".ToUpper
                                    Dim pathToEXE As String = GlobalsFunctions.getEXEPath()
                                    Dim strID As String = ""
                                    If System.IO.Directory.Exists(pathToEXE) Then
                                        If pathToEXE <> "" Then
                                            pathToEXE = System.IO.Path.Combine(pathToEXE, "id.txt")
                                            If System.IO.File.Exists(pathToEXE) = False Then
                                                GlobalsFunctions.SaveTextToFile("1", pathToEXE)
                                                strID = "1"
                                            Else
                                                strID = GlobalsFunctions.GetFileContents(pathToEXE)
                                                GlobalsFunctions.SaveTextToFile(CInt(strID) + 1, pathToEXE)
                                            End If




                                            UpdateField(autoAttFld.Name, strID, True, setRead)
                                        End If
                                    End If

                                Case "WOCurrent".ToUpper
                                    RaiseEvent GetWorkorder()
                                    If currentWO <> "" Then
                                        UpdateField(autoAttFld.Name, currentWO, True, setRead)
                                    Else
                                        UpdateField(autoAttFld.Name, currentWO, True, "FALSE")
                                    End If
                                    currentWO = ""


                                Case "NONE"
                                    UpdateField(autoAttFld.Name, "<NO_MOD>", True, "FALSE")
                                Case "ADDRESS"
                                    If m_FDR.Geometry IsNot Nothing Then
                                        If m_FDR.Geometry.IsValid Then

                                            If GlobalsFunctions.validateStreet() Then



                                                Dim pCenterPoint As Esri.ArcGIS.Mobile.Geometries.Point = GlobalsFunctions.GetGeometryCenterPoint(m_FDR.Geometry)

                                                Dim pFDT As FeatureDataTable = GlobalsFunctions.spatialQFeature(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName, pCenterPoint, m_Map, GlobalsFunctions.appConfig.IDPanel.SearchTolerence)
                                                If pFDT IsNot Nothing Then
                                                    If pFDT.Rows IsNot Nothing Then
                                                        If pFDT.Rows.Count > 0 Then

                                                            Dim pFDR As FeatureDataRow = GlobalsFunctions.getClosest(pFDT, pCenterPoint) 'CType(pFDT.Rows(0), FeatureDataRow)

                                                            If pFDR IsNot Nothing Then

                                                                Dim pStreetLine As Polyline = CType(pFDR.Geometry, Polyline)
                                                                Dim pRetCoord As Geometries.Coordinate = New Geometries.Coordinate
                                                                Dim pPartIdx As Integer = -1
                                                                Dim pSegIdx As Integer = -1
                                                                Dim pSqDist As Double = -1

                                                                If pStreetLine.GetNearestCoordinate(pCenterPoint, pRetCoord, pPartIdx, pSegIdx, pSqDist) Then
                                                                    Dim pRetVert As Geometries.Coordinate = New Geometries.Coordinate
                                                                    Dim pPartIdxVert As Integer = -1
                                                                    Dim pVertIdxVert As Integer = -1
                                                                    Dim pSqDistVert As Double = -1
                                                                    If pStreetLine.GetNearestVertex(pRetCoord, pRetVert, pPartIdxVert, pVertIdxVert, pSqDistVert) Then


                                                                        Dim pRetPnt As New Esri.ArcGIS.Mobile.Geometries.Point(pRetCoord)


                                                                        Dim pNewPrevCoordCol As New CoordinateCollection
                                                                        Dim pNewNextCoordCol As New CoordinateCollection
                                                                        Dim pNewPrevLine As Polyline
                                                                        Dim pNewNextLine As New Polyline

                                                                        If pVertIdxVert = 0 Then
                                                                            pNewNextCoordCol.Add(pStreetLine.GetPart(pPartIdxVert)(pVertIdxVert))
                                                                            pNewNextCoordCol.Add(pStreetLine.GetPart(pPartIdxVert)(pVertIdxVert + 1))
                                                                            pNewNextLine = New Polyline(pNewNextCoordCol)
                                                                            If pNewNextLine.Extent.Intersects(pRetPnt) Then
                                                                                pStreetLine.CurrentPartIndex = pPartIdx

                                                                                pStreetLine.CurrentCoordinateIndex = pVertIdxVert

                                                                                pStreetLine.InsertCoordinateAfter(pRetCoord)
                                                                            Else

                                                                            End If
                                                                        ElseIf pVertIdxVert = pStreetLine.GetPart(pPartIdxVert).Count - 1 Then
                                                                            pNewPrevCoordCol.Add(pStreetLine.GetPart(pPartIdxVert)(pVertIdxVert))

                                                                            pNewPrevCoordCol.Add(pStreetLine.GetPart(pPartIdxVert)(pVertIdxVert - 1))
                                                                            pNewPrevLine = New Polyline(pNewPrevCoordCol)
                                                                            'pNewPrevLine.Extent.Intersects()
                                                                            If pNewPrevLine.Extent.Intersects(pRetPnt) Then
                                                                                pStreetLine.CurrentPartIndex = pPartIdx

                                                                                pStreetLine.CurrentCoordinateIndex = pVertIdxVert

                                                                                pStreetLine.InsertCoordinate(pRetCoord)
                                                                            Else


                                                                            End If

                                                                        Else

                                                                            pNewPrevCoordCol.Add(pStreetLine.GetPart(pPartIdxVert)(pVertIdxVert))

                                                                            pNewPrevCoordCol.Add(pStreetLine.GetPart(pPartIdxVert)(pVertIdxVert - 1))
                                                                            pNewPrevLine = New Polyline(pNewPrevCoordCol)
                                                                            'pNewPrevLine.Extent.Intersects()
                                                                            If pNewPrevLine.Extent.Intersects(pRetPnt) Then
                                                                                pStreetLine.CurrentPartIndex = pPartIdx

                                                                                pStreetLine.CurrentCoordinateIndex = pVertIdxVert

                                                                                pStreetLine.InsertCoordinate(pRetCoord)
                                                                            Else

                                                                                pNewNextCoordCol.Add(pStreetLine.GetPart(pPartIdxVert)(pVertIdxVert))
                                                                                pNewNextCoordCol.Add(pStreetLine.GetPart(pPartIdxVert)(pVertIdxVert + 1))
                                                                                pNewNextLine = New Polyline(pNewNextCoordCol)
                                                                                If pNewNextLine.Extent.Intersects(pRetPnt) Then
                                                                                    pStreetLine.CurrentPartIndex = pPartIdx

                                                                                    pStreetLine.CurrentCoordinateIndex = pVertIdxVert

                                                                                    pStreetLine.InsertCoordinateAfter(pRetCoord)
                                                                                End If
                                                                            End If

                                                                        End If




                                                                        Dim pCoordColl As CoordinateCollection = New CoordinateCollection
                                                                        Dim bAtLoc As Boolean = False

                                                                        For Each part In pStreetLine.Parts
                                                                            For Each coord In part

                                                                                pCoordColl.Add(coord)
                                                                                If coord.X = pRetCoord.X And coord.Y = pRetCoord.Y Then
                                                                                    bAtLoc = True
                                                                                    Exit For
                                                                                End If
                                                                            Next
                                                                            If bAtLoc Then
                                                                                Exit For
                                                                            End If

                                                                        Next
                                                                        If bAtLoc Then
                                                                            Dim perAlong As Double = 0
                                                                            pStreetLine = CType(pFDR.Geometry, Polyline)

                                                                            If pCoordColl.Count > 1 Then


                                                                                Dim pNewPoly As Polyline = New Polyline(pCoordColl)
                                                                                Dim pSplitLen As Double = pNewPoly.Length

                                                                                Dim pLen As Double = pStreetLine.Length
                                                                                perAlong = (pSplitLen / pLen)
                                                                            End If
                                                                            Dim pStreetName As String = pFDR(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.NameField)


                                                                            Dim AddressFieldLeftFrom As String = pFDR(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerLeft)
                                                                            Dim AddressFieldLeftTo = pFDR(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperLeft)
                                                                            Dim AddressFieldRightFrom = pFDR(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerRight)
                                                                            Dim AddressFieldRightTo = pFDR(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperRight)


                                                                            Dim intLeft As Integer = -99
                                                                            Try
                                                                                intLeft = AddressFieldLeftFrom + (CInt((AddressFieldLeftTo - AddressFieldLeftFrom) * perAlong))
                                                                            Catch ex As Exception

                                                                            End Try


                                                                            Dim intRight As Integer = -99
                                                                            Try


                                                                                intRight = AddressFieldRightFrom + (CInt((AddressFieldRightTo - AddressFieldRightFrom) * perAlong))
                                                                            Catch ex As Exception

                                                                            End Try

                                                                            If intLeft > 0 Then
                                                                                pStreetName = intLeft & " " & pStreetName
                                                                            ElseIf intRight > 0 Then
                                                                                pStreetName = intRight & " " & pStreetName
                                                                            Else


                                                                            End If
                                                                            UpdateField(autoAttFld.Name, pStreetName, True, setRead)
                                                                        End If
                                                                    End If
                                                                End If
                                                            End If

                                                            pFDR = Nothing

                                                        End If
                                                    End If
                                                End If
                                                pFDT = Nothing
                                                pCenterPoint = Nothing
                                            End If
                                        End If
                                    End If

                                Case ""
                                    UpdateField(autoAttFld.Name, "<NO_MOD>", True, setRead)
                                Case "Intersect".ToUpper
                                    If m_FDR.Geometry IsNot Nothing Then
                                        If m_FDR.Geometry.IsValid Then

                                            If featDataRow IsNot Nothing Then

                                                If GlobalsFunctions.LayerNameMatches(featDataRow.FeatureSource, autoAttFld.IntersectLayer) Then
                                                    If autoAttFld.IntersectField = "[Name]" Then
                                                        UpdateField(autoAttFld.Name, featDataRow.FeatureSource.Name, True, setRead)
                                                        Continue For
                                                    ElseIf autoAttFld.IntersectField = "[FCName]" Then
                                                        UpdateField(autoAttFld.Name, featDataRow.FeatureSource.ServerFeatureClassName, True, setRead)
                                                        Continue For

                                                    ElseIf featDataRow.FeatureSource.Columns(autoAttFld.IntersectField) IsNot Nothing Then
                                                        If m_FDR.FeatureSource.Columns(autoAttFld.Name) IsNot Nothing Then
                                                            If featDataRow(autoAttFld.IntersectField) Is Nothing Then

                                                                UpdateField(autoAttFld.Name, DBNull.Value, True, setRead)
                                                                Continue For

                                                            ElseIf featDataRow(autoAttFld.IntersectField) Is DBNull.Value Then

                                                                UpdateField(autoAttFld.Name, DBNull.Value, True, setRead)
                                                                Continue For


                                                            Else
                                                                UpdateField(autoAttFld.Name, featDataRow(autoAttFld.IntersectField).ToString(), True, setRead)
                                                                Continue For


                                                            End If

                                                        End If

                                                    End If
                                                End If



                                            End If


                                            If pFl Is Nothing Then
                                                pFl = GlobalsFunctions.GetFeatureSource(autoAttFld.IntersectLayer, m_Map).FeatureSource
                                            Else
                                                If pFl.Name <> autoAttFld.IntersectLayer Then
                                                    pFl = GlobalsFunctions.GetFeatureSource(autoAttFld.IntersectLayer, m_Map).FeatureSource

                                                End If

                                            End If
                                            If pFl Is Nothing Then Continue For
                                            'Create a new query filter to find the asset being inspected
                                            If autoAttFld.IntersectField = "[Name]" Then
                                                UpdateField(autoAttFld.Name, pFl.Name, True, setRead)
                                                Continue For
                                            ElseIf autoAttFld.IntersectField = "[FCName]" Then
                                                UpdateField(autoAttFld.Name, pFl.ServerFeatureClassName, True, setRead)
                                                Continue For

                                            Else


                                                pQFilt = New QueryFilter
                                                'Create a new envelope to search by
                                                If m_FDR.Geometry.GeometryType = GeometryType.Point Then
                                                    Dim intBufferValueforPoint As Double
                                                    intBufferValueforPoint = GlobalsFunctions.bufferToMap(m_Map, GlobalsFunctions.appConfig.EditControlOptions.SnapTolerence) 'maptobuffer()
                                                    Dim pEnv As New Geometries.Envelope(CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Point).Coordinate, intBufferValueforPoint, intBufferValueforPoint)
                                                    pQFilt.Geometry = pEnv

                                                Else
                                                    pQFilt.Geometry = m_FDR.Geometry
                                                End If


                                                pQFilt.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect
                                                If pFl.GetFeatureCount(pQFilt) > 0 Then

                                                    pDRead = pFl.GetDataReader(pQFilt)
                                                    If pDRead.ReadFirst Then

                                                        If pFl.Columns(autoAttFld.IntersectField) IsNot Nothing Then
                                                            If m_FDR.FeatureSource.Columns(autoAttFld.Name) IsNot Nothing Then
                                                                If pDRead(autoAttFld.IntersectField) IsNot Nothing Then

                                                                    UpdateField(autoAttFld.Name, pDRead(autoAttFld.IntersectField).ToString(), True, setRead)
                                                                    Continue For




                                                                End If

                                                            End If

                                                        End If



                                                    End If
                                                    If pDRead.IsClosed = False Then
                                                        pDRead.Close()
                                                    End If



                                                Else
                                                    UpdateField(autoAttFld.Name, "", True, "FALSE")
                                                    ' SetReadOnly(spatIntFld.Name, False)

                                                End If
                                            End If
                                        End If

                                    Else
                                        UpdateField(autoAttFld.Name, "<NO_MOD>", True, "FALSE")
                                    End If

                                Case Else
                                    UpdateField(autoAttFld.Name, "<NO_MOD>", True, setRead)
                            End Select




                        Next

                    End If


                End If
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        Finally
            If pDRead IsNot Nothing Then
                If pDRead.IsClosed = False Then
                    pDRead.Close()
                End If

            End If
            pDRead = Nothing
            pQFilt = Nothing
            pFl = Nothing
        End Try
    End Sub
    Private Sub setRequiredColorsField(ByVal fieldInfo As String, ByVal RequiredColor As Boolean)
        Try
            Dim strArr() As String = fieldInfo.Split("|")

            Dim fieldName As String = strArr(0)



            For Each tbPage As TabPage In tbCntrlEdit.TabPages
                Dim pPnl() As Control = tbPage.Controls.Find("pnl" & fieldName, False)

                If pPnl IsNot Nothing Then
                    If pPnl.Count > 0 Then
                        setRequiredColorsField(pPnl(0), RequiredColor)

                    End If
                End If



            Next
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub setRequiredColorsField(ByVal inCnt As Control, ByVal RequiredColor As Boolean)
        Try
            Dim pEditFldOpt As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption

            If inCnt.Tag IsNot Nothing Then
                If TypeOf (inCnt.Tag) Is MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption Then

                    pEditFldOpt = inCnt.Tag

                End If
            End If
            If RequiredColor Then
                If m_EditOptions IsNot Nothing Then
                    If m_EditOptions.RequiredBoxColor <> "" Then
                        inCnt.BackColor = GlobalsFunctions.stringToColor(m_EditOptions.RequiredBoxColor)

                    End If
                End If

            Else
                If pEditFldOpt IsNot Nothing Then
                    If pEditFldOpt.BoxColor <> "" Then
                        If m_FDR.StoredEditSate <> EditState.NotDefined And m_ExistingColor <> Color.Empty Then
                            inCnt.BackColor = m_ExistingColor
                        Else
                            inCnt.BackColor = GlobalsFunctions.stringToColor(pEditFldOpt.BoxColor)
                        End If


                    End If
                Else
                    If m_FDR.StoredEditSate <> EditState.NotDefined And m_ExistingColor <> Color.Empty Then
                        inCnt.BackColor = m_ExistingColor

                    End If


                End If

            End If

            For Each cnt As Control In inCnt.Controls
                If TypeOf (cnt) Is Label Then
                    If RequiredColor Then
                        If m_EditOptions IsNot Nothing Then


                            If m_EditOptions.RequiredBackColor <> "" Then


                                cnt.BackColor = GlobalsFunctions.stringToColor(m_EditOptions.RequiredBackColor)

                            End If

                            If m_EditOptions.RequiredForeColor <> "" Then
                                cnt.ForeColor = GlobalsFunctions.stringToColor(m_EditOptions.RequiredForeColor)

                            End If
                        End If

                    Else
                        If pEditFldOpt IsNot Nothing Then
                            If pEditFldOpt.BackColor <> "" Then

                                If m_FDR.StoredEditSate <> EditState.NotDefined And m_ExistingColor <> Color.Empty Then
                                    cnt.BackColor = m_ExistingColor
                                Else
                                    cnt.BackColor = GlobalsFunctions.stringToColor(pEditFldOpt.BackColor)
                                End If


                            End If

                        End If
                        If pEditFldOpt IsNot Nothing Then
                            If pEditFldOpt.ForeColor <> "" Then

                                cnt.ForeColor = GlobalsFunctions.stringToColor(pEditFldOpt.ForeColor)

                            End If

                        End If

                    End If

                End If
            Next
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        Finally
        End Try

    End Sub

    Public Sub disableSaveBtn()
        Try




            'Loop through all tab pages looking for the save button
            Dim bDisable As Boolean = False
            'Determine whether to disable or enable the save button
            If GlobalsFunctions.m_GPS.GpsConnection.IsOpen = False And GlobalsFunctions.appConfig.EditControlOptions.RequireGPSForSaving.ToUpper() = "TRUE" Then
                bDisable = True
            End If
            If m_FDR Is Nothing Then
                bDisable = True
            Else
                If m_FDR.Geometry Is Nothing Then
                    bDisable = True

                ElseIf m_FDR.Geometry.IsEmpty Then
                    bDisable = True

                End If
                lstBoxError.Items.Clear()

                If m_LayerOp IsNot Nothing Then
                    For Each itm In m_LayerOp.Field
                        If itm.Required.ToUpper = "TRUE" Then
                            If (m_FDR(itm.Name) Is Nothing Or m_FDR(itm.Name) Is DBNull.Value Or m_FDR(itm.Name).ToString = "") And GetFieldControlVisibleState(itm.Name) Then
                                Dim cap As String
                                If itm.Caption = "" Then
                                    cap = m_FDR.Table.Columns(itm.Name).Caption
                                Else
                                    cap = itm.Caption
                                End If
                                lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, cap, GlobalsFunctions.appConfig.EditControlOptions.UIComponents.RequireFieldMessage))
                                bDisable = True
                            End If
                        End If
                    Next
                End If
                If m_FDR.HasErrors Then
                    'If spCntMain.Panel1Collapsed = True Then
                    '    spCntMain.Panel1Collapsed = False
                    '    ShuffleControls()
                    'End If

                    bDisable = True

                    Dim colInError() As DataColumn = m_FDR.GetColumnsInError
                    For i As Integer = 0 To colInError.GetLength(0) - 1
                        'If colInError(i).Caption = "SHAPE" And m_FDR.Geometry Is Nothing Then
                        'ElseIf colInError(i).Caption = "SHAPE" And m_FDR.Geometry.IsValid Then
                        'Else
                        If m_EditOptions IsNot Nothing Then


                            If m_EditOptions.RequiredBackColor = "" And _
                                m_EditOptions.RequiredBoxColor = "" And _
                                m_EditOptions.RequiredForeColor = "" Then
                            Else
                            End If

                            setRequiredColorsField(colInError(i).ColumnName, True)
                        End If
                        If colInError(i).ColumnName = m_FDR.FeatureSource.GeometryColumnName Then
                            lstBoxError.Items.Add(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NoGeometryMessage)

                        Else
                            lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, colInError(i).Caption, m_FDR.GetColumnError(colInError(i).ColumnName)))
                        End If



                        '  End If
                        'MsgBox(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, colInError(i).Caption, m_FDR.GetColumnError(colInError(i).ColumnName)))



                    Next


                End If

                'If spCntMain.Panel1Collapsed = False Then
                '    spCntMain.Panel1Collapsed = True
                '    ShuffleControls()
                'End If
                If bDisable = False Then
                    lstBoxError.Items.Clear()

                End If

            End If

            btnSave.Enabled = Not bDisable
            If bDisable Then
                btnSave.BackgroundImage = My.Resources.SaveGray
            Else
                btnSave.BackgroundImage = My.Resources.SaveGreen
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub disableDeleteBtn()
        Try



            If btnDelete.Visible = False Then Return

            'Loop through all tab pages looking for the save button
            Dim bDisable As Boolean = False
            'Determine whether to disable or enable the save button
            If m_FDR Is Nothing Then
                bDisable = True
            Else
                If m_FDR.StoredEditSate = EditState.NotDefined Then
                    bDisable = True


                Else
                    bDisable = False

                End If
            End If

            btnDelete.Enabled = Not bDisable
            If bDisable Then
                btnDelete.BackgroundImage = My.Resources.DeleteGray
            Else
                btnDelete.BackgroundImage = My.Resources.DeleteGreen
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub hideMoveButton()
        Try

            'Determine whether to disable or enable the save button
            If m_FDR Is Nothing Then
                btnMove.Visible = False
            Else
                If m_FDR.StoredEditSate = EditState.NotDefined Then
                    btnMove.Visible = False

                Else
                    btnMove.Visible = False

                End If
            End If

          

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub DisplayBlank()
        Try

            'Clear out the controls from the container
            m_FDR = Nothing
            tbCntrlEdit.TabPages.Clear()

            tbCntrlEdit.Controls.Clear()
            tbCntrlEdit.TabPages.Add(New TabPage(""))
            m_Map.Invalidate()

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub
    Private Sub ShuffleControls()
        m_shuffling = True

        Dim pTbPageCo() As TabPage = Nothing

        Dim pCurTabPage As TabPage = Nothing
        Dim pAttTabPage As TabPage = Nothing
        Try

            tbCntrlEdit.SuspendLayout()

            Dim intform As Integer
            If tbCntrlEdit.Parent.Parent.Height > tbCntrlEdit.Height Then
                intform = tbCntrlEdit.Parent.Parent.Height
            Else
                intform = tbCntrlEdit.Height
            End If
            'If m_EditBtns Is Nothing Then
            '    m_EditBtns = New List(Of Button)
            'Else
            '    m_EditBtns.Clear()

            'End If
            'If m_SaveBtns Is Nothing Then
            '    m_SaveBtns = New List(Of Button)
            'Else
            '    m_SaveBtns.Clear()

            'End If
            'If m_ClearBtns Is Nothing Then
            '    m_ClearBtns = New List(Of Button)
            'Else
            '    m_ClearBtns.Clear()

            'End If
            'Spacing between last control and the bottom of the page
            Dim pBottomPadding As Integer = 70
            If spCntMain.Panel1Collapsed = False Then
                pBottomPadding = pBottomPadding + spCntMain.Panel1.Height

            End If
            'Padding for the left of each control
            Dim pLeftPadding As Integer = 10
            'Spacing between firstcontrol and the top
            Dim pTopPadding As Integer = 3
            'Padding for the right of each control
            Dim pRightPadding As Integer = 15

            If tbCntrlEdit Is Nothing Then Return
            Dim pCurTabIdx As Integer = tbCntrlEdit.SelectedIndex
            pCurTabPage = New TabPage


            Dim strName As String
            If m_FL.Name.Length > 30 Then
                strName = m_FL.Name.Substring(0, 29) & ".."
            Else
                strName = m_FL.Name & ":"
            End If
            pCurTabPage.Name = strName
            pCurTabPage.Text = strName
            Dim pCntlNextTop As Integer = pTopPadding
            Dim pQFilt As QueryFilter = Nothing

            For Each tb As TabPage In tbCntrlEdit.TabPages
                If tb.Tag IsNot Nothing Then
                    If tb.Tag.ToString() = "Attachment" Then
                        pAttTabPage = tb
                    End If

                Else

                    If tb.Controls IsNot Nothing Then
                        While True
                            If tb.Controls.Count = 0 Then Exit While
                            Dim cnt As Control = tb.Controls(0)

                            If TypeOf cnt Is Button Then
                                tb.Controls.Remove(cnt)

                            Else


                                cnt.Top = pCntlNextTop
                                If (cnt.Top + cnt.Height) > intform - pBottomPadding Then




                                    If pTbPageCo Is Nothing Then
                                        ReDim Preserve pTbPageCo(0)
                                    Else
                                        ReDim Preserve pTbPageCo(pTbPageCo.Length)
                                    End If
                                    pTbPageCo(pTbPageCo.Length - 1) = pCurTabPage
                                    pCurTabPage = New TabPage

                                    If m_FL.Name.Length > 30 Then
                                        strName = m_FL.Name.Substring(0, 29) & ".."
                                    Else
                                        strName = m_FL.Name & ":"
                                    End If
                                    pCurTabPage.Name = strName
                                    pCurTabPage.Text = strName

                                    pCntlNextTop = pTopPadding
                                    'pBtn = Nothing

                                    cnt.Top = pCntlNextTop
                                End If
                                cnt.Width = tbCntrlEdit.Width
                                'Dim valsMatch As String = "NoEntry"
                                'Dim visToSet As Boolean = True
                                Dim entryFound As Boolean = False

                                Dim controlVisible As Boolean = True
                                Dim modeMatchs As Boolean = True


                                If TypeOf cnt Is Panel Then
                                    If cnt.Tag IsNot Nothing Then
                                        If TypeOf (cnt.Tag) Is MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption Then
                                            Dim fieldDet As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption = cnt.Tag
                                            If fieldDet.FilterFields IsNot Nothing Then
                                                If fieldDet.FilterFields.Count > 0 Then


                                                    For Each filtVals As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOptionFilterFields In fieldDet.FilterFields
                                                        If m_FDR IsNot Nothing Then



                                                            If filtVals.Mode <> "" Then
                                                                If m_Mode = filtVals.Mode Then
                                                                    modeMatchs = True
                                                                Else
                                                                    modeMatchs = False
                                                                End If
                                                            Else
                                                                modeMatchs = True
                                                            End If
                                                            If modeMatchs Then

                                                                If filtVals.FilterInfo.Count = 0 Then
                                                                    Boolean.TryParse(filtVals.Visibility, controlVisible)
                                                                    Exit For

                                                                Else


                                                                    For Each filtVal As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOptionFilterFieldsFilterInfo In filtVals.FilterInfo
                                                                        If entryFound Then
                                                                            Exit For
                                                                        End If
                                                                        ' For Each filtInfo As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOptionFilterFieldsFilterInfo In filtVal.FilterInfo
                                                                        If m_FL.Columns(filtVal.FieldName) IsNot Nothing Then
                                                                            For Each strVl As String In filtVal.FieldValue
                                                                                If strVl.ToString().ToUpper() = "[ISNULL]" Then
                                                                                    If m_FDR IsNot Nothing Then
                                                                                        If m_FDR.Item(filtVal.FieldName) IsNot DBNull.Value Then
                                                                                            If m_FDR.Item(filtVal.FieldName).ToString().Trim() <> "" Then
                                                                                                entryFound = False

                                                                                            Else
                                                                                                Boolean.TryParse(filtVals.Visibility, controlVisible)
                                                                                                entryFound = True
                                                                                                Exit For
                                                                                            End If
                                                                                        Else
                                                                                            Boolean.TryParse(filtVals.Visibility, controlVisible)
                                                                                            entryFound = True
                                                                                            Exit For
                                                                                        End If
                                                                                    End If

                                                                                ElseIf strVl.ToString().ToUpper() = "[NOTNULL]" Then
                                                                                    If m_FDR IsNot Nothing Then


                                                                                        If m_FDR.Item(filtVal.FieldName) IsNot DBNull.Value Then

                                                                                            Boolean.TryParse(filtVals.Visibility, controlVisible)
                                                                                            entryFound = True
                                                                                            Exit For



                                                                                        Else
                                                                                            Boolean.TryParse(filtVals.Visibility, controlVisible)
                                                                                            controlVisible = Not controlVisible

                                                                                            entryFound = True
                                                                                            Exit For

                                                                                            ' Continue For
                                                                                        End If
                                                                                    End If

                                                                                Else
                                                                                    If m_FDR IsNot Nothing Then


                                                                                        If m_FDR.Item(filtVal.FieldName).ToString() = strVl Then
                                                                                            Boolean.TryParse(filtVals.Visibility, controlVisible)
                                                                                            entryFound = True
                                                                                            Exit For
                                                                                        Else
                                                                                            entryFound = False

                                                                                        End If

                                                                                    End If

                                                                                End If
                                                                            Next


                                                                        End If

                                                                    Next
                                                                    If entryFound = True Then
                                                                        Boolean.TryParse(filtVals.Visibility, controlVisible)
                                                                        Exit For
                                                                    Else
                                                                        controlVisible = True

                                                                    End If

                                                                End If
                                                            End If
                                                        End If


                                                    Next

                                                End If

                                            End If
                                        End If
                                    End If
                                    'If valsMatch Then
                                    '    cnt.Visible = False
                                    'Else
                                    '    cnt.Visible = True
                                    'End If

                                    For Each pnlCnt As Control In cnt.Controls


                                        'If TypeOf pnlCnt Is TextBox Then
                                        'Else
                                        If TypeOf pnlCnt Is Button Then
                                            Dim controls() As Control = CType(CType(pnlCnt, Button).Parent, Panel).Controls.Find("txtEdit" & pnlCnt.Tag.ToString, False)
                                            If controls.Length = 1 Then
                                                controls(0).Width = controls(0).Width - pnlCnt.Width - 5
                                                pnlCnt.Left = controls(0).Width + controls(0).Left + 5

                                            End If
                                        ElseIf TypeOf pnlCnt Is CustomPanel Then
                                            pnlCnt.Width = cnt.Width - pRightPadding - pLeftPadding
                                            If pnlCnt.Controls.Count = 2 Then
                                                pnlCnt.Controls(0).Left = pLeftPadding
                                                pnlCnt.Controls(1).Left = CInt((pnlCnt.Width / 2))
                                            End If

                                        Else
                                            pnlCnt.Width = tbCntrlEdit.Width - pLeftPadding - pRightPadding
                                        End If

                                        'End If


                                    Next

                                End If


                                cnt.Visible = controlVisible
                                If controlVisible Then
                                    pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding
                                End If
                                pCurTabPage.Controls.Add(cnt)


                                If pCntlNextTop >= intform - pBottomPadding Then

                                    If pCurTabPage IsNot Nothing Then





                                        If pTbPageCo Is Nothing Then
                                            ReDim Preserve pTbPageCo(0)
                                        Else
                                            ReDim Preserve pTbPageCo(pTbPageCo.Length)
                                        End If
                                        pTbPageCo(pTbPageCo.Length - 1) = pCurTabPage
                                        pCurTabPage = New TabPage

                                        If m_FL.Name.Length > 30 Then
                                            strName = m_FL.Name.Substring(0, 29) & ".."
                                        Else
                                            strName = m_FL.Name & ":"
                                        End If
                                        pCurTabPage.Name = strName
                                        pCurTabPage.Text = strName

                                        pCntlNextTop = pTopPadding
                                        'pBtn = Nothing
                                    End If
                                End If
                            End If

                        End While
                    End If
                End If

            Next



     
            If pCurTabPage IsNot Nothing Then


                If pCurTabPage.Controls.Count > 0 Then

                    If pTbPageCo Is Nothing Then
                        ReDim Preserve pTbPageCo(0)
                    Else
                        ReDim Preserve pTbPageCo(pTbPageCo.Length)
                    End If

                    pTbPageCo(pTbPageCo.Length - 1) = pCurTabPage
                Else

                End If
            End If

            If tbCntrlEdit.TabPages IsNot Nothing Then

                Try


                    tbCntrlEdit.TabPages.Clear()
                    If pTbPageCo IsNot Nothing Then

                        For Each tbp As TabPage In pTbPageCo

                            tbCntrlEdit.TabPages.Add(tbp)

                            tbp.Visible = True
                            If m_FDR IsNot Nothing Then
                                If m_FDR.StoredEditSate = EditState.NotDefined And m_Mode <> "ID" Then

                                    tbp.BackColor = m_NewColor

                                ElseIf m_Mode <> "ID" Then
                                    tbp.BackColor = m_ExistingColor

                                End If
                            End If

                            tbp.Update()
                        Next
                    End If
                    If pAttTabPage IsNot Nothing Then
                        tbCntrlEdit.TabPages.Add(pAttTabPage)

                    End If

                    If tbCntrlEdit.TabPages.Count >= pCurTabIdx Then
                        tbCntrlEdit.SelectedIndex = pCurTabIdx
                    Else
                        tbCntrlEdit.SelectedIndex = tbCntrlEdit.TabPages.Count - 1
                    End If
                    If m_FDR IsNot Nothing Then

                        If m_FDR.StoredEditSate <> EditState.NotDefined Then
                            If m_FDR.Geometry Is Nothing Then
                                btnMove.Enabled = False
                            ElseIf m_FDR.Geometry.GeometryType = GeometryType.Point Then
                                btnMove.Enabled = True
                            Else
                                btnMove.Enabled = False
                            End If

                        Else
                            btnMove.Enabled = False

                        End If
                    Else
                        btnMove.Enabled = False

                    End If
                    Call btnMove_EnabledChanged(Nothing, Nothing)
                Catch ex As Exception

                End Try
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        Finally
            tbCntrlEdit.ResumeLayout()
            tbCntrlEdit.Refresh()
            'Dim selTab As TabPage = tbCntrlEdit.SelectedTab
            'For Each tabpg As TabPage In tbCntrlEdit.TabPages
            '    tbCntrlEdit.SelectedTab = tabpg

            'Next
            'tbCntrlEdit.SelectedTab = selTab


            For Each tabpg As TabPage In tbCntrlEdit.TabPages
                tabpg.Show()


            Next
          
            pTbPageCo = Nothing
            pCurTabPage = Nothing
            m_shuffling = False
        End Try

    End Sub
    Private Sub AddControls()
        Try
            m_HasFilter = False

            If m_FL Is Nothing Then Return
            btnMove.CheckState = CheckState.Unchecked

            ''Feature layer being Identified
            Dim pfl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = m_FL
            ''Map Layer from Cache
            'Dim msMapLayer As MobileCacheMapLayer
            ''Set the active layer
            'msMapLayer = CType(m_Map.MapLayers(m_FL.Name), MobileCacheMapLayer)
            'If msMapLayer Is Nothing Then Return
            ''Get the FeatureSource from the cache layer
            'If msMapLayer.Layer Is Nothing Then Return
            'pfl = CType(msMapLayer.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
            ''Exit if the layer is not found
            'If pfl Is Nothing Then Exit Sub


            If m_AttControlBox IsNot Nothing Then
                m_AttControlBox.ClearGraphLayer()
            End If
            'Clear out the controls from the container
            tbCntrlEdit.TabPages.Clear()

            tbCntrlEdit.Controls.Clear()



            'Controls to display attributes
            Dim pTbPg As TabPage = Nothing
            Dim pTbPgPic As TabPage = Nothing
            Dim pTabAtt As TabPage = Nothing

            Dim pTxtBox As TextBox
            Dim pLbl As Label
            Dim pNumBox As NumericUpDown
            Dim pBtn As Button
            Dim pPic As PictureBox
            Dim pCBox As ComboBox
            Dim pRDButton As RadioButton

            Dim pDateTime As DateTimePicker
            'Spacing between each control
            Dim intCtrlSpace As Integer = 5
            'Spacing between a lable and a control
            Dim intLabelCtrlSpace As Integer = 0

            'Spacing between last control and the bottom of the page
            Dim pBottomPadding As Integer = 110
            'Padding for the left of each control
            Dim pLeftPadding As Integer = 10
            'Spacing between firstcontrol and the top
            Dim pTopPadding As Integer = 5
            'Padding for the right of each control
            Dim pRightPadding As Integer = 10
            'Location of next control
            Dim pNextControlTop As Integer = pTopPadding
            Dim pNextControlTopPic As Integer = pTopPadding
            'Set the width of each control
            Dim pControlWidth As Integer = Me.Width - 10
            'used for sizing text, only used when text is larger then display
            Dim g As Graphics
            Dim s As SizeF


            'Used to loop through FeatureSource
            Dim pDCs As ReadOnlyColumnCollection
            Dim pDc As DataColumn
            Dim pSubTypeDefValue As Integer = 0

            'Get the columns for hte layer
            pDCs = pfl.Columns

            If pfl.SubtypeColumnIndex >= 0 Then
                pSubTypeDefValue = CInt(pfl.Columns(pfl.SubtypeColumnIndex).DefaultValue)
            End If


            'Field Name
            Dim strfld As String
            'Field Alias
            Dim strAli As String

            'The current tab count
            Dim intPgIdx As Integer = 1
            Dim intPgIdxPic As Integer = 1

            Dim obj As Object


            Dim pDcArrListTemp As New ArrayList(pDCs.ToArray())
            m_LayerOp = Nothing
            Dim pDcArrList As New List(Of MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption)


            If GlobalsFunctions.appConfig.LayerOptions.LayersFieldOptions.Layer.Count > 0 Then
                For Each layFldOrd As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerName In GlobalsFunctions.appConfig.LayerOptions.LayersFieldOptions.Layer
                    If layFldOrd.Name = pfl.Name Then
                        m_LayerOp = layFldOrd
                        pDcArrList.Clear()

                        For Each fldsOrd As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption In layFldOrd.Field

                            ' For Each fldOrd As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption In fldsOrd.OrderField
                            'CType(pDcArrList.Item(iCount), DataColumn)
                            If pfl.Columns.Contains(fldsOrd.Name) Then
                                If Not pDcArrList.Contains(fldsOrd) Then
                                    pDcArrList.Add(fldsOrd)

                                End If



                            End If
                            'Next
                        Next
                        Exit For

                    End If
                Next
            End If
            If pDcArrList.Count = 0 Then


                For Each pDc In pDCs
                    Dim pTmp As New MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption
                    pTmp.Name = pDc.ColumnName
                    pTmp.VisibleDefault = "True"
                    pDcArrList.Add(pTmp)
                Next
            End If
            Dim pDcOpt As New MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption

            For Each pDcOpt In pDcArrList
                pDc = pfl.Columns(pDcOpt.Name)
                If pDc.ReadOnly Then
                    Continue For

                End If
                'For Each pDc In pDCs

                obj = Nothing

                'Get the field names
                strfld = pDc.ColumnName
                strAli = pDc.Caption
                If pDcOpt IsNot Nothing Then
                    If pDcOpt.Caption IsNot Nothing Then
                        If pDcOpt.Caption.Trim <> "" Then
                            strAli = pDcOpt.Caption
                        End If
                    End If
                End If
                'if currently on  tab page, check to see if a new page is needed
                If pTbPg IsNot Nothing Then
                    'check If the top of the next control will fit
                    If pNextControlTop > pTbPg.Height - pBottomPadding Then
                        'pBtn = New Button
                        'pBtn.Name = "btnSaveEdit"
                        'pBtn.Text = "Save"
                        'pBtn.Font = m_Fnt
                        'pBtn.Top = pNextControlTop
                        'pBtn.AutoSize = True

                        'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                        'pTbPg.Controls.Add(pBtn)
                        'pBtn.Left = CInt((pTbPg.Width / 2) - pBtn.Width - 10)

                        'pBtn = New Button
                        'pBtn.Name = "btnClearEdit"
                        'pBtn.Text = "Clear"
                        'pBtn.Font = m_Fnt
                        'pBtn.Top = pNextControlTop
                        'pBtn.AutoSize = True

                        'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                        'pTbPg.Controls.Add(pBtn)
                        'pBtn.Left = CInt((pTbPg.Width / 2) + 10)


                        'a new page is needed
                        pNextControlTop = pTopPadding
                        'Change the page count
                        intPgIdx = intPgIdx + 1
                        'Add a new page
                        Dim strName As String
                        If pfl.Name.Length > 30 Then
                            strName = pfl.Name.Substring(0, 29) & ".."
                        Else
                            strName = pfl.Name & ":"
                        End If
                        tbCntrlEdit.TabPages.Add(strName & intPgIdx, strName & intPgIdx)
                        pTbPg = tbCntrlEdit.TabPages(strName & intPgIdx)
                        pTbPg.Visible = True

                        'tbCntrlEdit.TabPages.Add(GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx, GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx)
                        'pTbPg = tbCntrlEdit.TabPages(GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx)

                    End If
                Else
                    Dim strName As String
                    If pfl.Name.Length > 30 Then
                        strName = pfl.Name.Substring(0, 29) & ".."
                    Else
                        strName = pfl.Name & ":"
                    End If
                    'Add the first tab page
                    tbCntrlEdit.TabPages.Add(strName & intPgIdx, strName & intPgIdx)
                    pTbPg = tbCntrlEdit.TabPages(strName & intPgIdx)
                    pTbPg.Visible = True

                    'tbCntrlEdit.TabPages.Add(GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx, GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx)
                    'pTbPg = tbCntrlEdit.TabPages(intPgIdx - 1)

                End If

                'Check the field types
                If pfl.GeometryColumnName = strfld Or pfl.FidColumnName = strfld Then
                ElseIf (((UCase(strfld) = UCase("shape.len") Or UCase(strfld) = UCase("shape.area")) And
                        m_Mode = "ID")) And GlobalsFunctions.appConfig.IDPanel.ShowLengthFieldInID.ToUpper = "False".ToUpper Then



                    'Reserved Columns
                ElseIf pfl.SubtypeColumnName = strfld And m_Mode <> "ID" Then
                    'Create a lable for the field name
                    pLbl = New Label
                    'Apply the field alias to the field name
                    pLbl.Text = strAli & " (Set This Value First)"
                    'Link the field to the name of the control
                    pLbl.Name = "lblEdit" & strfld

                    'Add the control at the determined locaiton

                    pLbl.Left = pLeftPadding

                    pLbl.Top = pNextControlTop
                    If pDcOpt.ForeColor <> "" Then
                        pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                    Else
                        pLbl.ForeColor = Color.Blue
                    End If
                    If pDcOpt.BackColor <> "" Then
                        pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                    End If



                    'Apply global font
                    pLbl.Font = m_FntLbl
                    'Create a graphics object to messure the text
                    g = pLbl.CreateGraphics
                    s = g.MeasureString(pLbl.Text, pLbl.Font)

                    pLbl.Height = CInt(s.Height)
                    'If the text is larger then the control, truncate the control
                    If s.Width >= Me.Width Then
                        pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                    Else 'Use autosize if it fits
                        pLbl.AutoSize = True
                    End If
                    'Determine the locaiton for the next control
                    ' pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace


                    Dim pCV As CodedValueDomain = pfl.Subtypes

                    If pCV.Rows.Count = 0 Then
                        'MsgBox("ERROR: There are no values set up in the domain: " & pCV.TableName & " in layer: " & pfl.Name & ".  This field will be skipped")
                    Else
                        If pCV.Rows.Count = 2 And m_RadioOnTwo Then
                            Dim pNewGpBox As New CustomPanel

                            pNewGpBox.Tag = strfld
                            'pNewGpBox.BorderStyle = Windows.Forms.BorderStyle.None
                            'pNewGpBox.BorderColor = Pens.Transparent

                            pNewGpBox.BackColor = Color.White
                            '  pNewGpBox.BorderColor = Pens.LightGray
                            If pDcOpt.BoxColor <> "" Then
                                pNewGpBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                            End If
                            pNewGpBox.Width = pTbPg.Width - pRightPadding - pLeftPadding
                            pNewGpBox.Top = pNextControlTop
                            pNewGpBox.Left = pLeftPadding

                            pRDButton = New RadioButton
                            pRDButton.FlatStyle = FlatStyle.Flat
                            pRDButton.FlatAppearance.BorderSize = 0
                            AddHandler pRDButton.CheckedChanged, AddressOf controlLeave

                            pRDButton.Name = "Rdo1Sub"
                            'If pDcOpt.ForeColor <> "" Then
                            '    pRDButton.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pRDButton.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If

                            pRDButton.Tag = pCV.Rows(0)("Code")
                            ' pRDButton.Text = pCV.Rows(0)(1).ToString
                            If (pCV.Rows(0)("Value").ToString.Length > 14) Then
                                pRDButton.Text = pCV.Rows(0)("Value").ToString.Substring(0, 14)

                            Else
                                pRDButton.Text = pCV.Rows(0)("Value").ToString

                            End If
                            pRDButton.Left = pLeftPadding

                            pRDButton.AutoSize = True
                            ' AddHandler pRDButton.Leave, AddressOf controlLeave
                            ' AddHandler pRDButton.Enter, AddressOf controlEntered
                            pNewGpBox.Controls.Add(pRDButton)


                            pNewGpBox.Height = pRDButton.Height + 12
                            pRDButton.Top = CInt(pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2)


                            pRDButton = New RadioButton
                            AddHandler pRDButton.CheckedChanged, AddressOf controlLeave
                            pRDButton.FlatStyle = FlatStyle.Flat
                            pRDButton.FlatAppearance.BorderSize = 0

                            pRDButton.Name = "Rdo2Sub"

                            pRDButton.Tag = pCV.Rows(1)("Code")
                            ' pRDButton.Text = pCV.Rows(1)("Value").ToString
                            If (pCV.Rows(1)("Value").ToString.Length > 14) Then
                                pRDButton.Text = pCV.Rows(1)("Value").ToString.Substring(0, 14)

                            Else
                                pRDButton.Text = pCV.Rows(1)("Value").ToString

                            End If
                            pRDButton.Left = CInt(pNewGpBox.Width / 2)


                            pRDButton.AutoSize = True
                            ' AddHandler pRDButton.Leave, AddressOf controlLeave
                            'AddHandler pRDButton.Enter, AddressOf controlEntered
                            'AddHandler pNewGpBox.Leave, AddressOf controlLeave
                            'AddHandler pNewGpBox.Enter, AddressOf controlEntered

                            pNewGpBox.Controls.Add(pRDButton)
                            pRDButton.Top = CInt(pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2)




                            Dim pPnl As Panel = New Panel

                            pPnl.Name = "pnl" & strfld

                            pPnl.Tag = pDcOpt
                            If pDcOpt.FilterFields IsNot Nothing Then
                                If pDcOpt.FilterFields.Count > 0 Then
                                    m_HasFilter = True
                                End If


                            End If
                            pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                            If pDcOpt.ForeColor <> "" Then
                                pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                            Else
                                pDcOpt.ForeColor = GlobalsFunctions.ColorToString(pLbl.ForeColor)


                            End If
                            If pDcOpt.BackColor <> "" Then
                                pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                            Else
                                pDcOpt.BackColor = GlobalsFunctions.ColorToString(pLbl.BackColor)
                            End If


                            If pDcOpt.BoxColor <> "" Then
                                pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)
                            Else
                                pDcOpt.BoxColor = GlobalsFunctions.ColorToString(pPnl.BackColor)
                            End If


                            'If pDcOpt.ForeColor <> "" Then

                            '    pRDButton.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pRDButton.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            pLbl.Top = 0
                            pNewGpBox.Top = 5 + pLbl.Height

                            pPnl.Width = pControlWidth
                            pPnl.Margin = New Padding(0)
                            pPnl.Padding = New Padding(0)





                            pPnl.Top = pNextControlTop
                            pPnl.Left = 0
                            pPnl.Height = pNewGpBox.Height + pLbl.Height + 10
                            pPnl.Controls.Add(pLbl)
                            pPnl.Controls.Add(pNewGpBox)

                            pTbPg.Controls.Add(pPnl)
                            pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace
                            pNewGpBox = Nothing
                            '  pPf = Nothing

                        Else
                            pCBox = New ComboBox
                            pCBox.Tag = strfld
                            pCBox.Name = "cboEdt" & strfld
                            pCBox.Left = pLeftPadding
                            pCBox.Top = pNextControlTop
                            pCBox.Width = pControlWidth - pLeftPadding - pRightPadding
                            pCBox.Height = pCBox.Height + 5
                            pCBox.DropDownStyle = ComboBoxStyle.DropDownList

                            'Orig Mike
                            'pCV.Columns(0).AllowDBNull = True
                            'pCV.Columns(1).AllowDBNull = True

                            'pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never

                            'pCBox.DataSource = pCV


                            '*****

                            'Orig from Marie
                            'Dim tempDomainValues As Dictionary(Of Object, String) = New Dictionary(Of Object, String)
                            'For Each tempRow As DataRow In pCV.Rows
                            '    tempDomainValues.Add(tempRow.Item("Code"), tempRow.Item("Value").ToString())
                            'Next

                            'If pDc.AllowDBNull Then
                            '    tempDomainValues.Add(DBNull.Value, GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown)
                            'End If

                            'pCBox.DataSource = New BindingSource(tempDomainValues, Nothing)
                            '******

                            pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never
                            'pCV.Columns(0).AllowDBNull = True
                            'pCV.Columns(1).AllowDBNull = True
                            'If pDc.AllowDBNull Or 1 = 1 Then
                            '    Dim pDT As DataTable

                            '    pDT = pCV.DefaultView.ToTable()
                            '    Dim pDR As DataRow = pDT.NewRow
                            '    pDR.Item(0) = DBNull.Value
                            '    pDR.Item(1) = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                            '    pDT.Rows.InsertAt(pDR, 0)

                            '    'pDT.Rows.Add(DBNull.Value, GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown)

                            '    For Each dr As DataRow In pDT.Rows

                            '        pCBox.Items.Add(New cValue(dr(1).ToString, dr(0)))

                            '    Next
                            '    pCBox.SelectedIndex = 0

                            '    pDR = Nothing
                            'Else
                            'pCBox.DataSource = pCV
                            ' For Each dr As DataRow In pCV.Rows

                            '    pCBox.Items.Add(New cValue(dr(1).ToString, dr(0)))

                            'Next

                            'End If
                            Dim pDT As DataTable

                            pDT = pCV.DefaultView.ToTable()


                            Dim dr As DataRow
                            Dim intSubIdx As Integer = -1

                            For h As Integer = 0 To pDT.Rows.Count - 1
                                dr = pDT.Rows(h)

                                If dr(0) = pSubTypeDefValue Then
                                    intSubIdx = h

                                End If
                                pCBox.Items.Add(New cValue(dr(1).ToString, dr(0)))

                            Next
                            pCBox.SelectedIndex = intSubIdx




                            'pCBox.DisplayMember = "Value"
                            'pCBox.ValueMember = "Code"
                            pCBox.DisplayMember = "Display"
                            pCBox.ValueMember = "Value"
                            'If pDc.DefaultValue IsNot Nothing Then
                            '    If pDc.DefaultValue IsNot DBNull.Value Then
                            '        pCBox.Text = pDc.DefaultValue
                            '    ElseIf pDc.AllowDBNull Or 1 = 1 Then
                            '        pCBox.Text = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                            '    Else
                            '        pCBox.Text = pCBox.DataSource.Rows(0)(1).ToString
                            '    End If
                            'ElseIf pDc.AllowDBNull Or 1 = 1 Then
                            '    pCBox.Text = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                            'Else
                            '    pCBox.Text = pCBox.DataSource.Rows(0)(1).ToString
                            'End If

                            'pCBox.Text = pDc.DefaultValue

                            ' pCmdBox.MaxLength = pDc.MaxLength
                            'pCBox.Text = pSubTypeDefValue





                            AddHandler pCBox.SelectionChangeCommitted, AddressOf cmbSubTypChange_Click
                            'AddHandler pCBox.SelectedIndexChanged, AddressOf controlLeave





                            Dim pPnl As Panel = New Panel
                            pPnl.Name = "pnl" & strfld

                            pPnl.Tag = pDcOpt
                            If pDcOpt.FilterFields IsNot Nothing Then
                                If pDcOpt.FilterFields.Count > 0 Then
                                    m_HasFilter = True
                                End If


                            End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pCBox.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pCBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.BoxColor <> "" Then
                            '    pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                            'End If

                            If pDcOpt.ForeColor <> "" Then
                                pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                            Else
                                pDcOpt.ForeColor = GlobalsFunctions.ColorToString(pLbl.ForeColor)


                            End If
                            If pDcOpt.BackColor <> "" Then
                                pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                            Else
                                pDcOpt.BackColor = GlobalsFunctions.ColorToString(pLbl.BackColor)
                            End If


                            If pDcOpt.BoxColor <> "" Then
                                pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)
                            Else
                                pDcOpt.BoxColor = GlobalsFunctions.ColorToString(pPnl.BackColor)
                            End If


                            pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                            pLbl.Top = 0
                            pCBox.Top = 5 + pLbl.Height

                            pPnl.Width = pControlWidth
                            pPnl.Margin = New Padding(0)
                            pPnl.Padding = New Padding(0)





                            pPnl.Top = pNextControlTop
                            pPnl.Left = 0
                            pPnl.Height = pCBox.Height + pLbl.Height + 15
                            pPnl.Controls.Add(pLbl)
                            pPnl.Controls.Add(pCBox)

                            pTbPg.Controls.Add(pPnl)
                            pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace
                            'pCBox.Text = pCV.Rows(0)("Value").ToString
                            'Try

                            '    pCBox.SelectedIndex = 0
                            'Catch ex As Exception

                            'End Try

                        End If
                    End If

                Else
                    If pfl.HasSubtypes Then
                        Dim pSubCol As DataColumn = pfl.Columns(pfl.SubtypeColumnIndex)


                        'MsgBox("Fix Dom")
                        obj = CType(pDc, SourceDataColumn).GetDomain(pSubCol.DefaultValue)
                        'obj = pfl.Domain(pSubCol.DefaultValue, pDc.ColumnName)

                        pSubCol = Nothing

                    Else
                        'MsgBox("Fix Dom")
                        obj = CType(pDc, SourceDataColumn).GetDomain(0)
                        'obj = pfl.Domain(0, pDc.ColumnName)
                    End If

                    If obj Is Nothing Then


                        If pDc.DataType Is System.Type.GetType("System.String") Or _
                           pDc.DataType Is System.Type.GetType("System.Integer") Or _
                           pDc.DataType Is System.Type.GetType("System.Int32") Or _
                            pDc.DataType Is System.Type.GetType("System.Int16") Or _
                           pDc.DataType Is System.Type.GetType("System.Boolean") Or _
                           pDc.DataType Is System.Type.GetType("System.Double") Or _
                           pDc.DataType Is System.Type.GetType("System.Single") Or _
                           (pDc.DataType Is System.Type.GetType("System.DateTime") And m_Mode = "ID") Then

                            'Create a lable for the field name
                            pLbl = New Label
                            'Apply the field alias to the field name
                            pLbl.Text = strAli
                            'Link the field to the name of the control
                            pLbl.Name = "lblEdit" & strfld
                            'Add the control at the determined locaiton
                            pLbl.Left = pLeftPadding

                            pLbl.Top = pNextControlTop
                            'Apply global font
                            pLbl.Font = m_FntLbl
                            'Create a graphics object to messure the text
                            g = pLbl.CreateGraphics
                            s = g.MeasureString(pLbl.Text, pLbl.Font)

                            pLbl.Height = CInt(s.Height)
                            'If the text is larger then the control, truncate the control
                            If s.Width >= Me.Width Then
                                pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                            Else 'Use autosize if it fits
                                pLbl.AutoSize = True
                            End If



                            'Determine the locaiton for the next control
                            '   pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace

                            'Create a new control to display the attributes                    
                            pTxtBox = New TextBox

                            'Tag the control with the field it represents
                            pTxtBox.Tag = Trim(strfld)
                            'Name the field with the field name
                            pTxtBox.Name = "txtEdit" & strfld
                            'Locate the control on the display
                            pTxtBox.Left = pLeftPadding
                            pTxtBox.Top = pNextControlTop
                            If m_Mode = "ID" Then
                                pTxtBox.ReadOnly = True
                                pTxtBox.BackColor = m_BColor
                                pTxtBox.ForeColor = m_FColor
                            End If
                            pTxtBox.Width = pControlWidth - pLeftPadding - pRightPadding
                            If pDc.DataType Is System.Type.GetType("System.String") Then
                                'Make the box taller if it is a long field
                                Try
                                    If pDc.MaxLength > CInt(GlobalsFunctions.appConfig.ApplicationSettings.MultiLineThreshold.Threshold) Then
                                        pTxtBox.Multiline = True
                                        pTxtBox.Height = pTxtBox.Height * CInt(GlobalsFunctions.appConfig.ApplicationSettings.MultiLineThreshold.SizeFactor)

                                    End If
                                Catch ex As Exception
                                    If pDc.MaxLength > 125 Then
                                        pTxtBox.Multiline = True
                                        pTxtBox.Height = pTxtBox.Height * 3

                                    End If
                                End Try


                            End If
                            If pDc.MaxLength > 0 Then
                                pTxtBox.MaxLength = pDc.MaxLength
                            End If


                            'Apply global font
                            pTxtBox.Font = m_Fnt
                            'Determine the locaiton for the next contro
                            '   pNextControlTop = pTxtBox.Top + pTxtBox.Height + intCtrlSpace
                            'Add the controls
                            '  pTbPg.Controls.Add(pLbl)
                            '  pTbPg.Controls.Add(pTxtBox)



                            'Group into panels to assist resizing
                            Dim pPnl As Panel = New Panel
                            pPnl.Name = "pnl" & strfld

                            pPnl.Tag = pDcOpt
                            If pDcOpt.FilterFields IsNot Nothing Then
                                If pDcOpt.FilterFields.Count > 0 Then
                                    m_HasFilter = True
                                End If


                            End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pTxtBox.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then

                            '    pTxtBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)


                            'End If
                            'If pDcOpt.BoxColor <> "" Then
                            '    pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                            'End If
                            pTxtBox.BackColor = Color.White

                            If pDcOpt.ForeColor <> "" Then
                                pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                            Else
                                pDcOpt.ForeColor = GlobalsFunctions.ColorToString(pLbl.ForeColor)




                            End If
                            If pDcOpt.BackColor <> "" Then
                                pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                            Else
                                pDcOpt.BackColor = GlobalsFunctions.ColorToString(pLbl.BackColor)
                            End If


                            If pDcOpt.BoxColor <> "" Then
                                pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)
                            Else
                                pDcOpt.BoxColor = GlobalsFunctions.ColorToString(pPnl.BackColor)
                            End If


                            pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                            pLbl.Top = 0
                            pTxtBox.Top = 5 + pLbl.Height
                            pPnl.Width = pControlWidth
                            pPnl.Margin = New Padding(0)
                            pPnl.Padding = New Padding(0)





                            pPnl.Top = pNextControlTop
                            pPnl.Left = 0
                            pPnl.Height = pTxtBox.Height + pLbl.Height + 10

                            pPnl.Controls.Add(pLbl)
                            AddHandler pTxtBox.Leave, AddressOf controlLeave
                            AddHandler pTxtBox.Enter, AddressOf controlEntered


                            pPnl.Controls.Add(pTxtBox)
                            pTbPg.Controls.Add(pPnl)
                            pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace
                        ElseIf pDc.DataType Is System.Type.GetType("System.DateTime") Then
                            'Create a lable for the field name
                            pLbl = New Label
                            'Apply the field alias to the field name
                            pLbl.Text = strAli
                            'Link the field to the name of the control
                            pLbl.Name = "lblEdit" & strfld
                            'Add the control at the determined locaiton
                            pLbl.Left = pLeftPadding

                            '   pLbl.Top = pNextControlTop
                            'Apply global font
                            pLbl.Font = m_FntLbl
                            'Create a graphics object to messure the text
                            g = pLbl.CreateGraphics
                            s = g.MeasureString(pLbl.Text, pLbl.Font)

                            pLbl.Height = CInt(s.Height)
                            'If the text is larger then the control, truncate the control
                            If s.Width >= Me.Width Then
                                pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                            Else 'Use autosize if it fits
                                pLbl.AutoSize = True
                            End If
                            'Determine the locaiton for the next control
                            '   pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace

                            pDateTime = New DateTimePicker
                            pDateTime.Font = m_Fnt
                            'pDateTime.CustomFormat = "m/d/yyyy"
                            pDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
                            Dim format As String = GlobalsFunctions.appConfig.EditControlOptions.DateTimeDisplayFormat
                            If format Is Nothing Then

                                pDateTime.CustomFormat = "MM/dd/yyyy"
                            Else
                                pDateTime.CustomFormat = format

                            End If

                            pDateTime.ShowCheckBox = True
                            pDateTime.Tag = strfld
                            pDateTime.Name = "dtEdt" & strfld
                            pDateTime.Left = pLeftPadding
                            '   pDateTime.Top = pNextControlTop
                            pDateTime.Width = pControlWidth - pLeftPadding - pRightPadding



                            '  pTbPg.Controls.Add(pLbl)
                            ' pTbPg.Controls.Add(pDateTime)


                            'Determine the locaiton for the next control
                            'pNextControlTop = pDateTime.Top + pDateTime.Height + intCtrlSpace
                            Dim pPnl As Panel = New Panel
                            pPnl.Name = "pnl" & strfld

                            pPnl.Tag = pDcOpt
                            If pDcOpt.FilterFields IsNot Nothing Then
                                If pDcOpt.FilterFields.Count > 0 Then
                                    m_HasFilter = True
                                End If


                            End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pDateTime.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pDateTime.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.BoxColor <> "" Then
                            '    pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                            'End If

                            If pDcOpt.ForeColor <> "" Then
                                pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                            Else
                                pDcOpt.ForeColor = GlobalsFunctions.ColorToString(pLbl.ForeColor)


                            End If
                            If pDcOpt.BackColor <> "" Then
                                pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                            Else
                                pDcOpt.BackColor = GlobalsFunctions.ColorToString(pLbl.BackColor)
                            End If


                            If pDcOpt.BoxColor <> "" Then
                                pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)
                            Else
                                pDcOpt.BoxColor = GlobalsFunctions.ColorToString(pPnl.BackColor)
                            End If

                            pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                            pLbl.Top = 0
                            pDateTime.Top = 5 + pLbl.Height
                            pPnl.Width = pControlWidth
                            pPnl.Margin = New Padding(0)
                            pPnl.Padding = New Padding(0)





                            pPnl.Top = pNextControlTop
                            pPnl.Left = 0
                            pPnl.Height = pDateTime.Height + pLbl.Height + 10
                            pPnl.Controls.Add(pLbl)
                            AddHandler pDateTime.Leave, AddressOf controlLeave
                            AddHandler pDateTime.Enter, AddressOf controlEntered

                            pPnl.Controls.Add(pDateTime)
                            pTbPg.Controls.Add(pPnl)
                            pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace



                        ElseIf (pDc.DataType.FullName = "System.Drawing.Bitmap" And m_Mode = "ID") Or (pDc.DataType Is System.Type.GetType("System.Byte[]") And m_Mode = "ID") Then
                            '   If pTbPgPic Is Nothing Then
                            tbCntrlEdit.TabPages.Add("Image" & ":" & intPgIdxPic, "Image" & ":" & intPgIdxPic)
                            pTbPgPic = tbCntrlEdit.TabPages("Image" & ":" & intPgIdxPic)
                            intPgIdxPic = intPgIdxPic + 1
                            pNextControlTopPic = pTopPadding


                            'Create a lable for the field name
                            pLbl = New Label
                            'Apply the field alias to the field name
                            pLbl.Text = strAli
                            'Link the field to the name of the control
                            pLbl.Name = "lblAtt" & strfld
                            'Add the control at the determined locaiton
                            pLbl.Left = pLeftPadding
                            pLbl.Top = pNextControlTopPic
                            'Apply global font
                            pLbl.Font = m_FntLbl
                            'Create a graphics object to messure the text
                            g = pLbl.CreateGraphics
                            s = g.MeasureString(pLbl.Text, pLbl.Font)
                            pLbl.Height = CInt(s.Height)
                            'If the text is larger then the control, truncate the control
                            If s.Width >= Me.Width Then
                                pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                            Else 'Use autosize if it fits
                                pLbl.AutoSize = True
                            End If
                            'Determine the locaiton for the next control
                            pNextControlTopPic = CInt(pLbl.Top + s.Height + intLabelCtrlSpace)



                            'Create a new control to display the attributes                    
                            pPic = New PictureBox
                            'Disable the control
                            '  pPic.ReadOnly = True
                            'Tag the control with the field it represents
                            pPic.Tag = Trim(strfld)
                            'Name the field with the field name
                            pPic.Name = "picAtt" & strfld
                            'Locate the control on the display
                            pPic.Left = pLeftPadding
                            pPic.Top = pNextControlTopPic
                            pPic.Width = pControlWidth - pLeftPadding - pRightPadding
                            pPic.Height = pControlWidth - pLeftPadding - pRightPadding
                            pPic.BackgroundImageLayout = ImageLayout.Stretch

                            'Apply global font
                            pPic.Font = m_Fnt



                            'Add the controls
                            pTbPgPic.Controls.Add(pLbl)
                            pTbPgPic.Controls.Add(pPic)






                        ElseIf pDc.DataType.FullName = "System.Drawing.Bitmap" Or pDc.DataType Is System.Type.GetType("System.Byte[]") Then

                            'Create a lable for the field name
                            pLbl = New Label
                            'Apply the field alias to the field name
                            pLbl.Text = strAli
                            'Link the field to the name of the control
                            pLbl.Name = "lblEdit" & strfld
                            'Add the control at the determined locaiton
                            pLbl.Left = pLeftPadding
                            pLbl.Top = pNextControlTop
                            'Apply global font
                            pLbl.Font = m_FntLbl
                            'Create a graphics object to messure the text
                            g = pLbl.CreateGraphics
                            s = g.MeasureString(pLbl.Text, pLbl.Font)
                            pLbl.Height = CInt(s.Height)
                            'If the text is larger then the control, truncate the control
                            If s.Width >= Me.Width Then
                                pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                            Else 'Use autosize if it fits
                                pLbl.AutoSize = True
                            End If
                            'Determine the locaiton for the next control
                            '   pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace



                            'Create a new control to display the attributes                    
                            pTxtBox = New TextBox
                            'Disable the control
                            '  pPic.ReadOnly = True
                            'Tag the control with the field it represents
                            pTxtBox.Tag = Trim(strfld)
                            'Name the field with the field name
                            pTxtBox.Name = "txtEdit" & strfld
                            'Locate the control on the display
                            pTxtBox.Left = pLeftPadding
                            pTxtBox.Top = pNextControlTop
                            pTxtBox.Width = pControlWidth - pLeftPadding - pRightPadding - pTxtBox.Height
                            If pDc.DataType Is System.Type.GetType("System.String") Then
                                'Make the box taller if it is a long field
                                Try
                                    If pDc.MaxLength > CInt(GlobalsFunctions.appConfig.ApplicationSettings.MultiLineThreshold.Threshold) Then
                                        pTxtBox.Multiline = True
                                        pTxtBox.Height = pTxtBox.Height * CInt(GlobalsFunctions.appConfig.ApplicationSettings.MultiLineThreshold.SizeFactor)

                                    End If
                                Catch ex As Exception
                                    If pDc.MaxLength > 125 Then
                                        pTxtBox.Multiline = True
                                        pTxtBox.Height = pTxtBox.Height * 3

                                    End If
                                End Try


                            End If
                            If pDc.MaxLength > 0 Then
                                pTxtBox.MaxLength = pDc.MaxLength
                            End If

                            pTxtBox.BackgroundImageLayout = ImageLayout.Stretch

                            'Apply global font
                            pTxtBox.Font = m_Fnt

                            pBtn = New Button
                            pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                            pBtn.FlatAppearance.BorderSize = 0

                            pBtn.Tag = Trim(strfld)
                            'Name the field with the field name
                            pBtn.Name = "btnEdit" & strfld
                            'Locate the control on the display
                            pBtn.Left = pTxtBox.Left + pTxtBox.Width + 5
                            pBtn.Top = pNextControlTop
                            Dim img As System.Drawing.Bitmap

                            'img = My.Resources.Open2
                            If pDcOpt.Options IsNot Nothing Then



                                If pDcOpt.Options.ToUpper = "Browse".ToUpper Then
                                    img = My.Resources.Open2
                                    AddHandler pBtn.Click, AddressOf btnLoadImgClick

                                ElseIf pDcOpt.Options.ToUpper = "Camera".ToUpper Then
                                    img = My.Resources.CameraSmall
                                    AddHandler pBtn.Click, AddressOf btnCamClick
                                ElseIf pDcOpt.Options.ToUpper = "Prompt".ToUpper Then
                                    img = My.Resources.greenQuestion
                                    AddHandler pBtn.Click, AddressOf btnPromptClick

                                Else

                                    img = My.Resources.CameraSmall
                                    AddHandler pBtn.Click, AddressOf btnCamClick
                                End If
                            Else

                                img = My.Resources.CameraSmall
                                AddHandler pBtn.Click, AddressOf btnCamClick

                            End If

                            img.MakeTransparent(img.GetPixel(img.Width - 1, 1))

                            pBtn.BackgroundImageLayout = ImageLayout.Center
                            pBtn.BackgroundImage = img
                            img = Nothing
                            pBtn.Width = pTxtBox.Height
                            pBtn.Height = pTxtBox.Height

                            'AddHandler pBtn.Click, AddressOf btnLoadImgClick
                            'Determine the locaiton for the next contro
                            '    pNextControlTop = pTxtBox.Top + pTxtBox.Height + intCtrlSpace


                            'Add the controls
                            'pTbPg.Controls.Add(pBtn)
                            'pTbPg.Controls.Add(pLbl)
                            'pTbPg.Controls.Add(pTxtBox)

                            Dim pPnl As Panel = New Panel
                            pPnl.Name = "pnl" & strfld

                            pPnl.Tag = pDcOpt
                            If pDcOpt.FilterFields IsNot Nothing Then
                                If pDcOpt.FilterFields.Count > 0 Then
                                    m_HasFilter = True
                                End If


                            End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pTxtBox.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pTxtBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.BoxColor <> "" Then
                            '    pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                            'End If

                            If pDcOpt.ForeColor <> "" Then
                                pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                            Else
                                pDcOpt.ForeColor = GlobalsFunctions.ColorToString(pLbl.ForeColor)


                            End If
                            If pDcOpt.BackColor <> "" Then
                                pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                            Else
                                pDcOpt.BackColor = GlobalsFunctions.ColorToString(pLbl.BackColor)
                            End If


                            If pDcOpt.BoxColor <> "" Then
                                pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)
                            Else
                                pDcOpt.BoxColor = GlobalsFunctions.ColorToString(pPnl.BackColor)
                            End If

                            pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                            pLbl.Top = 0
                            pTxtBox.Top = 5 + pLbl.Height
                            pBtn.Top = pTxtBox.Top
                            pPnl.Width = pControlWidth
                            pPnl.Margin = New Padding(0)
                            pPnl.Padding = New Padding(0)





                            pPnl.Top = pNextControlTop
                            pPnl.Left = 0
                            pPnl.Height = pTxtBox.Height + pLbl.Height + 10
                            pPnl.Controls.Add(pLbl)
                            pPnl.Controls.Add(pTxtBox)
                            AddHandler pTxtBox.Leave, AddressOf controlLeave
                            AddHandler pTxtBox.Enter, AddressOf controlEntered

                            pPnl.Controls.Add(pBtn)
                            pTbPg.Controls.Add(pPnl)
                            pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace
                        End If
                    Else
                        If TypeOf obj Is CodedValueDomain And m_Mode <> "ID" Then
                            Dim pCV As CodedValueDomain

                            'Create a lable for the field name
                            pLbl = New Label
                            'Apply the field alias to the field name
                            pLbl.Text = strAli
                            'Link the field to the name of the control
                            pLbl.Name = "lblEdit" & strfld
                            'Add the control at the determined locaiton
                            pLbl.Left = pLeftPadding
                            pLbl.Top = pNextControlTop
                            'Apply global font
                            pLbl.Font = m_FntLbl
                            'Create a graphics object to messure the text
                            g = pLbl.CreateGraphics
                            s = g.MeasureString(pLbl.Text, pLbl.Font)
                            pLbl.Height = CInt(s.Height)
                            'If the text is larger then the control, truncate the control
                            If s.Width >= Me.Width Then
                                pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                            Else 'Use autosize if it fits
                                pLbl.AutoSize = True
                            End If
                            'Determine the locaiton for the next control
                            '    pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace

                            pCV = CType(obj, CodedValueDomain)
                            ' pTbPg.Controls.Add(pLbl)
                            If pCV.Rows.Count = 0 Then
                                'MsgBox("ERROR: There are no values set up in the domain: " & pCV.TableName & " in layer: " & pfl.Name & ".  This field will be skipped")
                            Else
                                If pCV.Rows.Count = 2 And m_RadioOnTwo Then
                                    Dim pNewGpBox As New CustomPanel

                                    pNewGpBox.Tag = strfld & "|" & pCV.TableName

                                    pNewGpBox.BackColor = Color.White
                                    ' pNewGpBox.BorderColor = Pens.Transparent
                                    ' pNewGpBox.BorderStyle = Windows.Forms.BorderStyle.None

                                    If pDcOpt.BoxColor <> "" Then
                                        pNewGpBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                                    End If
                                    '  pNewGpBox.BorderColor = Pens.LightGray

                                    pNewGpBox.Width = pTbPg.Width - pRightPadding - pLeftPadding
                                    pNewGpBox.Top = pNextControlTop
                                    pNewGpBox.Left = pLeftPadding

                                    pRDButton = New RadioButton
                                    AddHandler pRDButton.CheckedChanged, AddressOf controlLeave

                                    pRDButton.FlatStyle = FlatStyle.Flat
                                    pRDButton.FlatAppearance.BorderSize = 0

                                    pRDButton.Name = "Rdo1"
                                    'If pDcOpt.ForeColor <> "" Then
                                    '    pRDButton.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                                    'End If
                                    'If pDcOpt.BackColor <> "" Then
                                    '    pRDButton.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                                    'End If

                                    pRDButton.Tag = pCV.Rows(0)("Code")
                                    If (pCV.Rows(0)("Value").ToString.Length > 14) Then
                                        pRDButton.Text = pCV.Rows(0)("Value").ToString.Substring(0, 14)

                                    Else
                                        pRDButton.Text = pCV.Rows(0)("Value").ToString

                                    End If
                                    'Dim pPf As SizeF = pRDButton.CreateGraphics.MeasureString(pRDButton.Text, pRDButton.Font)

                                    ''pRDButton.Height = pPf.Height
                                    'pRDButton.Width = pPf.Width + 25

                                    pRDButton.Left = pLeftPadding
                                    'AddHandler pRDButton.Leave, AddressOf controlLeave
                                    'AddHandler pRDButton.Enter, AddressOf controlEntered
                                    pRDButton.AutoSize = True
                                    pNewGpBox.Controls.Add(pRDButton)
                                    Dim g2 As Graphics = pRDButton.CreateGraphics

                                    Dim ps As SizeF = g2.MeasureString(pRDButton.Text, m_Fnt)

                                    pNewGpBox.Height = CInt(ps.Height + (ps.Height * 0.8)) ' pRDButton.Height + (pRDButton.Height / 4)
                                    'pRDButton.Top = CInt(pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2)

                                    pRDButton.Top = CInt(CInt(pNewGpBox.Height / 2) - (ps.Height / 2) - (ps.Height * 0.3))


                                    pRDButton = New RadioButton
                                    AddHandler pRDButton.CheckedChanged, AddressOf controlLeave

                                    pRDButton.FlatStyle = FlatStyle.Flat
                                    pRDButton.FlatAppearance.BorderSize = 0

                                    pRDButton.Name = "Rdo2"
                                    pRDButton.Tag = pCV.Rows(1)("Code")
                                    'pRDButton.Text = CStr(pCV.Rows(1)("Value"))
                                    If (pCV.Rows(1)("Value").ToString.Length > 14) Then
                                        pRDButton.Text = pCV.Rows(1)("Value").ToString.Substring(0, 14)

                                    Else
                                        pRDButton.Text = pCV.Rows(1)("Value").ToString

                                    End If
                                    pRDButton.Left = CInt(pNewGpBox.Width / 2)

                                    'pPf = pRDButton.CreateGraphics.MeasureString(pRDButton.Text, pRDButton.Font)
                                    'pRDButton.Height = pPf.Height
                                    'pRDButton.Width = pPf.Width + 25


                                    pRDButton.AutoSize = True

                                    pNewGpBox.Controls.Add(pRDButton)
                                    'AddHandler pRDButton.Leave, AddressOf controlLeave
                                    'AddHandler pRDButton.Enter, AddressOf controlEntered
                                    'AddHandler pNewGpBox.Leave, AddressOf controlLeave
                                    'AddHandler pNewGpBox.Enter, AddressOf controlEntered
                                    'pRDButton.Top = CInt(pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2)
                                    pRDButton.Top = CInt(pNewGpBox.Height / 2 - (ps.Height / 2) - (ps.Height * 0.3))

                                    ' pTbPg.Controls.Add(pNewGpBox)

                                    '  pNextControlTop = pNewGpBox.Top + pNewGpBox.Height + 7 + intLabelCtrlSpace


                                    Dim pPnl As Panel = New Panel
                                    pPnl.Name = "pnl" & strfld

                                    pPnl.Tag = pDcOpt
                                    If pDcOpt.FilterFields IsNot Nothing Then
                                        If pDcOpt.FilterFields.Count > 0 Then
                                            m_HasFilter = True
                                        End If


                                    End If
                                    'If pDcOpt.ForeColor <> "" Then
                                    '    pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                                    'End If
                                    'If pDcOpt.BackColor <> "" Then
                                    '    pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                                    'End If
                                    'If pDcOpt.ForeColor <> "" Then
                                    '    pRDButton.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                                    'End If
                                    'If pDcOpt.BackColor <> "" Then
                                    '    pRDButton.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                                    'End If
                                    'If pDcOpt.BoxColor <> "" Then
                                    '    pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                                    'End If
                                    If pDcOpt.ForeColor <> "" Then
                                        pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                                    Else
                                        pDcOpt.ForeColor = GlobalsFunctions.ColorToString(pLbl.ForeColor)


                                    End If
                                    If pDcOpt.BackColor <> "" Then
                                        pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                                    Else
                                        pDcOpt.BackColor = GlobalsFunctions.ColorToString(pLbl.BackColor)
                                    End If


                                    If pDcOpt.BoxColor <> "" Then
                                        pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)
                                    Else
                                        pDcOpt.BoxColor = GlobalsFunctions.ColorToString(pPnl.BackColor)
                                    End If


                                    pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                                    pLbl.Top = 0
                                    pNewGpBox.Top = 5 + pLbl.Height
                                    pNewGpBox.BackColor = Color.White

                                    pPnl.Width = pControlWidth
                                    pPnl.Margin = New Padding(0)
                                    pPnl.Padding = New Padding(0)





                                    pPnl.Top = pNextControlTop
                                    pPnl.Left = 0
                                    pPnl.Height = pNewGpBox.Height + pLbl.Height + 10
                                    pPnl.Controls.Add(pLbl)
                                    pPnl.Controls.Add(pNewGpBox)

                                    pTbPg.Controls.Add(pPnl)
                                    pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace
                                    pNewGpBox = Nothing
                                    '  pPf = Nothing

                                Else
                                    pCBox = New ComboBox
                                    pCBox.Tag = strfld & "|" & pCV.TableName
                                    pCBox.Name = "cboEdt" & strfld
                                    pCBox.Left = pLeftPadding
                                    pCBox.Top = pNextControlTop
                                    pCBox.Width = pControlWidth - pLeftPadding - pRightPadding
                                    pCBox.Height = pCBox.Height + 5
                                    pCBox.DropDownStyle = ComboBoxStyle.DropDownList

                                    'pCV.Columns(0).AllowDBNull = False
                                    'pCV.Columns(1).AllowDBNull = False
                                    ''If pCV.Rows(0).Item(0) IsNot DBNull.Value Then
                                    ''    Dim pNR As DataRow = pCV.NewRow

                                    ''    pNR.Item("Value") = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                    ''    pCV.Rows.InsertAt(pNR, 0)
                                    ''End If
                                    'pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never

                                    'pCBox.DataSource = pCV




                                    pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never
                                    pCV.Columns(0).AllowDBNull = True
                                    pCV.Columns(1).AllowDBNull = True
                                    If pDc.AllowDBNull Or 1 = 1 Then
                                        Dim pDT As DataTable
                                        pDT = pCV.DefaultView.ToTable()
                                        Dim pDR As DataRow = pDT.NewRow
                                        pDR.Item(0) = DBNull.Value
                                        pDR.Item(1) = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                        pDT.Rows.InsertAt(pDR, 0)

                                        'pDT.Rows.Add(DBNull.Value, GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown)
                                        ' pCBox.DataSource = pDT
                                        For Each dr As DataRow In pDT.Rows

                                            pCBox.Items.Add(New cValue(dr(1).ToString, dr(0)))

                                        Next
                                    Else
                                        'pCBox.DataSource = pCV
                                        For Each dr As DataRow In pCV.Rows

                                            pCBox.Items.Add(New cValue(dr(1).ToString, dr(0)))

                                        Next

                                    End If



                                    'pCBox.DisplayMember = "Value"
                                    'pCBox.ValueMember = "Code"
                                    pCBox.DisplayMember = "Display"
                                    pCBox.ValueMember = "Value"
                                    If pDc.DefaultValue IsNot Nothing Then
                                        If pDc.DefaultValue IsNot DBNull.Value Then
                                            pCBox.Text = pDc.DefaultValue
                                        ElseIf pDc.AllowDBNull Or 1 = 1 Then
                                            pCBox.Text = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                            'pCBox.SelectedItem = pCBox.Items(0)
                                        Else
                                            pCBox.Text = pCBox.DataSource.Rows(0)(1).ToString
                                        End If
                                    ElseIf pDc.AllowDBNull Or 1 = 1 Then
                                        pCBox.Text = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                    Else
                                        pCBox.Text = pCBox.DataSource.Rows(0)(1).ToString
                                    End If



                                    ' pCmdBox.MaxLength = pDc.MaxLength





                                    Dim pPnl As Panel = New Panel
                                    pPnl.Name = "pnl" & strfld

                                    pPnl.Tag = pDcOpt
                                    If pDcOpt.FilterFields IsNot Nothing Then
                                        If pDcOpt.FilterFields.Count > 0 Then
                                            m_HasFilter = True
                                        End If


                                    End If
                                    'If pDcOpt.ForeColor <> "" Then
                                    '    pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                                    'End If
                                    'If pDcOpt.BackColor <> "" Then
                                    '    pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                                    'End If
                                    'If pDcOpt.ForeColor <> "" Then
                                    '    pCBox.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                                    'End If
                                    'If pDcOpt.BackColor <> "" Then
                                    '    pCBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                                    'End If
                                    'If pDcOpt.BoxColor <> "" Then
                                    '    pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                                    'End If
                                    If pDcOpt.ForeColor <> "" Then
                                        pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                                    Else
                                        pDcOpt.ForeColor = GlobalsFunctions.ColorToString(pLbl.ForeColor)


                                    End If
                                    If pDcOpt.BackColor <> "" Then
                                        pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                                    Else
                                        pDcOpt.BackColor = GlobalsFunctions.ColorToString(pLbl.BackColor)
                                    End If


                                    If pDcOpt.BoxColor <> "" Then
                                        pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)
                                    Else
                                        pDcOpt.BoxColor = GlobalsFunctions.ColorToString(pPnl.BackColor)
                                    End If
                                    pCBox.BackColor = Color.White

                                    pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                                    pLbl.Top = 0
                                    pCBox.Top = 5 + pLbl.Height

                                    pPnl.Width = pControlWidth
                                    pPnl.Margin = New Padding(0)
                                    pPnl.Padding = New Padding(0)





                                    pPnl.Top = pNextControlTop
                                    pPnl.Left = 0
                                    pPnl.Height = pCBox.Height + pLbl.Height + 15
                                    pPnl.Controls.Add(pLbl)
                                    pPnl.Controls.Add(pCBox)
                                    AddHandler pCBox.SelectedIndexChanged, AddressOf controlLeave
                                    'AddHandler pCBox.Leave, AddressOf controlLeave
                                    AddHandler pCBox.Enter, AddressOf controlEntered

                                    pTbPg.Controls.Add(pPnl)
                                    pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace

                                    '   pTbPg.Controls.Add(pCBox)
                                    ' MsgBox(pCBox.Items.Count)
                                    pCBox.Visible = True

                                    'pCBox.Text = pCV.Rows(0)(1).ToString
                                    'Try

                                    'pCBox.SelectedIndex = 0
                                    'Catch ex As Exception

                                    'End Try
                                    pCBox.Visible = True
                                    pCBox.Refresh()

                                    '  pNextControlTop = pCBox.Top + pCBox.Height + 7 + intLabelCtrlSpace
                                End If
                            End If


                        ElseIf TypeOf obj Is RangeValueDomain And m_Mode <> "ID" Then
                            Dim pRV As RangeValueDomain
                            'Create a lable for the field name
                            pLbl = New Label
                            'Apply the field alias to the field name
                            pLbl.Text = strAli
                            'Link the field to the name of the control
                            pLbl.Name = "lblEdit" & strfld
                            'Add the control at the determined locaiton
                            pLbl.Left = pLeftPadding
                            pLbl.Top = pNextControlTop
                            'Apply global font
                            pLbl.Font = m_FntLbl
                            'Create a graphics object to messure the text
                            g = pLbl.CreateGraphics
                            s = g.MeasureString(pLbl.Text, pLbl.Font)
                            pLbl.Height = CInt(s.Height)
                            'If the text is larger then the control, truncate the control
                            If s.Width >= Me.Width Then
                                pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                            Else 'Use autosize if it fits
                                pLbl.AutoSize = True
                            End If
                            'Determine the locaiton for the next control
                            '  pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace



                            pRV = CType(obj, RangeValueDomain)
                            pNumBox = New NumericUpDown
                            '    AddHandler pNumBox.MouseDown, AddressOf numericClickEvt_MouseDown




                            If pDc.DataType Is System.Type.GetType("System.String") Then

                            ElseIf pDc.DataType Is System.Type.GetType("System.Integer") Then
                                pNumBox.DecimalPlaces = 0
                            ElseIf pDc.DataType Is System.Type.GetType("System.Int32") Then
                                pNumBox.DecimalPlaces = 0
                            ElseIf pDc.DataType Is System.Type.GetType("System.Boolean") Then

                            ElseIf pDc.DataType Is System.Type.GetType("System.Double") Then

                                pNumBox.DecimalPlaces = 2 'pDc.DataType.
                            ElseIf pDc.DataType Is System.Type.GetType("System.Single") Then

                                pNumBox.DecimalPlaces = 1 'pDc.DataType.
                            ElseIf pDc.DataType Is System.Type.GetType("System.DateTime") Then

                            End If

                            pNumBox.Minimum = CDec(pRV.MinimumValue)
                            pNumBox.Maximum = CDec(pRV.MaximumValue)
                            Dim pf As NumericUpDownAcceleration = New NumericUpDownAcceleration(3, CInt((pNumBox.Maximum - pNumBox.Minimum) * 0.02))


                            pNumBox.Accelerations.Add(pf)

                            pNumBox.Tag = strfld & "|" & pRV.Name
                            pNumBox.Name = "numEdt" & strfld
                            pNumBox.Left = pLeftPadding
                            pNumBox.BackColor = Color.White
                            pNumBox.Top = pNextControlTop
                            pNumBox.Width = pControlWidth - pLeftPadding - pRightPadding

                            ' pTbPg.Controls.Add(pLbl)

                            'pTbPg.Controls.Add(pNumBox)

                            'Determine the locaiton for the next control
                            '  pNextControlTop = pNumBox.Top + pNumBox.Height + intLabelCtrlSpace




                            Dim pPnl As Panel = New Panel
                            pPnl.Name = "pnl" & strfld

                            pPnl.Tag = pDcOpt
                            If pDcOpt.FilterFields IsNot Nothing Then
                                If pDcOpt.FilterFields.Count > 0 Then
                                    m_HasFilter = True
                                End If


                            End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pNumBox.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pNumBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.BoxColor <> "" Then
                            '    pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                            'End If
                            If pDcOpt.ForeColor <> "" Then
                                pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                            Else
                                pDcOpt.ForeColor = GlobalsFunctions.ColorToString(pLbl.ForeColor)


                            End If
                            If pDcOpt.BackColor <> "" Then
                                pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                            Else
                                pDcOpt.BackColor = GlobalsFunctions.ColorToString(pLbl.BackColor)
                            End If


                            If pDcOpt.BoxColor <> "" Then
                                pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)
                            Else
                                pDcOpt.BoxColor = GlobalsFunctions.ColorToString(pPnl.BackColor)
                            End If

                            pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                            pLbl.Top = 0
                            pNumBox.Top = 5 + pLbl.Height

                            pPnl.Width = pControlWidth
                            pPnl.Margin = New Padding(0)
                            pPnl.Padding = New Padding(0)





                            pPnl.Top = pNextControlTop
                            pPnl.Left = 0
                            pPnl.Height = pNumBox.Height + pLbl.Height + 15
                            pPnl.Controls.Add(pLbl)
                            pPnl.Controls.Add(pNumBox)
                            AddHandler pNumBox.Leave, AddressOf controlLeave
                            AddHandler pNumBox.Enter, AddressOf controlEntered

                            pTbPg.Controls.Add(pPnl)
                            pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace
                        Else
                            'Create a lable for the field name
                            pLbl = New Label
                            'Apply the field alias to the field name
                            pLbl.Text = strAli
                            'Link the field to the name of the control
                            pLbl.Name = "lblEdit" & strfld
                            'Add the control at the determined locaiton
                            pLbl.Left = pLeftPadding

                            pLbl.Top = pNextControlTop
                            'Apply global font
                            pLbl.Font = m_FntLbl
                            'Create a graphics object to messure the text
                            g = pLbl.CreateGraphics
                            s = g.MeasureString(pLbl.Text, pLbl.Font)

                            pLbl.Height = CInt(s.Height)
                            'If the text is larger then the control, truncate the control
                            If s.Width >= Me.Width Then
                                pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                            Else 'Use autosize if it fits
                                pLbl.AutoSize = True
                            End If



                            'Determine the locaiton for the next control
                            '   pNextControlTop = pLbl.Top + s.Height + intLabelCtrlSpace

                            'Create a new control to display the attributes                    
                            pTxtBox = New TextBox

                            'Tag the control with the field it represents
                            pTxtBox.Tag = Trim(strfld)
                            'Name the field with the field name
                            pTxtBox.Name = "txtEdit" & strfld
                            'Locate the control on the display
                            pTxtBox.Left = pLeftPadding
                            pTxtBox.Top = pNextControlTop
                            If m_Mode = "ID" Then
                                pTxtBox.ReadOnly = True

                            End If
                            pTxtBox.Width = pControlWidth - pLeftPadding - pRightPadding
                            If pDc.DataType Is System.Type.GetType("System.String") Then
                                'Make the box taller if it is a long field
                                Try
                                    If pDc.MaxLength > CInt(GlobalsFunctions.appConfig.ApplicationSettings.MultiLineThreshold.Threshold) Then
                                        pTxtBox.Multiline = True
                                        pTxtBox.Height = pTxtBox.Height * CInt(GlobalsFunctions.appConfig.ApplicationSettings.MultiLineThreshold.SizeFactor)

                                    End If
                                Catch ex As Exception
                                    If pDc.MaxLength > 125 Then
                                        pTxtBox.Multiline = True
                                        pTxtBox.Height = pTxtBox.Height * 3

                                    End If
                                End Try


                            End If
                            If pDc.MaxLength > 0 Then
                                pTxtBox.MaxLength = pDc.MaxLength
                            End If


                            'Apply global font
                            pTxtBox.Font = m_Fnt
                            'Determine the locaiton for the next contro
                            '   pNextControlTop = pTxtBox.Top + pTxtBox.Height + intCtrlSpace
                            'Add the controls
                            '  pTbPg.Controls.Add(pLbl)
                            '  pTbPg.Controls.Add(pTxtBox)



                            'Group into panels to assist resizing
                            Dim pPnl As Panel = New Panel
                            pPnl.Name = "pnl" & strfld

                            pPnl.Tag = pDcOpt
                            If pDcOpt.FilterFields IsNot Nothing Then
                                If pDcOpt.FilterFields.Count > 0 Then
                                    m_HasFilter = True
                                End If


                            End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.ForeColor <> "" Then
                            '    pTxtBox.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                            'End If
                            'If pDcOpt.BackColor <> "" Then
                            '    pTxtBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                            'End If
                            'If pDcOpt.BoxColor <> "" Then
                            '    pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                            'End If
                            If pDcOpt.ForeColor <> "" Then
                                pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)
                            Else
                                pDcOpt.ForeColor = GlobalsFunctions.ColorToString(pLbl.ForeColor)


                            End If
                            If pDcOpt.BackColor <> "" Then
                                pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                            Else
                                pDcOpt.BackColor = GlobalsFunctions.ColorToString(pLbl.BackColor)
                            End If


                            If pDcOpt.BoxColor <> "" Then
                                pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)
                            Else
                                pDcOpt.BoxColor = GlobalsFunctions.ColorToString(pPnl.BackColor)
                            End If

                            pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                            pLbl.Top = 0
                            pTxtBox.Top = 5 + pLbl.Height
                            pPnl.Width = pControlWidth
                            pPnl.Margin = New Padding(0)
                            pPnl.Padding = New Padding(0)





                            pPnl.Top = pNextControlTop
                            pPnl.Left = 0
                            pPnl.Height = pTxtBox.Height + pLbl.Height + 10
                            pPnl.Controls.Add(pLbl)
                            AddHandler pTxtBox.Leave, AddressOf controlLeave
                            AddHandler pTxtBox.Enter, AddressOf controlEntered

                            pPnl.Controls.Add(pTxtBox)
                            pTbPg.Controls.Add(pPnl)
                            pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace
                        End If

                    End If

                End If



            Next 'pDC

            m_AttMan = pfl.AttachmentManager


            If m_AttMan.HasAttachments Then
                tbCntrlEdit.TabPages.Add("Attachments", "Attachments") '("Image" & ":" & intPgIdxPic, "Image" & ":" & intPgIdxPic)
                pTabAtt = tbCntrlEdit.TabPages("Attachments")


                m_AttControlBox = New AttachmentControl(m_Map)
                AddHandler m_AttControlBox.AttachmentSelected, AddressOf AttachmentSelected
                AddHandler m_AttControlBox.DeleteAttachment, AddressOf DeleteAttachment

                ' pAttControlBox.ListBox.ValueMember = "Name"
                m_AttControlBox.Dock = DockStyle.Fill
                'Dim pAttList As List(Of Attachment) = m_AttMan.GetAttachments(m_CurrentRow.Fid)
                ' pLstBox.DataSource = pAttList
                'For Each att As Attachment In pAttList
                '    pLstBox.Items.Add(att)

                'Next
                pTabAtt.Controls.Add(m_AttControlBox)
                pTabAtt.Visible = True
                'm_TabControl.TabPages.Add(pTabAtt)
                pTabAtt.Tag = "Attachment"
                pTabAtt.Update()
            End If
            If pTbPg.Controls.Count = 0 Then
                tbCntrlEdit.TabPages.Remove(pTbPg)

            Else
                'pBtn = New Button
                'pBtn.Name = "btnSaveEdit"
                'pBtn.Text = "Save"
                'pBtn.Font = m_Fnt
                'pBtn.Top = pNextControlTop
                'pBtn.AutoSize = True

                'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                'pTbPg.Controls.Add(pBtn)
                'pBtn.Left = CInt((pTbPg.Width / 2) - pBtn.Width - 10)

                'pBtn = New Button
                'pBtn.Name = "btnClearEdit"
                'pBtn.Text = "Clear"
                'pBtn.Font = m_Fnt
                'pBtn.Top = pNextControlTop
                'pBtn.AutoSize = True

                'AddHandler pBtn.Click, AddressOf ClearSaveButtonClick
                'pTbPg.Controls.Add(pBtn)
                'pBtn.Left = CInt((pTbPg.Width / 2) + 10)
            End If



            If pfl.HasSubtypes And m_Mode <> "ID" Then
                SubtypeChange(pSubTypeDefValue, pfl.SubtypeColumnName)

            End If
            'cleanup
            pBtn = Nothing
            pDCs = Nothing
            pDc = Nothing

            pTbPg = Nothing

            pTxtBox = Nothing
            pLbl = Nothing
            pNumBox = Nothing

            pRDButton = Nothing

            pCBox = Nothing
            pDateTime = Nothing
            pfl = Nothing
            'msMapLayer = Nothing
            g = Nothing
            s = Nothing
            tbCntrlEdit.ResumeLayout()
            tbCntrlEdit.Refresh()
            ShuffleControls()

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try

    End Sub
    Private Function SaveFormToRecord() As Boolean
        Try


            'Make sure the row is valid
            If m_FDR Is Nothing Then Return False
            'Make sure there is valid Geometry
            If m_FDR.Geometry Is Nothing Then Return False
            If m_FDR.Geometry.IsEmpty Then Return False
            'Get the Data Table associated with the record

            'Loop through each tab page
            m_pTabPagAtt = Nothing


            For Each tbpg As Control In tbCntrlEdit.TabPages

                If tbpg.Tag IsNot Nothing Then
                    If tbpg.Tag.ToString() = "Attachment" Then
                        m_pTabPagAtt = tbpg
                    End If
                Else

                    'Loop through all controls on a tab page
                    For Each cCntrl As Control In tbpg.Controls
                        If TypeOf cCntrl Is Panel Then

                            For Each cCntrlPnl As Control In cCntrl.Controls
                                If cCntrlPnl.Visible Then


                                    Dim strFld As String
                                    Dim pDC As DataColumn
                                    If TypeOf cCntrlPnl Is CustomPanel Then
                                        'RadioButtons

                                        Dim pCsPn As CustomPanel = CType(cCntrlPnl, CustomPanel)
                                        strFld = pCsPn.Tag.ToString
                                        If strFld.IndexOf("|") > 0 Then
                                            strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                        End If
                                        For Each rdCn As Control In pCsPn.Controls
                                            If TypeOf rdCn Is RadioButton Then
                                                If CType(rdCn, RadioButton).Checked Then
                                                    m_FDR.Item(strFld) = CType(rdCn, RadioButton).Tag
                                                    Exit For

                                                End If
                                            End If
                                        Next

                                    ElseIf TypeOf cCntrlPnl Is TextBox Then
                                        'TextBoxes
                                        strFld = CType(cCntrlPnl, TextBox).Tag.ToString
                                        If strFld.IndexOf("|") > 0 Then
                                            strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                        End If

                                        pDC = m_DT.Columns(strFld)

                                        'Check input on the screen
                                        If CType(cCntrlPnl, TextBox).Text Is DBNull.Value Then

                                            If m_FDR.Item(strFld) Is DBNull.Value Then
                                            ElseIf m_FDR.Item(strFld).ToString = "" Then
                                            Else

                                                If pDC.DataType Is System.Type.GetType("System.String") Then
                                                    m_FDR.Item(strFld) = String.Empty
                                                Else
                                                    If pDC.AllowDBNull = False Then
                                                        'MsgBox("A null value is entered were it is not allowed, exiting")
                                                        lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, pDC.Caption, "Null Not Allowed"))
                                                        Return False
                                                    End If
                                                    m_FDR.Item(strFld) = DBNull.Value '0
                                                End If

                                            End If
                                        ElseIf CType(cCntrlPnl, TextBox).Text = "" Then
                                            If m_FDR.Item(strFld) Is DBNull.Value Then
                                            ElseIf m_FDR.Item(strFld).ToString = "" Then
                                            Else

                                                If pDC.DataType Is System.Type.GetType("System.String") Then
                                                    m_FDR.Item(strFld) = String.Empty
                                                Else
                                                    If pDC.AllowDBNull = False Then
                                                        lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, pDC.Caption, "Null Not Allowed"))
                                                        'MsgBox("A null value is entered were it is not allowed, exiting" & vbCrLf & "Field: " & pDC.ColumnName)
                                                        Return False
                                                    End If
                                                    m_FDR.Item(strFld) = DBNull.Value '0
                                                End If

                                            End If
                                        ElseIf m_FDR.Item(strFld).ToString = CType(cCntrlPnl, TextBox).Text Then
                                        Else
                                            If pDC.DataType Is System.Type.GetType("System.String") Then
                                                m_FDR.Item(strFld) = CType(cCntrlPnl, TextBox).Text
                                            ElseIf pDC.DataType Is System.Type.GetType("System.Byte[]") Then


                                                Try


                                                    m_FDR.Item(strFld) = ConvertImageToByteArray(New Bitmap(CType(cCntrlPnl, TextBox).Text), GetImageFormat(CType(cCntrlPnl, TextBox).Text))
                                                Catch ex As Exception
                                                    Dim st As New StackTrace
                                                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                                                    st = Nothing


                                                End Try
                                            ElseIf pDC.DataType.FullName = "System.Drawing.Bitmap" Then


                                                Try


                                                    m_FDR.Item(strFld) = New Bitmap(CType(cCntrlPnl, TextBox).Text)

                                                Catch ex As Exception
                                                    Dim st As New StackTrace
                                                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                                                    st = Nothing


                                                End Try
                                            Else
                                                If IsNumeric(CType(cCntrlPnl, TextBox).Text) Then
                                                    m_FDR.Item(strFld) = CType(cCntrlPnl, TextBox).Text
                                                Else
                                                    'MsgBox("Only enter Numeric values" & vbCrLf & "Field: " & pDC.ColumnName)
                                                    lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, pDC.Caption, "Value is not the correct type"))
                                                    Return False
                                                End If

                                            End If


                                        End If

                                    ElseIf TypeOf cCntrlPnl Is ComboBox Then
                                        strFld = CType(cCntrlPnl, ComboBox).Tag.ToString
                                        If strFld.IndexOf("|") > 0 Then
                                            strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                        End If
                                        pDC = m_DT.Columns(strFld)
                                        If CType(cCntrlPnl, ComboBox).SelectedItem Is DBNull.Value And CType(cCntrlPnl, ComboBox).SelectedValue Is DBNull.Value Then
                                            If m_FDR.Item(strFld) Is DBNull.Value Then
                                                If pDC.AllowDBNull = False Then
                                                    'MsgBox("A null value is entered were it is not allowed, exiting" & vbCrLf & "Field: " & pDC.ColumnName)
                                                    lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, pDC.Caption, "Null Not Allowed"))
                                                    Return False
                                                End If
                                            Else

                                                If pDC.AllowDBNull = False Then
                                                    'MsgBox("A null value is entered were it is not allowed, exiting" & vbCrLf & "Field: " & pDC.ColumnName)
                                                    lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, pDC.Caption, "Null Not Allowed"))
                                                    Return False
                                                End If
                                                If m_DT.Columns(CType(cCntrlPnl, ComboBox).Tag.ToString).DataType Is System.Type.GetType("System.String") Then
                                                    m_FDR.Item(strFld) = DBNull.Value 'String.Empty
                                                Else

                                                    m_FDR.Item(strFld) = DBNull.Value '0
                                                End If

                                            End If
                                        ElseIf CType(cCntrlPnl, ComboBox).SelectedItem Is Nothing And CType(cCntrlPnl, ComboBox).SelectedValue Is Nothing Then
                                            If m_FDR.Item(strFld) Is DBNull.Value Then
                                            ElseIf m_FDR.Item(strFld).ToString = "" Then
                                            Else

                                                If pDC.AllowDBNull = False Then
                                                    'MsgBox("A null value is entered were it is not allowed, exiting" & vbCrLf & "Field: " & pDC.ColumnName)
                                                    lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, pDC.Caption, "Null Not Allowed"))
                                                    Return False

                                                End If
                                                If m_DT.Columns(CType(cCntrlPnl, ComboBox).Tag.ToString).DataType Is System.Type.GetType("System.String") Then
                                                    m_FDR.Item(strFld) = CType(m_DT.Columns.Item(CType(cCntrlPnl, ComboBox).Tag.ToString), DataColumn).DefaultValue
                                                Else

                                                    m_FDR.Item(strFld) = CType(m_DT.Columns.Item(CType(cCntrlPnl, ComboBox).Tag.ToString), DataColumn).DefaultValue
                                                End If

                                            End If
                                        ElseIf CType(cCntrlPnl, ComboBox).SelectedValue Is Nothing Or CType(cCntrlPnl, ComboBox).SelectedValue Is DBNull.Value Then

                                            m_FDR.Item(strFld) = CType(cCntrlPnl, ComboBox).SelectedItem.Value

                                        Else


                                            m_FDR.Item(strFld) = CType(cCntrlPnl, ComboBox).SelectedValue



                                        End If

                                    ElseIf TypeOf cCntrlPnl Is DateTimePicker Then
                                        strFld = CType(cCntrlPnl, DateTimePicker).Tag.ToString
                                        If strFld.IndexOf("|") > 0 Then
                                            strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                        End If

                                        pDC = m_DT.Columns(strFld)
                                        If CType(cCntrlPnl, DateTimePicker).Checked = False Then
                                            If m_FDR.Item(strFld) Is DBNull.Value Then
                                            ElseIf m_FDR.Item(strFld).ToString = "" Then
                                            Else

                                                If pDC.AllowDBNull = False Then
                                                    'MsgBox("A null value is entered were it is not allowed, exiting" & vbCrLf & "Field: " & pDC.ColumnName)
                                                    lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, pDC.Caption, "Null Not Allowed"))
                                                    Return False
                                                End If
                                                m_FDR.Item(strFld) = DBNull.Value
                                            End If
                                        ElseIf m_FDR.Item(strFld).ToString = CType(cCntrlPnl, DateTimePicker).Value.ToString Then
                                        Else
                                            m_FDR.FeatureSource.Columns(strFld).ReadOnly = False

                                            m_FDR.Item(strFld) = CType(cCntrlPnl, DateTimePicker).Value
                                        End If




                                    ElseIf TypeOf cCntrlPnl Is NumericUpDown Then
                                        strFld = CType(cCntrlPnl, NumericUpDown).Tag.ToString
                                        If strFld.IndexOf("|") > 0 Then
                                            strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                        End If
                                        pDC = m_DT.Columns(strFld)

                                        If CType(cCntrlPnl, NumericUpDown).ReadOnly = True Then
                                            If m_FDR.Item(strFld) Is DBNull.Value Then
                                            ElseIf m_FDR.Item(strFld).ToString = "" Then
                                            Else

                                                If pDC.AllowDBNull = False Then
                                                    'MsgBox("A null value is entered were it is not allowed, exiting" & vbCrLf & "Field: " & pDC.ColumnName)
                                                    lstBoxError.Items.Add(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, pDC.Caption, "Null Not Allowed"))
                                                    Return False
                                                End If
                                                m_FDR.Item(strFld) = DBNull.Value
                                            End If
                                        ElseIf m_FDR.Item(strFld).ToString = CType(cCntrlPnl, NumericUpDown).Value.ToString Then
                                        Else
                                            If m_FDR.Table.Columns(strFld).DataType Is System.Type.GetType("System.String") Then
                                                m_FDR.Item(strFld) = CType(cCntrlPnl, NumericUpDown).Value.ToString
                                            ElseIf m_FDR.Table.Columns(strFld).DataType Is System.Type.GetType("System.Double") Then
                                                m_FDR.Item(strFld) = CDbl(CType(cCntrlPnl, NumericUpDown).Value)
                                            ElseIf m_FDR.Table.Columns(strFld).DataType Is System.Type.GetType("System.Single") Then

                                                m_FDR.Item(strFld) = CSng(CType(cCntrlPnl, NumericUpDown).Value)

                                            Else

                                                m_FDR.Item(strFld) = CType(cCntrlPnl, NumericUpDown).Value

                                            End If


                                        End If


                                    End If
                                    pDC = Nothing
                                End If
                            Next

                        End If
                    Next
                End If

            Next
            Try


                If m_FDR.RowState = DataRowState.Detached Then
                    Try
                        m_DT.Rows.Add(m_FDR)

                    Catch ex As Exception
                        m_DT = m_FL.GetDataTable()

                        Dim pTr As FeatureDataRow = m_DT.NewRow()
                        pTr.Geometry = m_FDR.Geometry
                        For v As Integer = 0 To m_DT.Columns.Count - 1

                            If m_DT.GeometryColumnIndex = v Then
                            ElseIf m_DT.FidColumnIndex = v Then
                            ElseIf UCase(m_DT.Columns(v).ColumnName) = UCase("shape.len") Or UCase(m_DT.Columns(v).ColumnName) = UCase("shape.area") Then

                            Else
                                pTr(v) = m_FDR(v)
                            End If




                        Next
                        m_DT.Rows.Add(pTr) '
                        m_FDR = pTr

                    End Try


                ElseIf m_FDR.RowState = DataRowState.Modified Then
                    'm_FDR.StoredEditSate
                End If

            Catch ex As Exception
                Dim st As New StackTrace
                MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                st = Nothing

            End Try

            If m_FDR.HasErrors() Then
                Dim colInError() As DataColumn = m_FDR.GetColumnsInError
                For i As Integer = 0 To colInError.GetLength(0) - 1
                    MsgBox(String.Format(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.OnSaveRecordErrorMessage, colInError(i).Caption, m_FDR.GetColumnError(colInError(i).ColumnName)))


                Next
                Return False
            Else
                '  m_FDR.AcceptChanges()
                '  pDT.SaveInFeatureSource()



            End If





        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return False

        End Try
        Return True
    End Function
    Private m_DT As FeatureDataTable
    Private Sub getGPSCoords()
        m_GPSVal = New GPSLocationDetails()
        m_GPSVal.Altitude = GlobalsFunctions.m_GPS.GpsConnection.Altitude
        m_GPSVal.Course = GlobalsFunctions.m_GPS.GpsConnection.Course
        m_GPSVal.CourseMagnetic = GlobalsFunctions.m_GPS.GpsConnection.CourseMagnetic
        m_GPSVal.DateTime = GlobalsFunctions.m_GPS.GpsConnection.DateTime
        m_GPSVal.FixSatelliteCount = GlobalsFunctions.m_GPS.GpsConnection.FixSatelliteCount
        m_GPSVal.FixStatus = GlobalsFunctions.m_GPS.GpsConnection.FixStatus

        m_GPSVal.GeoidHeight = GlobalsFunctions.m_GPS.GpsConnection.GeoidHeight
        m_GPSVal.HorizontalDilutionOfPrecision = GlobalsFunctions.m_GPS.GpsConnection.HorizontalDilutionOfPrecision
        m_GPSVal.Latitude = GlobalsFunctions.m_GPS.GpsConnection.Latitude
        ' gpsD.LatitudeToDegreeMinutesSeconds = GlobalsFunctions.m_GPS.GpsConnection.LatitudeToDegreeMinutesSeconds
        m_GPSVal.Longitude = GlobalsFunctions.m_GPS.GpsConnection.Longitude
        ' gpsD.LongitudeToDegreeMinutesSeconds = GlobalsFunctions.m_GPS.GpsConnection.LongitudeToDegreeMinutesSeconds
        m_GPSVal.PositionDilutionOfPrecision = GlobalsFunctions.m_GPS.GpsConnection.PositionDilutionOfPrecision
        m_GPSVal.SpatialReference = GlobalsFunctions.m_GPS.GpsConnection.SpatialReference
        m_GPSVal.Speed = GlobalsFunctions.m_GPS.GpsConnection.Speed
        m_GPSVal.VerticalDilutionOfPrecision = GlobalsFunctions.m_GPS.GpsConnection.VerticalDilutionOfPrecision

    End Sub
    Private Function SaveRecordToLayer() As Boolean
        Try

            'Make sure the row is valid
            If m_FDR Is Nothing Then Return False
            'Make sure there is valid Geometry
            If m_FDR.Geometry Is Nothing Then Return False
            If m_FDR.Geometry.IsEmpty Then Return False
            'Get the Data Table associated with the record
            If m_GPSStatus And m_GPSVal Is Nothing Then
                getGPSCoords()
            End If
            SaveRecordFinal()


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return False
        End Try
    End Function
    Private Function SaveRecordFinal() As Boolean
        Try

            Dim pAtt As Attachment = Nothing

            If m_GPSVal IsNot Nothing Then
                If m_FDR.Table.Columns("Long") IsNot Nothing Then
                    If m_FDR.Table.Columns("Long").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("Long") = m_GPSVal.Longitude.ToString()
                    Else
                        m_FDR("Long") = m_GPSVal.Longitude
                    End If
                End If


                If m_FDR.Table.Columns("Longitude") IsNot Nothing Then
                    If m_FDR.Table.Columns("Longitude").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("Longitude") = m_GPSVal.Longitude.ToString()
                    Else
                        m_FDR("Longitude") = m_GPSVal.Longitude
                    End If


                End If
                If m_FDR.Table.Columns("X") IsNot Nothing Then
                    If m_FDR.Table.Columns("X").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("X") = m_GPSVal.Longitude.ToString()
                    Else
                        m_FDR("X") = m_GPSVal.Longitude
                    End If

                End If
                If m_FDR.Table.Columns("XCoord") IsNot Nothing Then
                    If m_FDR.Table.Columns("XCoord").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("XCoord") = m_GPSVal.Longitude.ToString()
                    Else
                        m_FDR("XCoord") = m_GPSVal.Longitude
                    End If

                End If
                If m_FDR.Table.Columns("Lat") IsNot Nothing Then

                    If m_FDR.Table.Columns("Lat").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("Lat") = m_GPSVal.Latitude.ToString()
                    Else
                        m_FDR("Lat") = m_GPSVal.Latitude
                    End If

                End If
                If m_FDR.Table.Columns("Latitude") IsNot Nothing Then
                    If m_FDR.Table.Columns("Latitude").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("Latitude") = m_GPSVal.Latitude.ToString()
                    Else
                        m_FDR("Latitude") = m_GPSVal.Latitude

                    End If

                End If

                If m_FDR.Table.Columns("Y") IsNot Nothing Then

                    If m_FDR.Table.Columns("Y").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("Y") = m_GPSVal.Latitude.ToString()
                    Else
                        m_FDR("Y") = m_GPSVal.Latitude
                    End If

                End If

                If m_FDR.Table.Columns("YCoord") IsNot Nothing Then

                    If m_FDR.Table.Columns("YCoord").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("YCoord") = m_GPSVal.Latitude.ToString()
                    Else
                        m_FDR("YCoord") = m_GPSVal.Latitude
                    End If

                End If
                If m_FDR.Table.Columns("PDOP") IsNot Nothing Then

                    If m_FDR.Table.Columns("PDOP").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("PDOP") = m_GPSVal.PositionDilutionOfPrecision.ToString()
                    Else
                        m_FDR("PDOP") = m_GPSVal.PositionDilutionOfPrecision
                    End If

                End If

                If m_FDR.Table.Columns("HDOP") IsNot Nothing Then
                    If m_FDR.Table.Columns("HDOP").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("HDOP") = m_GPSVal.HorizontalDilutionOfPrecision.ToString()
                    Else
                        m_FDR("HDOP") = m_GPSVal.HorizontalDilutionOfPrecision

                    End If

                End If

                If m_FDR.Table.Columns("VDOP") IsNot Nothing Then

                    If m_FDR.Table.Columns("VDOP").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("VDOP") = m_GPSVal.VerticalDilutionOfPrecision.ToString()
                    Else
                        m_FDR("VDOP") = m_GPSVal.VerticalDilutionOfPrecision
                    End If

                End If

                If m_FDR.Table.Columns("SatCount") IsNot Nothing Then
                    If m_FDR.Table.Columns("SatCount").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("SatCount") = m_GPSVal.FixSatelliteCount.ToString()
                    Else
                        m_FDR("SatCount") = m_GPSVal.FixSatelliteCount
                    End If


                End If

                If m_FDR.Table.Columns("FixStatus") IsNot Nothing Then
                    If m_FDR.Table.Columns("FixStatus").DataType Is System.Type.GetType("System.String") Then
                        m_FDR("FixStatus") = m_GPSVal.FixStatus.ToString()
                    Else
                        m_FDR("FixStatus") = m_GPSVal.FixStatus
                    End If


                End If
            End If
            'If m_FDR.RowState = DataRowState.Unchanged Then
            '    m_FDR.SetModified()

            'End If

            '  m_DT.SaveInFeatureSource()
            '   m_DT.AcceptChanges()
            Try
                m_FDR.Table.SaveInFeatureSource()
                m_FDR.Table.AcceptChanges()
            Catch ex As Exception

            End Try
          
            ' m_FL.SaveEdits(m_DT)
            Try
                m_FL.SaveEdits(m_FDR.Table)
            Catch ex As Exception

            End Try
            Dim lstAtt As List(Of Attachment) = New List(Of Attachment)

            If m_pTabPagAtt IsNot Nothing Then

                For j As Integer = CType(m_pTabPagAtt.Controls(0), AttachmentControl).ListBox.Items.Count - 1 To 0 Step -1
                    Dim itm As Object = CType(m_pTabPagAtt.Controls(0), AttachmentControl).ListBox.Items(j)



                    If TypeOf itm Is attFiles Then
                        pAtt = New Attachment(m_FDR.FeatureSource, m_FDR.Fid, CType(itm, attFiles).fileName)
                        m_AttMan.AddAttachment(pAtt, CType(itm, attFiles).filePath, FileOperation.CopyFile)
                        CType(m_pTabPagAtt.Controls(0), AttachmentControl).ListBox.Items.Remove(itm)
                        lstAtt.Add(pAtt)


                    End If
                Next
                For Each att As Attachment In lstAtt
                    CType(m_pTabPagAtt.Controls(0), AttachmentControl).ListBox.Items.Add(att)

                Next
                For Each attID As Integer In m_lstAttToDel
                    m_AttMan.DeleteAttachment(attID)
                Next

            End If



            If m_DT.HasErrors Then
                Dim rowsInError() As DataRow = m_DT.GetErrors
                For i As Integer = 0 To rowsInError.GetLength(0) - 1
                    MsgBox(rowsInError(i).RowError)
                Next
                Return False

            End If
            If m_LogEdit Then
                saveCurrentRecordToXML()
            End If
            Dim pRetGeo As Geometries.Geometry = m_FDR.Geometry
            Dim pRetFID As Integer = m_FDR.Fid
            Dim pRetName As String = m_FDR.Table.FeatureSource.Name
            If m_bNewRecordAfterSave Then
                m_FDR = m_DT.NewRow
                loadRecordEditor(True)
                LoadAutoAttributes(m_FDR)

                disableSaveBtn()
                disableDeleteBtn()
                If m_pTabPagAtt IsNot Nothing Then


                    CType(m_pTabPagAtt.Controls(0), AttachmentControl).ListBox.Items.Clear()
                End If

            End If
            m_GPSVal = Nothing

            'Raise the event to notify the record was saved
            RaiseEvent RecordSaved(pRetName, pRetGeo, pRetFID)
            Return True

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return False
        End Try

    End Function
    Private Sub saveCurrentRecordToXML()
        Try

            If System.IO.Directory.Exists(Application.StartupPath & "\Backup\") = False Then
                System.IO.Directory.CreateDirectory(Application.StartupPath & "\Backup\")

            End If
            Dim XMLFilename As String = Application.StartupPath & "\Backup\" & m_FDR.Table.TableName & ".xml"
            If System.IO.File.Exists(XMLFilename) Then

                Dim xmlDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument
                xmlDoc.Load(XMLFilename)
                Dim xml As System.Xml.XmlDocument = WriteDataTable(m_FDR, xmlDoc)
                xml.Save(XMLFilename)

                xmlDoc = Nothing
                xml = Nothing
            Else
                Dim xmlDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument
                Dim pElem As System.Xml.XmlElement = xmlDoc.CreateElement("FeatureBackup")
                xmlDoc.AppendChild(pElem)

                Dim xml As System.Xml.XmlDocument = WriteDataTable(m_FDR, xmlDoc)
                xml.Save(XMLFilename)

                pElem = Nothing
                xmlDoc = Nothing
                xml = Nothing
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try

    End Sub
    Private Function WriteDataTable(ByVal pDRow As FeatureDataRow, ByVal xml As System.Xml.XmlDocument) As System.Xml.XmlDocument
        Try


            Dim pElem As System.Xml.XmlElement = xml("FeatureBackup")

            Dim xmlRow As System.Xml.XmlElement = xml.CreateElement(pDRow.Table.TableName.ToString.Replace(" ", "_"))
            For Each col As DataColumn In pDRow.Table.Columns

                Dim xmlCol As System.Xml.XmlElement = xml.CreateElement(col.ColumnName)
                xmlCol.InnerText = pDRow(col).ToString()
                xmlRow.AppendChild(xmlCol)
            Next
            pElem.AppendChild(xmlRow)

            Return xml
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return Nothing

        End Try
    End Function


    Private Function GetImageFormat(ByVal filename As String) As System.Drawing.Imaging.ImageFormat
        'Checks the image being saved for the proper formate
        'Gets the Ext of the image
        Dim ext As String = System.IO.Path.GetExtension(filename)


        Dim imgFmt As System.Drawing.Imaging.ImageFormat = Nothing


        Select Case UCase(ext)


            Case UCase(".jpg")
                imgFmt = System.Drawing.Imaging.ImageFormat.Jpeg
                Exit Select
            Case UCase(".gif")
                imgFmt = System.Drawing.Imaging.ImageFormat.Gif
                Exit Select
            Case UCase(".bmp")
                imgFmt = System.Drawing.Imaging.ImageFormat.Jpeg
                Exit Select
            Case UCase(".tif")
                imgFmt = System.Drawing.Imaging.ImageFormat.Tiff
                Exit Select
            Case Else

                imgFmt = System.Drawing.Imaging.ImageFormat.Jpeg

                Exit Select
        End Select

        Return imgFmt
    End Function
    Private Function ConvertImageToByteArray(ByVal imageToConvert As System.Drawing.Image, ByVal imageFormat As System.Drawing.Imaging.ImageFormat) As Byte()

        Dim Ret() As Byte = Nothing
        Try


            Using ms As System.IO.MemoryStream = New System.IO.MemoryStream()
                'Saves the image to a byte Array
                imageToConvert.Save(ms, imageFormat)
                Ret = ms.ToArray()
            End Using

        Catch

        End Try
        Return Ret


    End Function

    Private Sub btnHyper_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Try

            'Opens the linked doc in the default program
            Dim pCnt As Control = CType(sender, Control)
            Dim pHyperString As String
            'Checks to see if the link is in the text or tag
            If TypeOf pCnt Is TextBox Then
                pHyperString = pCnt.Text
            Else
                pHyperString = pCnt.Tag.ToString
            End If
            'Make sure there is no custom linking in the hyperlink
            If pHyperString.IndexOf("|") > 0 Then

                pHyperString = Trim(pHyperString.Substring(pHyperString.IndexOf("|") + 1))

            End If
            RaiseEvent HyperClick(pHyperString)



        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try


    End Sub
    Private Sub RemoveButton(ByVal cont As Control)
        Try

            For Each pCntrl As Control In cont.Controls
                'If the control is a tabpage, loop through each control on the page
                If TypeOf pCntrl Is TabPage Then
                    For Each iCntrl As Control In pCntrl.Controls
                        'If it is button
                        For Each cCntrl As Control In iCntrl.Controls


                            If TypeOf cCntrl Is Button Then
                                If cCntrl.Name.Contains("btnHyper") Then
                                    'Get the text field linked to it and resize it
                                    Dim lContr() As Control = iCntrl.Controls.Find("txtEdit" & cCntrl.Name.Remove(0, 8), False)
                                    If lContr.Length > 0 Then
                                        Dim pTextBox As TextBox = CType(lContr(0), TextBox)
                                        pTextBox.Width = iCntrl.Width - pTextBox.Left * 2 'pTextBox.Width + pTextBox.Height
                                        'Add the hyperlink cursor
                                        pTextBox.Cursor = Cursors.Default

                                        'Reset the color
                                        'lContr(0).ForeColor = Color.Black


                                        pTextBox.BackColor = m_BaseColor

                                        'reset the font
                                        'cCntrl.Font = m_Fnt
                                        'cCntrl.Invalidate()

                                        RemoveHandler pTextBox.Click, AddressOf btnHyper_Click

                                    End If
                                    'remove the control
                                    iCntrl.Controls.Remove(cCntrl)
                                    pCntrl.Refresh()
                                    pCntrl.Update()

                                End If
                            End If
                        Next
                    Next
                End If
            Next
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub
    Private Sub loadRecordEditor(Optional ByVal boolClear As Boolean = False)
        Try
            'Sub to load a record to the editor
            m_SettingAtts = True
            m_lstAttToDel = New List(Of Integer)

            Dim strFld As String
            'Determine if the layer has subtypes
            Dim bSubType As Boolean = m_FDR.FeatureSource.HasSubtypes
            'Gets the feature layer 
            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = m_FDR.FeatureSource
            Dim pBtn As Button
            Dim pBtnPadding As Integer

            'If m_FDR = "" Then

            'End 
            Dim bExistingFeat As Boolean = True
            If m_FDR.StoredEditSate <> EditState.NotDefined Then
                bExistingFeat = True
                If m_FDR Is Nothing Then
                    btnMove.Enabled = False
                ElseIf m_FL.GeometryType = GeometryType.Point Then
                    btnMove.Enabled = True
                Else
                    btnMove.Enabled = False
                End If


            Else
                btnMove.Enabled = False
                btnMove.BackgroundImage = My.Resources.GrayMove

                bExistingFeat = False
            End If

            RemoveButton(tbCntrlEdit)

            'If the layer has subtypes, load the subtype value first
            If bSubType And m_Mode <> "ID" Then
                'Loop through each control in the tab control
                For Each pCntrl As Control In tbCntrlEdit.Controls
                    'If the control is a tabpage
                    If TypeOf pCntrl Is TabPage Then
                        'Loop through each ocntrol on the tab oage
                        For Each cCntrl As Control In pCntrl.Controls
                            'If the control is a combo box(used for domains)
                            If TypeOf cCntrl Is Panel Then

                                For Each cCntrlPnl As Control In cCntrl.Controls
                                    If TypeOf cCntrlPnl Is ComboBox Then
                                        'Get the field
                                        strFld = CType(cCntrlPnl, ComboBox).Tag.ToString
                                        'Make sure no link is specified
                                        If strFld.IndexOf("|") > 0 Then
                                            strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                        End If
                                        'If the field is the subtype field
                                        If pFL.SubtypeColumnName = strFld Then
                                            'Set the value
                                            If m_FDR.Item(strFld) IsNot DBNull.Value Then

                                                For Each rwItm As Object In CType(cCntrlPnl, ComboBox).Items

                                                    If TypeOf rwItm Is cValue Then
                                                        Dim drv As cValue = rwItm

                                                        If drv.Value.ToString() = m_FDR.Item(strFld).ToString() Then
                                                            CType(cCntrlPnl, ComboBox).SelectedItem = rwItm
                                                            '  cmbSubTypChange_Click(CType(cCntrlPnl, ComboBox), Nothing)
                                                        End If

                                                    Else
                                                        Dim drv As DataRow = rwItm

                                                        If drv.Item(0).ToString() = m_FDR.Item(strFld).ToString() Then
                                                            CType(cCntrlPnl, ComboBox).SelectedItem = rwItm
                                                        End If

                                                    End If



                                                Next
                                            Else
                                                CType(cCntrlPnl, ComboBox).SelectedIndex = 0
                                            End If
                                            setRequiredColorsField(cCntrlPnl.Parent, False)

                                            ''Raise the subtype change event, this loads all the proper domains based on the subtype value
                                            Call cmbSubTypChange_Click(CType(cCntrlPnl, ComboBox), Nothing)

                                            Exit For

                                        End If



                                    End If
                                Next cCntrlPnl
                            End If
                        Next
                    End If

                Next
            End If
            'Loop through all the controls and set their value
            For Each pCntrl As Control In tbCntrlEdit.Controls
                If TypeOf pCntrl Is TabPage Then

                    Dim idxOut As Integer = 0
                    Dim cCntrl As Control
                    For idxOut = 0 To pCntrl.Controls.Count - 1
                        cCntrl = pCntrl.Controls(idxOut)

                        If TypeOf cCntrl Is AttachmentControl Then

                            Dim pSpCont As SplitContainer = CType(CType(cCntrl, AttachmentControl).Controls(0), SplitContainer)

                            Dim pLstBox As ListBox = CType(CType(pSpCont.Panel2.Controls(0), SplitContainer).Panel1.Controls(0), ListBox)

                            pLstBox.Items.Clear()

                            m_AttMan = m_FDR.FeatureSource.AttachmentManager


                            Dim pAttList As List(Of Attachment) = m_AttMan.GetAttachments(m_FDR.Fid)
                            For Each att As Attachment In pAttList
                                pLstBox.Items.Add(att)

                            Next
                            pLstBox = Nothing

                        ElseIf TypeOf cCntrl Is Panel Then
                            Dim cCntrlPnl As Control
                            Dim idx As Integer = 0
                            For idx = 0 To cCntrl.Controls.Count - 1
                                cCntrlPnl = cCntrl.Controls(idx)
                                If TypeOf cCntrlPnl Is CustomPanel Then
                                    'Get the Field
                                    strFld = CType(cCntrlPnl, CustomPanel).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Get the target value
                                    Dim pTargetVal As String = ""
                                    If m_FDR.Item(strFld) IsNot DBNull.Value Then
                                        pTargetVal = m_FDR.Item(strFld).ToString
                                    ElseIf m_FDR.Item(strFld) Is DBNull.Value Then
                                        If bExistingFeat Or boolClear Then
                                            pTargetVal = ""
                                        Else


                                            If pFL.Columns(strFld).DefaultValue Is DBNull.Value Then
                                                pTargetVal = ""
                                            Else
                                                pTargetVal = pFL.Columns(strFld).DefaultValue.ToString
                                            End If
                                        End If

                                    ElseIf CStr(m_FDR.Item(strFld)) = "" Then
                                        If bExistingFeat Or boolClear Then
                                            pTargetVal = ""
                                        Else
                                            If pFL.Columns(strFld).DefaultValue Is DBNull.Value Then
                                                pTargetVal = ""
                                            Else
                                                pTargetVal = CStr(pFL.Columns(strFld).DefaultValue)
                                            End If
                                        End If

                                    Else
                                        If bExistingFeat Or boolClear Then
                                            pTargetVal = ""
                                        Else
                                            If pFL.Columns(strFld).DefaultValue Is DBNull.Value Then
                                                pTargetVal = ""
                                            Else
                                                pTargetVal = CStr(pFL.Columns(strFld).DefaultValue)
                                            End If

                                        End If

                                    End If

                                    Dim pCsPn As CustomPanel = CType(cCntrlPnl, CustomPanel)
                                    'Loop through the checkboxes to set the proper value

                                    For Each rdCn As Control In pCsPn.Controls
                                        If TypeOf rdCn Is RadioButton Then

                                            If (bExistingFeat = False) And (pTargetVal = "" Or boolClear) Then
                                                RemoveHandler CType(rdCn, RadioButton).CheckedChanged, AddressOf controlLeave

                                                CType(rdCn, RadioButton).Checked = False
                                                AddHandler CType(rdCn, RadioButton).CheckedChanged, AddressOf controlLeave


                                            Else
                                                If pTargetVal = "" Then
                                                    RemoveHandler CType(rdCn, RadioButton).CheckedChanged, AddressOf controlLeave

                                                    CType(rdCn, RadioButton).Checked = False
                                                    AddHandler CType(rdCn, RadioButton).CheckedChanged, AddressOf controlLeave


                                                End If
                                                If rdCn.Tag.ToString = pTargetVal Then
                                                    RemoveHandler CType(rdCn, RadioButton).CheckedChanged, AddressOf controlLeave

                                                    CType(rdCn, RadioButton).Checked = True
                                                    AddHandler CType(rdCn, RadioButton).CheckedChanged, AddressOf controlLeave

                                                    Exit For


                                                End If
                                            End If
                                            ''setRequiredColorsField(rdCn, False)
                                        Else
                                            ' setRequiredColorsField(rdCn, False)

                                        End If

                                    Next
                                    setRequiredColorsField(cCntrl, False)

                                    'If the control is a text box
                                ElseIf TypeOf cCntrlPnl Is TextBox Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, TextBox).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    Dim obj As Object
                                    Try


                                        If bSubType Then
                                            If strFld = pFL.Columns(pFL.SubtypeColumnIndex).ColumnName Then
                                                obj = m_FL.Subtypes

                                            Else
                                                'MsgBox("Fix Dom")
                                                obj = m_FL.Columns(strFld).GetDomain(m_FDR.Item(pFL.SubtypeColumnIndex))
                                                'obj = m_FL.Domain(m_FDR.Item(pFL.SubtypeColumnIndex), strFld)
                                            End If

                                        Else
                                            '  MsgBox("Fix Dom")
                                            obj = m_FL.Columns(strFld).GetDomain(0)
                                            'obj = m_FL.GetDomain(0, strFld)
                                        End If
                                    Catch ex As Exception
                                        obj = Nothing

                                    End Try













                                    'Set the Value
                                    If m_FDR.Item(strFld) IsNot DBNull.Value Then
                                        'Used to check for hyper links
                                        If m_FDR.Item(strFld).ToString.Contains("http") Or m_FDR.Item(strFld).ToString.Contains(":\") Or m_FDR.Item(strFld).ToString.Contains("\\") Then
                                            'Create a new button to launch the hyperlink
                                            pBtn = New Button
                                            'Resize text box 

                                            CType(cCntrlPnl, TextBox).Width = CType(cCntrlPnl, TextBox).Width - 20
                                            CType(cCntrlPnl, TextBox).Cursor = Cursors.Hand
                                            'cCntrl.ForeColor = Color.LightBlue
                                            m_BaseColor = CType(cCntrlPnl, TextBox).BackColor
                                            CType(cCntrlPnl, TextBox).BackColor = Color.LightBlue
                                            'cCntrl.Font = m_FntHyper
                                            'cCntrl.Invalidate()
                                            'Size and locate contrl
                                            pBtn.Width = 25
                                            pBtn.Height = 25

                                            pBtn.Left = CType(cCntrlPnl, TextBox).Left + CType(cCntrlPnl, TextBox).Width + pBtnPadding
                                            pBtn.Top = CType(cCntrlPnl, TextBox).Top
                                            'Remove any text on the button
                                            pBtn.Text = ""
                                            'Apply and allign the hyperlnk image
                                            pBtn.ImageAlign = ContentAlignment.MiddleCenter
                                            pBtn.BackgroundImage = My.Resources.hyper
                                            pBtn.BackgroundImageLayout = ImageLayout.Stretch
                                            'Tag the button with the hyperlink value
                                            pBtn.Tag = m_FDR.Item(strFld).ToString
                                            'Name the button with the field
                                            pBtn.Name = "btnHyper" & strFld
                                            'add the handler to launc the hyperlink
                                            AddHandler pBtn.Click, AddressOf btnHyper_Click
                                            'add the button
                                            cCntrl.Controls.Add(pBtn)
                                            'Attach a handler to the text box it self
                                            AddHandler CType(cCntrlPnl, TextBox).Click, AddressOf btnHyper_Click
                                            'Add the hyperlink cursor



                                        End If
                                        Dim pDataColumn = m_FDR.Table.Columns.Item(strFld)
                                        Dim pDisplayString As String = m_FDR.Item(strFld).ToString
                                        pDisplayString = m_FDR.Item(strFld).ToString()

                                        If pDataColumn IsNot Nothing And pDataColumn.DataType Is System.Type.GetType("System.DateTime") Then

                                            ''Dim myutils As ConfigUtils = New ConfigUtils()
                                            Dim format As String = GlobalsFunctions.appConfig.EditControlOptions.DateTimeDisplayFormat
                                            If format Is Nothing Then
                                                'MessageBox.Show("DateTimeDisplayFormat configuration not found.")
                                                format = "MM/dd/yyyy"
                                            ElseIf format.Trim = "" Then
                                                'MessageBox.Show("DateTimeDisplayFormat configuration not found.")
                                                format = "MM/dd/yyyy"
                                            End If
                                            pDisplayString = CType(m_FDR.Item(strFld), DateTime).ToString(format)



                                        ElseIf obj IsNot Nothing Then



                                            'Check to see if we need to translate to value to description from a domain

                                            If TypeOf obj Is CodedValueDomain Then
                                                'Get the coded value domain
                                                Dim pCvd As CodedValueDomain = CType(obj, CodedValueDomain)

                                                Dim pDrs() As DataRow
                                                'Check value column data type
                                                If pCvd.Columns("Code").DataType IsNot System.Type.GetType("System.String") Then
                                                    pDrs = pCvd.Select("Code = " & pDisplayString)
                                                Else
                                                    pDrs = pCvd.Select("Code = '" & pDisplayString & "'")
                                                End If
                                                'Get the domain description
                                                If pDrs.Length > 0 Then
                                                    If pDrs(0)("Value") IsNot DBNull.Value Then
                                                        pDisplayString = pDrs(0)("Value").ToString
                                                    End If
                                                End If

                                            Else

                                            End If
                                        End If
                                        CType(cCntrlPnl, TextBox).Text = pDisplayString

                                    Else
                                        If bExistingFeat Or boolClear Then
                                            CType(cCntrlPnl, TextBox).Text = ""
                                        End If

                                    End If
                                    setRequiredColorsField(CType(cCntrlPnl, TextBox).Parent, False)
                                    'if the control is a combo box(domain)
                                ElseIf TypeOf cCntrlPnl Is ComboBox Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, ComboBox).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Skip the subtype column
                                    If pFL.SubtypeColumnName <> strFld Then
                                        'Set the value
                                        If m_FDR.Item(strFld) IsNot DBNull.Value Then
                                            If m_FDR.Item(strFld).ToString = "" Or m_FDR.Item(strFld) Is DBNull.Value Then
                                                '   Dim pCV As CodedValueDomain = CType(cCntrlPnl, ComboBox).DataSource
                                                '   Dim i As Integer = pCV.Rows.Count
                                                'If CType(cCntrlPnl, ComboBox).DataSource IsNot Nothing Then
                                                '    CType(cCntrlPnl, ComboBox).Text = CType(cCntrlPnl, ComboBox).DataSource.Rows(0)("Value")
                                                'End If
                                                If bExistingFeat Or boolClear Then
                                                    CType(cCntrlPnl, ComboBox).SelectedIndex = 0
                                                End If

                                            Else

                                                ' CType(cCntrlPnl, ComboBox).SelectedValue = m_FDR.Item(strFld)
                                                ' CType(cCntrlPnl, ComboBox).Text = m_FDR.Item(strFld).ToString
                                                'If m_FDR.Item(strFld).ToString <> "" And CType(cCntrlPnl, ComboBox).Text = "" Then
                                                If True Then
                                                    Dim intT As Integer = 0

                                                    For Each rwItm As Object In CType(cCntrlPnl, ComboBox).Items
                                                        If TypeOf rwItm Is cValue Then
                                                            Dim drv As cValue = rwItm

                                                            If drv.Value.ToString() = m_FDR.Item(strFld).ToString() Then
                                                                CType(cCntrlPnl, ComboBox).SelectedIndex = intT

                                                            End If

                                                        Else
                                                            Dim drv As DataRow = rwItm

                                                            If drv.Item(0).ToString = m_FDR.Item(strFld) Then
                                                                CType(cCntrlPnl, ComboBox).SelectedIndex = intT

                                                            End If

                                                        End If
                                                        intT = intT + 1
                                                    Next

                                                End If




                                            End If


                                            'CType(cCntrlPnl, ComboBox).Text = m_FDR.Item(strFld).ToString
                                        Else
                                            If CType(cCntrlPnl, ComboBox).Items.Count > 0 Then
                                                CType(cCntrlPnl, ComboBox).SelectedIndex = 0
                                            End If

                                        End If
                                    End If
                                    setRequiredColorsField(cCntrlPnl.Parent, False)
                                    ' Dim t As String = ""
                                    ' t = t

                                    'if the contorl is a data time field
                                ElseIf TypeOf cCntrlPnl Is DateTimePicker Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, DateTimePicker).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Get and set the value
                                    If m_FDR.Item(strFld) IsNot DBNull.Value Then
                                        CType(cCntrlPnl, DateTimePicker).Text = m_FDR.Item(strFld).ToString
                                        CType(cCntrlPnl, DateTimePicker).Checked = True
                                    Else
                                        CType(cCntrlPnl, DateTimePicker).Checked = False

                                    End If
                                    setRequiredColorsField(cCntrlPnl.Parent, False)

                                    'If the field is a range domain
                                ElseIf TypeOf cCntrlPnl Is NumericUpDown Then
                                    'Get the field
                                    strFld = CType(cCntrlPnl, NumericUpDown).Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    'Get and set the value
                                    If m_FDR.Item(strFld) Is DBNull.Value Then
                                        '   CType(cCntrlPnl, NumericUpDown).ReadOnly = True
                                        'CType(cCntrlPnl, NumericUpDown).Value = Nothing

                                    ElseIf CDbl(m_FDR.Item(strFld)) > CType(cCntrlPnl, NumericUpDown).Maximum Or _
                                       CDbl(m_FDR.Item(strFld)) < CType(cCntrlPnl, NumericUpDown).Minimum Then
                                        ' CType(cCntrlPnl, NumericUpDown).ReadOnly = True
                                        '  CType(cCntrlPnl, NumericUpDown).Value = Nothing
                                        ' CType(cCntrlPnl, NumericUpDown).Value = DBNull.Value
                                        CType(cCntrlPnl, NumericUpDown).Value = CDec(m_FDR.Item(strFld).ToString)
                                    Else
                                        CType(cCntrlPnl, NumericUpDown).Value = CDec(m_FDR.Item(strFld).ToString)


                                    End If
                                    setRequiredColorsField(cCntrlPnl.Parent, False)



                                End If
                            Next

                        End If

                    Next
                End If
            Next
            If m_FDR.Geometry IsNot Nothing Then
                ' Me.Geometry = m_FDR.Geometry
                If m_Mode = "ID" Then
                    If Me.Geometry.GeometryType = Esri.ArcGIS.Mobile.Geometries.GeometryType.Point Then
                        GlobalsFunctions.flashGeo(m_FDR.Geometry, m_Map, m_penFlash, m_brushFlash)
                    Else
                        GlobalsFunctions.flashGeo(m_FDR.Geometry, m_Map, m_penLineFlash, m_brushFlash)
                    End If
                End If
            End If
            ShuffleControls()
            disableSaveBtn()

            'disableSaveBtn()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        Finally
            m_SettingAtts = False
        End Try
    End Sub

#End Region

#Region "Events"
    Private m_ExitValue As String
    Private Sub controlLeave(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try


            If m_Map.IsDisposed Then Return
            If m_shuffling Then Return
            If m_SettingAtts Then Return
            Dim txtToSet As String
            Dim valOfCombo As String

            If TypeOf (sender) Is DateTimePicker Then
                If CType(sender, DateTimePicker).Checked = False Then
                    txtToSet = ""
                Else
                    txtToSet = sender.text.ToString()
                End If

            ElseIf TypeOf (sender) Is RadioButton Then
                If (sender.parent.controls.count = 2) Then

                    If CType(sender.parent.controls(0), RadioButton).Checked = True Then
                        txtToSet = CType(sender.parent.controls(0), RadioButton).Tag
                        If sender.checked = False Then Return

                    ElseIf CType(sender.parent.controls(1), RadioButton).Checked = True Then
                        txtToSet = CType(sender.parent.controls(1), RadioButton).Tag
                        If sender.checked = False Then Return

                    Else
                        txtToSet = ""
                    End If

                End If
            ElseIf TypeOf (sender) Is ComboBox Then
                If CType(sender, ComboBox).Text Is Nothing Then
                    txtToSet = Nothing

                ElseIf CType(sender, ComboBox).Text Is DBNull.Value Then
                    txtToSet = Nothing

                Else

                    txtToSet = CType(sender, ComboBox).Text
                    valOfCombo = txtToSet
                    For Each item As cValue In CType(sender, ComboBox).Items
                        If item.Display = CType(sender, ComboBox).Text Then
                            If item.Value Is DBNull.Value Then
                                valOfCombo = Nothing

                            Else
                                valOfCombo = item.Value
                            End If


                        End If
                    Next

                End If

            Else
                txtToSet = sender.text.ToString()
            End If



            If (TypeOf (sender) Is RadioButton) Then

                If UpdateField(sender.parent.tag, txtToSet, False) = False Then


                End If
            ElseIf (TypeOf (sender) Is ComboBox) Then
                If m_FDR IsNot Nothing Then

                    Dim strTagInfo() As String = sender.tag.ToString.Split("|")



                    If m_FDR(strTagInfo(0).ToString()).ToString <> txtToSet Then
                        If UpdateField(sender.tag, valOfCombo, False) = False Then


                        End If
                    Else
                        Exit Sub

                    End If
                End If
            Else
                If m_FDR IsNot Nothing Then

                    Dim strTagInfo() As String = sender.tag.ToString.Split("|")

                    If m_FDR(strTagInfo(0)).ToString <> txtToSet Then 'If m_ExitValue <> txtToSet Then

                        If UpdateField(sender.tag, txtToSet, False) = False Then

                            sender.text = m_ExitValue
                        End If
                    End If
                End If

            End If


            If m_EditOptions IsNot Nothing Then
                If m_EditOptions.RequiredBackColor = "" And _
                                   m_EditOptions.RequiredBoxColor = "" And _
                                   m_EditOptions.RequiredForeColor = "" Then
                Else

                    setRequiredColorsField(sender.tag.ToString(), False)
                End If


            End If



            'If m_FDR.HasErrors() Then
            '    Dim colInError() As DataColumn = m_FDR.GetColumnsInError
            '    For i As Integer = 0 To colInError.GetLength(0) - 1
            '        '   MsgBox("The value in " + colInError(i).Caption & " is not valid or is required." & vbCrLf & m_FDR.GetColumnError(colInError(i).ColumnName))

            '    Next

            '    btnSave.Enabled = False
            '    btnSave.BackgroundImage = My.Resources.SaveGray

            '    'btnSave.BackgroundImage = My.Resources.SaveGray
            'Else
            '    btnSave.Enabled = True
            '    btnSave.BackgroundImage = My.Resources.SaveGreen

            'End If

            If m_HasFilter Then
                If TypeOf (sender) Is RadioButton Then

                    RemoveHandler CType(sender, RadioButton).CheckedChanged, AddressOf controlLeave
                    ShuffleControls()
                    AddHandler CType(sender, RadioButton).CheckedChanged, AddressOf controlLeave

                ElseIf TypeOf (sender) Is ComboBox Then
                    RemoveHandler CType(sender, ComboBox).SelectedIndexChanged, AddressOf controlLeave
                    'RemoveHandler CType(sender, ComboBox).Leave, AddressOf controlLeave
                    RemoveHandler CType(sender, Control).Enter, AddressOf controlEntered

                    ShuffleControls()
                    If txtToSet Is Nothing Then
                        CType(sender, ComboBox).SelectedValue = DBNull.Value
                    Else
                        CType(sender, ComboBox).SelectedValue = txtToSet
                    End If

                    AddHandler CType(sender, ComboBox).SelectedIndexChanged, AddressOf controlLeave
                    'AddHandler CType(sender, ComboBox).Leave, AddressOf controlLeave
                    AddHandler CType(sender, Control).Enter, AddressOf controlEntered
                Else

                    RemoveHandler CType(sender, Control).Leave, AddressOf controlLeave
                    RemoveHandler CType(sender, Control).Enter, AddressOf controlEntered

                    ShuffleControls()
                    AddHandler CType(sender, Control).Leave, AddressOf controlLeave
                    AddHandler CType(sender, Control).Enter, AddressOf controlEntered
                End If

            End If
            disableSaveBtn()
            disableDeleteBtn()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Sub controlChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub controlEntered(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If TypeOf (sender) Is DateTimePicker Then
            If CType(sender, DateTimePicker).Checked = False Then
                m_ExitValue = ""
            Else
                m_ExitValue = sender.text.ToString()
            End If
            'ElseIf TypeOf (sender) Is RadioButton Then
            '    If (sender.parent.controls.count = 2) Then

            '        If CType(sender.parent.controls(0), RadioButton).Checked = True Then
            '            m_ExitValue = CType(sender.parent.controls(0), RadioButton).Tag
            '        ElseIf CType(sender.parent.controls(1), RadioButton).Checked = True Then
            '            m_ExitValue = CType(sender.parent.controls(1), RadioButton).Tag
            '        Else
            '            m_ExitValue = ""
            '        End If
            '        MsgBox(m_ExitValue)

            '    End If
        Else
            m_ExitValue = sender.text.ToString()
        End If


    End Sub




    Private Sub cmbSubTypChange_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If CType(sender, ComboBox).SelectedIndex = -1 Then Return

        SubtypeChange(CInt(CType(sender, ComboBox).SelectedItem.value), CType(sender, ComboBox).Tag.ToString)

    End Sub

    Private Sub SubtypeChange(ByVal value As Integer, ByVal SubtypeField As String)
        Try
            Dim intSubVal As Integer = value



            'Exit if the layer is not found
            If m_FL Is Nothing Then Exit Sub
            If m_FDR IsNot Nothing Then
                If m_FDR.Item(m_FL.SubtypeColumnIndex).ToString() <> value.ToString() Then

                    m_FDR.Item(m_FL.SubtypeColumnIndex) = value

                End If

            End If
            Dim strFld As String
            Dim pCBox As ComboBox
            Dim pNUP As NumericUpDown
            Dim pCV As CodedValueDomain
            Dim pRg As RangeValueDomain



            'Spacing between last control and the bottom of the page
            Dim pBottomPadding As Integer = 70
            'Padding for the left of each control
            Dim pLeftPadding As Integer = 10
            'Spacing between firstcontrol and the top
            Dim pTopPadding As Integer = 10
            'Padding for the right of each control
            Dim pRightPadding As Integer = 10

            Dim existDomName As String = ""

            'Loop through all controls 
            For Each tbPg As TabPage In tbCntrlEdit.TabPages

                For Each cntrl As Control In tbPg.Controls

                    'If the control is a combobox, then reapply the domain
                    If TypeOf cntrl Is Panel Then

                        For Each cntrlPnl As Control In cntrl.Controls
                            existDomName = ""
                            If TypeOf cntrlPnl Is ComboBox Then

                                pCBox = CType(cntrlPnl, ComboBox)
                                If SubtypeField <> pCBox.Tag.ToString Then

                                    'Get the Field
                                    strFld = pCBox.Tag.ToString
                                    If strFld.IndexOf("|") > 0 Then
                                        Dim strArr() As String = strFld.Split("|")


                                        strFld = Trim(strArr(0))
                                        existDomName = Trim(strArr(1))
                                    End If
                                    'Get the domain
                                    ' MsgBox("Fix Doma")
                                    'pCV = CType(m_FL.Domain(intSubVal, strFld), CodedValueDomain)
                                    pCV = CType(m_FL.Columns(strFld).GetDomain(intSubVal), CodedValueDomain)

                                    If pCV Is Nothing Then
                                        pCBox.DataSource = Nothing
                                        pCBox.Items.Clear()
                                        pCBox.DropDownStyle = ComboBoxStyle.DropDown
                                        pCBox.Text = ""
                                        pCBox.Tag = strFld & "|" & ""
                                    Else
                                        pCBox.DropDownStyle = ComboBoxStyle.DropDownList
                                        If existDomName <> pCV.TableName Then

                                            pCBox.Tag = strFld & "|" & pCV.TableName

                                            'If the domain has two values, remove the combo box and add a custompanel
                                            If pCV.Rows.Count = 0 Then
                                                'MsgBox("ERROR: There are no values set up in the domain: " & pCV.TableName & " in layer: " & m_FL.Name & ".  This field will be skipped")
                                            Else
                                                If pCV.Rows.Count = 2 And m_RadioOnTwo Then
                                                    Dim pNewGpBox As New CustomPanel
                                                    Dim pRDButton As RadioButton
                                                    pNewGpBox.Tag = pCBox.Tag
                                                    pNewGpBox.BorderStyle = Windows.Forms.BorderStyle.None
                                                    pNewGpBox.BackColor = Color.White
                                                    '  pNewGpBox.BorderColor = Pens.LightGray

                                                    pNewGpBox.Width = pCBox.Width
                                                    pNewGpBox.Top = pCBox.Top
                                                    pNewGpBox.Left = pCBox.Left

                                                    pRDButton = New RadioButton
                                                    AddHandler pRDButton.CheckedChanged, AddressOf controlLeave

                                                    pRDButton.Name = "Rdo1"
                                                    pRDButton.Tag = pCV.Rows(0)("Code")
                                                    ' pRDButton.Text = pCV.Rows(0)("Value").ToString
                                                    If (pCV.Rows(0)("Value").ToString.Length > 14) Then
                                                        pRDButton.Text = pCV.Rows(0)("Value").ToString.Substring(0, 14)

                                                    Else
                                                        pRDButton.Text = pCV.Rows(0)("Value").ToString

                                                    End If
                                                    pRDButton.Left = pLeftPadding

                                                    pRDButton.AutoSize = True
                                                    ' AddHandler pRDButton.CheckedChanged, AddressOf controlLeave
                                                    'AddHandler pRDButton.Enter, AddressOf controlEntered
                                                    pNewGpBox.Controls.Add(pRDButton)


                                                    pNewGpBox.Height = pRDButton.Height + 12
                                                    pRDButton.Top = CInt(pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2)


                                                    pRDButton = New RadioButton
                                                    AddHandler pRDButton.CheckedChanged, AddressOf controlLeave
                                                    pRDButton.Name = "Rdo2"

                                                    pRDButton.Tag = pCV.Rows(1)("Code")
                                                    'pRDButton.Text = pCV.Rows(1)("Value").ToString
                                                    If (pCV.Rows(1)("Value").ToString.Length > 14) Then
                                                        pRDButton.Text = pCV.Rows(1)("Value").ToString.Substring(0, 14)

                                                    Else
                                                        pRDButton.Text = pCV.Rows(1)("Value").ToString

                                                    End If
                                                    pRDButton.Left = CInt(pNewGpBox.Width / 2)

                                                    pRDButton.AutoSize = True
                                                    'AddHandler pRDButton.Leave, AddressOf controlLeave

                                                    'AddHandler pRDButton.Enter, AddressOf controlEntered

                                                    'AddHandler pNewGpBox.Leave, AddressOf controlLeave
                                                    'AddHandler pNewGpBox.Enter, AddressOf controlEntered
                                                    pNewGpBox.Controls.Add(pRDButton)

                                                    pRDButton.Top = CInt(pNewGpBox.Height / 2 - pRDButton.Height / 2 - 2)


                                                    tbPg.Controls.Add(pNewGpBox)
                                                    Try

                                                        tbPg.Controls.Remove(pCBox)
                                                        'Dim cnts() As Control = tbPg.Controls.Find("lblEdit" & strFld, False)
                                                        'If cnts.Length > 0 Then
                                                        '    tbPg.Controls.Remove(cnts(0))
                                                        'End If


                                                    Catch ex As Exception

                                                    End Try

                                                    pNewGpBox = Nothing
                                                    pRDButton = Nothing

                                                Else
                                                    'Set the domain value
                                                    'pCV.Columns(0).AllowDBNull = True
                                                    'pCV.Columns(1).AllowDBNull = True

                                                    'pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never

                                                    'pCBox.DataSource = pCV


                                                    pCBox.Items.Clear()

                                                    pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never
                                                    pCV.Columns(0).AllowDBNull = True
                                                    pCV.Columns(1).AllowDBNull = True
                                                    If m_FL.Columns(strFld).AllowDBNull Then
                                                        Dim pDT As DataTable
                                                        pDT = pCV.DefaultView.ToTable()
                                                        'pDT.Rows.Add(DBNull.Value, GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown)
                                                        Dim pDR As DataRow = pDT.NewRow
                                                        pDR.Item(0) = DBNull.Value
                                                        pDR.Item(1) = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                                        pDT.Rows.InsertAt(pDR, 0)
                                                        For Each dr As DataRow In pDT.Rows

                                                            pCBox.Items.Add(New cValue(dr(1).ToString, dr(0)))

                                                        Next
                                                    Else
                                                        'pCBox.DataSource = pCV
                                                        For Each dr As DataRow In pCV.Rows

                                                            pCBox.Items.Add(New cValue(dr(1).ToString, dr(0)))

                                                        Next

                                                    End If



                                                    'pCBox.DisplayMember = "Value"
                                                    'pCBox.ValueMember = "Code"
                                                    pCBox.DisplayMember = "Display"
                                                    pCBox.ValueMember = "Value"
                                                    pCBox.Visible = True
                                                    pCBox.Refresh()
                                                    If m_FL.Columns(strFld).DefaultValue IsNot Nothing Then
                                                        If m_FL.Columns(strFld).DefaultValue IsNot DBNull.Value Then
                                                            For Each itm As cValue In pCBox.Items
                                                                If itm.Value = m_FL.Columns(strFld).DefaultValue Then
                                                                    pCBox.SelectedItem = itm

                                                                End If
                                                            Next
                                                            '  pCBox.Text = m_FL.Columns(strFld).DefaultValue.ToString()
                                                        ElseIf m_FL.Columns(strFld).AllowDBNull Then
                                                            pCBox.SelectedItem = pCBox.Items.Item(0)


                                                            'Try



                                                            '    pCBox.Text = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown.ToString()

                                                            'Catch ex As Exception

                                                            'End Try
                                                        Else
                                                            If pCBox.DataSource Is Nothing Then
                                                                pCBox.SelectedItem = pCBox.Items(0)
                                                            Else
                                                                pCBox.Text = pCBox.DataSource.Rows(0)("Value").ToString
                                                            End If


                                                        End If
                                                    ElseIf m_FL.Columns(strFld).AllowDBNull Then
                                                        ' pCBox.Text = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                                    Else
                                                        If pCBox.DataSource Is Nothing Then
                                                            pCBox.SelectedItem = pCBox.Items(0)
                                                            'pCBox.SelectedItem = pCBox.Items.Item(0)
                                                        Else
                                                            pCBox.Text = pCBox.DataSource.Rows(0)("Value").ToString
                                                        End If


                                                        '  pCBox.Text = pCBox.DataSource.Rows(0)("Value").ToString
                                                    End If

                                                    ' pCBox.Text = pCV.Rows(0)("Value").ToString
                                                End If

                                            End If
                                        End If

                                    End If
                                End If
                                'If the contorl is a coded value domain with two values
                            ElseIf TypeOf cntrlPnl Is CustomPanel Then


                                'Get the Field
                                strFld = cntrlPnl.Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    Dim strArr() As String = strFld.Split("|")


                                    strFld = Trim(strArr(0))
                                    existDomName = Trim(strArr(1))
                                End If
                                'Get the domain

                                'MsgBox("Fix Domain")

                                pCV = CType(m_FL.Columns(strFld).GetDomain(intSubVal), CodedValueDomain)

                                'pCV = CType(m_FL.Domain(intSubVal, strFld), CodedValueDomain)
                                If pCV Is Nothing Then
                                    pCBox.DataSource = Nothing
                                    pCBox.Items.Clear()
                                    pCBox.DropDownStyle = ComboBoxStyle.DropDown
                                    pCBox.Text = ""
                                    pCBox.Tag = strFld & "|" & ""
                                Else
                                    If existDomName <> pCV.TableName Then
                                        pCBox.Tag = strFld & "|" & pCV.TableName
                                        'If the domain has more than two values, remove the custompanel and add a combo box 
                                        If pCV.Rows.Count = 0 Then
                                            'MsgBox("ERROR: There are no values set up in the domain: " & pCV.TableName & " in layer: " & m_FL.Name & ".  This field will be skipped")
                                        Else
                                            If pCV.Rows.Count = 2 And m_RadioOnTwo Then
                                                Try
                                                    'Set up the proper domain values
                                                    Dim pRdoBut As RadioButton
                                                    pRdoBut = CType(cntrlPnl.Controls("Rdo1"), RadioButton)
                                                    pRdoBut.Tag = pCV.Rows(0)("Code")
                                                    pRdoBut.Text = pCV.Rows(0)("Value").ToString

                                                    pRdoBut = CType(cntrlPnl.Controls("Rdo2"), RadioButton)
                                                    pRdoBut.Tag = pCV.Rows(1)("Code")
                                                    pRdoBut.Text = pCV.Rows(1)("Value").ToString
                                                Catch ex As Exception

                                                End Try

                                            Else

                                                pCBox = New ComboBox
                                                pCBox.Tag = strFld & "|" & pCV.TableName
                                                pCBox.Name = "cboEdt" & strFld
                                                pCBox.Left = cntrlPnl.Left
                                                pCBox.Top = cntrlPnl.Top
                                                pCBox.Width = cntrlPnl.Width
                                                pCBox.Height = pCBox.Height + 5
                                                pCBox.DropDownStyle = ComboBoxStyle.DropDownList

                                                'pCV.Columns(0).AllowDBNull = True
                                                'pCV.Columns(1).AllowDBNull = True
                                                ''If pCV.Rows(0).Item(0) IsNot DBNull.Value Then
                                                ''    Dim pNR As DataRow = pCV.NewRow

                                                ''    pNR.Item("Value") = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                                ''    pCV.Rows.InsertAt(pNR, 0)
                                                ''End If
                                                'pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never

                                                'pCBox.DataSource = pCV


                                                pCBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.Never
                                                pCV.Columns(0).AllowDBNull = True
                                                pCV.Columns(1).AllowDBNull = True
                                                If m_FL.Columns(strFld).AllowDBNull Then
                                                    Dim pDT As DataTable
                                                    pDT = pCV.DefaultView.ToTable()
                                                    'pDT.Rows.Add(DBNull.Value, GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown)
                                                    Dim pDR As DataRow = pDT.NewRow
                                                    pDR.Item(0) = DBNull.Value
                                                    pDR.Item(1) = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                                    pDT.Rows.InsertAt(pDR, 0)
                                                    For Each dr As DataRow In pDT.Rows

                                                        pCBox.Items.Add(New cValue(dr(1).ToString, dr(0)))

                                                    Next
                                                Else
                                                    'pCBox.DataSource = pCV
                                                    For Each dr As DataRow In pCV.Rows

                                                        pCBox.Items.Add(New cValue(dr(1).ToString, dr(0)))

                                                    Next

                                                End If



                                                'pCBox.DisplayMember = "Value"
                                                'pCBox.ValueMember = "Code"
                                                pCBox.DisplayMember = "Display"
                                                pCBox.ValueMember = "Value"
                                                If m_FL.Columns(strFld).DefaultValue IsNot Nothing Then
                                                    If m_FL.Columns(strFld).DefaultValue IsNot DBNull.Value Then
                                                        pCBox.Text = m_FL.Columns(strFld).DefaultValue
                                                    ElseIf m_FL.Columns(strFld).AllowDBNull Then
                                                        pCBox.Text = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                                    Else
                                                        pCBox.Text = pCBox.DataSource.Rows(0)("Value").ToString
                                                    End If
                                                ElseIf m_FL.Columns(strFld).AllowDBNull Then
                                                    pCBox.Text = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.NullValueDropDown
                                                Else
                                                    pCBox.Text = pCBox.DataSource.Rows(0)("Value").ToString
                                                End If


                                                ' pCmdBox.MaxLength = pDc.MaxLength


                                                tbPg.Controls.Add(pCBox)
                                                ' MsgBox(pCBox.Items.Count)

                                                'pCBox.Text = pCV.Rows(0)("Value").ToString
                                                pCBox.Visible = True
                                                pCBox.Refresh()

                                                tbPg.Controls.Remove(cntrlPnl)

                                                pCBox = Nothing

                                            End If
                                        End If
                                    End If

                                End If
                                'If the contorl is a range domain
                            ElseIf TypeOf cntrlPnl Is NumericUpDown Then
                                'get the control
                                pNUP = CType(cntrlPnl, NumericUpDown)
                                'Get the field
                                strFld = pNUP.Tag.ToString
                                If strFld.IndexOf("|") > 0 Then
                                    Dim strArr() As String = strFld.Split("|")


                                    strFld = Trim(strArr(0))
                                    existDomName = Trim(strArr(1))
                                End If

                                'Get the domain

                                'MsgBox("Fix Domain")
                                pRg = CType(m_FL.Columns(strFld).GetDomain(intSubVal), RangeValueDomain)

                                'pRg = CType(m_FL.Domain(intSubVal, strFld), RangeValueDomain)
                                If pRg Is Nothing Then
                                    pNUP.Enabled = False

                                Else
                                    If existDomName <> pRg.Name Then
                                        pNUP.Tag = strFld & "|" & pRg.Name
                                        pNUP.Enabled = True
                                        pNUP.Minimum = CDec(pRg.MinimumValue)
                                        pNUP.Maximum = CDec(pRg.MaximumValue)
                                    End If
                                End If


                                pNUP.Refresh()
                            End If
                        Next

                    End If
                Next
            Next
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub
    Private Sub btnPromptClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim pSelectFrm As New frmSelectOption(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.PromptPictureMessage, GlobalsFunctions.appConfig.EditControlOptions.UIComponents.PromptPictureCamera, GlobalsFunctions.appConfig.EditControlOptions.UIComponents.PromptPictureBrowse)

        pSelectFrm.ShowDialog()
        If pSelectFrm.selectedOption = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.PromptPictureCamera Then
            pSelectFrm = Nothing
            Call btnCamClick(sender, e)


        ElseIf pSelectFrm.selectedOption = GlobalsFunctions.appConfig.EditControlOptions.UIComponents.PromptPictureBrowse Then
            pSelectFrm = Nothing
            Call btnLoadImgClick(sender, e)
        Else
            pSelectFrm = Nothing

        End If
    End Sub

    Private Sub btnCamClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Try

            Dim controls() As Control = CType(CType(sender, Button).Parent, Panel).Controls.Find("txtEdit" & CType(sender, Button).Tag.ToString, False)
            'If the control was found
            If controls.Length > 0 Then
                Dim strPath As String = GlobalsFunctions.getImageFromCam().ToString()

                If UpdateField(CType(sender, Button).Tag.ToString, strPath, False, "FALSE") = False Then
                    controls(0).Text = ""
                Else
                    controls(0).Text = strPath
                End If

            End If

            If m_HasFilter Then
                ShuffleControls()
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub btnLoadImgClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            'Opens a dialog to browse out for an image
            Dim openFileDialog1 As System.Windows.Forms.OpenFileDialog

            openFileDialog1 = New System.Windows.Forms.OpenFileDialog()



            'Filter the image types
            openFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF"
            'If the user selects an image
            If openFileDialog1.ShowDialog() = DialogResult.OK Then
                'Set the path of the image to the text box
                Dim controls() As Control = CType(CType(sender, Button).Parent, Panel).Controls.Find("txtEdit" & CType(sender, Button).Tag.ToString, False)
                'If the control was found
                If controls.Length > 0 Then

                    If UpdateField(CType(sender, Button).Tag, openFileDialog1.FileName, False, "FALSE") = False Then
                        controls(0).Text = ""
                    Else
                        controls(0).Text = openFileDialog1.FileName
                    End If


                End If


            End If
            If m_HasFilter Then
                ShuffleControls()
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub



    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        btnMove.CheckState = CheckState.Unchecked
        ' btnMove.Checked = False
        If m_FDR Is Nothing Then Return

        setCurRec(m_FDR.Table.NewRow)
        ' loadRecordEditor(True)
        disableSaveBtn()
        disableDeleteBtn()
        '  RaiseEvent MoveGeo(False)
        RaiseEvent RecordClear()

        m_Map.Invalidate()



    End Sub
    Private Sub pBtnFlashClick(ByVal sender As Object, ByVal e As System.EventArgs)
        'called by the zoom or flash button
        If m_FDR Is Nothing Then Return
        If m_FDR.Geometry Is Nothing Then Return
        If m_FDR.Geometry.IsEmpty Then Return
        Dim pGeo As Geometry = CType(m_FDR.Geometry.Clone, Geometry)


        If CType(sender, Button).Name.Contains("FlashAtt") Then
            If pGeo.GeometryType = GeometryType.Point Then
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_penFlash, m_brushFlash)
            Else
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_penLineFlash, m_brushFlash)
            End If

        ElseIf CType(sender, Button).Name.Contains("RouteTo") Then
            ' RaiseEvent RouteTo(m_CurrentRow.Geometry, m_CurrentRow(m_CurrentRow.FeatureSource.DisplayColumnIndex).ToString)
        Else

            GlobalsFunctions.zoomTo(pGeo, m_Map)
            If pGeo.GeometryType = GeometryType.Point Then
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_penFlash, m_brushFlash)
            Else
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_penLineFlash, m_brushFlash)
            End If
        End If


    End Sub
    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        'Saves the record
        If m_AttControlBox IsNot Nothing Then
            If m_AttControlBox.hasSketches Then
                Dim pSelectFrm As New frmSelectOption(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.PromptForSaveSketch, GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.YesText, GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.NoText)

                pSelectFrm.ShowDialog()
                If pSelectFrm.selectedOption = GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.YesText Then

                    m_AttControlBox.saveSketch()

                Else
                    m_AttControlBox.ClearGraphLayer()

                End If
            End If

        End If
        saveRecord()

        If CloseOnSave Then
            Me.ParentForm.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.ParentForm.Close()


        End If
        If btnMove.Enabled Then
            If btnMove.Checked Then
                btnMove.Checked = False

            End If
        End If
        If tbCntrlEdit IsNot Nothing Then
            If tbCntrlEdit.TabPages IsNot Nothing Then
                If tbCntrlEdit.TabPages.Count > 0 Then
                    tbCntrlEdit.SelectedIndex = 0
                End If
            End If
        End If
    End Sub

    Private Sub btnSave_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.EnabledChanged
        If btnSave.Enabled Then
            btnSave.BackgroundImage = My.Resources.SaveGreen

        Else
            btnSave.BackgroundImage = My.Resources.SaveGray

        End If
    End Sub
    Private Sub btnDelete_Click(sender As System.Object, e As System.EventArgs) Handles btnDelete.Click

        Dim pSelectFrm As New frmSelectOption(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.PromptForDelete, GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.YesText, GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.NoText)

        pSelectFrm.ShowDialog()
        If pSelectFrm.selectedOption = GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.YesText Then

            deleteRecord()


        End If
        pSelectFrm = Nothing
        m_Map.Invalidate()
        m_Map.Refresh()
    End Sub
    Private Sub btnDelete_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.EnabledChanged
        If btnDelete.Enabled Then
            btnDelete.BackgroundImage = My.Resources.DeleteGreen

        Else
            btnDelete.BackgroundImage = My.Resources.DeleteGray

        End If
    End Sub


    Private Sub btnMove_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMove.CheckedChanged
        btnMoveStatusChanged()
    End Sub
    Public Sub hideMoveMessage()
        RaiseEvent RaisePermMessage("", False)
        m_Map.Invalidate()
    End Sub
    Public Sub btnMoveStatusChanged()

        If btnMove.Enabled Then
            If btnMove.Checked Then
                btnGPSLoc.Enabled = True
                RaiseEvent MoveGeo(True)
                RaiseEvent RaisePermMessage(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.EditingGeoMessage, True)
                btnMove.BackgroundImage = My.Resources.MoveRed
                'If m_GPSVal = "" Then
                '    btnGPSLoc.Enabled = True

                'End If
            Else
                btnGPSLoc.Enabled = False
                RaiseEvent MoveGeo(False)
                RaiseEvent RaisePermMessage("", False)
                btnMove.BackgroundImage = My.Resources.moveGreen
            End If
        Else
            btnGPSLoc.Enabled = False
            RaiseEvent MoveGeo(False)
            RaiseEvent RaisePermMessage("", False)
            btnMove.BackgroundImage = My.Resources.GrayMove

        End If




        m_Map.Invalidate()
    End Sub

    Private Sub EditControl_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Try
            If m_Map.Disposing = False Then
                '   m_Map.Dispose()
            End If
            m_Map = Nothing
            m_FDR = Nothing
            If m_Fnt IsNot Nothing Then
                m_Fnt.Dispose()
            End If
            m_Fnt = Nothing
            If m_FntLbl IsNot Nothing Then
                m_FntLbl = Nothing
            End If
            m_FntLbl = Nothing
            m_CloseParentOnSave = Nothing

            m_DrawGeo = Nothing
            m_Pen = Nothing
            m_Brush = Nothing
            m_PointSize = Nothing

            If m_penLineFlash IsNot Nothing Then

                ' m_penLine.Dispose()
            End If
            m_penLineFlash = Nothing
            If m_penFlash IsNot Nothing Then
                '      m_pen.Dispose()
            End If
            m_penFlash = Nothing

            If m_brushFlash IsNot Nothing Then
                m_brushFlash.Dispose()
            End If
            m_brushFlash = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub EditControl_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        relocatebtns()

    End Sub
    'Handles resizing the control,
    Private Sub EditControl_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Try
            'If there is a record set
            If m_FDR IsNot Nothing Then
                'Shuffle the controls

                ShuffleControls()
                'Enable/Disable the buttons
                disableSaveBtn()
                disableDeleteBtn()

            End If
            If MyBase.Height > 60 Then
                If Me.Visible Then
                    If spCntMain.Panel1Collapsed = False Then
                        If MyBase.Height - spCntMain.Panel1.Height > 60 Then
                            spltContEdit.SplitterDistance = MyBase.Height - spCntMain.Panel1.Height - 60
                        End If

                    Else
                        spltContEdit.SplitterDistance = MyBase.Height - 60
                    End If

                End If
                relocatebtns()

            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Public Sub relocatebtns()
        Dim intNumOn As Integer = 0

        For Each Control In pnlEditBtns.Controls
            If TypeOf (Control) Is Button Then
                If CType(Control, Button).Visible Then
                    intNumOn = intNumOn + 1
                End If
            ElseIf TypeOf (Control) Is CheckBox Then
                If CType(Control, CheckBox).Visible Then
                    intNumOn = intNumOn + 1
                End If
            End If
        Next
        Dim btnLoc As Double = (pnlEditBtns.Width) / (intNumOn + 1)

        Dim btnHt As Double = pnlEditBtns.Height / 2
        Dim curLoc As Double = btnLoc
        For Each Control In pnlEditBtns.Controls
            If TypeOf (Control) Is Button Then
                If CType(Control, Button).Visible Then
                    CType(Control, Button).Left = CInt(curLoc - (CType(Control, Button).Width / 2))
                    CType(Control, Button).Top = CInt(btnHt - (CType(Control, Button).Height / 2))
                    curLoc = curLoc + btnLoc

                End If
            ElseIf TypeOf (Control) Is CheckBox Then
                If CType(Control, CheckBox).Visible Then
                    CType(Control, CheckBox).Left = CInt(curLoc - (CType(Control, CheckBox).Width / 2))
                    CType(Control, CheckBox).Top = CInt(btnHt - (CType(Control, CheckBox).Height / 2))
                    curLoc = curLoc + btnLoc
                End If
            End If
        Next



        'btnSave.Left = 15
        'btnClear.Left = spltContEdit.Width - 15 - btnClear.Width
        'If btnMove.Visible And btnGPSLoc.Visible Then
        '    btnMove.Left = CInt(btnMove.Parent.Width / 2 - btnMove.Width - 15)
        '    btnGPSLoc.Left = CInt(btnGPSLoc.Parent.Width / 2 + 15)

        'ElseIf btnMove.Visible Then
        '    btnMove.Left = CInt((btnMove.Parent.Width / 2) - (btnMove.Width / 2))

        'ElseIf btnGPSLoc.Visible Then
        '    btnGPSLoc.Left = CInt((btnGPSLoc.Parent.Width / 2) - (btnGPSLoc.Width / 2))

        'End If

    End Sub
    Private Sub SplitContainer1_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles spltContEdit.Resize

    End Sub

    Private Sub m_Map_Paint(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapPaintEventArgs) Handles m_Map.MapPaint
        Try

            If m_FDR Is Nothing Then Return
            If m_FDR.RowState = DataRowState.Deleted Then Return
            If e.MapSurface Is Nothing Then Return

            'If drawing the geo of the editable record
            If m_DrawGeo Then
                If m_FDR.RowState <> DataRowState.Deleted Then


                    If m_FDR.Geometry IsNot Nothing Then
                        If m_FDR.Geometry.IsValid Then


                            e.MapSurface.DrawGeometry(m_Pen, m_Brush, m_PointSize, m_FDR.Geometry)
                            If (m_FDR.Geometry.GeometryType = GeometryType.Polyline) Then
                                If m_FDR.Geometry.CurrentCoordinate IsNot Nothing Then
                                    e.MapSurface.DrawGeometry(m_Pen, m_Brush, m_PointSize, New Esri.ArcGIS.Mobile.Geometries.Point(m_FDR.Geometry.CurrentCoordinate))
                                End If


                            End If
                        Else
                            If (m_FDR.Geometry.GeometryType = GeometryType.Polygon) Then

                            ElseIf (m_FDR.Geometry.GeometryType = GeometryType.Polyline) Then

                                If CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polyline).Parts(0).Count = 1 Then
                                    e.MapSurface.DrawGeometry(m_Pen, m_Brush, m_PointSize, New Esri.ArcGIS.Mobile.Geometries.Point(CType(m_FDR.Geometry, Esri.ArcGIS.Mobile.Geometries.Polyline).Parts(0)(0)))

                                End If
                            End If


                        End If
                    End If
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub
#End Region

    Public Sub EnableGPS(status As Boolean)
        btnGPSLoc.Enabled = status

    End Sub

    Private Sub btnGPSLoc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGPSLoc.Click
        btnGPSLoc.Enabled = False

        m_GPSVal = Nothing

        RaiseEvent CheckGPS()

    End Sub

    Private Sub spltContEdit_Panel2_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles spltContEdit.Panel2.Paint

    End Sub

    Private Sub btnMove_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMove.EnabledChanged
        If btnMove.Visible = False Then Return

        If btnMove.Enabled Then
            If btnMove.Checked = True Then
                btnGPSLoc.Enabled = m_GPSStatus
            Else
                btnGPSLoc.Enabled = False
            End If

        Else
            If m_FDR IsNot Nothing Then
                If m_FDR.StoredEditSate = EditState.NotDefined Then
                    btnGPSLoc.Enabled = m_GPSStatus

                Else
                    btnGPSLoc.Enabled = False
                End If
            Else
                btnGPSLoc.Enabled = False
            End If

        End If
    End Sub
    Private Sub btnGPSLoc_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGPSLoc.EnabledChanged
        If btnGPSLoc.Enabled Then
            btnGPSLoc.BackgroundImage = My.Resources.SatGreen

        Else
            btnGPSLoc.BackgroundImage = My.Resources.SatImageGraypng
        End If

    End Sub

    Private Sub btnSave_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles btnSave.Paint

    End Sub

    Private Sub btnSave_Validated(sender As System.Object, e As System.EventArgs) Handles btnSave.Validated

    End Sub

    Private Sub AttachmentSelected(filename As attFiles)


    End Sub

    Private Sub DeleteAttachment(id As Integer)
        m_lstAttToDel.Add(id)


    End Sub


End Class
