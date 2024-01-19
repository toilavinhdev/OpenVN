import { Filter } from "./filter-model";

export class PaginationRequest {
  public pageIndex: number = 0;

  public pageSize: number = 20;

  public filter = new Filter();

  public sorts: SortModel[] = [];
}

export class SortModel {
  public fieldName = "";

  public sortAscending = true;

  constructor(fieldName = "", sortAscending = true) {
    this.fieldName = fieldName;
    this.sortAscending = sortAscending;
  }
}
