export class CookieHelper {
  public static setCookie(name: string, value: any, exdays: number) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = name + "=" + value + ";" + expires + ";path=/";
  }

  public static getCookie(name: string) {
    const nameEQ = name + '=';
    for (const cookie of document.cookie.split('; ')) {
      if (cookie.indexOf(nameEQ) === 0) {
        const value = cookie.substring(nameEQ.length);
        return decodeURIComponent(value); // returns first found cookie
      }
    }
    return null;
  }

  public static removeCookie(name: string) {
    document.cookie = name + '=; path=/; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
  }
}
