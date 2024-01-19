import { BaseLocation } from "./base-location";
import { District } from "./district";

export class Province extends BaseLocation {
  public slug = "";

  public districts: District[] = [];
}
