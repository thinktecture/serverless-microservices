import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router
} from "@angular/router";
import { Injectable } from "@angular/core";
import { OAuthService } from "angular-oauth2-oidc";
import { environment } from "../../../environments/environment";
import { Observable } from "rxjs";

@Injectable()
export class IsAuthenticated implements CanActivate {
  constructor(private _oauthService: OAuthService, private _router: Router) {
    if (!environment.loginRoute) {
      throw new Error("Login route has not been configured.");
    }
  }

  public canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const result = this._oauthService.hasValidAccessToken();

    if (!result) {
      this._router.navigate([environment.loginRoute], {
        queryParams: {
          redirectTo: state.url
        }
      });

      return false;
    }

    return true;
  }
}
