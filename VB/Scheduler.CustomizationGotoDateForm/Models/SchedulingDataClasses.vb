Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Collections
Imports DevExpress.Web.Mvc

Namespace Scheduler.CustomizationGotoDateForm.Models
	Public NotInheritable Class SchedulingDBDataProvider
		Private Const SchedulingDBDataContextKey As String = "DXNorthwindDataContext"

		Private Sub New()
		End Sub
		Public Shared ReadOnly Property DB() As SchedulingDataClassesDataContext
			Get
				If HttpContext.Current.Items(SchedulingDBDataContextKey) Is Nothing Then
					HttpContext.Current.Items(SchedulingDBDataContextKey) = New SchedulingDataClassesDataContext()
				End If
				Return CType(HttpContext.Current.Items(SchedulingDBDataContextKey), SchedulingDataClassesDataContext)
			End Get
		End Property
		Public Shared Function GetResources() As IEnumerable
			Return _
				From res In DB.DBResources _
				Where res.ResourceID < 6 _
				Select New With {Key .ID = res.ResourceID, Key .Model = res.ResourceName}
		End Function
		Public Shared Function GetAppointments() As IList(Of Schedule)
			Dim key As String = "Apts"
			Dim carScheduling As IList(Of Schedule) = CType(HttpContext.Current.Session(key), IList(Of Schedule))

			If carScheduling Is Nothing Then
				carScheduling = ( _
						From schedule In DB.DBAppointments _
						Select New Schedule() With {.ID = schedule.UniqueID, .Subject = schedule.Subject, .Description = schedule.Description, .StartTime = schedule.StartDate, .EndTime = schedule.EndDate, .EventType = schedule.Type, .Label = schedule.Label, .AllDay = CBool(schedule.AllDay), .Location = schedule.Location, .ResourceID = schedule.ResourceID.ToString(), .Status = schedule.Status, .RecurrenceInfo = schedule.RecurrenceInfo}).ToList()
				HttpContext.Current.Session(key) = carScheduling
			End If
			Return carScheduling
		End Function
		Public Shared Sub InsertSchedule(ByVal schedule As Schedule)
			If schedule Is Nothing Then
				Return
			End If

			Dim editableSchedule As New Schedule()
			editableSchedule.Assign(schedule)
			editableSchedule.ID = GenerateScheduleID()
			GetAppointments().Add(editableSchedule)
		End Sub
		Public Shared Sub UpdateSchedule(ByVal schedule As Schedule)
			If schedule Is Nothing Then
				Return
			End If

			Dim editableSchedule As Schedule = GetScheduleByID(schedule.ID)
			editableSchedule.Assign(schedule)
		End Sub
		Public Shared Sub DeleteCarScheduling(ByVal schedule As Schedule)
			If schedule Is Nothing Then
				Return
			End If

			Dim editableSchedule As Schedule = GetScheduleByID(schedule.ID)
			If editableSchedule IsNot Nothing Then
				GetAppointments().Remove(editableSchedule)
			End If
		End Sub
		Public Shared Function GetScheduleByID(ByVal id As Integer) As Schedule
			Return ( _
					From schedule In GetAppointments() _
					Where schedule.ID = id _
					Select schedule).FirstOrDefault()
		End Function
		Private Shared Function GenerateScheduleID() As Integer
			Dim carScheduling As IList(Of Schedule) = GetAppointments()
			Return If((carScheduling.Count() > 0), carScheduling.Last().ID + 1, 0)
		End Function
	End Class

	Public Class Schedule
		Private privateID As Integer
		Public Property ID() As Integer
			Get
				Return privateID
			End Get
			Set(ByVal value As Integer)
				privateID = value
			End Set
		End Property
		Private privateSubject As String
		Public Property Subject() As String
			Get
				Return privateSubject
			End Get
			Set(ByVal value As String)
				privateSubject = value
			End Set
		End Property
		Private privateDescription As String
		Public Property Description() As String
			Get
				Return privateDescription
			End Get
			Set(ByVal value As String)
				privateDescription = value
			End Set
		End Property
		Private privateStartTime? As DateTime
		Public Property StartTime() As DateTime?
			Get
				Return privateStartTime
			End Get
			Set(ByVal value? As DateTime)
				privateStartTime = value
			End Set
		End Property
		Private privateEndTime? As DateTime
		Public Property EndTime() As DateTime?
			Get
				Return privateEndTime
			End Get
			Set(ByVal value? As DateTime)
				privateEndTime = value
			End Set
		End Property
		Private privateEventType? As Integer
		Public Property EventType() As Integer?
			Get
				Return privateEventType
			End Get
			Set(ByVal value? As Integer)
				privateEventType = value
			End Set
		End Property
		Private privateLabel? As Integer
		Public Property Label() As Integer?
			Get
				Return privateLabel
			End Get
			Set(ByVal value? As Integer)
				privateLabel = value
			End Set
		End Property
		Private privateAllDay As Boolean
		Public Property AllDay() As Boolean
			Get
				Return privateAllDay
			End Get
			Set(ByVal value As Boolean)
				privateAllDay = value
			End Set
		End Property
		Private privateLocation As String
		Public Property Location() As String
			Get
				Return privateLocation
			End Get
			Set(ByVal value As String)
				privateLocation = value
			End Set
		End Property
		Private privateResourceID As String
		Public Property ResourceID() As String
			Get
				Return privateResourceID
			End Get
			Set(ByVal value As String)
				privateResourceID = value
			End Set
		End Property
		Private privateStatus? As Integer
		Public Property Status() As Integer?
			Get
				Return privateStatus
			End Get
			Set(ByVal value? As Integer)
				privateStatus = value
			End Set
		End Property
		Private privateRecurrenceInfo As String
		Public Property RecurrenceInfo() As String
			Get
				Return privateRecurrenceInfo
			End Get
			Set(ByVal value As String)
				privateRecurrenceInfo = value
			End Set
		End Property

		Public Overridable Sub Assign(ByVal schedule As Schedule)
			If schedule Is Nothing Then
				Return
			End If

			ID = schedule.ID
			Subject = schedule.Subject
			Description = schedule.Description
			StartTime = schedule.StartTime
			EndTime = schedule.EndTime
			EventType = schedule.EventType
			Label = schedule.Label
			AllDay = schedule.AllDay
			Location = schedule.Location
			ResourceID = schedule.ResourceID
			Status = schedule.Status
			RecurrenceInfo = schedule.RecurrenceInfo
		End Sub
	End Class

	Public Class SchedulerDataObject
		Private privateAppointments As IEnumerable
		Public Property Appointments() As IEnumerable
			Get
				Return privateAppointments
			End Get
			Set(ByVal value As IEnumerable)
				privateAppointments = value
			End Set
		End Property
		Private privateResources As IEnumerable
		Public Property Resources() As IEnumerable
			Get
				Return privateResources
			End Get
			Set(ByVal value As IEnumerable)
				privateResources = value
			End Set
		End Property
	End Class

	Public Class SchedulerHelper
		Private Shared appointmentStorage_Renamed As MVCxAppointmentStorage
		Public Shared ReadOnly Property AppointmentStorage() As MVCxAppointmentStorage
			Get
				If appointmentStorage_Renamed Is Nothing Then
					appointmentStorage_Renamed = CreateAppointmentStorage()
				End If
				Return appointmentStorage_Renamed
			End Get
		End Property

		Private Shared Function CreateAppointmentStorage() As MVCxAppointmentStorage
			Dim appointmentStorage As New MVCxAppointmentStorage()
			appointmentStorage.Mappings.AppointmentId = "ID"
			appointmentStorage.Mappings.Start = "StartTime"
			appointmentStorage.Mappings.End = "EndTime"
			appointmentStorage.Mappings.Subject = "Subject"
			appointmentStorage.Mappings.Description = "Description"
			appointmentStorage.Mappings.Location = "Location"
			appointmentStorage.Mappings.AllDay = "AllDay"
			appointmentStorage.Mappings.Type = "EventType"
			appointmentStorage.Mappings.RecurrenceInfo = "RecurrenceInfo"
			appointmentStorage.Mappings.ReminderInfo = "ReminderInfo"
			appointmentStorage.Mappings.Label = "Label"
			appointmentStorage.Mappings.Status = "Status"
			appointmentStorage.Mappings.ResourceId = "CarId"
			Return appointmentStorage
		End Function

		Private Shared resourceStorage_Renamed As MVCxResourceStorage
		Public Shared ReadOnly Property ResourceStorage() As MVCxResourceStorage
			Get
				If resourceStorage_Renamed Is Nothing Then
					resourceStorage_Renamed = CreateResourceStorage()
				End If
				Return resourceStorage_Renamed
			End Get
		End Property
		Private Shared Function CreateResourceStorage() As MVCxResourceStorage
			Dim resourceStorage As New MVCxResourceStorage()
			resourceStorage.Mappings.ResourceId = "ID"
			resourceStorage.Mappings.Caption = "Model"
			Return resourceStorage
		End Function

	Public Shared ReadOnly Property DataObject() As SchedulerDataObject
		Get
            Dim sdo As New SchedulerDataObject()
            sdo.Appointments = SchedulingDBDataProvider.GetAppointments()
            sdo.Resources = SchedulingDBDataProvider.GetResources()
            Return sdo
		End Get
	End Property
	End Class

End Namespace