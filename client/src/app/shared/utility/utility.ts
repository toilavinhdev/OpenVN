import { SnackBarParameter } from "src/app/models/snackbar.param";
import { SnackBar } from "../components/element/snackbar/snackbar.component";
import { SessionStorageKey } from "../constants/sessionstorage.key";
import { DeviceType } from "../enumerations/device.enum";
import { DateHelper } from "../helpers/date.helper";
import { TranslationService } from "../services/base/translation.service";

export class Utility {
  public static videoExtensions = [
    "m2v",
    "mpg",
    "mp2",
    "mpeg",
    "mpe",
    "mpv",
    "mp4",
    "m4p",
    "m4v",
    "mov",
  ];

  public static imageExtensions = [
    "apng",
    "avif",
    "gif",
    "jpg",
    "jpeg",
    "jfif",
    "pjpeg ",
    "pjp",
    "png",
    "svg",
    "webp",
  ];

  public static log(message?: any, ...options: any[]) {
    let logs = sessionStorage.getItem(SessionStorageKey.SESSION_LOG);
    let arr = [];
    message = `[${DateHelper.getTimeOnly(new Date())}]: ` + message + options.join(' ');

    if (!logs) {
      arr.push(message);
    } else {
      arr = JSON.parse(logs);
      arr.push(message);
    }
    sessionStorage.setItem(SessionStorageKey.SESSION_LOG, JSON.stringify(arr));
    console.customize(message, ...options);
  }

  public static changeTitle(title: string) {
    if (title.isEmpty())
      return;
    (document.querySelector('title') as any).textContent = title;
  }

  public static featureIsInDevelopment(e: any) {
    try {
      e?.stopPropagation();
    } catch {
      // do anything here
    }
    finally {
      // MessageBox.information(new Message(this, { content: TranslationService.VALUES['COMMON']['FEATURE_IS_IN_DEVELOPMENT'] }));
      SnackBar.warning(new SnackBarParameter(this, TranslationService.VALUES['COMMON']['FEATURE_IS_IN_DEVELOPMENT'], 2000));
    }
  }

  public static randomInRange(start: number, end: number) {
    return Math.floor(Math.random() * (end - start + 1) + start);
  }

  public static formatCurrency(value: string | number) {
    const re = '\\d(?=(\\d{3})+$)';
    return parseInt(value + "").toFixed(Math.max(0, ~~0)).replace(new RegExp(re, 'g'), '$&,');
  }

  public static removeHtml(input: string) {
    var div = document.createElement("div");
    div.innerHTML = input;
    return div.textContent || div.innerText || "";
  }

  public static getDevice(): DeviceType {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
      return DeviceType.Mobile;
    }
    return DeviceType.Desktop;
  }

  public static isNumber(input: string) {
    try {
      parseInt(input);
      return true;

    } catch {
      return false;
    }
  }

  public static isVideo(url: string) {
    return this.videoExtensions.find(ext => url.toLowerCase().includes(`.${ext}`)) != null;
  }

  public static isImage(url: string) {
    return this.imageExtensions.find(ext => url.toLowerCase().includes(`.${ext}`)) != null;
  }

  public static bytesToKilobytes(bytes: number) {
    return bytes / 1024;
  }

  public static bytesToMegabytes(bytes: number) {
    return Utility.bytesToKilobytes(bytes) / 1024;
  }

  public static bytesToGigabytes(bytes: number) {
    return Utility.bytesToMegabytes(bytes) / 1024;
  }

  public static displaySize(bytes: number): string {
    if (bytes >= Math.pow(1024, 3)) {
      return Utility.bytesToGigabytes(bytes).toFixed(2) + " GB";
    }
    if (bytes >= Math.pow(1024, 2)) {
      return Utility.bytesToMegabytes(bytes).toFixed(2) + " MB";
    }
    return Utility.bytesToKilobytes(bytes).toFixed(2) + " KB";
  }

  public static mapExtensionToClassName(fileExtension: string) {
    switch (fileExtension) {
      case ".rar":
        return "rar";
      case ".exe":
        return "exe";
      case ".sql":
        return "sql";
      case ".html":
        return "html";
      case ".xlsx":
        return "xlsx";
      case ".log":
        return "log";
      case ".json":
        return "json";
      case ".css":
        return "css";
      case ".doc":
        return "doc";
      case ".pdf":
        return "pdf";
      case ".txt":
        return "txt";
      case ".m2v":
      case ".mpg":
      case ".mp2":
      case ".mpeg":
      case ".mpe":
      case ".mpv":
      case ".mp4":
      case ".m4p":
      case ".m4v":
      case ".mov":
        return "video";
      case ".apng":
      case ".avif":
      case ".gif":
      case ".jpg":
      case ".jpeg":
      case ".jfif":
      case ".pjpeg ":
      case ".pjp":
      case ".png":
      case ".svg":
      case ".webp":
        return "image";
      default:
        return "other";
    }
  }
}
