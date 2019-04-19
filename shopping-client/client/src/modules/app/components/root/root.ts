import { Component } from "@angular/core";
import { DesktopIntegrationService } from "../../services/desktopIntegrationService";
import { PushService } from "../../services/pushService";
import { OAuthService } from "angular-oauth2-oidc";
import { resourceOwnerConfig } from "../../../../environments/environment";

@Component({
  selector: "app-root",
  templateUrl: "root.html",
  styleUrls: ["root.scss"]
})
export class RootComponent {
  constructor(
    private _securityService: OAuthService,
    private _pushService: PushService,
    private _desktopIntegration: DesktopIntegrationService
  ) {
    this.initOAuth();

    this._desktopIntegration.register();
  }

  private initOAuth() {
    this._securityService.setStorage(localStorage);
    this._securityService.configure(resourceOwnerConfig);
    this._securityService.loadDiscoveryDocument();

    if (this._securityService.getAccessToken()) {
      this._pushService.start();
    };
  }
}
