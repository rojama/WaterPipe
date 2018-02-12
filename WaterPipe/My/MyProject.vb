Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Diagnostics
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports WaterPipe

Namespace WaterPipe.My
    <HideModuleName, StandardModule, GeneratedCode("MyTemplate", "8.0.0.0")> _
    Friend NotInheritable Class MyProject
        ' Properties
        <HelpKeyword("My.Application")> _
        Friend Shared ReadOnly Property Application As MyApplication
            Get
                Return MyProject.m_AppObjectProvider.GetInstance
            End Get
        End Property

        <HelpKeyword("My.Computer")> _
        Friend Shared ReadOnly Property Computer As MyComputer
            Get
                Return MyProject.m_ComputerObjectProvider.GetInstance
            End Get
        End Property

        <HelpKeyword("My.Forms")> _
        Friend Shared ReadOnly Property Forms As MyForms
            Get
                Return MyProject.m_MyFormsObjectProvider.GetInstance
            End Get
        End Property

        <HelpKeyword("My.User")> _
        Friend Shared ReadOnly Property User As User
            Get
                Return MyProject.m_UserObjectProvider.GetInstance
            End Get
        End Property

        <HelpKeyword("My.WebServices")> _
        Friend Shared ReadOnly Property WebServices As MyWebServices
            Get
                Return MyProject.m_MyWebServicesObjectProvider.GetInstance
            End Get
        End Property


        ' Fields
        Private Shared ReadOnly m_AppObjectProvider As ThreadSafeObjectProvider([Of] MyApplication) = New ThreadSafeObjectProvider([Of] MyApplication)
        Private Shared ReadOnly m_ComputerObjectProvider As ThreadSafeObjectProvider([Of] MyComputer) = New ThreadSafeObjectProvider([Of] MyComputer)
        Private Shared m_MyFormsObjectProvider As ThreadSafeObjectProvider([Of] MyForms) = New ThreadSafeObjectProvider([Of] MyForms)
        Private Shared ReadOnly m_MyWebServicesObjectProvider As ThreadSafeObjectProvider([Of] MyWebServices) = New ThreadSafeObjectProvider([Of] MyWebServices)
        Private Shared ReadOnly m_UserObjectProvider As ThreadSafeObjectProvider([Of] User) = New ThreadSafeObjectProvider([Of] User)

        ' Nested Types
        <MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms"), EditorBrowsable(EditorBrowsableState.Never)> _
        Friend NotInheritable Class MyForms
            ' Methods
            <DebuggerHidden> _
            Private Shared Function Create__Instance__([Of] T As { Form, New })(ByVal Instance As T) As T
                Dim local As T
                If ((Not Instance Is Nothing) AndAlso Not Instance.IsDisposed) Then
                    Return Instance
                End If
                If (Not MyForms.m_FormBeingCreated Is Nothing) Then
                    If MyForms.m_FormBeingCreated.ContainsKey(GetType(T)) Then
                        Throw New InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", New String(0  - 1) {}))
                    End If
                Else
                    MyForms.m_FormBeingCreated = New Hashtable
                End If
                MyForms.m_FormBeingCreated.Add(GetType(T), Nothing)
                Try 
                    local = Activator.CreateInstance([Of] T)
                Catch obj1 As Object When (?)
                    Dim exception As TargetInvocationException
                    Throw New InvalidOperationException(Utils.GetResourceString("WinForms_SeeInnerException", New String() { exception.InnerException.Message }), exception.InnerException)
                Finally
                    MyForms.m_FormBeingCreated.Remove(GetType(T))
                End Try
                Return local
            End Function

            <DebuggerHidden> _
            Private Sub Dispose__Instance__([Of] T As Form)(ByRef instance As T)
                instance.Dispose
                instance = CType(Nothing, T)
            End Sub

            <EditorBrowsable(EditorBrowsableState.Never)> _
            Public Overrides Function Equals(ByVal o As Object) As Boolean
                Return MyBase.Equals(RuntimeHelpers.GetObjectValue(o))
            End Function

            <EditorBrowsable(EditorBrowsableState.Never)> _
            Public Overrides Function GetHashCode() As Integer
                Return MyBase.GetHashCode
            End Function

            <EditorBrowsable(EditorBrowsableState.Never)> _
            Friend Function [GetType]() As Type
                Return GetType(MyForms)
            End Function

            <EditorBrowsable(EditorBrowsableState.Never)> _
            Public Overrides Function ToString() As String
                Return MyBase.ToString
            End Function


            ' Properties
            Public Property AboutBox As AboutBox
                Get
                    Me.m_AboutBox = MyForms.Create__Instance__([Of] AboutBox)(Me.m_AboutBox)
                    Return Me.m_AboutBox
                End Get
                Set(ByVal Value As AboutBox)
                    If (Not Value Is Me.m_AboutBox) Then
                        If (Not Value Is Nothing) Then
                            Throw New ArgumentException("Property can only be set to Nothing")
                        End If
                        Me.Dispose__Instance__([Of] AboutBox)((Me.m_AboutBox))
                    End If
                End Set
            End Property

            Public Property HighScoreDialog As HighScoreDialog
                Get
                    Me.m_HighScoreDialog = MyForms.Create__Instance__([Of] HighScoreDialog)(Me.m_HighScoreDialog)
                    Return Me.m_HighScoreDialog
                End Get
                Set(ByVal Value As HighScoreDialog)
                    If (Not Value Is Me.m_HighScoreDialog) Then
                        If (Not Value Is Nothing) Then
                            Throw New ArgumentException("Property can only be set to Nothing")
                        End If
                        Me.Dispose__Instance__([Of] HighScoreDialog)((Me.m_HighScoreDialog))
                    End If
                End Set
            End Property

            Public Property MainForm As MainForm
                Get
                    Me.m_MainForm = MyForms.Create__Instance__([Of] MainForm)(Me.m_MainForm)
                    Return Me.m_MainForm
                End Get
                Set(ByVal Value As MainForm)
                    If (Not Value Is Me.m_MainForm) Then
                        If (Not Value Is Nothing) Then
                            Throw New ArgumentException("Property can only be set to Nothing")
                        End If
                        Me.Dispose__Instance__([Of] MainForm)((Me.m_MainForm))
                    End If
                End Set
            End Property

            Public Property SplashScreen As SplashScreen
                Get
                    Me.m_SplashScreen = MyForms.Create__Instance__([Of] SplashScreen)(Me.m_SplashScreen)
                    Return Me.m_SplashScreen
                End Get
                Set(ByVal Value As SplashScreen)
                    If (Not Value Is Me.m_SplashScreen) Then
                        If (Not Value Is Nothing) Then
                            Throw New ArgumentException("Property can only be set to Nothing")
                        End If
                        Me.Dispose__Instance__([Of] SplashScreen)((Me.m_SplashScreen))
                    End If
                End Set
            End Property


            ' Fields
            Public m_AboutBox As AboutBox
            <ThreadStatic> _
            Private Shared m_FormBeingCreated As Hashtable
            Public m_HighScoreDialog As HighScoreDialog
            Public m_MainForm As MainForm
            Public m_SplashScreen As SplashScreen
        End Class

        <MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", ""), EditorBrowsable(EditorBrowsableState.Never)> _
        Friend NotInheritable Class MyWebServices
            ' Methods
            <DebuggerHidden> _
            Private Shared Function Create__Instance__([Of] T As New)(ByVal instance As T) As T
                If (instance Is Nothing) Then
                    Return Activator.CreateInstance([Of] T)
                End If
                Return instance
            End Function

            <DebuggerHidden> _
            Private Sub Dispose__Instance__([Of] T)(ByRef instance As T)
                instance = CType(Nothing, T)
            End Sub

            <EditorBrowsable(EditorBrowsableState.Never), DebuggerHidden> _
            Public Overrides Function Equals(ByVal o As Object) As Boolean
                Return MyBase.Equals(RuntimeHelpers.GetObjectValue(o))
            End Function

            <DebuggerHidden, EditorBrowsable(EditorBrowsableState.Never)> _
            Public Overrides Function GetHashCode() As Integer
                Return MyBase.GetHashCode
            End Function

            <EditorBrowsable(EditorBrowsableState.Never), DebuggerHidden> _
            Friend Function [GetType]() As Type
                Return GetType(MyWebServices)
            End Function

            <DebuggerHidden, EditorBrowsable(EditorBrowsableState.Never)> _
            Public Overrides Function ToString() As String
                Return MyBase.ToString
            End Function

        End Class

        <ComVisible(False), EditorBrowsable(EditorBrowsableState.Never)> _
        Friend NotInheritable Class ThreadSafeObjectProvider([Of] T As New)
            ' Properties
            Friend ReadOnly Property GetInstance As T
                Get
                    If (ThreadSafeObjectProvider([Of] T).m_ThreadStaticValue Is Nothing) Then
                        ThreadSafeObjectProvider([Of] T).m_ThreadStaticValue = Activator.CreateInstance([Of] T)
                    End If
                    Return ThreadSafeObjectProvider([Of] T).m_ThreadStaticValue
                End Get
            End Property


            ' Fields
            <CompilerGenerated, ThreadStatic> _
            Private Shared m_ThreadStaticValue As T
        End Class
    End Class
End Namespace

