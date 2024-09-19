from azure.ai.formrecognizer import AnalyzeResult
from azure.ai.formrecognizer import DocumentAnalysisClient
import logging

def document_intelligence_ocr(document_analysis_client: DocumentAnalysisClient, image_url: str):
    """Calls a Document Intelligence Azure Service to OCR a given image by URL.

    Args:
        document_analysis_client (DocumentAnalysisClient): A document analysis client object.
        image_url (str): The URL of the image that will be OCR'd .

    Returns:
        str: OCR'd results of the image in string format.
    """
    
    document_url = image_url
    poller = document_analysis_client.begin_analyze_document_from_url(model_id="prebuilt-read", document_url=document_url)
    result: AnalyzeResult = poller.result()

    for page in result.pages:
        logging.info(f"----Analyzing layout from page #{page.page_number}----")
        logging.info(f"Page has width: {page.width} and height: {page.height}, measured with unit: {page.unit}")

    ocr_result = ""
    if result.paragraphs:
        for paragraph in result.paragraphs:
            ocr_result += paragraph.content

    return ocr_result