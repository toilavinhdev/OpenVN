export class DateHelper {
  public static months = [
    { id: 1, text: "Tháng 1" },
    { id: 2, text: "Tháng 2" },
    { id: 3, text: "Tháng 3" },
    { id: 4, text: "Tháng 4" },
    { id: 5, text: "Tháng 5" },
    { id: 6, text: "Tháng 6" },
    { id: 7, text: "Tháng 7" },
    { id: 8, text: "Tháng 8" },
    { id: 9, text: "Tháng 9" },
    { id: 10, text: "Tháng 10" },
    { id: 11, text: "Tháng 11" },
    { id: 12, text: "Tháng 12" },
  ];

  public static firstOfYear = new Date(new Date().getFullYear(), 0, 1);

  public static firstOfNextYear = new Date(new Date().getFullYear() + 1, 0, 1);

  public static lastOfYear = new Date(DateHelper.firstOfNextYear.setSeconds(DateHelper.firstOfNextYear.getSeconds() - 1));

  public static currentMonth = new Date().getMonth() + 1;

  public static currentYear = new Date().getFullYear();

  public static daysInThisMonth = DateHelper.getDaysInMonth(DateHelper.currentMonth, DateHelper.currentYear);

  public static now = new Date();

  public static todayISO = new Date().toISOString().split('T')[0];

  public static getExactlyDate(date: Date) {
    return new Date(date);
  }

  public static getTimeOnly(date: Date) {
    let hour = date.getHours();
    let minute = date.getMinutes();
    let second = date.getSeconds();

    const hourStr = hour < 10 ? '0' + hour : hour;
    const minuteStr = minute < 10 ? '0' + minute : minute;
    const secondStr = second < 10 ? '0' + second : second;

    return `${hourStr}:${minuteStr}:${secondStr}`;
  }

  public static formatDate(date: Date) {
    if (date == null) {
      return '';
    }

    date = new Date(date);
    let day = date.getDate();
    let month = date.getMonth() + 1;
    let dayStr = day < 10 ? `0${day}` : day;
    let monthStr = month < 10 ? `0${month}` : month;

    return `${dayStr}-${monthStr}-${date.getFullYear()}`;
  }

  public static getDaysInMonth(month: number, year: number) {
    if (month <= 0 || month > 12 || year <= 0)
      return 0;
    return new Date(year, month, 0).getDate();
  }

  public static getDiffDays(date1: Date, date2: Date) {
    if (!date1 || !date2)
      return 0;
    const diffTime = Math.abs(date2.getTime() - date1.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  }

  public static compare(date1: Date, date2: Date) {
    if (!date1 || !date2)
      throw new Error("Agruments cannot be null");

    return date1.getTime() - date2.getTime();
  }

  public static getDayVietnamName(date?: Date) {
    let dayInWeek = -1;
    if (!date) {
      dayInWeek = new Date().getDay();
    } else {
      dayInWeek = new Date(date).getDay();
    }
    if (dayInWeek == 0) {
      return "Chủ nhật";
    }
    return `Thứ ${dayInWeek + 1}`;
  }

  public static displayTimeAgo(time: Date) {
    const now = new Date().getTime();
    const totalSeconds = Math.max((now - new Date(time).getTime()) / 1000, 0);
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = Math.floor((totalSeconds % 3600) % 60);

    if (hours < 1) {
      if (minutes < 1) {
        if (seconds <= 30) {
          return "Vừa xong";
        }
        return `${seconds}s ago`;
      }
      return `${minutes}m ago`;
    }
    if (hours < 24) {
      return `${hours}h ago`;
    }
    return `${Math.floor(hours / 24)}d ago`;
  }
}
