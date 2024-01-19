import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonHeaderComponent } from 'src/app/shared/components/element/common-header/common-header.component';
import { DeviceType } from 'src/app/shared/enumerations/device.enum';
import { NotebookType } from 'src/app/shared/enumerations/notebook-type.enum';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { ConfigService } from 'src/app/shared/services/config/config.service';
import { TinyEditorService } from 'src/app/shared/services/tiny-editor/tiny-editor.service';
import { CategoryWithNotes } from './base-notebook-view';
import { NotebookAddComponent } from './notebook-add/notebook-add.component';
import { NotebookListComponent } from './notebook-list/notebook-list.component';

@Component({
  selector: 'app-notebook-v2',
  templateUrl: './notebook-v2.component.html',
  styleUrls: ['./notebook-v2.component.scss']
})
export class NotebookV2Component implements OnInit, AfterViewInit {
  DeviceType = DeviceType;

  NotebookType = NotebookType;

  isScrolled = false;

  query = "";

  config = this.tinyEditorService.getConfig(window.innerHeight * 0.01, () => console.customize("load from app"));

  searchWidth = Math.max(200, Math.min(window.innerWidth * 0.3, 480));

  @ViewChild("notebookList")
  notebookList: NotebookListComponent;

  @ViewChild("notebook")
  notebookInstance: ElementRef;

  @ViewChild("notebookAdd")
  notebookAddInstance: NotebookAddComponent;

  @ViewChild("commonHeader")
  commonHeader: CommonHeaderComponent;

  constructor(
    public configService: ConfigService,
    public tinyEditorService: TinyEditorService,
    public sharedService: SharedService,
    public translationService: TranslationService,
    public transferService: TransferDataService,
  ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit() {
    if (SharedService.DeviceType == DeviceType.Desktop) {
      (document.querySelector("#searchInstance input") as any).focus();
    }
  }

  refresh(event) {
    this.notebookList.loadData();
  }

  reload() {
    setTimeout(() => {
      this.notebookList.loadData();
    }, 50);
  }

  onScroll(e) {
    if (this.notebookInstance.nativeElement.scrollTop) {
      this.isScrolled = true;
    }
    else {
      this.isScrolled = false;
    }
  }

  addNote(category: CategoryWithNotes) {
    this.notebookAddInstance.openPopup(category.id);
  }

  changeNotebookType(event) {
    if (event.value) {
      this.transferService.changeNotebookTypeEvent.emit(NotebookType.Kanban);
    } else {
      this.transferService.changeNotebookTypeEvent.emit(NotebookType.List);
    }
  }
}
