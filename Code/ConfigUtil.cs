using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace UmbracoShipTac.Code
{
    public static class ConfigUtil
    {

        //One place to refer to all Web.config keys used by the class.
        private enum ConfigUtilKeys
        {
            [Description("HostingPlace")]
            HostingPlace,
                        [Description("ResetPasswordTimeOutInHours")]
                ResetPasswordTimeOutInHours,
            [Description("GoogleRecaptchaSecret")]
            GoogleRecaptchaSecret,
            [Description("OverrideEmail")]
            OverrideEmail,
            [Description("Email_Server")]
            EmailServer,
            [Description("Email_TechSupport")]
            EmailOfTechSupport,
            [Description("CriticalErrorEmail")]
            CriticalErrorEmail,
            [Description("CriticalErrorEmailCC")]
            CriticalErrorEmailCC,
            [Description("Email_ResourceCenter")]
            EmailOfResourceCenter,
            [Description("WebEnvironment")]
            WebEnvironment,
            [Description("ShiptalkUrl")]
            ShiptalkUrl,
            [Description("EmailServerRequiresAuthentication")]
            EmailServerRequiresAuthentication,
            [Description("EmailServerUserName")]
            EmailServerUserName,
            [Description("EmailServerPassword")]
            EmailServerPassword,
            [Description("support_phone")]
            SHIPtalkSupportPhone,
            [Description("Session_TimeOut")]
            SessionTimeOut,
            [Description("PendingUserRegistrationDays")]
            PendingUserRegistrationDays,
            [Description("SecureAllPages")]
            SecureAllPages,
            [Description("EmailValidationRegEx")]
            EmailValidationRegEx,
            [Description("AllStateRootId")]
            AllStateRootId,
            [Description("SmtpHost")]
            SmtpHost,
            [Description("SmtpPort")]
            SmtpPort,
            [Description("SmtpCredentialsUid")]
            SmtpCredentialsUid,
            [Description("SmtpCredentialsPwd")]
            SmtpCredentialsPwd,
            [Description("SmtpConnectionTimeout")]
            SmtpConnectionTimeout,
                        [Description("SmtpSendUsing")]
            SmtpSendUsing,
                        [Description("SmtpAuthenticate")]
                        SmtpAuthenticate,
            [Description("ResourceUnpublishedId")]
            ResourceUnpublishedId,
                        [Description("ResourceRootId")]
            ResourceRootId,
            [Description("ResourceR1Id")]
            ResourceR1Id,
            [Description("ResourceR2Id")]
            ResourceR2Id,
            [Description("ResourceR3Id")]
            ResourceR3Id,
            [Description("ResourceR4Id")]
            ResourceR4Id,
            [Description("ResourceFeatureId")]
            ResourceFeatureId,
                                    [Description("EventRootId")]
            EventRootId,
            [Description("EventE1Id")]
            EventE1Id,
            [Description("EventE2Id")]
            EventE2Id,
            [Description("EventE3Id")]
            EventE3Id,
            [Description("EventE4Id")]
            EventE4Id,


            [Description("MediaRootId")]
            MediaRootId,

             [Description("FeatureRootId")]
            FeatureRootId
            
        }

        private static readonly int SESSION_TIMEOUT_DEFAULT_IN_MINS = 40;

        static ConfigUtil()
        {
            ////All Non-Nullable values, meaning key/value pair that MUST exist in config file are loaded here.
            GoogleRecaptchaSecret = GetNonNullableParamValue<string>(ConfigUtilKeys.GoogleRecaptchaSecret);

            ////User Registration; Login related
            //PasswordMinLength = GetNonNullableParamValue<int>(ConfigUtilKeys.PasswordMinLength);
            //PasswordMaxLength = GetNonNullableParamValue<int>(ConfigUtilKeys.PasswordMaxLength);
            //PasswordWarningAfterHowManyDays = GetNonNullableParamValue<int>(ConfigUtilKeys.PasswordWarningAfterHowManyDays);
            //PendingUserRegistrationDays = GetNonNullableParamValue<int>(ConfigUtilKeys.PendingUserRegistrationDays);

            ////Emai related
            //HostingPlace = GetNonNullableParamValue<string>(ConfigUtilKeys.HostingPlace);
            //OverrideEmailAddress = GetNullableReferenceTypeValue<string>(ConfigUtilKeys.OverrideEmail) + string.Empty;
            //MustOverrideEmail = !(OverrideEmailAddress == string.Empty);

            //smtp Emai related
            SmtpHost = GetNonNullableParamValue<string>(ConfigUtilKeys.SmtpHost);
            SmtpPort = GetNonNullableParamValue<int>(ConfigUtilKeys.SmtpPort);
            SmtpCredentialsUid = GetNonNullableParamValue<string>(ConfigUtilKeys.SmtpCredentialsUid);
            SmtpCredentialsPwd = GetNonNullableParamValue<string>(ConfigUtilKeys.SmtpCredentialsPwd);
            SmtpConnectionTimeout = GetNonNullableParamValue<string>(ConfigUtilKeys.SmtpConnectionTimeout);
            SmtpSendUsing = GetNonNullableParamValue<int>(ConfigUtilKeys.SmtpSendUsing);
            SmtpAuthenticate = GetNonNullableParamValue<int>(ConfigUtilKeys.SmtpAuthenticate);

            CriticalErrorEmail = GetNonNullableParamValue<string>(ConfigUtilKeys.CriticalErrorEmail);
            CriticalErrorEmailCC = GetNonNullableParamValue<string>(ConfigUtilKeys.CriticalErrorEmailCC);

            EmailOfResourceCenter = GetNonNullableParamValue<string>(ConfigUtilKeys.EmailOfResourceCenter);

            ////Regular expression for validating email addresses
            //EmailValidationRegex = GetNonNullableParamValue<string>(ConfigUtilKeys.EmailValidationRegEx);

            ////Web site environment related
            WebEnvironment = GetNonNullableParamValue<string>(ConfigUtilKeys.WebEnvironment);


            ShiptalkUrl = GetNonNullableParamValue<string>(ConfigUtilKeys.ShiptalkUrl);
            //EmailServerRequiresAuthentication = GetNonNullableParamValue<bool>(ConfigUtilKeys.EmailServerRequiresAuthentication);
            //SecureAllPages = GetNonNullableParamValue<bool>(ConfigUtilKeys.SecureAllPages);

            ////Registration / Email Confirmation / Password Reset etc.,
            PasswordResetUrl = ShiptalkUrl + (ShiptalkUrl.EndsWith("/") ? "" : "/") + "resetpassword";
            EmailConfirmationUrl = ShiptalkUrl + (ShiptalkUrl.EndsWith("/") ? "" : "/") + "umbraco/surface/authsurface/RenderVerifyEmail";

            //Contact Shiptalk, Support
            ShiptalkSupportPhone = GetNonNullableParamValue<string>(ConfigUtilKeys.SHIPtalkSupportPhone);

            //Session stuff
            SessionTimeOutInMinutes = GetNullableValueTypeValue<int>(ConfigUtilKeys.SessionTimeOut) ?? SESSION_TIMEOUT_DEFAULT_IN_MINS;


            //umbracoID
            AllStateRootId = GetNonNullableParamValue<int>(ConfigUtilKeys.AllStateRootId);

            MediaRootId = GetNonNullableParamValue<int>(ConfigUtilKeys.MediaRootId);

            ResourceUnpublishedId = GetNonNullableParamValue<int>(ConfigUtilKeys.ResourceUnpublishedId);

            ResourceRootId = GetNonNullableParamValue<int>(ConfigUtilKeys.ResourceRootId);
            ResourceR1Id = GetNonNullableParamValue<int>(ConfigUtilKeys.ResourceR1Id);
            ResourceR2Id = GetNonNullableParamValue<int>(ConfigUtilKeys.ResourceR2Id);
            ResourceR3Id = GetNonNullableParamValue<int>(ConfigUtilKeys.ResourceR3Id);
            ResourceR4Id = GetNonNullableParamValue<int>(ConfigUtilKeys.ResourceR4Id);
            ResourceFeatureId = GetNonNullableParamValue<int>(ConfigUtilKeys.ResourceFeatureId);

            FeatureRootId = GetNonNullableParamValue<int>(ConfigUtilKeys.FeatureRootId);

            EventRootId = GetNonNullableParamValue<int>(ConfigUtilKeys.EventRootId);
            EventE1Id = GetNonNullableParamValue<int>(ConfigUtilKeys.EventE1Id);
            EventE2Id = GetNonNullableParamValue<int>(ConfigUtilKeys.EventE2Id);
            EventE3Id = GetNonNullableParamValue<int>(ConfigUtilKeys.EventE3Id);
            EventE4Id = GetNonNullableParamValue<int>(ConfigUtilKeys.EventE4Id);

            


            ResetPasswordTimeOutInHours = GetNonNullableParamValue<int>(ConfigUtilKeys.ResetPasswordTimeOutInHours);





        }



        #region Public Gets/Private sets


        public static string GoogleRecaptchaSecret { get; private set; }

        public static string OverrideEmailAddress { get; private set; }

        public static string HostingPlace { get; private set; }
        public static string EmailServer { get; private set; }
        public static string EmailServerUserName { get; private set; }
        public static string EmailServerPassword { get; private set; }

        public static string EmailOfTechSupport { get; private set; }

        public static string CriticalErrorEmail { get; private set; }
        public static string CriticalErrorEmailCC { get; private set; }

        public static string EmailOfResourceCenter { get; private set; }
        public static string ShiptalkSupportPhone { get; private set; }
        public static string ShiptalkUrl { get; private set; }
        public static string WebEnvironment { get; private set; }

        public static string EmailConfirmationUrl { get; private set; }
        public static string PasswordResetUrl { get; private set; }

        public static string EmailValidationRegex { get; private set; }

        public static bool MustOverrideEmail { get; private set; }
        public static bool EmailServerRequiresAuthentication { get; private set; }
        public static bool SecureAllPages { get; private set; }
        public static int PendingUserRegistrationDays { get; private set; }

        //smtp related
        public static int SmtpSendUsing { get; private set; }
        public static int SmtpAuthenticate { get; private set; }
        
        public static int SmtpPort { get; private set; }
        public static string SmtpCredentialsUid { get; private set; }
        public static string SmtpCredentialsPwd { get; private set; }
        public static string SmtpHost { get; private set; }
        public static string SmtpConnectionTimeout { get; private set; }

        public static int AllStateRootId { get; private set; }
        public static int MediaRootId { get; private set; }


        public static int ResourceUnpublishedId { get; private set; }

        public static int ResourceRootId { get; private set; }
        public static int ResourceR1Id { get; private set; }
        public static int ResourceR2Id { get; private set; }
        public static int ResourceR3Id { get; private set; }
        public static int ResourceR4Id { get; private set; }

        public static int ResourceFeatureId { get; private set; }

        public static int FeatureRootId { get; private set; }



        public static int EventRootId { get; private set; }
        public static int EventE1Id { get; private set; }
        public static int EventE2Id { get; private set; }
        public static int EventE3Id { get; private set; }
        public static int EventE4Id { get; private set; }

        //Session stuff
        public static int SessionTimeOutInMinutes { get; private set; }

        public static int ResetPasswordTimeOutInHours { get; private set; }


        #endregion



        #region Private Methods used to retrieve from config file. All public properties demand nullable or non-nullable values.
        /// <summary>
        /// This method returns null if value not found. Does not throw an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private static Nullable<T> GetNullableValueTypeValue<T>(ConfigUtilKeys paramName) where T : struct, IConvertible
        {
            string val = GetVal(paramName.Description()) + string.Empty;
            if (val != string.Empty)
                return (T)Convert.ChangeType(val, typeof(T));
            else
                return null;

        }

        /// <summary>
        /// This method will return null if value is not found. Does not throw exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private static T GetNullableReferenceTypeValue<T>(ConfigUtilKeys paramName) where T : class, IConvertible
        {
            string val = GetVal(paramName.Description()) + string.Empty;
            if (val != string.Empty)
                return (T)Convert.ChangeType(val, typeof(T));
            else
                return null;

        }

        /// <summary>
        /// Use for non-nullable values. If the value is null, exception is thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private static T GetNonNullableParamValue<T>(ConfigUtilKeys paramName)
        {
            string val = GetVal(paramName.Description()) + string.Empty;
            if (val != string.Empty)
                return (T)Convert.ChangeType(val, typeof(T));
            else
                throw new ConfigurationErrorsException(
                    string.Format("Application Setting \"{0}\" is required for the application to function correctly.", paramName.Description()));
        }

        /// <summary>
        /// The one and only method or access point for the actual Config value.
        /// Any API change in .Net framework to access AppSettinsg will affect this method alone.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private static string GetVal(string Key)
        {
            return ConfigurationManager.AppSettings[Key] + string.Empty;
        }

        #endregion
    }
}