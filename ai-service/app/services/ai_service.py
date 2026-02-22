from typing import Dict

class AiInsightsService:
    """
    AI Service for generating spending insights and suggestions.
    Currently uses rule-based logic. Ready for LLM integration.
    """
    
    def generate_summary(self, total_amount: float, category_breakdown: Dict[str, float]) -> str:
        """Generate spending summary based on expense data"""
        
        if total_amount == 0:
            return "No expenses recorded for this period."
        
        # Find top spending category
        top_category = max(category_breakdown.items(), key=lambda x: x[1]) if category_breakdown else ("Unknown", 0)
        top_category_name, top_category_amount = top_category
        top_category_percentage = (top_category_amount / total_amount * 100) if total_amount > 0 else 0
        
        # Count categories
        active_categories = len([v for v in category_breakdown.values() if v > 0])
        
        summary = (
            f"You spent ${total_amount:.2f} across {active_categories} categories this month. "
            f"Your highest spending was in {top_category_name} (${top_category_amount:.2f}, "
            f"{top_category_percentage:.1f}% of total). "
        )
        
        # Add spending pattern insight
        if top_category_percentage > 50:
            summary += f"Consider diversifying your budget as {top_category_name} dominates your expenses."
        elif active_categories > 5:
            summary += "Your spending is well-distributed across multiple categories."
        
        return summary
    
    def generate_suggestions(self, total_amount: float, category_breakdown: Dict[str, float]) -> list[str]:
        """Generate personalized saving suggestions"""
        
        suggestions = []
        
        if total_amount == 0:
            return ["Start tracking your expenses to get personalized insights!"]
        
        # Analyze each category
        for category, amount in category_breakdown.items():
            percentage = (amount / total_amount * 100) if total_amount > 0 else 0
            
            if category.lower() in ["food & dining", "food"] and percentage > 30:
                suggestions.append(
                    f"🍽️ Food spending is {percentage:.1f}% of your budget. "
                    "Try meal prepping to save 20-30% on dining costs."
                )
            
            elif category.lower() in ["shopping"] and percentage > 25:
                suggestions.append(
                    f"🛍️ Shopping represents {percentage:.1f}% of expenses. "
                    "Consider a 30-day rule: wait before non-essential purchases."
                )
            
            elif category.lower() in ["entertainment"] and percentage > 20:
                suggestions.append(
                    f"🎬 Entertainment is {percentage:.1f}% of spending. "
                    "Look for free community events or streaming service bundles."
                )
            
            elif category.lower() in ["transportation"] and percentage > 25:
                suggestions.append(
                    f"🚗 Transportation costs are {percentage:.1f}%. "
                    "Consider carpooling, public transit, or biking for short trips."
                )
        
        # General suggestions based on total
        if total_amount > 3000:
            suggestions.append(
                "💰 High monthly spending detected. Try the 50/30/20 rule: "
                "50% needs, 30% wants, 20% savings."
            )
        
        if len(suggestions) == 0:
            suggestions.append("✅ Your spending looks balanced! Keep tracking to maintain good habits.")
            suggestions.append("📊 Set category budgets to get more targeted insights.")
        
        return suggestions[:5]  # Return max 5 suggestions
    
    # Future: LLM integration method
    async def generate_with_llm(self, prompt: str) -> str:
        """
        Placeholder for future LLM integration.
        Can integrate OpenAI, Anthropic, or AWS Bedrock here.
        """
        # Example structure:
        # response = await openai_client.chat.completions.create(
        #     model="gpt-4",
        #     messages=[{"role": "user", "content": prompt}]
        # )
        # return response.choices[0].message.content
        pass
