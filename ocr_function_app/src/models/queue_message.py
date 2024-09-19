from pydantic import BaseModel
from datetime import datetime

class Payload(BaseModel):
    DeviceId: str
    Path: str
    Type: str

class PackageLabelScanEvent(BaseModel):
    TimeStamp: datetime
    Type: str
    Payload: Payload