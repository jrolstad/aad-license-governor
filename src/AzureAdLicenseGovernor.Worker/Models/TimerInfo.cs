namespace AzureAdLicenseGovernor.Worker.Functions
{
    public class TimerInfo
    {
        public TimerScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }
}
