import { NextRequest, NextResponse } from 'next/server';

interface ExpenseData {
  categoryId: string;
  amount: number;
  description: string;
  date: string;
  notes?: string;
}

let categoriesCache: any[] = [];

async function getCategories() {
  if (categoriesCache.length > 0) return categoriesCache;
  
  try {
    const response = await fetch('http://localhost:5000/api/categories');
    categoriesCache = await response.json();
    return categoriesCache;
  } catch (error) {
    return [];
  }
}

async function extractExpense(message: string): Promise<ExpenseData | null> {
  const lowerMsg = message.toLowerCase();
  
  const amountPatterns = [
    /(?:rs\.?|rupees?|₹)\s*(\d+(?:\.\d{2})?)/i,
    /(\d+(?:\.\d{2})?)\s*(?:rs\.?|rupees?|₹)/i,
    /\$\s*(\d+(?:\.\d{2})?)/,
    /(\d+(?:\.\d{2})?)\s*(?:dollars?|bucks?)/i,
    /(?:spent|paid|cost|bought for)\s+(\d+)/i,
    /(\d+)\s+(?:on|for)/i,
  ];
  
  let amount = 0;
  for (const pattern of amountPatterns) {
    const match = message.match(pattern);
    if (match) {
      amount = parseFloat(match[1]);
      break;
    }
  }
  
  if (!amount) return null;
  
  const categories = await getCategories();
  
  const keywordMap: Record<string, string[]> = {
    'food': ['food', 'lunch', 'dinner', 'breakfast', 'meal', 'restaurant', 'groceries', 'grocery', 'snack', 'coffee', 'tea'],
    'transportation': ['transport', 'taxi', 'uber', 'ola', 'bus', 'train', 'metro', 'fuel', 'gas', 'petrol', 'diesel', 'ride'],
    'shopping': ['shopping', 'shop', 'clothes', 'shirt', 'shoes', 'dress', 'purchase', 'bought', 'buy'],
    'entertainment': ['entertainment', 'movie', 'cinema', 'game', 'concert', 'party', 'fun'],
    'healthcare': ['health', 'doctor', 'medicine', 'hospital', 'medical', 'pharmacy', 'clinic'],
    'bills': ['bill', 'electricity', 'water', 'internet', 'phone', 'utility', 'utilities'],
    'home rent': ['rent', 'house rent', 'home rent', 'apartment'],
  };
  
  let categoryId = categories.find(c => c.name.toLowerCase() === 'others')?.id;
  
  for (const [catName, keywords] of Object.entries(keywordMap)) {
    if (keywords.some(kw => lowerMsg.includes(kw))) {
      const found = categories.find(c => c.name.toLowerCase() === catName);
      if (found) {
        categoryId = found.id;
        break;
      }
    }
  }
  
  if (!categoryId && categories.length > 0) {
    categoryId = categories[0].id;
  }
  
  return {
    categoryId,
    amount,
    description: message.trim(),
    date: new Date().toISOString().split('T')[0],
  };
}

export async function POST(request: NextRequest) {
  try {
    const { message } = await request.json();
    
    if (!message) {
      return NextResponse.json({ error: 'Message is required' }, { status: 400 });
    }
    
    const lowerMsg = message.toLowerCase();
    
    const expenseKeywords = ['spent', 'paid', 'bought', 'purchased', 'cost', 'expense'];
    const hasExpenseIntent = expenseKeywords.some(kw => lowerMsg.includes(kw)) || /\d+/.test(message);
    
    if (hasExpenseIntent) {
      const expense = await extractExpense(message);
      if (expense) {
        return NextResponse.json({
          message: `✅ Got it! Added ₹${expense.amount} expense. Anything else?`,
          expense,
        });
      }
    }
    
    if (/^(hi|hello|hey|hola)/i.test(lowerMsg)) {
      return NextResponse.json({
        message: '👋 Hello! I can help you track expenses. Try saying "I spent 500 on groceries" or "Paid 200 for taxi"',
      });
    }
    
    if (lowerMsg.includes('help')) {
      return NextResponse.json({
        message: '💡 I can help you:\n• Add expenses - "I spent 500 on food"\n• Track spending - "Paid 200 for uber"\n• Just tell me what you spent!',
      });
    }
    
    return NextResponse.json({
      message: '🤔 I can help you track expenses! Try: "I spent 500 on groceries" or "Paid 200 for taxi"',
    });
    
  } catch (error) {
    console.error('Chat error:', error);
    return NextResponse.json({
      message: 'Sorry, something went wrong. Please try again.',
    }, { status: 500 });
  }
}
