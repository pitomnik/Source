VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IEMember"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' PopOops is pop-up blocking solution for Internet Explorer
' Copyright (C) 2002-2004 Shahin Gasanov
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

Option Explicit

Private WithEvents IEWindow As SHDocVw.InternetExplorer
Attribute IEWindow.VB_VarHelpID = -1

Private Sub Class_Terminate()
    Set IEWindow = Nothing
End Sub

Public Property Set Instance(ByVal Instance As SHDocVw.InternetExplorer)
    Set IEWindow = Instance
End Property

Public Property Get Handle() As Long
    Handle = IEWindow.hWnd
End Property

Public Sub Clear()
    IEWindow.Navigate2 "about:blank"
End Sub

Public Sub Abort()
    IEWindow.Stop
End Sub

Private Sub IEWindow_NewWindow2(ppDisp As Object, Cancel As Boolean)
    Cancel = Not (IsWindowClicked(IEWindow.hWnd) Or IsKeyPressed(vbKeyControl) Or IsKeyPressed(vbKeyShift))
    If Cancel Then CallEvent
End Sub
