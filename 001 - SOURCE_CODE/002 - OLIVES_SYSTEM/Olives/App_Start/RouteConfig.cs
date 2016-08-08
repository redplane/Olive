using System.Web.Mvc;
using System.Web.Routing;

namespace Olives
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // website route
            routes.MapRoute("IntroPatient", "intro/patient", new {controller = "Home", action = "Index"});
            routes.MapRoute("IntroDoctor", "intro/doctor", new {controller = "Home", action = "Index"});
            routes.MapRoute("Signin", "signin", new {controller = "Home", action = "Index"});
            routes.MapRoute("Signup", "signup", new {controller = "Home", action = "Index"});
            routes.MapRoute("SigninPatient", "signup/patient", new {controller = "Home", action = "Index"});
            routes.MapRoute("SigninDoctor", "signin/doctor", new {controller = "Home", action = "Index"});
            routes.MapRoute("ForgotPwd", "forgotpwd", new {controller = "Home", action = "Index"});
            routes.MapRoute("Contact", "contact", new {controller = "Home", action = "Index"});
            routes.MapRoute("PrivacyPolicy", "privacypolicy", new {controller = "Home", action = "Index"});
            routes.MapRoute("TermOfService", "termofservice", new {controller = "Home", action = "Index"});
            routes.MapRoute("SettingPassword", "settings/password", new {controller = "Home", action = "Index"});
            routes.MapRoute("SettingNotification", "settings/notification", new {controller = "Home", action = "Index"});
            routes.MapRoute("SettingBilling", "settings/billing", new {controller = "Home", action = "Index"});
            routes.MapRoute("ProfileSearchDoctors", "profiles/search/doctors",
                new {controller = "Home", action = "Index"});
            routes.MapRoute("PatientProfile", "patients/{acid}",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientAppointment", "patients/{acid}/appointments",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientAppointmentDetail", "patients/{acid}/appointments/{apid}",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional, apid = UrlParameter.Optional});
            routes.MapRoute("PatientAppointmentPending", "patients/{acid}/appointments/pending",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientStatsBP", "patients/{acid}/stats/bloodpressure",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientStatsBS", "patients/{acid}/stats/bloodsugar",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientStatsHB", "patients/{acid}/stats/heartbeat",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientMedIndex", "patients/{acid}/medindex",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientDiary", "patients/{acid}/diary",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientMedRecords", "patients/{acid}/medrecords",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientPrescriptions", "patients/{acid}/prescriptions",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("PatientMedRecordDetail", "patients/{acid}/medrecords/{mrid}",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional, mrid = UrlParameter.Optional});
            routes.MapRoute("PatientExpNotes", "patients/{acid}/medrecords/{mrid}/expnotes/add",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional, mrid = UrlParameter.Optional});
            routes.MapRoute("PatientExpNoteDetail", "patients/{acid}/medrecords/{mrid}/expnotes/{enid}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    acid = UrlParameter.Optional,
                    mrid = UrlParameter.Optional,
                    enid = UrlParameter.Optional
                });
            routes.MapRoute("PatientPrescriptionAdd", "patients/{acid}/medrecords/{mrid}/prescriptions/add",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional, mrid = UrlParameter.Optional});
            routes.MapRoute("PatientPrescriptionDetail", "patients/{acid}/medrecords/{mrid}/prescriptions/{pid}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    acid = UrlParameter.Optional,
                    mrid = UrlParameter.Optional,
                    pid = UrlParameter.Optional
                });
            routes.MapRoute("PatientPrescriptionEdit", "patients/{acid}/medrecords/{mrid}/prescriptions/{pid}/edit",
                new
                {
                    controller = "Home",
                    action = "Index",
                    acid = UrlParameter.Optional,
                    mrid = UrlParameter.Optional,
                    pid = UrlParameter.Optional
                });
            routes.MapRoute("DoctorProfile", "doctors/{acid}",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("DoctorAppointments", "doctors/{acid}/appointments",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("DoctorAppointmentsPending", "doctors/{acid}/appointments/pending",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            routes.MapRoute("DoctorAppointmentDetail", "doctors/{acid}/appointments/{apid}",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional, apid = UrlParameter.Optional});
            routes.MapRoute("DoctorPatients", "doctors/{acid}/patients",
                new {controller = "Home", action = "Index", acid = UrlParameter.Optional});


            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}