interface Array<T> {

  /**
   * Loại bỏ reference
   */
  removeRef(): Array<T>;

  /**
   * Loại bỏ null, undefined
   */
  cleanArray(): void
}

Array.prototype.removeRef = function () {
  return JSON.parse(JSON.stringify(this));
}

Array.prototype.cleanArray = function () {
  return this.filter(item => item !== null && item !== undefined);
}
