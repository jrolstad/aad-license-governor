using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Services
{
    public static class ClockService
    {
        static DateTime? _frozenTime = null;

        public static DateTime Now
        {
            get { return _frozenTime.HasValue ? _frozenTime.Value : DateTime.Now; }
        }

        public static void Freeze()
        {
            if (_frozenTime.HasValue) return;

            _frozenTime = DateTime.Now;
        }

        public static void Thaw()
        {
            if (!_frozenTime.HasValue) return;

            _frozenTime = null;
        }
    }
}
