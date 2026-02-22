import { expenseApi, budgetApi, authApi } from '@/lib/api/services';
import apiClient from '@/lib/api/client';

jest.mock('@/lib/api/client');

describe('API Services', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('expenseApi', () => {
    it('should create expense', async () => {
      const mockExpense = {
        id: '1',
        categoryId: '1',
        categoryName: 'Food',
        categoryIcon: '🍔',
        categoryColor: '#FF0000',
        amount: 100,
        description: 'Lunch',
        date: '2024-01-01',
        createdAt: '2024-01-01',
      };

      (apiClient.post as jest.Mock).mockResolvedValue({ data: mockExpense });

      const result = await expenseApi.createExpense({
        categoryId: '1',
        amount: 100,
        description: 'Lunch',
        date: '2024-01-01',
      });

      expect(result).toEqual(mockExpense);
      expect(apiClient.post).toHaveBeenCalledWith('/expenses', expect.any(Object));
    });

    it('should get expenses', async () => {
      const mockExpenses = [{ id: '1', amount: 100 }];
      (apiClient.get as jest.Mock).mockResolvedValue({ data: mockExpenses });

      const result = await expenseApi.getExpenses();

      expect(result).toEqual(mockExpenses);
      expect(apiClient.get).toHaveBeenCalled();
    });

    it('should delete expense', async () => {
      (apiClient.delete as jest.Mock).mockResolvedValue({});

      await expenseApi.deleteExpense('1');

      expect(apiClient.delete).toHaveBeenCalledWith('/expenses/1');
    });
  });

  describe('budgetApi', () => {
    it('should set salary', async () => {
      (apiClient.post as jest.Mock).mockResolvedValue({ data: { message: 'Success' } });

      await budgetApi.setSalary(50000);

      expect(apiClient.post).toHaveBeenCalledWith('/budget/salary', { monthlySalary: 50000 });
    });

    it('should get budget overview', async () => {
      const mockOverview = {
        monthlySalary: 50000,
        totalAllocated: 30000,
        totalSpent: 20000,
        remaining: 30000,
        budgets: [],
      };

      (apiClient.get as jest.Mock).mockResolvedValue({ data: mockOverview });

      const result = await budgetApi.getOverview();

      expect(result).toEqual(mockOverview);
    });
  });

  describe('authApi', () => {
    it('should register user', async () => {
      const mockResponse = { token: 'test-token', email: 'test@example.com' };
      (apiClient.post as jest.Mock).mockResolvedValue({ data: mockResponse });

      const result = await authApi.register({
        email: 'test@example.com',
        password: 'password123',
        firstName: 'John',
        lastName: 'Doe',
      });

      expect(result).toEqual(mockResponse);
    });

    it('should login user', async () => {
      const mockResponse = { token: 'test-token', email: 'test@example.com' };
      (apiClient.post as jest.Mock).mockResolvedValue({ data: mockResponse });

      const result = await authApi.login({
        email: 'test@example.com',
        password: 'password123',
      });

      expect(result).toEqual(mockResponse);
    });
  });
});
