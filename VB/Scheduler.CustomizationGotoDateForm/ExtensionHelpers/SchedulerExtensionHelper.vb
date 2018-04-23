Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports DevExpress.Web.ASPxScheduler.Internal
Imports DevExpress.Web.Mvc
Imports DevExpress.XtraScheduler.Services

Namespace Scheduler.CustomizationGotoDateForm.ExtensionHelpers
	Public Class MVCxSchedulerGotoDateFormCallbackCommand
		Inherits GotoDateFormCallbackCommand
		Public Sub New(ByVal control As MVCxScheduler)
			MyBase.New(control)
		End Sub

		Private privateNewDate As DateTime
		Public Shadows Property NewDate() As DateTime
			Get
				Return privateNewDate
			End Get
			Protected Set(ByVal value As DateTime)
				privateNewDate = value
			End Set
		End Property

		Protected Overrides Sub ParseParameters(ByVal parameters As String)
			Dim rawDate As String = HttpContext.Current.Request("NewDate")
			NewDate = DateTime.Parse(rawDate)
		End Sub
		Protected Overrides Sub ExecuteCore()
			Dim service As IDateTimeNavigationService = CType(Control.GetService(GetType(IDateTimeNavigationService)), IDateTimeNavigationService)
			If service IsNot Nothing Then
				service.GoToDate(NewDate)
			End If
		End Sub
	End Class
End Namespace