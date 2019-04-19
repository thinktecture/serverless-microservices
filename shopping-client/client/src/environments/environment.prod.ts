import { AuthConfig } from "angular-oauth2-oidc";

export const environment = {
  production: true,
  hmr: false,

  loginRoute: "/login",
  notificationsServiceBaseUrl: "https://notifications.serverlessmicroservices.net/api/",
  ordersApiBaseUrl: "https://orders.serverlessmicroservices.net/api/",
  productsApiBaseUrl: "https://products.serverlessmicroservices.net/api/"
};

export const resourceOwnerConfig: AuthConfig = {
  issuer: "https://cw-serverless-microservices-identity.azurewebsites.net",
  clientId: "resourceowner",
  dummyClientSecret: "no-really-a-secret",
  scope: "openid profile email api",
  oidc: false
};
