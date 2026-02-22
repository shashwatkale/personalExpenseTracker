from pydantic import BaseModel
from typing import Dict

class ExpenseDataRequest(BaseModel):
    user_id: str
    total_amount: float
    category_breakdown: Dict[str, float]

class SummaryResponse(BaseModel):
    summary: str

class SuggestionsResponse(BaseModel):
    suggestions: list[str]
