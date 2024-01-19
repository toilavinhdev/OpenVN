import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from "@angular/router";
import { of } from "rxjs";
import { Observable } from "rxjs/internal/Observable";
import { TransferDataService } from "../services/base/transfer-data.service";
import { HubConnectionService } from "../services/base/hub-connection.service";
import { switchMap } from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class AnonymousResolver<T> implements Resolve<T> {

  constructor(
    public router: Router,
    public transferService: TransferDataService,
    public hubService: HubConnectionService
  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<T> | Promise<T> | T | any {
    return this.hubService
      .start()
      .pipe(
        switchMap(() => {
          this.hubService.isAuthenticated = false;
          this.transferService.resolvedEvent.emit();
          return of(true);
        })
      );
  }
}
