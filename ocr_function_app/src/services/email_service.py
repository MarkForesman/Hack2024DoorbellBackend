from azure.communication.email import EmailClient
from models.employee import Employee
import logging

def send_email_service(connection_string:str, sender_address: str, employee: Employee, image_url: str) -> None:
    """Connects to a Communication Service service and sends an email.

    Args:
        connection_string (str): Connection string of the Communication Service.
        sender_address (str): Sender address that was configured within the Communication Service.
        employee (Employee): Employee object that contains the name and email address of an employee.
        image_url (str): Image URL of the label to append to the message of the email.
    """
    
    try:
        client = EmailClient.from_connection_string(connection_string)

        message = {
        "senderAddress": sender_address,
        "recipients": {
            "to": [
                {"address": employee.email_address}
            ]
        },
        "content": {
            "subject": "Package Received!",
            "html": f"""
                <html>
                    <body>
                        <p>Hello {employee.name}! A package has arrived for you.</p>
                        <img src="{image_url}" alt="Image Description" />
                    </body>
                </html>
            """
        }
    }

        client.begin_send(message)

    except Exception as ex:
        logging.error(ex)