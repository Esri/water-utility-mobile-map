<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AttributeDisplay
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AttributeDisplay))
        Me.gpBoxAttLay = New System.Windows.Forms.Panel()
        Me.pnlAttributes = New System.Windows.Forms.Panel()
        Me.spltAttTools = New System.Windows.Forms.SplitContainer()
        Me.pnlCurrentLayer = New System.Windows.Forms.Panel()
        Me.lblCurrentLayer = New System.Windows.Forms.Label()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.btnWaypoint = New System.Windows.Forms.Button()
        Me.btnGPSLoc = New System.Windows.Forms.Button()
        Me.btnFlash = New System.Windows.Forms.Button()
        Me.btnRouteTo = New System.Windows.Forms.Button()
        Me.btnZoomTo = New System.Windows.Forms.Button()
        Me.gpBoxAttLay.SuspendLayout()
        Me.pnlAttributes.SuspendLayout()
        Me.spltAttTools.Panel2.SuspendLayout()
        Me.spltAttTools.SuspendLayout()
        Me.pnlCurrentLayer.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpBoxAttLay
        '
        Me.gpBoxAttLay.Controls.Add(Me.pnlAttributes)
        Me.gpBoxAttLay.Controls.Add(Me.pnlCurrentLayer)
        Me.gpBoxAttLay.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpBoxAttLay.Location = New System.Drawing.Point(0, 0)
        Me.gpBoxAttLay.Name = "gpBoxAttLay"
        Me.gpBoxAttLay.Size = New System.Drawing.Size(308, 595)
        Me.gpBoxAttLay.TabIndex = 2
        '
        'pnlAttributes
        '
        Me.pnlAttributes.Controls.Add(Me.spltAttTools)
        Me.pnlAttributes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlAttributes.Location = New System.Drawing.Point(0, 35)
        Me.pnlAttributes.Name = "pnlAttributes"
        Me.pnlAttributes.Size = New System.Drawing.Size(308, 560)
        Me.pnlAttributes.TabIndex = 1
        '
        'spltAttTools
        '
        Me.spltAttTools.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltAttTools.IsSplitterFixed = True
        Me.spltAttTools.Location = New System.Drawing.Point(0, 0)
        Me.spltAttTools.Name = "spltAttTools"
        Me.spltAttTools.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'spltAttTools.Panel2
        '
        Me.spltAttTools.Panel2.Controls.Add(Me.btnEdit)
        Me.spltAttTools.Panel2.Controls.Add(Me.btnWaypoint)
        Me.spltAttTools.Panel2.Controls.Add(Me.btnGPSLoc)
        Me.spltAttTools.Panel2.Controls.Add(Me.btnFlash)
        Me.spltAttTools.Panel2.Controls.Add(Me.btnRouteTo)
        Me.spltAttTools.Panel2.Controls.Add(Me.btnZoomTo)
        Me.spltAttTools.Panel2MinSize = 55
        Me.spltAttTools.Size = New System.Drawing.Size(308, 560)
        Me.spltAttTools.SplitterDistance = 499
        Me.spltAttTools.TabIndex = 0
        '
        'pnlCurrentLayer
        '
        Me.pnlCurrentLayer.Controls.Add(Me.lblCurrentLayer)
        Me.pnlCurrentLayer.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlCurrentLayer.Location = New System.Drawing.Point(0, 0)
        Me.pnlCurrentLayer.Name = "pnlCurrentLayer"
        Me.pnlCurrentLayer.Size = New System.Drawing.Size(308, 35)
        Me.pnlCurrentLayer.TabIndex = 0
        '
        'lblCurrentLayer
        '
        Me.lblCurrentLayer.Location = New System.Drawing.Point(0, 0)
        Me.lblCurrentLayer.Name = "lblCurrentLayer"
        Me.lblCurrentLayer.Size = New System.Drawing.Size(308, 35)
        Me.lblCurrentLayer.TabIndex = 0
        Me.lblCurrentLayer.Text = "Label1"
        Me.lblCurrentLayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnEdit
        '
        Me.btnEdit.BackgroundImage = Global.MobileControls.My.Resources.Resources.Pen
        Me.btnEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnEdit.FlatAppearance.BorderSize = 0
        Me.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnEdit.Location = New System.Drawing.Point(80, 4)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(50, 50)
        Me.btnEdit.TabIndex = 5
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'btnWaypoint
        '
        Me.btnWaypoint.BackgroundImage = Global.MobileControls.My.Resources.Resources.NavTooBlue
        Me.btnWaypoint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnWaypoint.FlatAppearance.BorderSize = 0
        Me.btnWaypoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnWaypoint.Location = New System.Drawing.Point(126, 3)
        Me.btnWaypoint.Name = "btnWaypoint"
        Me.btnWaypoint.Size = New System.Drawing.Size(50, 50)
        Me.btnWaypoint.TabIndex = 4
        Me.btnWaypoint.UseVisualStyleBackColor = True
        '
        'btnGPSLoc
        '
        Me.btnGPSLoc.BackgroundImage = Global.MobileControls.My.Resources.Resources.SatImageBlue
        Me.btnGPSLoc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnGPSLoc.FlatAppearance.BorderSize = 0
        Me.btnGPSLoc.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnGPSLoc.Location = New System.Drawing.Point(188, 4)
        Me.btnGPSLoc.Name = "btnGPSLoc"
        Me.btnGPSLoc.Size = New System.Drawing.Size(50, 50)
        Me.btnGPSLoc.TabIndex = 3
        Me.btnGPSLoc.UseVisualStyleBackColor = True
        '
        'btnFlash
        '
        Me.btnFlash.BackgroundImage = Global.MobileControls.My.Resources.Resources.BlueFlashSmall
        Me.btnFlash.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnFlash.FlatAppearance.BorderSize = 0
        Me.btnFlash.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFlash.Location = New System.Drawing.Point(245, 4)
        Me.btnFlash.Name = "btnFlash"
        Me.btnFlash.Size = New System.Drawing.Size(50, 50)
        Me.btnFlash.TabIndex = 2
        Me.btnFlash.UseVisualStyleBackColor = True
        '
        'btnRouteTo
        '
        Me.btnRouteTo.BackgroundImage = CType(resources.GetObject("btnRouteTo.BackgroundImage"), System.Drawing.Image)
        Me.btnRouteTo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnRouteTo.FlatAppearance.BorderSize = 0
        Me.btnRouteTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRouteTo.Location = New System.Drawing.Point(65, 4)
        Me.btnRouteTo.Name = "btnRouteTo"
        Me.btnRouteTo.Size = New System.Drawing.Size(50, 50)
        Me.btnRouteTo.TabIndex = 1
        Me.btnRouteTo.UseVisualStyleBackColor = True
        '
        'btnZoomTo
        '
        Me.btnZoomTo.BackgroundImage = Global.MobileControls.My.Resources.Resources.ZoomInOut
        Me.btnZoomTo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnZoomTo.FlatAppearance.BorderSize = 0
        Me.btnZoomTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnZoomTo.Location = New System.Drawing.Point(10, 4)
        Me.btnZoomTo.Name = "btnZoomTo"
        Me.btnZoomTo.Size = New System.Drawing.Size(50, 50)
        Me.btnZoomTo.TabIndex = 0
        Me.btnZoomTo.UseVisualStyleBackColor = True
        '
        'AttributeDisplay
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.gpBoxAttLay)
        Me.Name = "AttributeDisplay"
        Me.Size = New System.Drawing.Size(308, 595)
        Me.gpBoxAttLay.ResumeLayout(False)
        Me.pnlAttributes.ResumeLayout(False)
        Me.spltAttTools.Panel2.ResumeLayout(False)
        Me.spltAttTools.ResumeLayout(False)
        Me.pnlCurrentLayer.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpBoxAttLay As System.Windows.Forms.Panel
    Friend WithEvents pnlAttributes As System.Windows.Forms.Panel
    Friend WithEvents spltAttTools As System.Windows.Forms.SplitContainer
    Friend WithEvents btnGPSLoc As System.Windows.Forms.Button
    Friend WithEvents btnFlash As System.Windows.Forms.Button
    Friend WithEvents btnRouteTo As System.Windows.Forms.Button
    Friend WithEvents btnZoomTo As System.Windows.Forms.Button
    Friend WithEvents pnlCurrentLayer As System.Windows.Forms.Panel
    Friend WithEvents lblCurrentLayer As System.Windows.Forms.Label
    Friend WithEvents btnWaypoint As System.Windows.Forms.Button
    Friend WithEvents btnEdit As System.Windows.Forms.Button

End Class
