import { Component, OnInit } from '@angular/core';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';

@Component({
  selector: 'access-denied',
  template: '<div class="access-denied">access denined <a href="/sign-out" style="margin-left: 8px;">thoát</a> </div>',
  styles: ['.access-denied { padding: 16px; }']
})
export class AccessDeniedComponent implements OnInit {
  constructor(private transferService: TransferDataService) { }

  ngOnInit(): void {
    this.transferService.resolvedEvent.emit();
  }
}
