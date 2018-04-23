Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports Scheduler.CustomizationGotoDateForm.Models
Imports DevExpress.Web.Mvc

Namespace Scheduler.CustomizationGotoDateForm
	Public Class SchedulerController
		Inherits Controller
		Public Function Index() As ActionResult
			Return RedirectToAction("Scheduler")
		End Function
		Public Function Scheduler() As ActionResult
			Return View("Scheduler", SchedulerHelper.DataObject)
		End Function
		Public Function SchedulerPartial() As ActionResult
			Return PartialView("SchedulerPartial", SchedulerHelper.DataObject)
		End Function
		Public Function EditAppointmentAction() As ActionResult
			UpdateDataObject()
			Return PartialView("SchedulerPartial", SchedulerHelper.DataObject)
		End Function

		Private Sub UpdateDataObject()
			InsertAppointment()
			UpdateAppointments()
			RemoveAppoinments()
		End Sub
		Private Sub InsertAppointment()
			Dim schedule As Schedule = SchedulerExtension.GetAppointmentToInsert(Of Schedule)("scheduler", SchedulerHelper.DataObject.Appointments, SchedulerHelper.DataObject.Resources, SchedulerHelper.AppointmentStorage, SchedulerHelper.ResourceStorage)
			SchedulingDBDataProvider.InsertSchedule(schedule)
		End Sub
		Private Sub UpdateAppointments()
			Dim schedules() As Schedule = SchedulerExtension.GetAppointmentsToUpdate(Of Schedule)("scheduler", SchedulerHelper.DataObject.Appointments, SchedulerHelper.DataObject.Resources, SchedulerHelper.AppointmentStorage, SchedulerHelper.ResourceStorage)
			For Each schedule In schedules
				SchedulingDBDataProvider.UpdateSchedule(schedule)
			Next schedule
		End Sub
		Private Sub RemoveAppoinments()
			Dim schedules() As Schedule = SchedulerExtension.GetAppointmentsToRemove(Of Schedule)("scheduler", SchedulerHelper.DataObject.Appointments, SchedulerHelper.DataObject.Resources, SchedulerHelper.AppointmentStorage, SchedulerHelper.ResourceStorage)
			For Each schedule In schedules
				SchedulingDBDataProvider.DeleteCarScheduling(schedule)
			Next schedule
		End Sub
	End Class
End Namespace
