Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports WaterPipe.My

Namespace WaterPipe
    <DesignerGenerated> _
    Public NotInheritable Class AboutBox
        Inherits Form
        ' Methods
        <DebuggerNonUserCode> _
        Public Sub New()
            AddHandler MyBase.Load, New EventHandler(AddressOf Me.AboutBox_Load)
            Me.InitializeComponent
        End Sub

        Private Sub AboutBox_Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim title As String
            If (MyProject.Application.Info.Title <> "") Then
                title = MyProject.Application.Info.Title
            Else
                title = Path.GetFileNameWithoutExtension(MyProject.Application.Info.AssemblyName)
            End If
            Me.Text = String.Format("About {0}", title)
            Me.LabelProductName.Text = MyProject.Application.Info.ProductName
            Me.LabelVersion.Text = String.Format("Version {0}", MyProject.Application.Info.Version.ToString)
            Me.LabelCopyright.Text = MyProject.Application.Info.Copyright
            Me.LabelCompanyName.Text = MyProject.Application.Info.CompanyName
            Me.TextBoxDescription.Text = MyProject.Application.Info.Description
        End Sub

        <DebuggerNonUserCode> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing AndAlso (Not Me.components Is Nothing)) Then
                Me.components.Dispose
            End If
            MyBase.Dispose(disposing)
        End Sub

        <DebuggerStepThrough> _
        Private Sub InitializeComponent()
            Dim manager As New ComponentResourceManager(GetType(AboutBox))
            Me.TableLayoutPanel = New TableLayoutPanel
            Me.LogoPictureBox = New PictureBox
            Me.LabelProductName = New Label
            Me.LabelVersion = New Label
            Me.LabelCopyright = New Label
            Me.LabelCompanyName = New Label
            Me.TextBoxDescription = New TextBox
            Me.OKButton = New Button
            Me.TableLayoutPanel.SuspendLayout
            DirectCast(Me.LogoPictureBox, ISupportInitialize).BeginInit
            Me.SuspendLayout
            Me.TableLayoutPanel.ColumnCount = 2
            Me.TableLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33!))
            Me.TableLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 67!))
            Me.TableLayoutPanel.Controls.Add(Me.LogoPictureBox, 0, 0)
            Me.TableLayoutPanel.Controls.Add(Me.LabelProductName, 1, 0)
            Me.TableLayoutPanel.Controls.Add(Me.LabelVersion, 1, 1)
            Me.TableLayoutPanel.Controls.Add(Me.LabelCopyright, 1, 2)
            Me.TableLayoutPanel.Controls.Add(Me.LabelCompanyName, 1, 3)
            Me.TableLayoutPanel.Controls.Add(Me.TextBoxDescription, 1, 4)
            Me.TableLayoutPanel.Controls.Add(Me.OKButton, 1, 5)
            Me.TableLayoutPanel.Dock = DockStyle.Fill
            Dim point As New Point(9, 9)
            Me.TableLayoutPanel.Location = point
            Me.TableLayoutPanel.Name = "TableLayoutPanel"
            Me.TableLayoutPanel.RowCount = 6
            Me.TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10!))
            Me.TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10!))
            Me.TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10!))
            Me.TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10!))
            Me.TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 50!))
            Me.TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10!))
            Dim size As New Size(&H18C, &H102)
            Me.TableLayoutPanel.Size = size
            Me.TableLayoutPanel.TabIndex = 0
            Me.LogoPictureBox.Dock = DockStyle.Fill
            Me.LogoPictureBox.Image = DirectCast(manager.GetObject("LogoPictureBox.Image"), Image)
            point = New Point(3, 3)
            Me.LogoPictureBox.Location = point
            Me.LogoPictureBox.Name = "LogoPictureBox"
            Me.TableLayoutPanel.SetRowSpan(Me.LogoPictureBox, 6)
            size = New Size(&H7C, &HFC)
            Me.LogoPictureBox.Size = size
            Me.LogoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
            Me.LogoPictureBox.TabIndex = 0
            Me.LogoPictureBox.TabStop = False
            Me.LabelProductName.Dock = DockStyle.Fill
            point = New Point(&H88, 0)
            Me.LabelProductName.Location = point
            Dim padding As New Padding(6, 0, 3, 0)
            Me.LabelProductName.Margin = padding
            size = New Size(0, &H11)
            Me.LabelProductName.MaximumSize = size
            Me.LabelProductName.Name = "LabelProductName"
            size = New Size(&H101, &H11)
            Me.LabelProductName.Size = size
            Me.LabelProductName.TabIndex = 0
            Me.LabelProductName.Text = "Product Name"
            Me.LabelProductName.TextAlign = ContentAlignment.MiddleLeft
            Me.LabelVersion.Dock = DockStyle.Fill
            point = New Point(&H88, &H19)
            Me.LabelVersion.Location = point
            padding = New Padding(6, 0, 3, 0)
            Me.LabelVersion.Margin = padding
            size = New Size(0, &H11)
            Me.LabelVersion.MaximumSize = size
            Me.LabelVersion.Name = "LabelVersion"
            size = New Size(&H101, &H11)
            Me.LabelVersion.Size = size
            Me.LabelVersion.TabIndex = 0
            Me.LabelVersion.Text = "Version"
            Me.LabelVersion.TextAlign = ContentAlignment.MiddleLeft
            Me.LabelCopyright.Dock = DockStyle.Fill
            point = New Point(&H88, 50)
            Me.LabelCopyright.Location = point
            padding = New Padding(6, 0, 3, 0)
            Me.LabelCopyright.Margin = padding
            size = New Size(0, &H11)
            Me.LabelCopyright.MaximumSize = size
            Me.LabelCopyright.Name = "LabelCopyright"
            size = New Size(&H101, &H11)
            Me.LabelCopyright.Size = size
            Me.LabelCopyright.TabIndex = 0
            Me.LabelCopyright.Text = "Copyright"
            Me.LabelCopyright.TextAlign = ContentAlignment.MiddleLeft
            Me.LabelCompanyName.Dock = DockStyle.Fill
            point = New Point(&H88, &H4B)
            Me.LabelCompanyName.Location = point
            padding = New Padding(6, 0, 3, 0)
            Me.LabelCompanyName.Margin = padding
            size = New Size(0, &H11)
            Me.LabelCompanyName.MaximumSize = size
            Me.LabelCompanyName.Name = "LabelCompanyName"
            size = New Size(&H101, &H11)
            Me.LabelCompanyName.Size = size
            Me.LabelCompanyName.TabIndex = 0
            Me.LabelCompanyName.Text = "Company Name"
            Me.LabelCompanyName.TextAlign = ContentAlignment.MiddleLeft
            Me.TextBoxDescription.Dock = DockStyle.Fill
            point = New Point(&H88, &H67)
            Me.TextBoxDescription.Location = point
            padding = New Padding(6, 3, 3, 3)
            Me.TextBoxDescription.Margin = padding
            Me.TextBoxDescription.Multiline = True
            Me.TextBoxDescription.Name = "TextBoxDescription"
            Me.TextBoxDescription.ReadOnly = True
            Me.TextBoxDescription.ScrollBars = ScrollBars.Both
            size = New Size(&H101, &H7B)
            Me.TextBoxDescription.Size = size
            Me.TextBoxDescription.TabIndex = 0
            Me.TextBoxDescription.TabStop = False
            Me.TextBoxDescription.Text = manager.GetString("TextBoxDescription.Text")
            Me.OKButton.Anchor = (AnchorStyles.Right Or AnchorStyles.Bottom)
            Me.OKButton.DialogResult = DialogResult.Cancel
            point = New Point(&H13E, &HE8)
            Me.OKButton.Location = point
            Me.OKButton.Name = "OKButton"
            size = New Size(&H4B, &H17)
            Me.OKButton.Size = size
            Me.OKButton.TabIndex = 0
            Me.OKButton.Text = "&OK"
            Dim ef As New SizeF(6!, 13!)
            Me.AutoScaleDimensions = ef
            Me.AutoScaleMode = AutoScaleMode.Font
            Me.CancelButton = Me.OKButton
            size = New Size(&H19E, &H114)
            Me.ClientSize = size
            Me.Controls.Add(Me.TableLayoutPanel)
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "AboutBox"
            padding = New Padding(9)
            Me.Padding = padding
            Me.ShowInTaskbar = False
            Me.StartPosition = FormStartPosition.CenterParent
            Me.Text = "AboutBox"
            Me.TableLayoutPanel.ResumeLayout(False)
            Me.TableLayoutPanel.PerformLayout
            DirectCast(Me.LogoPictureBox, ISupportInitialize).EndInit
            Me.ResumeLayout(False)
        End Sub

        Private Sub OKButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.Close
        End Sub


        ' Properties
        Friend Overridable Property LabelCompanyName As Label
            Get
                Return Me._LabelCompanyName
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._LabelCompanyName = WithEventsValue
            End Set
        End Property

        Friend Overridable Property LabelCopyright As Label
            Get
                Return Me._LabelCopyright
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._LabelCopyright = WithEventsValue
            End Set
        End Property

        Friend Overridable Property LabelProductName As Label
            Get
                Return Me._LabelProductName
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._LabelProductName = WithEventsValue
            End Set
        End Property

        Friend Overridable Property LabelVersion As Label
            Get
                Return Me._LabelVersion
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._LabelVersion = WithEventsValue
            End Set
        End Property

        Friend Overridable Property LogoPictureBox As PictureBox
            Get
                Return Me._LogoPictureBox
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._LogoPictureBox = WithEventsValue
            End Set
        End Property

        Friend Overridable Property OKButton As Button
            Get
                Return Me._OKButton
            End Get
            Set(ByVal WithEventsValue As Button)
                If (Not Me._OKButton Is Nothing) Then
                    RemoveHandler Me._OKButton.Click, New EventHandler(AddressOf Me.OKButton_Click)
                End If
                Me._OKButton = WithEventsValue
                If (Not Me._OKButton Is Nothing) Then
                    AddHandler Me._OKButton.Click, New EventHandler(AddressOf Me.OKButton_Click)
                End If
            End Set
        End Property

        Friend Overridable Property TableLayoutPanel As TableLayoutPanel
            Get
                Return Me._TableLayoutPanel
            End Get
            Set(ByVal WithEventsValue As TableLayoutPanel)
                Me._TableLayoutPanel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property TextBoxDescription As TextBox
            Get
                Return Me._TextBoxDescription
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._TextBoxDescription = WithEventsValue
            End Set
        End Property


        ' Fields
        <AccessedThroughProperty("LabelCompanyName")> _
        Private _LabelCompanyName As Label
        <AccessedThroughProperty("LabelCopyright")> _
        Private _LabelCopyright As Label
        <AccessedThroughProperty("LabelProductName")> _
        Private _LabelProductName As Label
        <AccessedThroughProperty("LabelVersion")> _
        Private _LabelVersion As Label
        <AccessedThroughProperty("LogoPictureBox")> _
        Private _LogoPictureBox As PictureBox
        <AccessedThroughProperty("OKButton")> _
        Private _OKButton As Button
        <AccessedThroughProperty("TableLayoutPanel")> _
        Private _TableLayoutPanel As TableLayoutPanel
        <AccessedThroughProperty("TextBoxDescription")> _
        Private _TextBoxDescription As TextBox
        Private components As IContainer
    End Class
End Namespace

