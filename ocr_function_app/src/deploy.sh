zip -r ocr-func.zip * .env

az functionapp deployment source config-zip \
    --src "ocr-func.zip" \
    --name "funcOCRDoorbell" \
    --resource-group "RG-IAI-DOORBELL" \
    --build-remote true