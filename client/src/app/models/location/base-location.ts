export class BaseLocation {
  public id = "0";
  public name = "";
  public type = 0;
  public childrenCount = 0;
  public children: KLocation[] = [];
}

export class KLocation extends BaseLocation {
  public isExpanded = false;
  public loadingChildren = false;
}
