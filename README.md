# bbt.incoming-sms
Incoming SMS handler and multiplexer

To Run incoming-sms:
> dapr run --app-id incoming-sms dotnet run  --app-port 5261  --components-path \Components --enable-api-logging

To Run loan.sms.application:
> dapr run --app-id loan-sms dotnet run  --app-port 5215  --components-path \Components --enable-api-logging 