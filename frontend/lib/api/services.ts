import apiClient from './client';

export interface Expense {
  id: string;
  categoryId: string;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  amount: number;
  description: string;
  date: string;
  notes?: string;
  createdAt: string;
}

export interface Category {
  id: string;
  name: string;
  description?: string;
  icon: string;
  color: string;
}

export interface MonthlySummary {
  totalExpenses: number;
  expenseCount: number;
  categoryBreakdown: Record<string, number>;
  aiSummary: string;
  aiSuggestions: string[];
}

export const expenseApi = {
  getExpenses: async (startDate?: string, endDate?: string) => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    const { data } = await apiClient.get<Expense[]>(`/expenses?${params}`);
    return data;
  },

  getExpense: async (id: string) => {
    const { data } = await apiClient.get<Expense>(`/expenses/${id}`);
    return data;
  },

  createExpense: async (expense: {
    categoryId: string;
    amount: number;
    description: string;
    date: string;
    notes?: string;
  }) => {
    const { data } = await apiClient.post<Expense>('/expenses', expense);
    return data;
  },

  updateExpense: async (id: string, expense: {
    categoryId: string;
    amount: number;
    description: string;
    date: string;
    notes?: string;
  }) => {
    const { data } = await apiClient.put<Expense>(`/expenses/${id}`, expense);
    return data;
  },

  deleteExpense: async (id: string) => {
    await apiClient.delete(`/expenses/${id}`);
  },

  getMonthlySummary: async (year: number, month: number) => {
    const { data } = await apiClient.get<MonthlySummary>(`/expenses/summary/${year}/${month}`);
    return data;
  },
};

export const categoryApi = {
  getCategories: async () => {
    const { data } = await apiClient.get<Category[]>('/categories');
    return data;
  },
};

export const authApi = {
  register: async (userData: {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
  }) => {
    const { data } = await apiClient.post('/auth/register', userData);
    return data;
  },

  login: async (credentials: { email: string; password: string }) => {
    const { data } = await apiClient.post('/auth/login', credentials);
    return data;
  },

  loginWithGoogle: async (userData: { email: string; name: string; googleId: string }) => {
    const { data } = await apiClient.post('/auth/google', userData);
    return data;
  },

  loginWithPhone: async (phoneData: { phoneNumber: string; verificationCode: string }) => {
    const { data } = await apiClient.post('/auth/phone', phoneData);
    return data;
  },

  sendPhoneVerification: async (phoneNumber: string) => {
    const { data } = await apiClient.post('/auth/phone/send-code', { phoneNumber });
    return data;
  },
};
