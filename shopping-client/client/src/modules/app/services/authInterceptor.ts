import { Injectable, Injector } from "@angular/core";
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
} from "@angular/common/http";
import { OAuthService } from "angular-oauth2-oidc";
import { Observable } from "rxjs";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private securityService: OAuthService;

  constructor(private injector: Injector) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    let requestToForward = req;

    if (this.securityService === undefined) {
      this.securityService = this.injector.get(OAuthService);
    }

    if (this.securityService !== undefined) {
      const token = this.securityService.getAccessToken();

      if (token !== "") {
        const tokenValue = "Bearer " + token;
        requestToForward = req.clone({
          setHeaders: { Authorization: tokenValue }
        });
      }
    } else {
      // tslint:disable-next-line:no-console
      console.debug("OidcSecurityService undefined: NO auth header!");
    }

    return next.handle(requestToForward);
  }
}
