export class StringHelper {
  public static parseJwt(accessToken: string) {
    var base64Url = accessToken.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
  };

  public static isNullOrEmpty(input: string) {
    if (!input)
      return true;
    return input === "";
  }
}
