import { ActionExponent } from "src/app/shared/enumerations/permission.enum";

export class RoutingConfig {
  public path = "";
  public key = "";
  public actionExponents: ActionExponent[] = [];

  constructor(path: string, key: string, actionExponents?: ActionExponent[]) {
    this.path = path;
    this.key = key;
    this.actionExponents = actionExponents;
  }
}
