import { ActionExponent } from "src/app/shared/enumerations/permission.enum";
import { ColumnGrid } from "./column-grid.model";
import { DynamicFunction } from "src/app/shared/components/list-dynamic/mp-list-dynamic.component";

export class ListDynamicOption {
  public displayColumn: ColumnGrid[] = [];
  public controller = "";
  public serviceName = "";
  public pagingUrl = "";
  public addUrl = "";
  public updateUrl = "";
  public deleteUrl = "";
  public gridOnly = false;
  public enableRowDblclick = true;
  public enabledEdit = true;
  public enabledKeyEvent = true;
  public addPermissions: ActionExponent[] = [ActionExponent.Add];
  public customizeAddUrl = '';
  public customizeViewUrl = '';
  public customizeEditUrl = '';
  public customizeAddFunc!: Function;
  public customizeEditFunc!: Function;
  public customizeViewFunc!: Function;
  public callbackGetDataFailed: Function;
  public functions: DynamicFunction[] = [];
}
