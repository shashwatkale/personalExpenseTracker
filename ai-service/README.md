# AI Service - FastAPI

## Overview

Python FastAPI microservice for AI-powered expense insights.

## Features

- Spending summaries
- Personalized saving suggestions
- LLM-ready architecture

## Structure

```
ai-service/
├── app/
│   ├── main.py              # FastAPI app
│   ├── models/
│   │   └── schemas.py       # Pydantic models
│   ├── services/
│   │   └── ai_service.py    # Business logic
│   └── routers/
│       └── ai_router.py     # API endpoints
├── requirements.txt
└── .env
```

## Setup

```bash
# Create virtual environment
python -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate

# Install dependencies
pip install -r requirements.txt

# Run server
uvicorn app.main:app --reload --port 8000
```

## API Endpoints

### POST /ai/summary
Generate spending summary

**Request:**
```json
{
  "user_id": "uuid",
  "total_amount": 1500.00,
  "category_breakdown": {
    "Food & Dining": 450.00,
    "Transportation": 300.00,
    "Shopping": 750.00
  }
}
```

**Response:**
```json
{
  "summary": "You spent $1500.00 across 3 categories..."
}
```

### POST /ai/suggestions
Get saving suggestions

**Response:**
```json
{
  "suggestions": [
    "🍽️ Food spending is 30% of your budget...",
    "🛍️ Shopping represents 50% of expenses..."
  ]
}
```

## Future LLM Integration

The service is designed with LLM integration in mind:

```python
# Example OpenAI integration
from openai import AsyncOpenAI

client = AsyncOpenAI(api_key=os.getenv("OPENAI_API_KEY"))

async def generate_with_llm(self, prompt: str) -> str:
    response = await client.chat.completions.create(
        model="gpt-4",
        messages=[{"role": "user", "content": prompt}]
    )
    return response.choices[0].message.content
```

## Environment Variables

Create `.env` file:

```
ENVIRONMENT=development
LOG_LEVEL=info
OPENAI_API_KEY=your_key_here  # For future LLM integration
```
