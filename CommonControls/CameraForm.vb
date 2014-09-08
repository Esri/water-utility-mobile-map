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


Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Windows.Forms
Imports Panasonic.Toughbook.Camera
Imports Panasonic.Toughbook.Library

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
     

        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
    Friend WithEvents spMain As System.Windows.Forms.SplitContainer
    Friend WithEvents CameraViewPanel As System.Windows.Forms.Panel
    Friend WithEvents ActiveCameraMenuComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents ResolutionMenuComboBox As System.Windows.Forms.ComboBox

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.spMain = New System.Windows.Forms.SplitContainer()
        Me.CameraViewPanel = New System.Windows.Forms.Panel()
        Me.ActiveCameraMenuComboBox = New System.Windows.Forms.ComboBox()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.ResolutionMenuComboBox = New System.Windows.Forms.ComboBox()
        Me.spMain.Panel1.SuspendLayout()
        Me.spMain.Panel2.SuspendLayout()
        Me.spMain.SuspendLayout()
        Me.SuspendLayout()
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
        Me.spMain.Panel1.Controls.Add(Me.CameraViewPanel)
        '
        'spMain.Panel2
        '
        Me.spMain.Panel2.Controls.Add(Me.ResolutionMenuComboBox)
        Me.spMain.Panel2.Controls.Add(Me.ActiveCameraMenuComboBox)
        Me.spMain.Panel2.Controls.Add(Me.btnClose)
        Me.spMain.Panel2.Controls.Add(Me.btnStart)
        Me.spMain.Size = New System.Drawing.Size(676, 484)
        Me.spMain.SplitterDistance = 365
        Me.spMain.TabIndex = 6
        '
        'CameraViewPanel
        '
        Me.CameraViewPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CameraViewPanel.Location = New System.Drawing.Point(0, 0)
        Me.CameraViewPanel.Name = "CameraViewPanel"
        Me.CameraViewPanel.Size = New System.Drawing.Size(676, 365)
        Me.CameraViewPanel.TabIndex = 0
        '
        'ActiveCameraMenuComboBox
        '
        Me.ActiveCameraMenuComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ActiveCameraMenuComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ActiveCameraMenuComboBox.FormattingEnabled = True
        Me.ActiveCameraMenuComboBox.Location = New System.Drawing.Point(149, 13)
        Me.ActiveCameraMenuComboBox.Name = "ActiveCameraMenuComboBox"
        Me.ActiveCameraMenuComboBox.Size = New System.Drawing.Size(374, 33)
        Me.ActiveCameraMenuComboBox.TabIndex = 17
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.Color.Transparent
        Me.btnClose.BackgroundImage = Global.ESRI.ArcGISTemplates.My.Resources.Resources.CancelRed
        Me.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnClose.FlatAppearance.BorderSize = 0
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClose.Location = New System.Drawing.Point(567, 6)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(100, 100)
        Me.btnClose.TabIndex = 16
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'btnStart
        '
        Me.btnStart.BackgroundImage = Global.ESRI.ArcGISTemplates.My.Resources.Resources.GreenCam100
        Me.btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnStart.FlatAppearance.BorderSize = 0
        Me.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStart.Location = New System.Drawing.Point(11, 6)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(100, 100)
        Me.btnStart.TabIndex = 15
        '
        'ResolutionMenuComboBox
        '
        Me.ResolutionMenuComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ResolutionMenuComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ResolutionMenuComboBox.FormattingEnabled = True
        Me.ResolutionMenuComboBox.Location = New System.Drawing.Point(149, 70)
        Me.ResolutionMenuComboBox.Name = "ResolutionMenuComboBox"
        Me.ResolutionMenuComboBox.Size = New System.Drawing.Size(374, 33)
        Me.ResolutionMenuComboBox.TabIndex = 18
        '
        'CameraForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(676, 484)
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
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private m_LastPhoto As String

    'Panasonic drivers for camera
    Private ActiveCamera As Camera
    'Private ZoomOverlay As TextOverlay
    'Private ResolutionOverlay As TextOverlay
    Private Loaded As Boolean = False


    Private Sub CameraForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

            Dim cameras As Camera() = Camera.Cameras

            If cameras Is Nothing Then
                ' No devices so end the application
                MessageBox.Show("No Devices Detected.")
                Me.Close()
                Return
            ElseIf cameras.Length = 0 Then
                ' No devices so end the application
                MessageBox.Show("No Devices Detected.")
                Me.Close()
                Return
            End If

            ActiveCamera = cameras(0)
            ActiveCamera.StartPreview(ActiveCamera.CameraFormats(0), CameraViewPanel.Handle)
            loadActiveCameras()


            ' Determine text overlay size
            'Dim res As Size = ActiveCamera.ActiveCameraFormat.Resolution
            'Dim g As Graphics = CreateGraphics()
            'Dim zoomText As [String] = "Zoom x1.0"
            'Dim location As New Point(res.Width - TextOverlay.DetermineTextWidth(zoomText, 25.0F, g), res.Height - TextOverlay.DetermineTextHeight(zoomText, 25.0F, g))
            'Dim height As Integer = TextOverlay.DetermineTextHeight(zoomText, 25.0F, g)

            '' Create the text overlays
            'ResolutionOverlay = New TextOverlay(res.Width & "x" & res.Height, Color.White, 25, New Point(0, res.Height - height), g)
            'ZoomOverlay = New TextOverlay(zoomText, Color.White, 25.0F, location, g)

            '' Register the text overlays
            'ActiveCamera.AddTextOverlay(ResolutionOverlay)
            'ActiveCamera.AddTextOverlay(ZoomOverlay)
            ActiveCamera.CameraViewOrientation = RotateFlipType.RotateNoneFlipNone

            ' Register for display change events
            AddHandler Microsoft.Win32.SystemEvents.DisplaySettingsChanged, AddressOf displaySettingsChanged



            ' The form is now ready and loaded
            Loaded = True
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message, "Error")
        Catch ex As TypeInitializationException
            MessageBox.Show("Error loading Toughbook SDK", "Error")
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error")
        End Try
    End Sub
    Private Sub displaySettingsChanged(sender As Object, e As EventArgs)
        ' A screen change was made, so lets restart the camera preview
        ActiveCamera.EndPreview()
        ActiveCamera.StartPreview(ActiveCamera.CameraFormats(0), CameraViewPanel.Handle)

        ' May need to set the camera orientation since the screen orientation may have also changed
    End Sub

#Region "Active Camera Controls"
    Private Sub ActiveCameraMenuComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ActiveCameraMenuComboBox.SelectedIndexChanged
        Dim obj As [Object] = ActiveCameraMenuComboBox.SelectedItem
        Dim item As ComboBoxItem = TryCast(obj, ComboBoxItem)

        If item IsNot Nothing AndAlso TypeOf item.Value Is Camera Then
            Dim camera As Camera = DirectCast(item.Value, Camera)
            ActiveCamera.EndPreview()

            ActiveCamera = camera
            ActiveCamera.StartPreview(camera.CameraFormats(0), CameraViewPanel.Handle)


        End If
    End Sub

    Private Sub loadActiveCameras()
        Dim cameras As Camera() = Camera.Cameras
        ActiveCameraMenuComboBox.Items.Clear()
        ActiveCameraMenuComboBox.DisplayMember = "Text"
        ActiveCameraMenuComboBox.ValueMember = "Value"

        For Each camera__1 As Camera In cameras
            ActiveCameraMenuComboBox.Items.Add(New ComboBoxItem(camera__1.FriendlyName, camera__1))
            If camera__1 = ActiveCamera Then
                ActiveCameraMenuComboBox.SelectedIndex = ActiveCameraMenuComboBox.FindStringExact(camera__1.FriendlyName)
            End If
        Next
        loadResolutions()

    End Sub
    Private Sub loadResolutions()
        ResolutionMenuComboBox.DisplayMember = "Text"
        ResolutionMenuComboBox.ValueMember = "Value"


        Dim formats As Camera.CameraFormat() = ActiveCamera.CameraFormats
        For Each format As Camera.CameraFormat In formats
            If CInt(format.FrameRate) = CInt(ActiveCamera.FrameRate) Then
                Dim resolutionText As [String] = format.Resolution.Width.ToString() & "x" & format.Resolution.Height.ToString()
                If ResolutionMenuComboBox.FindStringExact(resolutionText) > 0 Then

                Else

                    Dim resolutions As New List(Of Size)()
                    resolutions.Add(format.Resolution)

                    ResolutionMenuComboBox.Items.Add(New ComboBoxItem(resolutionText, format))

                    If ActiveCamera.ActiveCameraFormat.Resolution = format.Resolution Then
                        ResolutionMenuComboBox.SelectedIndex = ResolutionMenuComboBox.FindStringExact(resolutionText)
                    End If
                End If
                
            Else

            End If

        Next
    End Sub
    Private Sub ResolutionMenuComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ResolutionMenuComboBox.SelectedIndexChanged
        Dim obj As [Object] = ResolutionMenuComboBox.SelectedItem
        Dim item As ComboBoxItem = TryCast(obj, ComboBoxItem)

        If TypeOf item.Value Is Camera.CameraFormat Then
            Dim format As Camera.CameraFormat = TryCast(item.Value, Camera.CameraFormat)
            If format <> ActiveCamera.ActiveCameraFormat Then
                ActiveCamera.ActiveCameraFormat = format

                'Dim g As Graphics = Me.CreateGraphics()
                'Dim zoomText As [String] = [String].Format("Zoom x{0:0.0}", ActiveCamera.ZoomPercentage)
                'Dim resolutionText As [String] = format.Resolution.Width & "x" & format.Resolution.Height
                'Dim width As Integer = TextOverlay.DetermineTextWidth(zoomText, 25, g)
                'Dim height As Integer = TextOverlay.DetermineTextHeight(resolutionText, 25, g)
                'Dim zoomLocation As New Point(format.Resolution.Width - width, format.Resolution.Height - height)

                'ZoomOverlay = New TextOverlay(zoomText, Color.White, 25, zoomLocation, g)
                'ResolutionOverlay = New TextOverlay(resolutionText, Color.White, 25, New Point(0, format.Resolution.Height - height), g)
                'ActiveCamera.UpdateTextOverlay(ResolutionOverlay)
                'ActiveCamera.UpdateTextOverlay(ZoomOverlay)
            End If
        End If
    End Sub
#End Region


    Public ReadOnly Property Photo As String
        Get
            Return m_LastPhoto
        End Get

    End Property

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        ' Determine the image file types to set the dialog filter
        Dim supportedFormats As ImageFormats = ActiveCamera.SupportedImageFormats

        Dim fileExt As String

        If (supportedFormats And ImageFormats.Png) <> ImageFormats.None Then
            fileExt = ".png"
        ElseIf (supportedFormats And ImageFormats.Jpeg) <> ImageFormats.None Then
            fileExt = ".jpg"
        ElseIf (supportedFormats And ImageFormats.Bmp) <> ImageFormats.None Then
            fileExt = ".bmp"
        End If
        m_LastPhoto = GlobalsFunctions.getRandomFileNameInLocation("MobilePhoto_{0}" & fileExt)

        If (ActiveCamera.IsPlaying) Then

            ActiveCamera.StopVideo()

        End If
        ActiveCamera.CaptureToFile(m_LastPhoto)
        ActiveCamera.EndPreview()
        Me.Close()

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        If (ActiveCamera.IsPlaying) Then

            ActiveCamera.StopVideo()

        End If
        ActiveCamera.EndPreview()
        Me.Close()

    End Sub
End Class
Friend Class ComboBoxItem
    Public Property Text() As [String]
        Get
            Return m_Text
        End Get
        Set(value As [String])
            m_Text = value
        End Set
    End Property
    Private m_Text As [String]
    Public Property Value() As [Object]
        Get
            Return m_Value
        End Get
        Set(value As [Object])
            m_Value = value
        End Set
    End Property
    Private m_Value As [Object]

    Public Sub New(text As [String], value As [Object])
        Me.Text = text
        Me.Value = value
    End Sub
End Class