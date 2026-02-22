import pytest
from app.services.ai_service import AiInsightsService

@pytest.fixture
def ai_service():
    return AiInsightsService()

def test_generate_summary_with_expenses(ai_service):
    # Arrange
    total = 1500.0
    breakdown = {
        "Food & Dining": 750.0,
        "Transportation": 300.0,
        "Shopping": 450.0
    }
    
    # Act
    result = ai_service.generate_summary(total, breakdown)
    
    # Assert
    assert "1500.00" in result
    assert "Food & Dining" in result
    assert "750.00" in result

def test_generate_summary_no_expenses(ai_service):
    # Arrange
    total = 0.0
    breakdown = {}
    
    # Act
    result = ai_service.generate_summary(total, breakdown)
    
    # Assert
    assert "No expenses" in result

def test_generate_suggestions_high_food_spending(ai_service):
    # Arrange
    total = 1000.0
    breakdown = {"Food & Dining": 400.0}  # 40% of total
    
    # Act
    result = ai_service.generate_suggestions(total, breakdown)
    
    # Assert
    assert len(result) > 0
    assert any("Food" in s or "🍽️" in s for s in result)

def test_generate_suggestions_high_shopping(ai_service):
    # Arrange
    total = 1000.0
    breakdown = {"Shopping": 300.0}  # 30% of total
    
    # Act
    result = ai_service.generate_suggestions(total, breakdown)
    
    # Assert
    assert len(result) > 0
    assert any("Shopping" in s or "🛍️" in s for s in result)

def test_generate_suggestions_balanced_spending(ai_service):
    # Arrange
    total = 1000.0
    breakdown = {
        "Food & Dining": 200.0,
        "Transportation": 150.0,
        "Shopping": 100.0
    }
    
    # Act
    result = ai_service.generate_suggestions(total, breakdown)
    
    # Assert
    assert len(result) > 0

def test_generate_suggestions_no_expenses(ai_service):
    # Arrange
    total = 0.0
    breakdown = {}
    
    # Act
    result = ai_service.generate_suggestions(total, breakdown)
    
    # Assert
    assert len(result) == 1
    assert "Start tracking" in result[0]

def test_generate_suggestions_max_five(ai_service):
    # Arrange
    total = 5000.0
    breakdown = {
        "Food & Dining": 1600.0,  # 32%
        "Transportation": 1300.0,  # 26%
        "Shopping": 1300.0,  # 26%
        "Entertainment": 1100.0  # 22%
    }
    
    # Act
    result = ai_service.generate_suggestions(total, breakdown)
    
    # Assert
    assert len(result) <= 5
