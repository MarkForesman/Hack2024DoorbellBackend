import azure.functions as func
from azure.storage.blob import BlobServiceClient
from azure.ai.formrecognizer import DocumentAnalysisClient
from azure.core.credentials import AzureKeyCredential
from services.ocr_service import document_intelligence_ocr
from utils.fuzzy_search import extract_name_from_label
from services.blob_downloader_service import download_blob_to_string, generate_blob_sas_token
from services.email_service import send_email_service
from models.employee import Employees
from models.queue_message import PackageLabelScanEvent
import logging
import os
import json
from dotenv import load_dotenv

# Configs
load_dotenv(override=True)
DI_ENDPOINT = os.environ["DOCUMENT_INTELLIGENCE_ENDPOINT"]
DI_KEY = os.environ["DOCUMENT_INTELLIGENCE_API_KEY"]
BLOB_CONNECTION_STRING = os.environ["STORAGE_ACCOUNT_CONNECTION_STRING"] # TODO: Change blob connection to account key
STR_ACCOUNT_KEY = os.environ["STORAGE_ACCOUNT_KEY"]
STR_CONFIG_CONTAINER_NAME = os.environ["CONFIG_STORAGE_CONTAINER_NAME"]
STR_DEVICES_CONTAINER_NAME = os.environ["DEVICES_CONTAINER_NAME"]
EMAIL_CONNECTION_STRING=os.environ["EMAIL_COMMUNICATION_CONNECTION_STRING"]
SENDER_ADDRESS=os.environ["COMMUNICATION_SENDER_ADDRESS"]
QUEUE_NAME=os.environ["STORAGE_QUEUE_NAME"]

# Clients
document_intelligence_client = DocumentAnalysisClient(endpoint=DI_ENDPOINT, credential=AzureKeyCredential(DI_KEY))
blob_service_client = BlobServiceClient.from_connection_string(BLOB_CONNECTION_STRING)
app = func.FunctionApp()

# JSON employee data
employee_json_data = download_blob_to_string(blob_service_client=blob_service_client,
                            container_name=STR_CONFIG_CONTAINER_NAME,
                            blob_name="employees.json")
employees = Employees(**json.loads(employee_json_data))

@app.function_name(name="QueueFunc")
@app.queue_trigger(arg_name="msg", queue_name=QUEUE_NAME,connection="AzureWebJobsStorage")  # Queue trigger
def package_notifier(msg: func.QueueMessage) -> None:
    """Looks for a device id and file image name from a queue message.
    Extracts the shipping label information and sends an email notification
    to a specific users email if a match is found. If not, sends a email
    to a group email.

    Args:
        msg (func.QueueMessage): The payload of the queue message, it should be expected to follow the
        object defined in models/queue_message
    """
    
    logging.info('Python queue trigger function processed a queue item: %s', msg.get_body().decode('utf-8'))
    
    message_body = msg.get_body().decode('utf-8')
    message_dict = json.loads(message_body)
    event = PackageLabelScanEvent(**message_dict)
    if event.Type == "PackageLabelScanEvent":
        blob_name = f"{event.Payload.DeviceId}/{event.Payload.Path}".strip()
        blob_sas = generate_blob_sas_token(blob_service_client=blob_service_client,
                                            container_name=STR_DEVICES_CONTAINER_NAME,
                                            blob_name=blob_name,
                                            account_key=STR_ACCOUNT_KEY)

        
        blob_url = f"{blob_service_client.url}{STR_DEVICES_CONTAINER_NAME}/{blob_name}?{blob_sas}".strip()
        names = [employee.name for employee in employees.employees]
        
        shipping_label = document_intelligence_ocr(document_analysis_client=document_intelligence_client, 
                                                   image_url=blob_url)
        fuzzy_result = extract_name_from_label(shipping_label=shipping_label, 
                                               employee_list=names)
        if fuzzy_result is None:
            # TODO: Add logic to handle when it's None
            print("It's None")
            pass
        found_employee = employees.find_employee_by_name(fuzzy_result)
        send_email_service(connection_string=EMAIL_CONNECTION_STRING, 
                           sender_address=SENDER_ADDRESS, 
                           employee=found_employee, image_url=blob_url)
    else:
        logging.error(f"Event: {event.Type} is not a valid event!")
        raise NotImplementedError()

@app.function_name(name="health_check")
@app.route(route="health", auth_level=func.AuthLevel.ANONYMOUS)
def health_check_function(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    return func.HttpResponse(
        "This HTTP triggered function executed successfully.",
        status_code=200
        )