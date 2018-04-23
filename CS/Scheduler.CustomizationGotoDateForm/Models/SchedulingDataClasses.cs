using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using DevExpress.Web.Mvc;
using System.Configuration;

namespace Scheduler.CustomizationGotoDateForm.Models
{
    public static class SchedulingDBDataProvider
    {
        const string SchedulingDBDataContextKey = "DXNorthwindDataContext";

        public static SchedulingDataClassesDataContext DB
        {
            get
            {
                if (HttpContext.Current.Items[SchedulingDBDataContextKey] == null)
                {
                    SchedulingDataClassesDataContext context = new SchedulingDataClassesDataContext();
                    HttpContext.Current.Items[SchedulingDBDataContextKey] = new SchedulingDataClassesDataContext();
                }
                return (SchedulingDataClassesDataContext)HttpContext.Current.Items[SchedulingDBDataContextKey];
            }
        }
        public static IEnumerable GetResources()
        {
            return from res in DB.DBResources where res.ResourceID < 6 select new { ID = res.ResourceID, Model = res.ResourceName };
        }
        public static IList<Schedule> GetAppointments()
        {
            string key = "Apts";
            IList<Schedule> carScheduling = (IList<Schedule>)HttpContext.Current.Session[key];

            if (carScheduling == null)
            {
                carScheduling = (from schedule in DB.DBAppointments
                                 select new Schedule()
                                 {
                                     ID = schedule.UniqueID,
                                     Subject = schedule.Subject,
                                     Description = schedule.Description,
                                     StartTime = schedule.StartDate,
                                     EndTime = schedule.EndDate,
                                     EventType = schedule.Type,
                                     Label = schedule.Label,
                                     AllDay = (bool)schedule.AllDay,
                                     Location = schedule.Location,
                                     ResourceID = schedule.ResourceID.ToString(),
                                     Status = schedule.Status,
                                     RecurrenceInfo = schedule.RecurrenceInfo
                                 }).ToList();
                HttpContext.Current.Session[key] = carScheduling;
            }
            return carScheduling;
        }
        public static void InsertSchedule(Schedule schedule)
        {
            if (schedule == null)
                return;

            Schedule editableSchedule = new Schedule();
            editableSchedule.Assign(schedule);
            editableSchedule.ID = GenerateScheduleID();
            GetAppointments().Add(editableSchedule);
        }
        public static void UpdateSchedule(Schedule schedule)
        {
            if (schedule == null)
                return;

            Schedule editableSchedule = GetScheduleByID(schedule.ID);
            editableSchedule.Assign(schedule);
        }
        public static void DeleteCarScheduling(Schedule schedule)
        {
            if (schedule == null)
                return;

            Schedule editableSchedule = GetScheduleByID(schedule.ID);
            if (editableSchedule != null)
                GetAppointments().Remove(editableSchedule);
        }
        public static Schedule GetScheduleByID(int id)
        {
            return (from schedule in GetAppointments() where schedule.ID == id select schedule).FirstOrDefault();
        }
        static int GenerateScheduleID()
        {
            IList<Schedule> carScheduling = GetAppointments();
            return (carScheduling.Count() > 0) ? carScheduling.Last().ID + 1 : 0;
        }
    }

    public class Schedule
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? EventType { get; set; }
        public int? Label { get; set; }
        public bool AllDay { get; set; }
        public string Location { get; set; }
        public string ResourceID { get; set; }
        public int? Status { get; set; }
        public string RecurrenceInfo { get; set; }

        public virtual void Assign(Schedule schedule)
        {
            if (schedule == null)
                return;

            ID = schedule.ID;
            Subject = schedule.Subject;
            Description = schedule.Description;
            StartTime = schedule.StartTime;
            EndTime = schedule.EndTime;
            EventType = schedule.EventType;
            Label = schedule.Label;
            AllDay = schedule.AllDay;
            Location = schedule.Location;
            ResourceID = schedule.ResourceID;
            Status = schedule.Status;
            RecurrenceInfo = schedule.RecurrenceInfo;
        }
    }

    public class SchedulerDataObject
    {
        public IEnumerable Appointments { get; set; }
        public IEnumerable Resources { get; set; }
    }

    public class SchedulerHelper
    {
        static MVCxAppointmentStorage appointmentStorage;
        public static MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (appointmentStorage == null)
                    appointmentStorage = CreateAppointmentStorage();
                return appointmentStorage;
            }
        }

        static MVCxAppointmentStorage CreateAppointmentStorage()
        {
            MVCxAppointmentStorage appointmentStorage = new MVCxAppointmentStorage();
            appointmentStorage.Mappings.AppointmentId = "ID";
            appointmentStorage.Mappings.Start = "StartTime";
            appointmentStorage.Mappings.End = "EndTime";
            appointmentStorage.Mappings.Subject = "Subject";
            appointmentStorage.Mappings.Description = "Description";
            appointmentStorage.Mappings.Location = "Location";
            appointmentStorage.Mappings.AllDay = "AllDay";
            appointmentStorage.Mappings.Type = "EventType";
            appointmentStorage.Mappings.RecurrenceInfo = "RecurrenceInfo";
            appointmentStorage.Mappings.ReminderInfo = "ReminderInfo";
            appointmentStorage.Mappings.Label = "Label";
            appointmentStorage.Mappings.Status = "Status";
            appointmentStorage.Mappings.ResourceId = "CarId";
            return appointmentStorage;
        }

        static MVCxResourceStorage resourceStorage;
        public static MVCxResourceStorage ResourceStorage
        {
            get
            {
                if (resourceStorage == null)
                    resourceStorage = CreateResourceStorage();
                return resourceStorage;
            }
        }
        static MVCxResourceStorage CreateResourceStorage()
        {
            MVCxResourceStorage resourceStorage = new MVCxResourceStorage();
            resourceStorage.Mappings.ResourceId = "ID";
            resourceStorage.Mappings.Caption = "Model";
            return resourceStorage;
        }

        public static SchedulerDataObject DataObject
        {
            get
            {
                return new SchedulerDataObject()
                {
                    Appointments = SchedulingDBDataProvider.GetAppointments(),
                    Resources = SchedulingDBDataProvider.GetResources()
                };
            }
        }
    }
}