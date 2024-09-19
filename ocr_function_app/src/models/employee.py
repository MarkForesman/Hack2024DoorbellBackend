from pydantic import BaseModel
from typing import Optional

class Employee(BaseModel):
    name: str
    email_address: str

class Employees(BaseModel):
    employees: list[Employee]

    def find_employee_by_name(self, name: str) -> Optional[Employee]:
        """Searches for an employee based on name

        Args:
            name (str): Employee name to lookup.

        Returns:
            Optional[Employee]: Returns an Employee object if the name is found or None
        """
        for employee in self.employees:
            if employee.name == name:
                return employee
        return None