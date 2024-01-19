
export class BaseModel {
  public id = "0";

  public createdDate: Date = new Date();

  public lastModifiedDate?: Date = null;

  constructor(obj?: object) {
    if(obj) {
      setTimeout( () => {
        Object.assign(this, obj);
      }, 0);
    }
  }
}
