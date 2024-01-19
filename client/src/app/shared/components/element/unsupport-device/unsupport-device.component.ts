import { Component, OnInit } from '@angular/core';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';

@Component({
  selector: 'app-unsupport-device',
  templateUrl: './unsupport-device.component.html',
  styleUrls: ['./unsupport-device.component.scss']
})
export class UnsupportDeviceComponent implements OnInit {

  constructor(public transferService: TransferDataService) { }

  ngOnInit(): void {
    this.transferService.resolvedEvent.emit();
  }

}
