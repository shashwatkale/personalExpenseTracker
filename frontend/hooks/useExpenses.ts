import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { expenseApi, categoryApi } from '@/lib/api/services';

export const useExpenses = (startDate?: string, endDate?: string) => {
  return useQuery({
    queryKey: ['expenses', startDate, endDate],
    queryFn: () => expenseApi.getExpenses(startDate, endDate),
  });
};

export const useExpense = (id: string) => {
  return useQuery({
    queryKey: ['expense', id],
    queryFn: () => expenseApi.getExpense(id),
    enabled: !!id,
  });
};

export const useCreateExpense = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: expenseApi.createExpense,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['expenses'] });
      queryClient.invalidateQueries({ queryKey: ['monthlySummary'] });
    },
  });
};

export const useUpdateExpense = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: any }) =>
      expenseApi.updateExpense(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['expenses'] });
      queryClient.invalidateQueries({ queryKey: ['monthlySummary'] });
    },
  });
};

export const useDeleteExpense = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: expenseApi.deleteExpense,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['expenses'] });
      queryClient.invalidateQueries({ queryKey: ['monthlySummary'] });
    },
  });
};

export const useMonthlySummary = (year: number, month: number) => {
  return useQuery({
    queryKey: ['monthlySummary', year, month],
    queryFn: () => expenseApi.getMonthlySummary(year, month),
  });
};

export const useCategories = () => {
  return useQuery({
    queryKey: ['categories'],
    queryFn: categoryApi.getCategories,
  });
};
