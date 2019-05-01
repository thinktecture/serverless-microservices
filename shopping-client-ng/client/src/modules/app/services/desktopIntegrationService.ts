import { Injectable, NgZone } from "@angular/core";
import { ElectronService } from "ngx-electron";
import { Router } from "@angular/router";
import { PlatformService } from "./platformService";

@Injectable()
export class DesktopIntegrationService {
  constructor(
    private _platform: PlatformService,
    private _electronService: ElectronService,
    private _zone: NgZone,
    private _router: Router
  ) {}

  public register() {
    if (this._platform.isElectron) {
      this._electronService.ipcRenderer.on("navigateTo", (event, data) => {
        this._zone.run(() => {
          this._router.navigate([data]);
        });
      });
    }
  }
}
