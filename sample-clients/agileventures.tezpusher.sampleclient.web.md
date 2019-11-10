# Sample Client for TaaS Web & Docker

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 6.0.3. Run `npm i -g @angular/cli` to install angular CLI globally.

## Configuration

Configure `_baseUrl` variable in [src/app/signalr.service.ts\#L60](https://github.com/agile-ventures/TaaS/blob/79552fdaf3df82d6b311112d6651b8e687ba5864/AgileVentures.TezPusher.SampleClient.Web/src/app/signalr.service.ts#L60) based on your environment. If you are running the Docker image from DockerHub \([https://hub.docker.com/r/tezoslive/agileventurestezpusherweb](https://hub.docker.com/r/tezoslive/agileventurestezpusherweb)\) it is easier to use HTTP endpoint at the moment.

## How to run

Run `npm install` to install all required dependencies. Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).

