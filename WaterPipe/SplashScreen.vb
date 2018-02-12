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
    Public NotInheritable Class SplashScreen
        Inherits Form
        ' Methods
        <DebuggerNonUserCode> _
        Public Sub New()
            AddHandler MyBase.Load, New EventHandler(AddressOf Me.SplashScreen1_Load)
            Me.InitializeComponent
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
            Dim manager As New ComponentResourceManager(GetType(SplashScreen))
            Me.MainLayoutPanel = New TableLayoutPanel
            Me.DetailsLayoutPanel = New TableLayoutPanel
            Me.Version = New Label
            Me.Copyright = New Label
            Me.ApplicationTitle = New Label
            Me.MainLayoutPanel.SuspendLayout
            Me.DetailsLayoutPanel.SuspendLayout
            Me.SuspendLayout
            Me.MainLayoutPanel.BackgroundImage = DirectCast(manager.GetObject("MainLayoutPanel.BackgroundImage"), Image)
            Me.MainLayoutPanel.BackgroundImageLayout = ImageLayout.Stretch
            Me.MainLayoutPanel.ColumnCount = 2
            Me.MainLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 243!))
            Me.MainLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 100!))
            Me.MainLayoutPanel.Controls.Add(Me.DetailsLayoutPanel, 1, 1)
            Me.MainLayoutPanel.Controls.Add(Me.ApplicationTitle, 1, 0)
            Me.MainLayoutPanel.Dock = DockStyle.Fill
            Dim point As New Point(0, 0)
            Me.MainLayoutPanel.Location = point
            Me.MainLayoutPanel.Name = "MainLayoutPanel"
            Me.MainLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 218!))
            Me.MainLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 38!))
            Dim size As New Size(&H1F0, &H12F)
            Me.MainLayoutPanel.Size = size
            Me.MainLayoutPanel.TabIndex = 0
            Me.DetailsLayoutPanel.Anchor = AnchorStyles.None
            Me.DetailsLayoutPanel.BackColor = Color.Transparent
            Me.DetailsLayoutPanel.ColumnCount = 1
            Me.DetailsLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 247!))
            Me.DetailsLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 142!))
            Me.DetailsLayoutPanel.Controls.Add(Me.Version, 0, 0)
            Me.DetailsLayoutPanel.Controls.Add(Me.Copyright, 0, 1)
            point = New Point(&HF6, &HDD)
            Me.DetailsLayoutPanel.Location = point
            Me.DetailsLayoutPanel.Name = "DetailsLayoutPanel"
            Me.DetailsLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 33!))
            Me.DetailsLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 33!))
            size = New Size(&HF7, &H4F)
            Me.DetailsLayoutPanel.Size = size
            Me.DetailsLayoutPanel.TabIndex = 1
            Me.Version.Anchor = AnchorStyles.None
            Me.Version.BackColor = Color.Transparent
            Me.Version.Font = New Font("Microsoft Sans Serif", 9!, FontStyle.Regular, GraphicsUnit.Point, 0)
            point = New Point(3, 9)
            Me.Version.Location = point
            Me.Version.Name = "Version"
            size = New Size(&HF1, 20)
            Me.Version.Size = size
            Me.Version.TabIndex = 1
            Me.Version.Text = "Version {0}.{1:00}"
            Me.Copyright.Anchor = AnchorStyles.None
            Me.Copyright.BackColor = Color.Transparent
            Me.Copyright.Font = New Font("Microsoft Sans Serif", 9!, FontStyle.Regular, GraphicsUnit.Point, 0)
            point = New Point(3, &H27)
            Me.Copyright.Location = point
            Me.Copyright.Name = "Copyright"
            size = New Size(&HF1, 40)
            Me.Copyright.Size = size
            Me.Copyright.TabIndex = 2
            Me.Copyright.Text = "Copyright"
            Me.ApplicationTitle.Anchor = AnchorStyles.None
            Me.ApplicationTitle.BackColor = Color.Transparent
            Me.ApplicationTitle.Font = New Font("Microsoft Sans Serif", 18!, FontStyle.Regular, GraphicsUnit.Point, 0)
            point = New Point(&HF6, 3)
            Me.ApplicationTitle.Location = point
            Me.ApplicationTitle.Name = "ApplicationTitle"
            size = New Size(&HF7, &HD4)
            Me.ApplicationTitle.Size = size
            Me.ApplicationTitle.TabIndex = 0
            Me.ApplicationTitle.Text = "ApplicationTitle"
            Me.ApplicationTitle.TextAlign = ContentAlignment.BottomLeft
            Dim ef As New SizeF(6!, 13!)
            Me.AutoScaleDimensions = ef
            Me.AutoScaleMode = AutoScaleMode.Font
            size = New Size(&H1F0, &H12F)
            Me.ClientSize = size
            Me.ControlBox = False
            Me.Controls.Add(Me.MainLayoutPanel)
            Me.FormBorderStyle = FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "SplashScreen"
            Me.ShowInTaskbar = False
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.MainLayoutPanel.ResumeLayout(False)
            Me.DetailsLayoutPanel.ResumeLayout(False)
            Me.ResumeLayout(False)
        End Sub

        Private Sub SplashScreen1_Load(ByVal sender As Object, ByVal e As EventArgs)
            If (MyProject.Application.Info.Title <> "") Then
                Me.ApplicationTitle.Text = MyProject.Application.Info.Title
            Else
                Me.ApplicationTitle.Text = Path.GetFileNameWithoutExtension(MyProject.Application.Info.AssemblyName)
            End If
            Me.Version.Text = String.Format(Me.Version.Text, MyProject.Application.Info.Version.Major, MyProject.Application.Info.Version.Minor)
            Me.Copyright.Text = MyProject.Application.Info.Copyright
        End Sub


        ' Properties
        Friend Overridable Property ApplicationTitle As Label
            Get
                Return Me._ApplicationTitle
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._ApplicationTitle = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Copyright As Label
            Get
                Return Me._Copyright
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._Copyright = WithEventsValue
            End Set
        End Property

        Friend Overridable Property DetailsLayoutPanel As TableLayoutPanel
            Get
                Return Me._DetailsLayoutPanel
            End Get
            Set(ByVal WithEventsValue As TableLayoutPanel)
                Me._DetailsLayoutPanel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property MainLayoutPanel As TableLayoutPanel
            Get
                Return Me._MainLayoutPanel
            End Get
            Set(ByVal WithEventsValue As TableLayoutPanel)
                Me._MainLayoutPanel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Version As Label
            Get
                Return Me._Version
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._Version = WithEventsValue
            End Set
        End Property


        ' Fields
        <AccessedThroughProperty("ApplicationTitle")> _
        Private _ApplicationTitle As Label
        <AccessedThroughProperty("Copyright")> _
        Private _Copyright As Label
        <AccessedThroughProperty("DetailsLayoutPanel")> _
        Private _DetailsLayoutPanel As TableLayoutPanel
        <AccessedThroughProperty("MainLayoutPanel")> _
        Private _MainLayoutPanel As TableLayoutPanel
        <AccessedThroughProperty("Version")> _
        Private _Version As Label
        Private components As IContainer
    End Class
End Namespace

