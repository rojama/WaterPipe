﻿Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Configuration
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Namespace WaterPipe.My
    <GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0"), EditorBrowsable(EditorBrowsableState.Advanced), CompilerGenerated> _
    Friend NotInheritable Class MySettings
        Inherits ApplicationSettingsBase
        ' Methods
        <DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Advanced)> _
        Private Shared Sub AutoSaveSettings(ByVal sender As Object, ByVal e As EventArgs)
            If MyProject.Application.SaveMySettingsOnExit Then
                MySettingsProperty.Settings.Save
            End If
        End Sub


        ' Properties
        Public Shared ReadOnly Property [Default] As MySettings
            Get
                If Not MySettings.addedHandler Then
                    Dim addedHandlerLockObject As Object = MySettings.addedHandlerLockObject
                    ObjectFlowControl.CheckForSyncLockOnValueType(addedHandlerLockObject)
                    SyncLock addedHandlerLockObject
                        If Not MySettings.addedHandler Then
                            AddHandler MyProject.Application.Shutdown, New ShutdownEventHandler(AddressOf MySettings.AutoSaveSettings)
                            MySettings.addedHandler = True
                        End If
                    End SyncLock
                End If
                Return MySettings.defaultInstance
            End Get
        End Property


        ' Fields
        Private Shared addedHandler As Boolean
        Private Shared addedHandlerLockObject As Object = RuntimeHelpers.GetObjectValue(New Object)
        Private Shared defaultInstance As MySettings = DirectCast(SettingsBase.Synchronized(New MySettings), MySettings)
    End Class
End Namespace

