using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.Mvc;
using DevExpress.XtraScheduler.Services;

namespace Scheduler.CustomizationGotoDateForm.ExtensionHelpers {
    public class MVCxSchedulerGotoDateFormCallbackCommand: GotoDateFormCallbackCommand {
        public MVCxSchedulerGotoDateFormCallbackCommand(MVCxScheduler control)
            : base(control) {
        }

        public new DateTime NewDate { get; protected set; }

        protected override void ParseParameters(string parameters) {
            string rawDate = HttpContext.Current.Request["NewDate"];
            NewDate = DateTime.Parse(rawDate);
        }
        protected override void ExecuteCore() {
            IDateTimeNavigationService service = (IDateTimeNavigationService)Control.GetService(typeof(IDateTimeNavigationService));
            if (service != null)
                service.GoToDate(NewDate);
        }
    }
}