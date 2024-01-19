export class ObjectHelper {
  /**
   * Clone, bỏ reference
   */
  public static clone(value: any): any {
    return JSON.parse(JSON.stringify(value));
  }

  public static clearSimpleValue(obj: any) {
    // Nếu là array
    if (Array.isArray(obj))
      return [];

    // Nếu là string
    if (typeof (obj) === 'string')
      return "";

    // Nếu là number
    if (typeof (obj) === 'number')
      return 0;

    // Nếu là boolean
    if (typeof (obj) === 'boolean')
      return false;

    return obj;
  }

  /**
   * Clear value
   */
  public static clearValue(obj: any): any {
    const result = this.clearSimpleValue(obj);
    if (!Array.isArray(result) && typeof (result) !== 'object') {
      return result;
    }

    // Nếu là object
    if (typeof (obj) === 'object') {
      const keys = Object.keys(obj);
      keys.forEach(key => {
        obj[key] = this.clearSimpleValue(obj[key]);
      })
    }

    return obj;
  }
}
