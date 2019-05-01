import { AuthConfig } from "angular-oauth2-oidc";

export const environment = {
  production: false,
  hmr: true,

  loginRoute: "/login",
  notificationsServiceBaseUrl: "http://localhost:7074/api/",
  ordersApiBaseUrl: "http://localhost:7071/api/",
  productsApiBaseUrl: "http://localhost:7073/api/"
};

export const resourceOwnerConfig: AuthConfig = {
  issuer: "https://cw-serverless-microservices-identity.azurewebsites.net",
  clientId: "resourceowner",
  dummyClientSecret: "no-really-a-secret",
  scope: "openid profile email api",
  oidc: false
};
