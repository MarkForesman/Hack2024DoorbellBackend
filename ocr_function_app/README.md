# OCR Function App

## Table of Contents

- [OCR Function App](#ocr-function-app)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
    - [Setup](#setup)
      - [Prerequisites](#prerequisites)
  - [Resources](#resources)

## Overview

This function app is apart of the Doorbell hack

This serves as the optical character recognition and email notification
service when a message is received in a queue. The expected message
that this function app receives contains data about a shipping label and device id and extracts
this information to automatically email an employee when a package has been received.

### Setup

#### Prerequisites

There are a few Azure services that need to be created for this function app to work end to end.

- Azure Communication Service
- Azure Email Communication Service
  - Note: These are two different resources and both are required
- Azure Storage Account
- Azure Document Intelligence

Once these are deployed, you will have to configure some of the services.

1. Azure Email Communication Service
   - You will have to provision a domain (see [this documentation](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/connect-email-communication-resource?pivots=azure-portal) for more information)
2. Azure Storage Account
   - Create a container that houses the JSON configuration [file](employees.json) (default container name is `config`).
     - This file contains the "source of truth" of employees and is what is used to compare against the OCR.
   - Create a container for shipping labels, (default container name is `packagelabels`)
   - Create a storage queue (default queue name is packagelabels)
3. The Azure Function relies on a specific type of queue message
  Sample queue message

  ```json
  {
    "TimeStamp":"2024-09-18T17:35:11.869781Z",
    "Type":"PackageLabelScanEvent",
    "Payload":{
      "DeviceId":"107",
      "Path":"mark_foresman_good.jpg",
      "Type":"PackageLabelScanEvent"
      }
  }
  ```

Make sure to follow this format. It will *only* continue if the message type is `PackageLabelScanEvent`.

4. There are a few environment variables that need to be configured, details on those are below.
   - To configure, you will need to copy the existing [.env.template](./src/.env.template) and
   rename it to to `.env`. Fill in the values as needed.

| Variable Name                                   | Description                                      |
|------------------------------------------------|--------------------------------------------------|
| `DOCUMENT_INTELLIGENCE_ENDPOINT`               | The endpoint URL for the document intelligence service within Azure. |
| `DOCUMENT_INTELLIGENCE_API_KEY`                | The API key used for authenticating requests to the document intelligence service. |
| `STORAGE_ACCOUNT_CONNECTION_STRING`            | The connection string used to connect to the storage account (e.g., Azure Blob Storage). |
| `STORAGE_ACCOUNT_KEY`                           | The key used for accessing the Azure Storage account directly. |
| `STORAGE_QUEUE_NAME`                           | The name of the Azure Storage Queue for processing package labels. |
| `DEVICES_CONTAINER_NAME`                       | The name of the container in Azure Blob Storage where device-related files are stored. |
| `CONFIG_STORAGE_CONTAINER_NAME`                | The name of the container in the storage account where configuration files are stored. |
| `EMAIL_COMMUNICATION_CONNECTION_STRING`        | The connection string used to connect to the email communication service. |
| `COMMUNICATION_SENDER_ADDRESS`                 | The email address from which communications will be sent. |
| `GROUP_EMAIL_ALIAS`                            | An alias for a group email address that can be used for sending emails to multiple recipients. |

## Resources

<https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/connect-email-communication-resource?pivots=azure-portal>
<https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/send-email?tabs=linux%2Cconnection-string%2Csend-email-and-get-status-async%2Csync-client&pivots=platform-azportal>
