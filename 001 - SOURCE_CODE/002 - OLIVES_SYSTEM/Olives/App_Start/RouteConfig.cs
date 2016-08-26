using System.Web.Mvc;
using System.Web.Routing;

namespace Olives
{
    public class RouteConfig
    {
        
        public static void RegisterRoutes(RouteCollection routes) { 
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // website route
            //routes.MapRoute("IntroPatient", "intro/patient", new {controller = "Home", action = "Index"});
            //routes.MapRoute("Messages", "messages", new { controller = "Home", action = "Index" });
            //routes.MapRoute("IntroDoctor", "intro/doctor", new {controller = "Home", action = "Index"});
            //routes.MapRoute("Signin", "signin", new {controller = "Home", action = "Index"});
            //routes.MapRoute("Signup", "signup", new {controller = "Home", action = "Index"});
            //routes.MapRoute("SigninPatient", "signup/patient", new {controller = "Home", action = "Index"});
            //routes.MapRoute("SigninDoctor", "signin/doctor", new {controller = "Home", action = "Index"});
            //routes.MapRoute("ForgotPwd", "forgotpwd", new {controller = "Home", action = "Index"});
            //routes.MapRoute("Contact", "contact", new {controller = "Home", action = "Index"});
            //routes.MapRoute("PrivacyPolicy", "privacypolicy", new {controller = "Home", action = "Index"});
            //routes.MapRoute("TermOfService", "termofservice", new {controller = "Home", action = "Index"});
            //routes.MapRoute("SettingPassword", "settings/password", new {controller = "Home", action = "Index"});
            //routes.MapRoute("SettingNotification", "settings/notification", new {controller = "Home", action = "Index"});
            //routes.MapRoute("SettingBilling", "settings/billing", new {controller = "Home", action = "Index"});
            //routes.MapRoute("ProfileSearchDoctors", "profiles/search/doctors",
            //    new {controller = "Home", action = "Index"});
            //routes.MapRoute("PatientProfile", "patients/{acid}",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("PatientAppointment", "patients/{acid}/appointments",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("PatientAppointmentDetail", "patients/{acid}/appointments/{apid}",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional, apid = UrlParameter.Optional});
            //routes.MapRoute("PatientAppointmentPending", "patients/{acid}/appointments/pending",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("PatientStatsBP", "patients/{acid}/stats/bloodpressure",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("PatientStatsBS", "patients/{acid}/stats/bloodsugar",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("PatientStatsHB", "patients/{acid}/stats/heartbeat",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("PatientMedIndex", "patients/{acid}/medindex",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("PatientDiary", "doctors/{acid}/diary",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});


            //routes.MapRoute("MedRecords", "medicalrecords",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("MedRecordsDetails", "medicalrecords/{mrid}",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });
            //routes.MapRoute("MedRecordsIndex", "medicalrecords/{mrid}/index",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });
            //routes.MapRoute("MedRecordsDetailsImage", "medicalrecords/{mrid}/images",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });
            //routes.MapRoute("MedRecordsDetailsImageUpload", "medicalrecords/{mrid}/images/upload",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });

            //routes.MapRoute("MedRecordsDetailsNotes", "medicalrecords/{mrid}/notes",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });
            //routes.MapRoute("MedRecordsDetailsNotesAdd", "medicalrecords/{mrid}/notes/add",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });
            //routes.MapRoute("MedRecordsDetailsNoteEdit", "medicalrecords/{mrid}/notes/edit",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });

            //routes.MapRoute("MedRecordsDetailsExp", "medicalrecords/{mrid}/experiments",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });
            //routes.MapRoute("MedRecordsDetailsExpAdd", "medicalrecords/{mrid}/experiments/add",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });
            //routes.MapRoute("MedRecordsDetailsExpEdit", "medicalrecords/{mrid}/experiments/edit",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });

            //routes.MapRoute("MedRecordsDetaisPres", "medicalrecords/{mrid}/prescriptions",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });
            //routes.MapRoute("MedRecordsDetailsPresAdd", "medicalrecords/{mrid}/prescriptions/add",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });
            //routes.MapRoute("MedRecordsDetailsPresEdit", "medicalrecords/{mrid}/prescriptions/edit",
            //    new { controller = "Home", action = "Index", mrid = UrlParameter.Optional });




            //routes.MapRoute("DoctorProfile", "doctors/{acid}",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("DoctorAppointments", "doctors/{acid}/appointments",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("DoctorAppointmentsPending", "doctors/{acid}/appointments/pending",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});
            //routes.MapRoute("DoctorAppointmentDetail", "doctors/{acid}/appointments/{apid}",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional, apid = UrlParameter.Optional});
            //routes.MapRoute("DoctorPatients", "doctors/{acid}/patients",
            //    new {controller = "Home", action = "Index", acid = UrlParameter.Optional});


            //routes.MapRoute(
            //    "Default",
            //    "{controller}/{action}/{id}",
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //    );
            
            routes.MapRoute(
                "Default",
                "{*url}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
              );

            routes.MapRoute(
                "Olives",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );

        }
    }
}