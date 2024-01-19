import { BaseLocation } from "./base-location";
import { Ward } from "./ward";

export class District extends BaseLocation {
  public provinceId = "0";
  public wards: Ward[] = [];
}
