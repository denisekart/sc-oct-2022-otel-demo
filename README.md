# sc-oct-2022-otel-demo

## OpenTelemetry demo

Features a multi service configuration

	- Service1
	- Service2

Demonstrates the following features:

- OpenTelemetry Logging
- OpenTelemetry Distributed Tracing
- OpenTelemetry Metrics
- Collector Configuration
- Processors Configuration
- Export pipelines
- Datadog Signal Correlation
- Microservice Tracing Correlation and Context Propagation
- Automatic Code Instrumentation
- Manual Code Instrumentation
- Custom Exporter Pipelines *

* A Stub Implementation

## Requirements for running the solution

A `.env` file should be placed at the root of the repository with the following content.

> Note: The `.env` file is not tracked by source control

```config
DD_API_KEY=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

Alternatively, `DD_API_KEY` environment variable can be specified directly in the `docker-compose.yaml` file.

## Starting the solution

Docker needs to be installed locally and `linux` platform must be the active platform for running the containers.

The following command will start the OpenTelemetry collector as well as Jaeger tracing backend and Prometheus metrics platform

```bash
> docker compose up -d
```

Collector endpoint: http://localhost:55679/debug/servicez
Jaeger endpoint: http://localhost:16686/
Prometheus endpoint: http://localhost:9090/

Start both project `Service1` and `Service2`.
You can start the projects using either `dotnet run` or manually in VisualStudio (or another IDE)..