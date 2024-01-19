import { BaseModel } from "../base/base-model";
import { ConfigValue } from "./config-value-model";

export class UserConfig extends BaseModel {
    public configValue: ConfigValue = new ConfigValue();
}
