import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'mp-combobox',
  templateUrl: './mp-combobox.component.html',
  styleUrls: ['./mp-combobox.component.scss']
})
export class MpComboboxComponent implements OnInit {

  @Input()
  dataSource: any[] = [];

  @Input()
  displayExpr = "";

  @Input()
  valueExpr = "";

  @Input()
  placeholder = "";

  @Input()
  noDataText = "No data";

  @Input()
  disabled = false;

  @Input()
  readOnly = false;

  @Input()
  width = undefined;

  @Input()
  height = undefined;

  @Input()
  label = "";

  @Input()
  hint = "";

  @Input()
  name = "";

  @Input()
  value: any;

  @Input()
  isFetching = false;

  @Input()
  enabledAdd = false;

  @Output()
  onOpened = new EventEmitter();

  @Output()
  onValueChanged = new EventEmitter();

  @Output()
  onEnterKey = new EventEmitter();

  @Output()
  onBlur = new EventEmitter();

  @Output()
  onAdd = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }


  public onOpenedFunc(e: any) {
    this.onOpened.emit(e);
  }
  public onValueChangedFunc(e: any) {
    this.onValueChanged.emit(e);
  }
  public onEnterKeyFunc(e: any) {
    this.onEnterKey.emit(e);
  }
  public onBlurFunc(e: any) {
    this.onBlur.emit(e);
  }
  public onAddFunc(e: any) {
    this.onAdd.emit(e);
  }

}
