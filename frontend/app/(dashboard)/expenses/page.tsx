'use client';

import { useState } from 'react';
import { useExpenses, useCategories, useCreateExpense, useDeleteExpense } from '@/hooks/useExpenses';
import { Plus, Trash2, Edit } from 'lucide-react';
import { format } from 'date-fns';

export default function ExpensesPage() {
  const [showForm, setShowForm] = useState(false);
  const { data: expenses, isLoading } = useExpenses();
  const { data: categories } = useCategories();
  const createExpense = useCreateExpense();
  const deleteExpense = useDeleteExpense();

  const [formData, setFormData] = useState({
    categoryId: '',
    amount: '',
    description: '',
    date: format(new Date(), 'yyyy-MM-dd'),
    notes: '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await createExpense.mutateAsync({
        categoryId: formData.categoryId,
        amount: parseFloat(formData.amount),
        description: formData.description,
        date: new Date(formData.date).toISOString(),
        notes: formData.notes || undefined,
      });
      setShowForm(false);
      setFormData({
        categoryId: '',
        amount: '',
        description: '',
        date: format(new Date(), 'yyyy-MM-dd'),
        notes: '',
      });
    } catch (error) {
      console.error('Failed to create expense:', error);
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure you want to delete this expense?')) {
      await deleteExpense.mutateAsync(id);
    }
  };

  if (isLoading) {
    return <div>Loading expenses...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Expenses</h1>
        <button
          onClick={() => setShowForm(!showForm)}
          className="btn-primary flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Add Expense</span>
        </button>
      </div>

      {/* Add Expense Form */}
      {showForm && (
        <div className="card">
          <h2 className="text-xl font-bold mb-4">New Expense</h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-2">Category</label>
                <select
                  value={formData.categoryId}
                  onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                  className="input"
                  required
                >
                  <option value="">Select category</option>
                  {categories?.map((cat) => (
                    <option key={cat.id} value={cat.id}>
                      {cat.icon} {cat.name}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Amount</label>
                <input
                  type="number"
                  step="0.01"
                  value={formData.amount}
                  onChange={(e) => setFormData({ ...formData, amount: e.target.value })}
                  className="input"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Description</label>
                <input
                  type="text"
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  className="input"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Date</label>
                <input
                  type="date"
                  value={formData.date}
                  onChange={(e) => setFormData({ ...formData, date: e.target.value })}
                  className="input"
                  required
                />
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Notes (Optional)</label>
              <textarea
                value={formData.notes}
                onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                className="input"
                rows={3}
              />
            </div>
            <div className="flex space-x-3">
              <button type="submit" className="btn-primary">
                {createExpense.isPending ? 'Saving...' : 'Save Expense'}
              </button>
              <button
                type="button"
                onClick={() => setShowForm(false)}
                className="btn-secondary"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Expenses List */}
      <div className="card">
        <h2 className="text-xl font-bold mb-4">All Expenses</h2>
        {expenses && expenses.length > 0 ? (
          <div className="space-y-3">
            {expenses.map((expense) => (
              <div
                key={expense.id}
                className="flex justify-between items-center p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
              >
                <div className="flex items-center space-x-4">
                  <span className="text-3xl">{expense.categoryIcon}</span>
                  <div>
                    <p className="font-semibold">{expense.description}</p>
                    <p className="text-sm text-gray-600">
                      {expense.categoryName} • {format(new Date(expense.date), 'MMM dd, yyyy')}
                    </p>
                    {expense.notes && (
                      <p className="text-sm text-gray-500 mt-1">{expense.notes}</p>
                    )}
                  </div>
                </div>
                <div className="flex items-center space-x-4">
                  <p className="font-bold text-xl">${expense.amount.toFixed(2)}</p>
                  <button
                    onClick={() => handleDelete(expense.id)}
                    className="text-red-600 hover:text-red-800"
                  >
                    <Trash2 className="w-5 h-5" />
                  </button>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p className="text-gray-500 text-center py-12">
            No expenses yet. Click "Add Expense" to get started!
          </p>
        )}
      </div>
    </div>
  );
}
