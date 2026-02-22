import pytest
from fastapi.testclient import TestClient
from app.main import app

client = TestClient(app)

def test_root_endpoint():
    response = client.get("/")
    assert response.status_code == 200
    assert response.json()["message"] == "Expense Tracker AI Service"

def test_health_check():
    response = client.get("/health")
    assert response.status_code == 200
    assert response.json()["status"] == "healthy"

def test_summary_endpoint():
    payload = {
        "user_id": "test-user",
        "total_amount": 1500.0,
        "category_breakdown": {
            "Food & Dining": 750.0,
            "Transportation": 300.0
        }
    }
    
    response = client.post("/ai/summary", json=payload)
    
    assert response.status_code == 200
    assert "summary" in response.json()
    assert "1500" in response.json()["summary"]

def test_suggestions_endpoint():
    payload = {
        "user_id": "test-user",
        "total_amount": 1500.0,
        "category_breakdown": {
            "Food & Dining": 750.0,
            "Shopping": 500.0
        }
    }
    
    response = client.post("/ai/suggestions", json=payload)
    
    assert response.status_code == 200
    assert "suggestions" in response.json()
    assert isinstance(response.json()["suggestions"], list)
    assert len(response.json()["suggestions"]) > 0

def test_summary_invalid_payload():
    payload = {"invalid": "data"}
    
    response = client.post("/ai/summary", json=payload)
    
    assert response.status_code == 422

def test_suggestions_zero_amount():
    payload = {
        "user_id": "test-user",
        "total_amount": 0.0,
        "category_breakdown": {}
    }
    
    response = client.post("/ai/suggestions", json=payload)
    
    assert response.status_code == 200
    assert len(response.json()["suggestions"]) > 0
