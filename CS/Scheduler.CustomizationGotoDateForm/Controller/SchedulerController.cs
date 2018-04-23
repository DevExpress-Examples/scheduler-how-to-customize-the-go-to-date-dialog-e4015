using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scheduler.CustomizationGotoDateForm.Models;
using DevExpress.Web.Mvc;

namespace Scheduler.CustomizationGotoDateForm {
    public class SchedulerController: Controller {
        public ActionResult Index() {
            return RedirectToAction("Scheduler");
        }
        public ActionResult Scheduler() {
            return View("Scheduler", SchedulerHelper.DataObject);
        }
        public ActionResult SchedulerPartial() {
            return PartialView("SchedulerPartial", SchedulerHelper.DataObject);
        }
        public ActionResult EditAppointmentAction() {
            UpdateDataObject();
            return PartialView("SchedulerPartial", SchedulerHelper.DataObject);
        }

        void UpdateDataObject() {
            InsertAppointment();
            UpdateAppointments();
            RemoveAppoinments();
        }
        void InsertAppointment() {
            Schedule schedule = SchedulerExtension.GetAppointmentToInsert<Schedule>("scheduler", SchedulerHelper.DataObject.Appointments,
                SchedulerHelper.DataObject.Resources, SchedulerHelper.AppointmentStorage, SchedulerHelper.ResourceStorage);
            SchedulingDBDataProvider.InsertSchedule(schedule);
        }
        void UpdateAppointments() {
            Schedule[] schedules = SchedulerExtension.GetAppointmentsToUpdate<Schedule>("scheduler", SchedulerHelper.DataObject.Appointments,
                SchedulerHelper.DataObject.Resources, SchedulerHelper.AppointmentStorage, SchedulerHelper.ResourceStorage);
            foreach (var schedule in schedules) {
                SchedulingDBDataProvider.UpdateSchedule(schedule);
            }
        }
        void RemoveAppoinments() {
            Schedule[] schedules = SchedulerExtension.GetAppointmentsToRemove<Schedule>("scheduler", SchedulerHelper.DataObject.Appointments,
                SchedulerHelper.DataObject.Resources, SchedulerHelper.AppointmentStorage, SchedulerHelper.ResourceStorage);
            foreach (var schedule in schedules) {
                SchedulingDBDataProvider.DeleteCarScheduling(schedule);
            }
        }
    }
}
