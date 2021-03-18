//------------------------------------------------------------------------------
// Copyright © 2014 Schell Games, LLC. All Rights Reserved.
//
// Contact: Tim Sweeney
//
// Created: Aug 2014
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SG.Core;

namespace SG.Vignettitor.Graph
{
    /// <summary>
    /// Collection of log messages of various severities about the validation
    /// of certain content.
    /// </summary>
    public class ContentValidation
    {
        public class ContentValidationItem
        {
            public NotifySeverity severity;
            public string message;
            public object context;
        }

        public NotifySeverity maxSeverity;
        public List<ContentValidationItem> validations = new List<ContentValidationItem>();

        public Dictionary<NotifySeverity, int> GetValidationCounts()
        {
            Dictionary<NotifySeverity, int> report = new Dictionary<NotifySeverity, int>();
            foreach (ContentValidationItem cvi in validations)
            {
                if (report.ContainsKey(cvi.severity)) report[cvi.severity]++;
                else report.Add(cvi.severity, 1);
            }
            return report;
        }

        [StringFormatMethod("unformatedMsg")]
        public void Trace(object context, string unformatedMsg, params object[] args)
        {
            Log(NotifySeverity.Trace, context, unformatedMsg, args);   
        }

        [StringFormatMethod("unformatedMsg")]
        public void Debug(object context, string unformatedMsg, params object[] args)
        {
            Log(NotifySeverity.Debug, context, unformatedMsg, args);
            maxSeverity = (NotifySeverity)Math.Max((int)maxSeverity, (int)NotifySeverity.Debug);
        }

        [StringFormatMethod("unformatedMsg")]
        public void Warning(object context, string unformatedMsg, params object[] args)
        {
            Log(NotifySeverity.Warning, context, unformatedMsg, args);
            maxSeverity = (NotifySeverity)Math.Max((int)maxSeverity, (int)NotifySeverity.Warning);
        }

        [StringFormatMethod("unformatedMsg")]
        public void Error(object context, string unformatedMsg, params object[] args)
        {
            Log(NotifySeverity.Error, context, unformatedMsg, args);
            maxSeverity = (NotifySeverity)Math.Max((int)maxSeverity, (int)NotifySeverity.Error);
        }

        public void Add(ContentValidation other)
        {
            validations.AddRange(other.validations);
            maxSeverity = (NotifySeverity)Math.Max((int)maxSeverity, (int)other.maxSeverity);
        }

        public void Clear()
        {
            validations.Clear();
            maxSeverity = NotifySeverity.Trace;
        }

        private void Log(NotifySeverity severity, object context, string unformatedMsg, params object[] args)
        {
            var validation = new ContentValidationItem()
                {
                    severity = severity,
                    context = context,
                    message = string.Format(unformatedMsg, args)
                };

            validations.Add(validation);
        }

        public void OutputToDebugLog()
        {
            for (int i = 0; i < validations.Count; i++)
            {
                ContentValidationItem validation = validations[i];
                UnityEngine.Object unityContext = validation.context as UnityEngine.Object;
                switch (validation.severity)
                {
                    case NotifySeverity.Error:
                        UnityEngine.Debug.LogError(validation.message, unityContext);
                        break;
                    case NotifySeverity.Warning:
                        UnityEngine.Debug.LogWarning(validation.message, unityContext);
                        break;
                    case NotifySeverity.Debug:
                    case NotifySeverity.Trace:
                        UnityEngine.Debug.Log(validation.message, unityContext);
                        break;
                }
            }
            if (validations.Count > 0)
                UnityEngine.Debug.Log(string.Format("Validation Count: {0} Max: {1}", validations.Count, maxSeverity));
        }

        /*
        public void OutputToNotify(Notify log)
        {
            for (int i = 0; i < validations.Count; i++)
            {
                ContentValidationItem validation = validations[i];
                UnityEngine.Object unityContext = validation.context as UnityEngine.Object;
                switch (validation.severity)
                {
                    case MessageTypes.Error:
                        log.ErrorNoTrace(unityContext, validation.message);
                        break;
                    case MessageTypes.Warning:
                        log.Warning(unityContext, validation.message);
                        break;
                    case MessageTypes.Info:
                        log.Info(unityContext, validation.message);
                        break;
                    case MessageTypes.Debug:
                        log.Debug(unityContext, validation.message);
                        break;
                }
            }
            if (validations.Count > 0)
                log.Info(null, "Validation Count: {0} Max: {1}", validations.Count, maxSeverity);
        }
        */
    }    
}
