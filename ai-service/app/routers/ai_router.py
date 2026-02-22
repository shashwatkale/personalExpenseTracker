from fastapi import APIRouter, HTTPException
from app.models.schemas import ExpenseDataRequest, SummaryResponse, SuggestionsResponse
from app.services.ai_service import AiInsightsService

router = APIRouter()
ai_service = AiInsightsService()

@router.post("/summary", response_model=SummaryResponse)
async def get_spending_summary(request: ExpenseDataRequest):
    """
    Generate AI-powered spending summary
    """
    try:
        summary = ai_service.generate_summary(
            request.total_amount,
            request.category_breakdown
        )
        return SummaryResponse(summary=summary)
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating summary: {str(e)}")

@router.post("/suggestions", response_model=SuggestionsResponse)
async def get_saving_suggestions(request: ExpenseDataRequest):
    """
    Generate personalized saving suggestions
    """
    try:
        suggestions = ai_service.generate_suggestions(
            request.total_amount,
            request.category_breakdown
        )
        return SuggestionsResponse(suggestions=suggestions)
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating suggestions: {str(e)}")
