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


Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Windows.Forms

Public Class CameraForm
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If m_TxtTimer IsNot Nothing Then
            m_TxtTimer.Stop()
            m_TxtTimer.Dispose()

        End If
        m_TxtTimer = Nothing

        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    Friend WithEvents spMain As System.Windows.Forms.SplitContainer
    Friend WithEvents spCam As System.Windows.Forms.SplitContainer
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents picCapture As System.Windows.Forms.PictureBox
    Friend WithEvents lstDevices As System.Windows.Forms.ListBox
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents lblRecSaved As System.Windows.Forms.Label

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents sfdImage As System.Windows.Forms.SaveFileDialog
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.sfdImage = New System.Windows.Forms.SaveFileDialog()
        Me.spMain = New System.Windows.Forms.SplitContainer()
        Me.spCam = New System.Windows.Forms.SplitContainer()
        Me.lblRecSaved = New System.Windows.Forms.Label()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.picCapture = New System.Windows.Forms.PictureBox()
        Me.lstDevices = New System.Windows.Forms.ListBox()
        Me.spMain.Panel1.SuspendLayout()
        Me.spMain.Panel2.SuspendLayout()
        Me.spMain.SuspendLayout()
        Me.spCam.Panel1.SuspendLayout()
        Me.spCam.Panel2.SuspendLayout()
        Me.spCam.SuspendLayout()
        CType(Me.picCapture, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'sfdImage
        '
        Me.sfdImage.FileName = "Webcam1"
        Me.sfdImage.Filter = "Bitmap|*.bmp"
        '
        'spMain
        '
        Me.spMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spMain.Location = New System.Drawing.Point(0, 0)
        Me.spMain.Name = "spMain"
        Me.spMain.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'spMain.Panel1
        '
        Me.spMain.Panel1.Controls.Add(Me.spCam)
        '
        'spMain.Panel2
        '
        Me.spMain.Panel2.Controls.Add(Me.lstDevices)
        Me.spMain.Panel2Collapsed = True
        Me.spMain.Size = New System.Drawing.Size(715, 418)
        Me.spMain.SplitterDistance = 364
        Me.spMain.TabIndex = 6
        '
        'spCam
        '
        Me.spCam.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spCam.IsSplitterFixed = True
        Me.spCam.Location = New System.Drawing.Point(0, 0)
        Me.spCam.Name = "spCam"
        '
        'spCam.Panel1
        '
        Me.spCam.Panel1.Controls.Add(Me.lblRecSaved)
        Me.spCam.Panel1.Controls.Add(Me.btnClose)
        Me.spCam.Panel1.Controls.Add(Me.btnSave)
        Me.spCam.Panel1.Controls.Add(Me.btnStart)
        '
        'spCam.Panel2
        '
        Me.spCam.Panel2.Controls.Add(Me.picCapture)
        Me.spCam.Size = New System.Drawing.Size(715, 418)
        Me.spCam.SplitterDistance = 121
        Me.spCam.TabIndex = 0
        '
        'lblRecSaved
        '
        Me.lblRecSaved.AutoSize = True
        Me.lblRecSaved.Font = New System.Drawing.Font("Microsoft Sans Serif", 21.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRecSaved.ForeColor = System.Drawing.Color.Red
        Me.lblRecSaved.Location = New System.Drawing.Point(15, 332)
        Me.lblRecSaved.Name = "lblRecSaved"
        Me.lblRecSaved.Size = New System.Drawing.Size(97, 66)
        Me.lblRecSaved.TabIndex = 15
        Me.lblRecSaved.Text = "Photo" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Saved"
        Me.lblRecSaved.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblRecSaved.Visible = False
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.Color.Transparent
        Me.btnClose.BackgroundImage = Global.Esri.ArcGISTemplates.My.Resources.Resources.ShutDown
        Me.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnClose.FlatAppearance.BorderSize = 0
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClose.Location = New System.Drawing.Point(12, 228)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(100, 100)
        Me.btnClose.TabIndex = 14
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'btnSave
        '
        Me.btnSave.BackColor = System.Drawing.Color.Transparent
        Me.btnSave.BackgroundImage = Global.Esri.ArcGISTemplates.My.Resources.Resources.SaveGreen100
        Me.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnSave.FlatAppearance.BorderSize = 0
        Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSave.Location = New System.Drawing.Point(12, 121)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(100, 100)
        Me.btnSave.TabIndex = 13
        Me.btnSave.UseVisualStyleBackColor = False
        '
        'btnStart
        '
        Me.btnStart.BackgroundImage = Global.Esri.ArcGISTemplates.My.Resources.Resources.GreenCam100
        Me.btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnStart.FlatAppearance.BorderSize = 0
        Me.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStart.Location = New System.Drawing.Point(12, 12)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(100, 100)
        Me.btnStart.TabIndex = 12
        '
        'picCapture
        '
        Me.picCapture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.picCapture.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picCapture.Location = New System.Drawing.Point(0, 0)
        Me.picCapture.Name = "picCapture"
        Me.picCapture.Size = New System.Drawing.Size(590, 418)
        Me.picCapture.TabIndex = 1
        Me.picCapture.TabStop = False
        '
        'lstDevices
        '
        Me.lstDevices.Dock = System.Windows.Forms.DockStyle.Top
        Me.lstDevices.FormattingEnabled = True
        Me.lstDevices.Location = New System.Drawing.Point(0, 0)
        Me.lstDevices.Name = "lstDevices"
        Me.lstDevices.Size = New System.Drawing.Size(150, 95)
        Me.lstDevices.TabIndex = 0
        '
        'CameraForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(715, 418)
        Me.ControlBox = False
        Me.Controls.Add(Me.spMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "CameraForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "  "
        Me.spMain.Panel1.ResumeLayout(False)
        Me.spMain.Panel2.ResumeLayout(False)
        Me.spMain.ResumeLayout(False)
        Me.spCam.Panel1.ResumeLayout(False)
        Me.spCam.Panel1.PerformLayout()
        Me.spCam.Panel2.ResumeLayout(False)
        Me.spCam.ResumeLayout(False)
        CType(Me.picCapture, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private m_LastPhoto As String
    Private m_tickMessage As String
    Private m_TxtTimer As System.Windows.Forms.Timer
    Private m_tickCount As Integer = 0
    Private m_msgFont As Font = New System.Drawing.Font("Tahoma", 16.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_msgFontLarge As Font = New System.Drawing.Font("Tahoma", 20.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_offset As Integer = 1
    Private m_msgX As Integer
    Private m_msgY As Integer
    Private m_msgPermX As Integer
    Private m_msgPermY As Integer
    Private m_msgBrush As Brush = Brushes.Red
    Private m_msgBrushFront As Brush = Brushes.White

    Const WM_CAP As Short = &H400S

    Const WM_CAP_DRIVER_CONNECT As Integer = WM_CAP + 10
    Const WM_CAP_DRIVER_DISCONNECT As Integer = WM_CAP + 11
    Const WM_CAP_EDIT_COPY As Integer = WM_CAP + 30

    Const WM_CAP_SET_PREVIEW As Integer = WM_CAP + 50
    Const WM_CAP_SET_PREVIEWRATE As Integer = WM_CAP + 52
    Const WM_CAP_SET_SCALE As Integer = WM_CAP + 53
    Const WS_CHILD As Integer = &H40000000
    Const WS_VISIBLE As Integer = &H10000000
    Const SWP_NOMOVE As Short = &H2S
    Const SWP_NOSIZE As Short = 1
    Const SWP_NOZORDER As Short = &H4S
    Const HWND_BOTTOM As Short = 1

    Dim iDevice As Integer = 0 ' Current device ID
    Dim hHwnd As Integer ' Handle to preview window

    Declare Function SendMessage Lib "user32" Alias "SendMessageA" _
        (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, _
        <MarshalAs(UnmanagedType.AsAny)> ByVal lParam As Object) As Integer

    Declare Function SetWindowPos Lib "user32" Alias "SetWindowPos" (ByVal hwnd As Integer, _
        ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer, _
        ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer

    Declare Function DestroyWindow Lib "user32" (ByVal hndw As Integer) As Boolean

    Declare Function capCreateCaptureWindowA Lib "avicap32.dll" _
        (ByVal lpszWindowName As String, ByVal dwStyle As Integer, _
        ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, _
        ByVal nHeight As Short, ByVal hWndParent As Integer, _
        ByVal nID As Integer) As Integer

    Declare Function capGetDriverDescriptionA Lib "avicap32.dll" (ByVal wDriver As Short, _
        ByVal lpszName As String, ByVal cbName As Integer, ByVal lpszVer As String, _
        ByVal cbVer As Integer) As Boolean
    Public ReadOnly Property Photo As String
        Get
            Return m_LastPhoto
        End Get

    End Property

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadDeviceList()
        If lstDevices.Items.Count > 0 Then
            btnStart.Enabled = True
            lstDevices.SelectedIndex = 0
            btnStart.Enabled = True
        Else
            lstDevices.Items.Add("No Capture Device")
            btnStart.Enabled = False
        End If

        'btnStop.Enabled = False
        'btnSave.Enabled = False
        picCapture.SizeMode = Windows.Forms.PictureBoxSizeMode.StretchImage
        m_TxtTimer = New System.Windows.Forms.Timer

        m_TxtTimer.Interval = 50
        'Add the handler for each tick
        AddHandler m_TxtTimer.Tick, AddressOf m_TxtTimer_Tick
    End Sub

    Private Sub LoadDeviceList()
        Dim strName As String = Space(100)
        Dim strVer As String = Space(100)
        Dim bReturn As Boolean
        Dim x As Integer = 0

        ' 
        ' Load name of all avialable devices into the lstDevices
        '

        Do
            '
            '   Get Driver name and version
            '
            bReturn = capGetDriverDescriptionA(x, strName, 100, strVer, 100)

            '
            ' If there was a device add device name to the list
            '
            If bReturn Then lstDevices.Items.Add(strName.Trim)
            x += 1
        Loop Until bReturn = False
    End Sub

    Private Sub OpenPreviewWindow()
        Dim iHeight As Integer = picCapture.Height
        Dim iWidth As Integer = picCapture.Width

        '
        ' Open Preview window in picturebox
        '
        hHwnd = capCreateCaptureWindowA(iDevice, WS_VISIBLE Or WS_CHILD, 0, 0, 640, _
            480, picCapture.Handle.ToInt32, 0)

        '
        ' Connect to device
        '
        If SendMessage(hHwnd, WM_CAP_DRIVER_CONNECT, iDevice, 0) Then
            '
            'Set the preview scale
            '
            SendMessage(hHwnd, WM_CAP_SET_SCALE, True, 0)

            '
            'Set the preview rate in milliseconds
            '
            SendMessage(hHwnd, WM_CAP_SET_PREVIEWRATE, 66, 0)

            '
            'Start previewing the image from the camera
            '
            SendMessage(hHwnd, WM_CAP_SET_PREVIEW, True, 0)

            '
            ' Resize window to fit in picturebox
            '
            SetWindowPos(hHwnd, HWND_BOTTOM, 0, 0, picCapture.Width, picCapture.Height, _
                    SWP_NOMOVE Or SWP_NOZORDER)

            'btnSave.Enabled = True
            'btnStop.Enabled = True
            ' btnStart.Enabled = False
        Else
            '
            ' Error connecting to device close window
            ' 
            DestroyWindow(hHwnd)

            '   btnSave.Enabled = False
        End If
    End Sub

    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click
        If btnStart.Tag = "On" Then
            btnStart.Tag = "Off"
            btnSave.Enabled = False

            btnSave.BackgroundImage = My.Resources.SaveDisable100
            btnStart.BackgroundImage = My.Resources.GreenCam100
            ClosePreviewWindow()
            btnSave.Enabled = False
            btnStart.Enabled = True
            ' lblRecSaved.Visible = True

            'btnStop.Enabled = False

        Else
            btnStart.Tag = "On"
            btnStart.BackgroundImage = My.Resources.RedCam100
            btnSave.Enabled = True
            btnSave.BackgroundImage = My.Resources.SaveGreen100

            iDevice = lstDevices.SelectedIndex
            OpenPreviewWindow()
            ' lblRecSaved.Visible = False

        End If

    End Sub

    Private Sub ClosePreviewWindow()
        '
        ' Disconnect from device
        '
        SendMessage(hHwnd, WM_CAP_DRIVER_DISCONNECT, iDevice, 0)

        '
        ' close window
        '

        DestroyWindow(hHwnd)
    End Sub

    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ClosePreviewWindow()
        btnSave.Enabled = False
        btnStart.Enabled = True
        'btnStop.Enabled = False
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim data As IDataObject
        Dim bmap As Image

        '
        ' Copy image to clipboard
        '
        SendMessage(hHwnd, WM_CAP_EDIT_COPY, 0, 0)

        '
        ' Get image from clipboard and convert it to a bitmap
        '
        data = Clipboard.GetDataObject()
        If data.GetDataPresent(GetType(System.Drawing.Bitmap)) Then
            bmap = CType(data.GetData(GetType(System.Drawing.Bitmap)), Image)
            

            m_LastPhoto = GlobalsFunctions.getRandomFileNameInLocation("MobilePhoto_{0}.bmp")
            bmap.Save(m_LastPhoto, Imaging.ImageFormat.Bmp)

            'End If
            showMessage("Image Saved")
            btnStart_Click(btnStart, Nothing)
            picCapture.Image = bmap




        End If
    End Sub
    Private Sub m_TxtTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs)
        'Preform the zoom on the ick
        'FixZoom(m_bZoomOut)
        If m_tickCount > 60 Then
            m_tickCount = 0
            m_tickMessage = ""
            picCapture.Invalidate()
            resizeHoverMessage()

            ' m_HoveringMessageBox.Visible = False

        Else
            m_tickCount = m_tickCount + 1
            resizeHoverMessage()

        End If
    End Sub

    Private Sub Form1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        'If btnStop.Enabled Then
        '    ClosePreviewWindow()
        'End If
        Try
            ClosePreviewWindow()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub CameraForm_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        btnStart.Tag = "On"
        btnStart.BackgroundImage = My.Resources.RedCam100
        iDevice = lstDevices.SelectedIndex
        OpenPreviewWindow()
    End Sub

    Private Sub picCapture_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles picCapture.Paint
        If m_tickCount > 0 And m_tickMessage <> "" Then

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX - m_offset, m_msgY - m_offset)
            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX - m_offset - m_offset, m_msgY - m_offset - m_offset)

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX + m_offset, m_msgY - m_offset)
            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX + m_offset + m_offset, m_msgY - m_offset - m_offset)

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX - m_offset, m_msgY + m_offset)
            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX - m_offset - m_offset, m_msgY + m_offset + m_offset)

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX + m_offset + m_offset, m_msgY + m_offset + m_offset)
            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX + m_offset, m_msgY + m_offset)

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrushFront, m_msgX, m_msgY)
        End If
    End Sub
    Private Sub showMessage(ByVal message As String)
        'm_HoveringMessageBox.Text = message
        'm_HoveringMessageBox.Visible = True
        m_tickMessage = message
        resizeHoverMessage()

        m_tickCount = 1
        m_TxtTimer.Start()

    End Sub
    Private Sub resizeHoverMessage()
        Try
            If picCapture Is Nothing Then Return

            If m_tickMessage <> "" Then
                Dim g As Graphics = picCapture.CreateGraphics
                Dim sz As SizeF = g.MeasureString(m_tickMessage, m_msgFont)

                m_msgX = CInt((picCapture.Width / 2)) - (sz.Width / 2) '- (m_HoveringMessageBox.Width / 2))


                m_msgY = CInt(picCapture.Height - 55 - sz.Height)




                g = Nothing
                sz = Nothing


            Else
                m_msgX = CInt((picCapture.Width / 2)) '- (m_HoveringMessageBox.Width / 2))
                m_msgY = CInt(picCapture.Height - picCapture.Height / 6)

            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Sub picCapture_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles picCapture.Resize
        Try

            Dim g As Graphics = picCapture.CreateGraphics
            Dim sz As SizeF = g.MeasureString(m_tickMessage, m_msgFontLarge)

            m_msgPermX = CInt((picCapture.Width / 2)) - (sz.Width / 2) '- (m_HoveringMessageBox.Width / 2))


            m_msgPermY = CInt(picCapture.Height - 35 - sz.Height)

            g = Nothing
            sz = Nothing
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub
End Class
