import { Component, HostBinding } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { PushService } from "../../services/pushService";
import { OAuthService } from "angular-oauth2-oidc";

@Component({
  selector: "app-security-login",
  styleUrls: ["login.scss"],
  templateUrl: "login.html"
})
export class LoginComponent {
  @HostBinding("class.box")
  public loginCssClass = true;

  public username: string;
  public password: string;
  public error: string;

  constructor(
    private _oauthService: OAuthService,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _pushService: PushService
  ) {}

  public submit(): void {
    this._oauthService
      .fetchTokenUsingPasswordFlowAndLoadUserProfile(
        this.username,
        this.password
      )
      .then(
        () => {
          this._pushService.start();
          this._router.navigate([
            this._activatedRoute.snapshot.queryParams["redirectTo"]
          ]);
        },
        error => (this.error = error)
      );
  }
}
