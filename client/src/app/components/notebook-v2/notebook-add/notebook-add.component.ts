import { Component, EventEmitter, Injector, Output } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { FormMode } from 'src/app/shared/enumerations/common.enum';
import { PopupService } from 'src/app/shared/services/base/popup.service';
import { NoteService } from 'src/app/shared/services/notebook/note.service';
import { CreateOrUpdateNoteComponent } from '../../z-popup/notebook/create-or-update-note/create-or-update-note.component';

@Component({
  selector: 'app-notebook-add',
  templateUrl: './notebook-add.component.html',
  styleUrls: ['./notebook-add.component.scss']
})
export class NotebookAddComponent extends BaseComponent {

  dialogRef: MatDialogRef<CreateOrUpdateNoteComponent>;

  isExpand = false;

  @Output()
  refreshData = new EventEmitter();

  constructor(
    public injector: Injector,
    public noteService: NoteService,
    public dialog: MatDialog,
    public popupService: PopupService,
  ) {
    super(injector);
  }

  public openPopup(catId: string) {
    const config = this.popupService.maxPingConfig();
    config['panelClass'] = 'fucking-create';
    config['data'] = {
      formMode: FormMode.Add,
      categoryId: catId
    };

    this.dialogRef = this.dialog.open(CreateOrUpdateNoteComponent, config);
    this.dialogRef.updatePosition({ top: '60px' });
    this.dialogRef.afterClosed().subscribe(shouldRefresh => {
      if (shouldRefresh) {
        this.refreshData.emit();
      }
    });
  }

  public expand() {
    this.isExpand = true;
  }

  public collapse() {
    this.isExpand = false;
  }
}
