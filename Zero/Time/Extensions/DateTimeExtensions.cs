using System;

namespace Zero.Time.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 获取一天开始时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDayStartTime(this DateTime time)
        {
            int year = time.Year;
            int month = time.Month;
            int day = time.Day;
            DateTime start = new DateTime(year, month, day, 0, 0, 0);
            return start;
        }
        /// <summary>
        /// 获取一天结束时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDayEndTime(this DateTime time)
        {
            int year = time.Year;
            int month = time.Month;
            int day = time.Day;
            DateTime end = new DateTime(year, month, day, 23, 59, 59);
            return end;
        }
        /// <summary>
        /// 获取月的第一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetMonthFirstDay(this DateTime time)
        {
            DateTime monthFirstDay = time.AddDays(1 - (time.Day)).GetDayStartTime();
            return monthFirstDay;
        }
        /// <summary>
        /// 获取月的最后一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetMonthLastDay(this DateTime time)
        {
            //获得某年某月的天数    
            int year = time.Year;
            int month = time.Month;
            int dayCount = DateTime.DaysInMonth(year, month);

            DateTime monthLastDay = time.GetMonthFirstDay().AddDays(dayCount - 1).GetDayEndTime();
            return monthLastDay;
        }
        /// <summary>
        /// 获取年开始的第一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetYearFirstDay(this DateTime time)
        {
            DateTime yearFirstDay = time.AddDays(1 - (time.Day))
                .AddMonths(1 - (time.Month))
                .GetDayStartTime();

            return yearFirstDay;
        }
        /// <summary>
        /// 获取年开始的最后一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetYearLastDay(this DateTime time)
        {
            DateTime yearLastDay = time.AddMonths((0 - time.Month) + 12).GetMonthLastDay();
            return yearLastDay;
        }
    }
}
