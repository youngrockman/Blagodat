using System;
using System.Collections.Generic;

namespace Libary
{
    public class Calculations
    {
        public string[] AvailablePeriods(TimeSpan[] startTimes, int[] durations, TimeSpan beginWorkingTime, TimeSpan endWorkingTime, int consultationTime)
        {
            TimeSpan consultationDuration = TimeSpan.FromMinutes(consultationTime);
            List<string> freeSlots = new List<string>();
            TimeSpan current = beginWorkingTime;

            for (int i = 0; i < startTimes.Length; i++)
            {
                TimeSpan start = startTimes[i];
                TimeSpan end = start.Add(TimeSpan.FromMinutes(durations[i]));

                while (current.Add(consultationDuration) <= start)
                {
                    freeSlots.Add($"{current:hh\\:mm}-{current.Add(consultationDuration):hh\\:mm}");
                    current = current.Add(consultationDuration);
                }

                if (current < end)
                {
                    current = end;
                }
            }

            while (current.Add(consultationDuration) <= endWorkingTime)
            {
                freeSlots.Add($"{current:hh\\:mm}-{current.Add(consultationDuration):hh\\:mm}");
                current = current.Add(consultationDuration);
            }

            return freeSlots.ToArray();
        }
    }
}