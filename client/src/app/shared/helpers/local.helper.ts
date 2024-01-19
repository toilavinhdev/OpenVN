export class LocalHelper {
  public static getAndParse(key: string) {
    const value = localStorage.getItem(key);
    return value ? JSON.parse(value) : {};
  }
}
