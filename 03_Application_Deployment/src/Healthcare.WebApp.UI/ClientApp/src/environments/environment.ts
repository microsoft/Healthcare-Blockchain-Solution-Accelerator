// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  services: {
    apiEndPoint: 'fabric:/Healthcare.AppHosting/Healthcare.BC.Application.API',
    trackerEndPoint: 'http://localhost:8083',
    indexerEndPoint: 'http://localhost:8082',
    escEndPoint: 'http://localhost:8081',
    proofsvcEndPoint: 'http://localhost:8084'
  },
  state: 'NY',
  baseUrl: 'http://localhost:8080'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
