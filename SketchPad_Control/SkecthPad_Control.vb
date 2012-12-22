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


Imports System.Drawing
Imports Esri.ArcGISTemplates.GlobalsFunctions

Imports System.Windows.forms

Public Class MobileSkecthPad

    ' The object we are currently drawing.
    Private m_NewDrawable As Drawable
    Private m_EraseDrawable As ArrayList

    ' Holds the picture we are drawing.
    Private m_Picture As New DrawablePicture(Color.White)
    Private m_Fnt As Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_backGroundPic As Bitmap = Nothing


    ' The current drawing attributes.
    Private m_CurrentLineWidth As Integer = 3
    Private m_CurrentForeColor As Color = Color.Black
    Private m_CurrentFillColor As Color = Color.White
    Private m_SketchTool As DrawTypes = DrawTypes.Pointer
    'Ink Collector
    ' Private WithEvents m_inkCollector As InkCollector
    Private m_SketchCanvas As Control
    Private m_picCanvas As PictureBox
    Private WithEvents m_pSpliCont As SplitContainer
    Public Event InkNotInstalled()
    Private m_PenLine As Pen = Pens.LightBlue
    Private m_BrushBackColor As Brush = Brushes.White

    Private m_BlockSize As Integer = 25
    Private m_Label As Boolean = True
    Private m_Scale As Integer = 10

    Public Event GetMapImage(ByVal width As Integer, ByVal height As Integer)

#Region "Canvas Events"
    ' Perform an action depending on the currently pushed tool.
    Private Sub m_picCanvas_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            ' See which button was pressed.
            If e.Button = MouseButtons.Right Then
                ' Right button. See if we're drawing something.
                If m_NewDrawable Is Nothing Then
                    ' We are not drawing. Ignore this button.
                Else
                    If TypeOf m_NewDrawable Is DrawablePolyLine Then

                        ' No longer watch for MouseMove or MouseUp.
                        RemoveHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove

                        RemoveHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                        RemoveHandler m_picCanvas.MouseDoubleClick, AddressOf NewDrawable_MouseDoubleClick
                        ' See if the new object is empty (e.g. a zero-length line).
                        If m_NewDrawable.IsEmpty() Then
                            ' Discard this object.
                            m_Picture.Remove(m_NewDrawable)
                        End If

                        ' We're no longer working with the new object.
                        m_NewDrawable = Nothing

                        ' Redraw.
                        m_picCanvas.Invalidate()
                        Return

                    End If
                End If
            Else
                m_EraseDrawable = New ArrayList
                ' Left button. See which tool is pushed.
                Select Case m_SketchTool
                    Case DrawTypes.None
                    Case DrawTypes.Ink
                        ' Start drawing a line.
                        If e.Clicks = 1 Then


                            If TypeOf m_NewDrawable Is DrawablePolygonFreehand Then
                                m_NewDrawable = New DrawablePolygonFreehand(m_CurrentForeColor, m_CurrentLineWidth, e.X, e.Y, m_Label, m_BlockSize, m_Scale)
                                m_Picture.Add(m_NewDrawable)
                            Else
                                m_NewDrawable = New DrawablePolygonFreehand(m_CurrentForeColor, m_CurrentLineWidth, e.X, e.Y, m_Label, m_BlockSize, m_Scale)
                                m_Picture.Add(m_NewDrawable)

                                AddHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove
                                AddHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                                AddHandler m_picCanvas.MouseDoubleClick, AddressOf NewDrawable_MouseDoubleClick
                            End If
                        Else

                        End If
                    Case DrawTypes.Pointer
                        ' Select an object.
                        m_Picture.SelectObjectAt(e.X, e.Y)
                    Case DrawTypes.Text
                        'Add Text.

                        Dim pStr As String = appConfig.SketchPanel.UIComponents.TextPromptText
                        Dim gr As Graphics = m_picCanvas.CreateGraphics()
                        Dim pSz As SizeF = gr.MeasureString(pStr, New Font("Veranda", 10, FontStyle.Bold))

                        m_NewDrawable = New DrawableText(m_CurrentForeColor, pStr, m_CurrentLineWidth, e.X, e.Y, CInt(e.X + pSz.Width), CInt(e.Y + pSz.Height))
                        m_Picture.Add(m_NewDrawable)

                    Case DrawTypes.Line
                        ' Start drawing a line.

                        m_NewDrawable = New DrawableLine(m_CurrentForeColor, m_CurrentLineWidth, e.X, e.Y, e.X, e.Y, m_Label, m_BlockSize, m_Scale)
                        m_Picture.Add(m_NewDrawable)
                        AddHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove
                        AddHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                    Case DrawTypes.Polyline
                        ' Start drawing a line.
                        If e.Clicks = 1 Then


                            If TypeOf m_NewDrawable Is DrawablePolyLine Then
                                m_NewDrawable = New DrawablePolyLine(m_CurrentForeColor, m_CurrentLineWidth, e.X, e.Y, e.X, e.Y, m_Label, m_BlockSize, m_Scale)
                                m_Picture.Add(m_NewDrawable)
                            Else
                                m_NewDrawable = New DrawablePolyLine(m_CurrentForeColor, m_CurrentLineWidth, e.X, e.Y, e.X, e.Y, m_Label, m_BlockSize, m_Scale)
                                m_Picture.Add(m_NewDrawable)

                                AddHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove
                                AddHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                                AddHandler m_picCanvas.MouseDoubleClick, AddressOf NewDrawable_MouseDoubleClick
                            End If
                        Else

                        End If
                    Case DrawTypes.Rectangle
                        ' Start drawing a rectangle.
                        m_NewDrawable = New DrawableRectangle(m_CurrentForeColor, m_CurrentFillColor, m_CurrentLineWidth, e.X, e.Y, e.X, e.Y)
                        m_Picture.Add(m_NewDrawable)
                        AddHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove
                        AddHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                        '       AddHandler m_picCanvas.MouseDoubleClick, AddressOf NewDrawable_MouseDoubleClick
                    Case DrawTypes.Ellipse
                        ' Start drawing an ellipse.
                        m_NewDrawable = New DrawableEllipse(m_CurrentForeColor, m_CurrentFillColor, m_CurrentLineWidth, e.X, e.Y, e.X, e.Y)
                        m_Picture.Add(m_NewDrawable)
                        AddHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove
                        AddHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                        '      AddHandler m_picCanvas.MouseDoubleClick, AddressOf NewDrawable_MouseDoubleClick
                    Case DrawTypes.Star
                        ' Start drawing a star.
                        m_NewDrawable = New DrawableStar(m_CurrentForeColor, m_CurrentFillColor, m_CurrentLineWidth, e.X, e.Y, e.X, e.Y)
                        m_Picture.Add(m_NewDrawable)
                        AddHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove
                        AddHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                        '    AddHandler m_picCanvas.MouseDoubleClick, AddressOf NewDrawable_MouseDoubleClick
                End Select

                ' Redraw.
                m_picCanvas.Invalidate()
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub drawGraphPaper(ByVal g As Graphics)

        g.FillRectangle(m_BrushBackColor, 0, 0, m_picCanvas.Width, m_picCanvas.Height)
        Dim pCurLine As Integer = 0
        While pCurLine < m_picCanvas.Width
            g.DrawLine(m_PenLine, pCurLine, 0, pCurLine, m_picCanvas.Height)


            pCurLine = pCurLine + m_BlockSize

        End While
        pCurLine = 0
        While pCurLine < m_picCanvas.Height
            g.DrawLine(m_PenLine, 0, pCurLine, m_picCanvas.Width, pCurLine)


            pCurLine = pCurLine + m_BlockSize

        End While
        'g.Dispose()
        'g = Nothing

    End Sub
    Private Sub m_picCanvas_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        Try
            If m_picCanvas.BackgroundImage Is Nothing Then
                'draw the graph paper
                drawGraphPaper(e.Graphics)


            End If

            'Draw the sketched graphics
            m_Picture.Draw(e.Graphics)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    ' On mouse move, continue drawing.
    Private Sub NewDrawable_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            If m_NewDrawable Is Nothing Then Exit Sub

            '   If m_NewDrawable Is Nothing Then Return
            ' Update the new line's coordinates.
            m_NewDrawable.NewPoint(e.X, e.Y)

            ' Redraw to show the new line.
            m_picCanvas.Invalidate()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    ' On mouse up, finish drawing the new object.
    Private Sub NewDrawable_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            If m_NewDrawable Is Nothing Then Return

            If TypeOf m_NewDrawable Is DrawablePolyLine Then
                ' Redraw.
                m_picCanvas.Invalidate()

            Else

                ' No longer watch for MouseMove or MouseUp.
                RemoveHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove
                RemoveHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                RemoveHandler m_picCanvas.MouseDoubleClick, AddressOf NewDrawable_MouseDoubleClick
                ' See if the new object is empty (e.g. a zero-length line).
                If m_NewDrawable.IsEmpty() Then
                    ' Discard this object.
                    m_Picture.Remove(m_NewDrawable)
                End If

                ' We're no longer working with the new object.
                m_NewDrawable = Nothing

                ' Redraw.
                m_picCanvas.Invalidate()
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub NewDrawable_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try


            If TypeOf m_NewDrawable Is DrawablePolyLine Then

                ' No longer watch for MouseMove or MouseUp.
                RemoveHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove
                RemoveHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                RemoveHandler m_picCanvas.MouseDoubleClick, AddressOf NewDrawable_MouseDoubleClick
                ' m_Picture.Drawables.RemoveAt(m_Picture.Drawables.Count - 1)

                ' See if the new object is empty (e.g. a zero-length line).
                'If m_NewDrawable.IsEmpty() Then
                '    ' Discard this object.
                '    m_Picture.Remove(m_NewDrawable)
                'End If
                Try


                    m_Picture.Delete(m_NewDrawable)
                Catch ex As Exception

                End Try
                Try


                    m_Picture.Remove(m_NewDrawable)
                Catch ex As Exception

                End Try
                ' We're no longer working with the new object.
                m_NewDrawable = Nothing

                ' Redraw.
                m_picCanvas.Invalidate()
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
#End Region
#Region "PublicFunction"
    Public Sub New(ByVal ControlForSketch As Control)
        Try
            'Set up controls
            m_SketchCanvas = ControlForSketch
            'Add the sketch pad
            AddSketchPad()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Public Function createImage(ByVal Clip As Boolean) As Bitmap
        Try
            'Create a new bitmap


            Return CreateBitmapClip(m_picCanvas.Width, m_picCanvas.Height)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return Nothing
        End Try
    End Function
    Public Function SaveImage(ByVal strFileName As String, ByVal Clip As Boolean) As Boolean
        Try
            'Save the image to a file
            m_Picture.SaveBitMapToFile(CreateBitmapClip(m_picCanvas.Width, m_picCanvas.Height), strFileName)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Function
    Public Function CreateBitmapClip(ByVal width As Integer, ByVal Height As Integer) As Bitmap

        Dim b2 As Bitmap
        Dim g1 As Graphics


        b2 = New Bitmap(width, Height)

        g1 = Graphics.FromImage(b2)



        Try
            'draw the graph paper
            drawGraphPaper(g1)
            'Draw the sketched graphics
            m_Picture.Draw(g1)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
        Return b2


    End Function
    Public Sub Dispose()
        Try

            If m_backGroundPic IsNot Nothing Then
                m_backGroundPic.Dispose()

            End If
            m_backGroundPic = Nothing

            m_NewDrawable = Nothing
            m_EraseDrawable = Nothing
            m_Picture = Nothing


            m_CurrentForeColor = Nothing
            m_CurrentFillColor = Nothing
            m_SketchTool = Nothing
            'If m_inkCollector IsNot Nothing Then
            '    m_inkCollector.Dispose()
            'End If

            'm_inkCollector = Nothing
            If m_SketchCanvas IsNot Nothing Then
                m_SketchCanvas.Dispose()
            End If
            m_SketchCanvas = Nothing
            If m_picCanvas IsNot Nothing Then
                m_picCanvas.Dispose()
            End If
            If m_pSpliCont IsNot Nothing Then
                m_pSpliCont.Dispose()
            End If
            m_pSpliCont = Nothing
            If m_Fnt IsNot Nothing Then
                m_Fnt.Dispose()
            End If
            m_Fnt = Nothing
            'If m_PenLine IsNot Nothing Then
            '    m_PenLine.Dispose()
            'End If
            m_PenLine = Nothing
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
#End Region
#Region "PrivateFunctions"
    Private Sub AddSketchPad()
        Try
            'Clear out the controls
            m_SketchCanvas.Controls.Clear()

            'add a new split container to hold the canvas and buttons
            m_pSpliCont = New SplitContainer
            'Suspend the redraw
            m_pSpliCont.SuspendLayout()
            'Dock to fill the container
            m_pSpliCont.Dock = DockStyle.Fill
            'Change the orientation
            m_pSpliCont.Orientation = Orientation.Horizontal

            m_pSpliCont.IsSplitterFixed = False
            m_pSpliCont.Panel1MinSize = 50
            m_pSpliCont.SplitterWidth = 5
            m_pSpliCont.SplitterDistance = 50
            m_pSpliCont.Update()

            m_pSpliCont.BorderStyle = BorderStyle.Fixed3D
            m_pSpliCont.ResumeLayout()


            m_picCanvas = New PictureBox
            m_picCanvas.Dock = DockStyle.Fill
            '      m_picCanvas.BackColor = Color.White

            AddHandler m_picCanvas.Paint, AddressOf m_picCanvas_Paint
            AddHandler m_picCanvas.MouseDown, AddressOf m_picCanvas_MouseDown

            Dim pToolStrip As ToolStrip = New ToolStrip
            pToolStrip.Font = m_Fnt
            pToolStrip.Dock = DockStyle.Fill
            'pToolStrip.ImageScalingSize = New Size(30, 30)

            AddHandler pToolStrip.ItemClicked, AddressOf SketchToolClicked


            Dim pToolStripItem As ToolStripButton
            Dim pToolStripDDBtn As ToolStripDropDownButton
            Dim pTlStrSept As ToolStripSeparator = New ToolStripSeparator
            Dim pToolStripMItem As ToolStripMenuItem


            pToolStripDDBtn = New ToolStripDropDownButton
            pToolStripDDBtn.Name = "SketchTools"
            pToolStripDDBtn.Tag = "Tool"
            pToolStripDDBtn.Text = appConfig.SketchPanel.UIComponents.ToolsText
            'pToolStripDDBtn.Font = m_Fnt

            AddHandler pToolStripDDBtn.DropDownItemClicked, AddressOf SketchToolClicked

            pToolStrip.Items.Add(pToolStripDDBtn)


            pToolStripMItem = New ToolStripMenuItem
            '  pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.SelectText
            pToolStripMItem.Name = "btnSketchPointer"
            pToolStripMItem.Tag = "Tool"
            pToolStripMItem.CheckState = CheckState.Checked
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)

            pToolStripMItem = New ToolStripMenuItem
            '  pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSketchLine"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.LineText
            pToolStripMItem.Tag = "Tool"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)


            pToolStripMItem = New ToolStripMenuItem
            '    pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSketchPolyline"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.PolylineText
            pToolStripMItem.Tag = "Tool"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)

            pToolStripMItem = New ToolStripMenuItem
            '   pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSketchText"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.TextText
            pToolStripMItem.Tag = "Tool"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)

            pToolStripMItem = New ToolStripMenuItem
            '    pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSketchInk"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.FreehandText
            pToolStripMItem.Tag = "Tool"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)
            pToolStripMItem = Nothing




            pToolStripDDBtn = New ToolStripDropDownButton
            pToolStripDDBtn.Name = "Options"
            pToolStripDDBtn.Tag = "Options"
            pToolStripDDBtn.Text = appConfig.SketchPanel.UIComponents.OptionsText
            AddHandler pToolStripDDBtn.DropDownItemClicked, AddressOf OptionClicked


            pToolStrip.Items.Add(pToolStripDDBtn)

            pToolStripMItem = New ToolStripMenuItem
            'pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnScale1"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.Block1Text
            pToolStripMItem.Tag = "ScaleValue:5"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)

            pToolStripMItem = New ToolStripMenuItem
            '   pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnScale2"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.Block2Text
            pToolStripMItem.Tag = "ScaleValue:10"
            pToolStripMItem.CheckState = CheckState.Checked
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)

            pToolStripMItem = New ToolStripMenuItem
            '     pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnScale3"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.Block3Text
            pToolStripMItem.Tag = "ScaleValue:20"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)


            pToolStripMItem = New ToolStripMenuItem
            '    pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnScale4"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.Block4Text
            pToolStripMItem.Tag = "ScaleValue:25"

            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)


            pToolStripMItem = New ToolStripMenuItem
            '    pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnScale5"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.Block5Text
            pToolStripMItem.Tag = "ScaleValue:50"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)


            pToolStripDDBtn.DropDownItems.Add(pTlStrSept)
            pTlStrSept = Nothing

            pToolStripMItem = New ToolStripMenuItem
            pToolStripMItem.CheckOnClick = True
            pToolStripMItem.CheckState = CheckState.Checked
            pToolStripMItem.Name = "btnLabel"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.LabelSketchText
            pToolStripMItem.Tag = "Label"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)
            AddHandler pToolStripMItem.Click, AddressOf btnLabelClick
            pToolStripMItem = Nothing
            pTlStrSept = New ToolStripSeparator
            pToolStripDDBtn.DropDownItems.Add(pTlStrSept)
            pTlStrSept = Nothing

            pToolStripMItem = New ToolStripMenuItem
            'pToolStripMItem.CheckOnClick = True
            'pToolStripMItem.CheckState = CheckState.Checked
            pToolStripMItem.Name = "btnBackColor"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.BackColorText
            pToolStripMItem.Tag = "BackColor"
            AddHandler pToolStripMItem.Paint, AddressOf BackColorPaint
            'pToolStripMItem.BackColor = Color.White
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)
            AddHandler pToolStripMItem.Click, AddressOf btnBackColorClick

            pToolStripMItem = New ToolStripMenuItem
            'pToolStripMItem.CheckOnClick = True
            'pToolStripMItem.CheckState = CheckState.Checked
            pToolStripMItem.Name = "btnGridColor"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.GridLineColorText
            pToolStripMItem.Tag = "GridColor"
            AddHandler pToolStripMItem.Paint, AddressOf gridColorPaint
            'pToolStripMItem.BackColor = Color.White
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)
            AddHandler pToolStripMItem.Click, AddressOf btnGridColorClick

            pToolStripMItem = New ToolStripMenuItem
            'pToolStripMItem.CheckOnClick = True
            'pToolStripMItem.CheckState = CheckState.Checked
            pToolStripMItem.Name = "btnDrawColor"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.DrawingColorText
            pToolStripMItem.Tag = "DrawColor"
            AddHandler pToolStripMItem.Paint, AddressOf drawColorPaint
            'pToolStripMItem.BackColor = Color.White
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)
            AddHandler pToolStripMItem.Click, AddressOf btnDrawColorClick

            'pToolStripMItem = New ToolStripMenuItem
            'pToolStripMItem.Name = "btnGetMapImage"
            'pToolStripMItem.Text = "Get Map Image"
            'pToolStripMItem.Tag = "MapImage"
            'pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)
            'AddHandler pToolStripMItem.Click, AddressOf btnMapImageClick




            pToolStripDDBtn = New ToolStripDropDownButton
            pToolStripDDBtn.Name = "GridSize"
            pToolStripDDBtn.Tag = "Grid Size"
            pToolStripDDBtn.Text = appConfig.SketchPanel.UIComponents.GridText
            AddHandler pToolStripDDBtn.DropDownItemClicked, AddressOf GridSizeClicked


            pToolStrip.Items.Add(pToolStripDDBtn)

            pToolStripMItem = New ToolStripMenuItem
            '    pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSize1"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.SmallestText
            pToolStripMItem.Tag = "GridSize:10"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)


            pToolStripMItem = New ToolStripMenuItem
            '      pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSize2"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.SmallText
            pToolStripMItem.Tag = "GridSize:20"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)


            pToolStripMItem = New ToolStripMenuItem
            '    pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSize3"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.NormalText
            pToolStripMItem.CheckState = CheckState.Checked
            pToolStripMItem.Tag = "GridSize:25"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)

            pToolStripMItem = New ToolStripMenuItem
            '   pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSize4"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.LargeText
            pToolStripMItem.Tag = "GridSize:50"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)

            pToolStripMItem = New ToolStripMenuItem
            '    pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSize5"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.LargerText
            pToolStripMItem.Tag = "GridSize:75"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)

            pToolStripMItem = New ToolStripMenuItem
            '   pToolStripMItem.CheckOnClick = True
            pToolStripMItem.Name = "btnSize6"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.LargestText
            pToolStripMItem.Tag = "GridSize:100"
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)


            pToolStripDDBtn = New ToolStripDropDownButton
            pToolStripDDBtn.Name = "Edits"
            pToolStripDDBtn.Tag = "Edits"
            pToolStripDDBtn.Text = appConfig.SketchPanel.UIComponents.EditText

            pToolStrip.Items.Add(pToolStripDDBtn)
            AddHandler pToolStripDDBtn.DropDownItemClicked, AddressOf EditItemClicked

            pToolStripMItem = New ToolStripMenuItem
            pToolStripMItem.Name = "btnSketchUndo"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.UndoText
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)



            pToolStripMItem = New ToolStripMenuItem
            pToolStripMItem.Name = "btnSketchRedo"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.RedoText
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)

            pToolStripMItem = New ToolStripMenuItem
            pToolStripMItem.Name = "btnSketchDelete"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.DeleteText
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)


            pToolStripMItem = New ToolStripMenuItem
            pToolStripMItem.Name = "btnSketchClear"
            pToolStripMItem.Text = appConfig.SketchPanel.UIComponents.ClearText
            pToolStripDDBtn.DropDownItems.Add(pToolStripMItem)


            pToolStripItem = New ToolStripButton

            pToolStripItem.Name = "btnSketchSave"
            pToolStripItem.Text = appConfig.SketchPanel.UIComponents.SaveText
            pToolStrip.Items.Add(pToolStripItem)



            m_pSpliCont.Panel1.Controls.Add(pToolStrip)

            m_pSpliCont.Panel2.Controls.Add(m_picCanvas)
            m_SketchCanvas.Controls.Add(m_pSpliCont)


            pToolStripItem = Nothing
            pToolStripDDBtn = Nothing
            pTlStrSept = Nothing
            pToolStripMItem = Nothing

            'If m_inkCollector Is Nothing Then
            '    Try


            '        m_inkCollector = New InkCollector(m_picCanvas.Handle)
            '    Catch ex As Exception
            '        RaiseEvent InkNotInstalled()
            '    End Try
            'End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Sub btnBackColorClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim pNewColorDia As ColorDialog = New ColorDialog
        pNewColorDia.SolidColorOnly = True
        Dim pDRes As DialogResult = pNewColorDia.ShowDialog()
        If pDRes = DialogResult.OK Then
            m_BrushBackColor = New SolidBrush(pNewColorDia.Color)
        End If
        m_picCanvas.Invalidate()
        'MsgBox(pDRes.ToString)
        '   

    End Sub
    Private Sub btnGridColorClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim pNewColorDia As ColorDialog = New ColorDialog
        pNewColorDia.SolidColorOnly = True
        Dim pDRes As DialogResult = pNewColorDia.ShowDialog()
        If pDRes = DialogResult.OK Then
            m_PenLine = New Pen(pNewColorDia.Color)
        End If
        m_picCanvas.Invalidate()
        'MsgBox(pDRes.ToString)
        '   

    End Sub
    Private Sub btnDrawColorClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim pNewColorDia As ColorDialog = New ColorDialog
        pNewColorDia.SolidColorOnly = True
        Dim pDRes As DialogResult = pNewColorDia.ShowDialog()
        If pDRes = DialogResult.OK Then
            m_CurrentForeColor = pNewColorDia.Color

        End If
        ' m_picCanvas.Invalidate()
        'MsgBox(pDRes.ToString)
        '   

    End Sub
    Private Sub btnMapImageClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        RaiseEvent GetMapImage(m_picCanvas.Width, m_picCanvas.Height)

    End Sub
    Private Sub btnLabelClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        m_Label = CType(sender, ToolStripMenuItem).Checked
        If m_NewDrawable IsNot Nothing Then
            m_NewDrawable.Label = m_Label
        End If
    End Sub
    Private Sub removeCheckState(ByVal pToolStrip As ToolStrip, ByVal pToolStripItem As ToolStripItem)
        Try
            For Each cntrl As ToolStripItem In pToolStrip.Items
                If TypeOf cntrl Is ToolStripButton Then
                    If cntrl.Name <> pToolStripItem.Name Then
                        CType(cntrl, ToolStripButton).Checked = False
                    End If
                ElseIf TypeOf cntrl Is ToolStripDropDownButton Then

                    Dim pTLDDB As ToolStripDropDownButton = CType(cntrl, ToolStripDropDownButton)
                    '  pTLDDB.Owner.BackColor = Nothing
                    For Each DDItem As ToolStripMenuItem In pTLDDB.DropDownItems


                        DDItem.Checked = False

                    Next
                    pTLDDB = Nothing
                End If
            Next
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Enum DrawTypes
        Line
        Polyline
        Pointer
        Rectangle
        Ellipse
        Star
        Text
        Ink
        None
    End Enum
    Private Sub changeSketchType(ByVal newDrawType As DrawTypes)
        Try
            If TypeOf m_NewDrawable Is DrawablePolyLine Then

                ' No longer watch for MouseMove or MouseUp.
                RemoveHandler m_picCanvas.MouseMove, AddressOf NewDrawable_MouseMove
                RemoveHandler m_picCanvas.MouseUp, AddressOf NewDrawable_MouseUp
                RemoveHandler m_picCanvas.MouseDoubleClick, AddressOf NewDrawable_MouseDoubleClick
                ' m_Picture.Drawables.RemoveAt(m_Picture.Drawables.Count - 1)


                ' Discard this object.
                m_Picture.Remove(m_NewDrawable)


                ' We're no longer working with the new object.
                m_NewDrawable = Nothing

                ' Redraw.
                m_picCanvas.Invalidate()
            End If
            m_SketchTool = newDrawType
            If newDrawType = DrawTypes.Ink Then
                ' StartInkDrawable()

            Else
                '  StopInkDrawable()
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub
    'Private Sub StartInkDrawable()
    '    Try


    '        If m_inkCollector Is Nothing Then
    '            Try

    '                m_inkCollector = New InkCollector(m_picCanvas.Handle)
    '            Catch ex As Exception
    '            RaiseEvent InkNotInstalled()
    '                Return
    '            End Try

    '        End If
    '    'RemoveHandler m_SketchCanvas.MouseDown, AddressOf m_SketchCanvas_MouseDown

    '    m_inkCollector.CollectionMode = CollectionMode.InkOnly

    '    m_inkCollector.Enabled = True
    '    m_inkCollector.DefaultDrawingAttributes.Color = m_CurrentForeColor


    '    '  m_inkCollector.Ink.Strokes.Clear()
    '    ' AddHandler m_SketchCanvas.MouseDown, AddressOf m_SketchCanvas_MouseDown
    '    Catch ex As Exception
    '      RaiseEvent InkNotInstalled()

    '    End Try
    'End Sub
    'Private Sub StopInkDrawable()
    '    Try
    '        If m_inkCollector Is Nothing Then
    '            Return
    '        End If
    '        m_inkCollector.Enabled = False
    '        m_inkCollector = Nothing
    '    Catch ex As Exception
    '        MsgBox("Error in the Stop Ink Drawing" & vbCrLf & ex.Message)
    '    End Try

    'End Sub
    'Private Sub LogStroke(ByVal e As Microsoft.Ink.InkCollectorStrokeEventArgs)
    '    Try

    '        Dim pNewDrawInk As DrawableInk = CType(m_NewDrawable, DrawableInk)
    '        pNewDrawInk.Stroke = m_inkCollector.Ink.Clone.Strokes

    '        Dim pStk As Stroke = m_inkCollector.Ink.Clone.Strokes.Item(0)
    '        Dim pPnt() As Point = pStk.GetPoints
    '        Dim pPntConverted(pStk.GetPoints.Length - 1) As PointF
    '        Dim pBeginPoint As Point = pPnt(0)
    '        Dim pEndPoint As Point = pPnt(pPnt.Length - 1)
    '        Dim i As Integer = 0

    '        For Each pp As Point In pPnt
    '            m_inkCollector.Renderer.InkSpaceToPixel(m_picCanvas.CreateGraphics, pp)
    '            pPntConverted(i) = New PointF(pp.X, pp.Y)

    '            i = i + 1
    '        Next


    '        pNewDrawInk.X1 = CInt(pPntConverted(0).X)
    '        pNewDrawInk.Y1 = CInt(pPntConverted(0).Y)
    '        pNewDrawInk.X2 = CInt(pPntConverted(pPntConverted.Length - 1).X)
    '        pNewDrawInk.Y2 = CInt(pPntConverted(pPntConverted.Length - 1).Y)
    '        pNewDrawInk.m_ConvertedPoints = pPntConverted

    '        m_inkCollector.Ink.DeleteStrokes()

    '        m_SketchCanvas.Invalidate()
    '        '  StartInkDrawable()
    '    Catch ex As Exception
    '        RaiseEvent InkNotInstalled()

    '    End Try
    'End Sub
    'Private Sub m_inkCollector_Stroke(ByVal sender As Object, ByVal e As Microsoft.Ink.InkCollectorStrokeEventArgs) Handles m_inkCollector.Stroke
    '    Try

    '        m_NewDrawable = New DrawableInk(m_CurrentForeColor, Nothing, m_CurrentLineWidth)
    '        m_Picture.Add(m_NewDrawable)
    '        LogStroke(e)
    '    Catch ex As Exception
    '     RaiseEvent InkNotInstalled()
    '    End Try
    'End Sub

    Private Sub Undo()
        Try
            If m_Picture.Drawables.Count = 0 Then Return
            Dim pDraw As Drawable = CType(m_Picture.Drawables(m_Picture.Drawables.Count - 1), Drawable)
            If pDraw.IsEmpty Then
                m_Picture.Remove(CType(m_Picture.Drawables(m_Picture.Drawables.Count - 1), Drawable))
            End If
            m_EraseDrawable.Add(m_Picture.Drawables(m_Picture.Drawables.Count - 1))
            m_Picture.Remove(CType(m_Picture.Drawables(m_Picture.Drawables.Count - 1), Drawable))
            m_picCanvas.Invalidate()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub redo()
        Try
            If m_EraseDrawable Is Nothing Then Return

            If m_EraseDrawable.Count = 0 Then Return
            m_Picture.Add(CType(m_EraseDrawable.Item(m_EraseDrawable.Count - 1), Drawable))
            m_EraseDrawable.RemoveAt(m_EraseDrawable.Count - 1)

            m_picCanvas.Invalidate()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub DeleteSelected()
        Try
            If m_Picture.Drawables.Count = 0 Then Return
            If m_Picture.SelectedDrawable Is Nothing Then Return



            m_EraseDrawable.Add(m_Picture.SelectedDrawable)
            m_Picture.Delete(m_Picture.SelectedDrawable)

            m_picCanvas.Invalidate()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
#End Region
#Region "Events"
    Private Sub BackColorPaint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        e.Graphics.FillRectangle(m_BrushBackColor, CType(sender, ToolStripMenuItem).Width - CType(sender, ToolStripMenuItem).Height - 5, 1, CType(sender, ToolStripMenuItem).Height - 2, CType(sender, ToolStripMenuItem).Height - 2)

        '        CType(sender, ToolStripMenuItem).Height


    End Sub
    Private Sub GridColorPaint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        e.Graphics.FillRectangle(New SolidBrush(m_PenLine.Color), CType(sender, ToolStripMenuItem).Width - CType(sender, ToolStripMenuItem).Height - 5, 1, CType(sender, ToolStripMenuItem).Height - 2, CType(sender, ToolStripMenuItem).Height - 2)

        '        CType(sender, ToolStripMenuItem).Height


    End Sub
    Private Sub DrawColorPaint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        e.Graphics.FillRectangle(New SolidBrush(m_CurrentForeColor), CType(sender, ToolStripMenuItem).Width - CType(sender, ToolStripMenuItem).Height - 5, 1, CType(sender, ToolStripMenuItem).Height - 2, CType(sender, ToolStripMenuItem).Height - 2)

        '        CType(sender, ToolStripMenuItem).Height


    End Sub
    Private Sub m_pSpliCont_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_pSpliCont.Resize
        m_pSpliCont.SplitterDistance = 50
    End Sub

    Private Sub EditItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)
        Try
            If TypeOf e.ClickedItem Is ToolStripDropDownButton Then Return


            If TypeOf e.ClickedItem Is ToolStripMenuItem Then
                Select Case e.ClickedItem.Name


                    Case "btnSketchUndo"
                        Undo()
                    Case "btnSketchRedo"
                        redo()
                    Case "btnSketchDelete"
                        DeleteSelected()
                    Case "btnSketchClear"
                        m_Picture.RemoveAll()
                        m_EraseDrawable.Clear()
                        m_picCanvas.Invalidate()
                End Select
            End If
            m_picCanvas.Invalidate()

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub GridSizeClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)
        Try
            If TypeOf e.ClickedItem Is ToolStripDropDownButton Then Return


            If TypeOf e.ClickedItem Is ToolStripMenuItem Then
                If CType(e.ClickedItem, ToolStripMenuItem).Tag.ToString().Contains("GridSize") Then
                    m_BlockSize = CInt(CType(e.ClickedItem, ToolStripMenuItem).Tag.ToString.Replace("GridSize:", ""))
                    If m_NewDrawable IsNot Nothing Then
                        m_NewDrawable.BlockSize = m_BlockSize
                    End If

                    For Each tlb As ToolStripItem In CType(sender, ToolStripDropDownButton).DropDownItems
                        If tlb IsNot e.ClickedItem Then
                            If tlb.Tag.ToString.Contains("GridSize") Then
                                If TypeOf tlb Is ToolStripMenuItem Then
                                    CType(tlb, ToolStripMenuItem).Checked = False

                                End If
                            End If
                        Else
                            CType(tlb, ToolStripMenuItem).Checked = True
                        End If
                    Next
                End If
            End If
            m_picCanvas.Invalidate()

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub OptionClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)
        Try
            If TypeOf e.ClickedItem Is ToolStripDropDownButton Then Return


            If TypeOf e.ClickedItem Is ToolStripMenuItem Then
                If CStr(CType(e.ClickedItem, ToolStripItem).Tag).Contains("Scale") Then
                    m_Scale = CInt(CType(e.ClickedItem, ToolStripItem).Tag.ToString.Replace("ScaleValue:", ""))
                    If m_NewDrawable IsNot Nothing Then
                        m_NewDrawable.FeetPerBlock = m_Scale
                    End If

                    For Each tlb As ToolStripItem In CType(sender, ToolStripDropDownButton).DropDownItems
                        If tlb IsNot e.ClickedItem Then
                            If TypeOf tlb Is ToolStripMenuItem Then
                                If tlb.Tag.ToString.Contains("Scale") Then

                                    CType(tlb, ToolStripMenuItem).CheckState = CheckState.Unchecked

                                End If
                            End If
                        Else
                            CType(tlb, ToolStripMenuItem).Checked = True
                        End If
                    Next
                End If
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub SketchToolClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)
        Try
            If TypeOf e.ClickedItem Is ToolStripDropDownButton Then Return


            If TypeOf e.ClickedItem Is ToolStripMenuItem Then
                If CType(e.ClickedItem, ToolStripMenuItem).Tag.ToString.Contains("Tool") Then
                    For Each tlb As ToolStripItem In CType(sender, ToolStripDropDownButton).DropDownItems
                        If tlb IsNot e.ClickedItem Then
                            If TypeOf tlb Is ToolStripMenuItem Then
                                If tlb.Tag.ToString.Contains("Tool") Then

                                    CType(tlb, ToolStripMenuItem).CheckState = CheckState.Unchecked

                                End If
                            End If
                        Else
                            CType(tlb, ToolStripMenuItem).Checked = True
                        End If
                    Next
                End If
            End If


            Select Case e.ClickedItem.Name
                Case "btnSketchPointer"
                    changeSketchType(DrawTypes.Pointer)
                Case "btnSketchInk"
                    changeSketchType(DrawTypes.Ink)

                Case "btnSketchPolyline"
                    changeSketchType(DrawTypes.Polyline)
                    ' CType(sender, ToolStripDropDownButton).BackColor = Color.Yellow
                Case "btnSketchLine"
                    changeSketchType(DrawTypes.Line)

                Case "btnSketchText"

                    changeSketchType(DrawTypes.Text)



                Case "btnSketchSave"

                    Dim saveFileDialog1 As System.Windows.Forms.SaveFileDialog

                    saveFileDialog1 = New System.Windows.Forms.SaveFileDialog()

                    'saveFileDialog1.CreatePrompt = True
                    saveFileDialog1.FileName = "sketch"

                    saveFileDialog1.Filter = "Jpg (*.jpg) |*.jpg|Bitmap (*.bmp) |*.bmp|Gif (*.gif)| *.gif"
                    If saveFileDialog1.ShowDialog() = DialogResult.OK Then
                        If SaveImage(saveFileDialog1.FileName, True) Then

                        End If
                    End If

            End Select

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub


#End Region
#Region "Properties"
    Public WriteOnly Property SetBackGroundImage() As Bitmap
        Set(ByVal value As Bitmap)
            m_backGroundPic = value
            m_picCanvas.BackgroundImage = m_backGroundPic
            m_picCanvas.BackgroundImageLayout = ImageLayout.Center
        End Set
    End Property
#End Region
End Class
